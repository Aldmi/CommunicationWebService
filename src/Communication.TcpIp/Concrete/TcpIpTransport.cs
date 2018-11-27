using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using DAL.Abstract.Entities.Options.Transport;
using Serilog;
using SerilogTimings.Extensions;
using Shared.Enums;
using Shared.Helpers;
using Shared.Types;
using Transport.Base.DataProvidert;
using Transport.Base.RxModel;
using Transport.TcpIp.Abstract;

namespace Transport.TcpIp.Concrete
{
    public class TcpIpTransport : ITcpIp
    {
        #region fields

        private TcpClient _terminalClient;
        private NetworkStream _terminalNetStream;

        private const int TimeCycleReOpened = 3000;
        private CancellationTokenSource _ctsCycleReOpened;
        private readonly ILogger _logger;

        #endregion



        #region prop

        public TcpIpOption Option { get; }
        public KeyTransport KeyTransport { get; }

        private bool _isOpen;
        public bool IsOpen
        {
            get => _isOpen;
            private set
            {
                if (value == _isOpen) return;
                _isOpen = value;
                IsOpenChangeRx.OnNext(new IsOpenChangeRxModel { IsOpen = _isOpen, TransportName = Option.Name });
            }
        }

        private string _statusString;
        public string StatusString
        {
            get => _statusString;
            private set
            {
                if (value == _statusString) return;
                _statusString = value;
                StatusStringChangeRx.OnNext(new StatusStringChangeRxModel { Status = _statusString, TransportName = Option.Name });
            }
        }

        private StatusDataExchange _statusDataExchange;
        public StatusDataExchange StatusDataExchange
        {
            get => _statusDataExchange;
            private set
            {
                if (value == _statusDataExchange) return;
                _statusDataExchange = value;
                StatusDataExchangeChangeRx.OnNext(new StatusDataExchangeChangeRxModel { StatusDataExchange = _statusDataExchange, TransportName = Option.Name });
            }
        }

        public bool IsCycleReopened { get; private set; }

        #endregion




        #region ctor

        public TcpIpTransport(TcpIpOption option, KeyTransport keyTransport, ILogger logger)
        {
            Option = option;
            KeyTransport = keyTransport;
            _logger = logger;
        }

        #endregion



        #region Rx

        public ISubject<IsOpenChangeRxModel> IsOpenChangeRx { get; } = new Subject<IsOpenChangeRxModel>();                                        //СОБЫТИЕ ИЗМЕНЕНИЯ ОТКРЫТИЯ/ЗАКРЫТИЯ ПОРТА
        public ISubject<StatusDataExchangeChangeRxModel> StatusDataExchangeChangeRx { get; } = new Subject<StatusDataExchangeChangeRxModel>();     //СОБЫТИЕ ИЗМЕНЕНИЯ ОТПРАВКИ ДАННЫХ ПО ПОРТУ
        public ISubject<StatusStringChangeRxModel> StatusStringChangeRx { get; } = new Subject<StatusStringChangeRxModel>();                       //СОБЫТИЕ ИЗМЕНЕНИЯ СТРОКИ СТАТУСА ПОРТА

        #endregion



        #region Methode

        public async Task<bool> CycleReOpened()
        {
            IsCycleReopened = true;
            _ctsCycleReOpened = new CancellationTokenSource();
            bool res = false;
            while (!_ctsCycleReOpened.IsCancellationRequested && !res)
            {
                res = await ReOpen();
                if (!res)
                {
                    _logger.Warning($"коннект для транспорта НЕ ОТКРЫТ: {KeyTransport}");
                    await Task.Delay(TimeCycleReOpened, _ctsCycleReOpened.Token);
                }
            }
            _logger.Information($"коннект для транспорта ОТКРЫТ: {KeyTransport}");
            IsCycleReopened = false;
            return true;
        }


        public void CycleReOpenedCancelation()
        {
            if (IsCycleReopened)
                _ctsCycleReOpened.Cancel();
        }


        public async Task<bool> ReOpen()
        {
            try
            {
                _terminalClient = new TcpClient { NoDelay = false };  //true - пакет будет отправлен мгновенно (при NetworkStream.Write). false - пока не собранно значительное кол-во данных отправки не будет.
                var ipAddress = IPAddress.Parse(Option.IpAddress);
                StatusString = $"Conect to {ipAddress} : {Option.IpPort} ...";
                await _terminalClient.ConnectAsync(ipAddress, Option.IpPort);
                _terminalNetStream = _terminalClient.GetStream();
                IsOpen = true;
                return true;
            }
            catch (Exception ex)
            {
                IsOpen = false;
                StatusString = $"Ошибка инициализации соединения: \"{ex.Message}\"";
                _logger.Error(ex, StatusString);
                Dispose();
            }
            return false;
        }


        public async Task<StatusDataExchange> DataExchangeAsync(int timeRespoune, ITransportDataProvider dataProvider, CancellationToken ct)
        {
            if (!IsOpen)
                return StatusDataExchange.None;

            if (dataProvider == null)
                return StatusDataExchange.None;

            StatusDataExchange = StatusDataExchange.Start;
            if (await SendDataAsync(dataProvider, ct))
            {
                try
                {
                    //var data = await TakeDataInstantlyAsync(dataProvider.CountSetDataByte, timeRespoune, ct);
                    var data = await TakeDataConstPeriodAsync(dataProvider.CountSetDataByte, timeRespoune, ct);
                    var res = dataProvider.SetDataByte(data);
                    if (!res)
                    {
                        StatusDataExchange = StatusDataExchange.EndWithError;
                        return StatusDataExchange;
                    }
                }
                catch (OperationCanceledException)
                {
                    StatusDataExchange = StatusDataExchange.EndWithCanceled;
                    return StatusDataExchange;
                }
                catch (TimeoutException)
                {
                    StatusDataExchange = StatusDataExchange.EndWithTimeout;
                    return StatusDataExchange;
                }
                catch (IOException ex)
                {
                    _logger.Error(ex, $"TcpIpTransport {KeyTransport}. IOException");
                    StatusDataExchange = StatusDataExchange.EndWithErrorCritical;
                    return StatusDataExchange;
                }
                catch (Exception ex)
                {
                    _logger.Fatal(ex, $"TcpIpTransport {KeyTransport}. Exception");
                    StatusDataExchange = StatusDataExchange.EndWithErrorCritical;
                    return StatusDataExchange;
                }
                StatusDataExchange = StatusDataExchange.End;
                return StatusDataExchange.End;
            }

            StatusDataExchange = StatusDataExchange.EndWithError;
            return StatusDataExchange;
        }


        public async Task<bool> SendDataAsync(ITransportDataProvider dataProvider, CancellationToken ct)
        {
            byte[] buffer = dataProvider.GetDataByte();
            try
            {
                if (_terminalClient != null && _terminalNetStream != null && _terminalClient.Client != null && _terminalClient.Client.Connected)
                {
                    await _terminalNetStream.WriteAsync(buffer, 0, buffer.Length, ct);
                    return true;
                }
            }
            catch (Exception ex)
            {
                StatusString = $"ИСКЛЮЧЕНИЕ SendDataToServer :{ex.Message}";
                _logger.Error(ex, $"TcpIpTransport/SendDataToServer {KeyTransport}");
            }
            return false;
        }


        /// <summary>
        /// Прием данных с пеерменным периодом.
        /// Прием заканчивается когда нужное кол-во данных поступит в входной буффер порта.
        /// </summary>
        public async Task<byte[]> TakeDataInstantlyAsync(int nbytes, int timeOut, CancellationToken ct)
        {
            using (_logger.TimeOperation("TimeOpertaion TakeDataAsync"))
            {
                byte[] bDataTemp = new byte[256];
                _terminalNetStream.ReadTimeout = timeOut;

                //TODO: создать task в котором считывать пока не считаем нужное кол-во байт. Прерывать этот task по таймауту  AsyncHelp.WithTimeout
                //int nByteTake=0;
                //while (true)
                //{
                //    nByteTake = _terminalNetStream.Read(bDataTemp, 0, nbytes);
                //    Task.Delay(500);
                //}
                // ctsTimeout = new CancellationTokenSource();//токен сработает по таймауту в функции WithTimeout
                // cts = CancellationTokenSource.CreateLinkedTokenSource(ctsTimeout.Token, ct); // Объединенный токен, сработает от выставленного ctsTimeout.Token или от ct
                //int nByteTake = await _terminalNetStream.ReadAsync(bDataTemp, 0, nbytes, cts.Token).WithTimeout(timeOut, ctsTimeout);
                var ctsTimeout = new CancellationTokenSource();//токен сработает по таймауту в функции WithTimeout
                var cts = CancellationTokenSource.CreateLinkedTokenSource(ctsTimeout.Token, ct); // Объединенный токен, сработает от выставленного ctsTimeout.Token или от ct
                int nByteTake = await _terminalNetStream.ReadAsync(bDataTemp, 0, nbytes, cts.Token).WithTimeout2CanceledTask(timeOut, ctsTimeout);
                if (nByteTake == nbytes)
                {
                    var bData = new byte[nByteTake];
                    Array.Copy(bDataTemp, bData, nByteTake);
                    return bData;
                }

                _logger.Warning($"TcpIpTransport/TakeDataAsync {KeyTransport}.  Кол-во считанных данных не верное  Принято= {nByteTake}  Ожидаем= {nbytes}");
                //TODO: добавить логирование если кол-во считанных данных не верное (помещать считаыннй НЕВЕРНЫЙ буфер в лог)
                return null;
            }
        }


        /// <summary>
        /// Прием данных с постоянным периодом.
        /// </summary>
        public async Task<byte[]> TakeDataConstPeriodAsync(int nbytes, int timeOut, CancellationToken ct)
        {           
            byte[] bDataTemp = new byte[256];

            //Ожидаем накопление данных в буффере
            _terminalNetStream.ReadTimeout = timeOut;
            _terminalClient.ReceiveTimeout = timeOut;
            await Task.Delay(timeOut, ct);

            //Мгновенно с ожиданием в 100мс вычитываем поступивщий буффер
            var ctsTimeout = new CancellationTokenSource();//токен сработает по таймауту в функции WithTimeout
            var cts = CancellationTokenSource.CreateLinkedTokenSource(ctsTimeout.Token, ct); // Объединенный токен, сработает от выставленного ctsTimeout.Token или от ct
            int nByteTake = await _terminalNetStream.ReadAsync(bDataTemp, 0, nbytes, cts.Token).WithTimeout2CanceledTask(10, ctsTimeout);
            if (nByteTake == nbytes)
            {
                var bData = new byte[nByteTake];
                Array.Copy(bDataTemp, bData, nByteTake);
                return bData;
            }

            _logger.Warning($"TcpIpTransport/TakeDataAsync {KeyTransport}.  Кол-во считанных данных не верное  Принято= {nByteTake}  Ожидаем= {nbytes}");
            //TODO: добавить логирование если кол-во считанных данных не верное (помещать считаыннй НЕВЕРНЫЙ буфер в лог)
            return null;          
        }

        #endregion



        #region Disposable

        public void Dispose()
        {
            if (_terminalNetStream != null)
            {
                _terminalNetStream.Close();
                StatusString = "Сетевой поток закрыт ...";
            }
            _terminalClient?.Client?.Close();
            _terminalClient?.Client?.Dispose();
            _terminalClient?.Dispose();
        }

        #endregion
    }
}