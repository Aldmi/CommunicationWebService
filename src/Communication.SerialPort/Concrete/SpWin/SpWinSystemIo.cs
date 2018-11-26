using System;
using System.IO.Ports;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using DAL.Abstract.Entities.Options.Transport;
using Shared.Enums;
using Shared.Helpers;
using Shared.Types;
using Transport.Base.DataProvidert;
using Transport.Base.RxModel;
using Transport.SerialPort.Abstract;
using Parity = System.IO.Ports.Parity;
using StopBits = System.IO.Ports.StopBits;


namespace Transport.SerialPort.Concrete.SpWin
{
    public class SpWinSystemIo : ISerailPort
    {
        #region fields

        private const int TimeCycleReOpened = 3000;
        private readonly System.IO.Ports.SerialPort _port; //COM порт
        private CancellationTokenSource _ctsCycleReOpened;

        #endregion




        #region prop

        public SerialOption Option { get; }
        public KeyTransport KeyTransport { get; }

        private bool _isOpen;
        public bool IsOpen
        {
            get => _isOpen;
            private set
            {
                if (value == _isOpen) return;
                _isOpen = value;
                IsOpenChangeRx.OnNext(new IsOpenChangeRxModel { IsOpen = _isOpen, TransportName = Option.Port });
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
                StatusStringChangeRx.OnNext(new StatusStringChangeRxModel { Status = _statusString, TransportName = Option.Port });
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
                StatusDataExchangeChangeRx.OnNext(new StatusDataExchangeChangeRxModel { StatusDataExchange = _statusDataExchange, TransportName = Option.Port });
            }
        }

        public bool IsCycleReopened { get; private set; }

        #endregion




        #region ctor

        public SpWinSystemIo(SerialOption option, KeyTransport keyTransport)
        {
            Option = option;
            KeyTransport = keyTransport;
            _port = new System.IO.Ports.SerialPort(option.Port)
            {
                BaudRate = option.BaudRate,
                DataBits = option.DataBits,
                StopBits = (StopBits) option.StopBits,
                Parity = (Parity) option.Parity,
                DtrEnable = option.DtrEnable,
                RtsEnable = option.RtsEnable
            };
        }

        #endregion




        #region Rx

        public ISubject<IsOpenChangeRxModel> IsOpenChangeRx { get; } =  new Subject<IsOpenChangeRxModel>();                                        //СОБЫТИЕ ИЗМЕНЕНИЯ ОТКРЫТИЯ/ЗАКРЫТИЯ ПОРТА
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
                res = ReOpenWithDispose();
                if (!res)
                {
                    Console.WriteLine($"коннект для транспорта НЕ ОТКРЫТ: {KeyTransport}");
                    await Task.Delay(TimeCycleReOpened, _ctsCycleReOpened.Token);
                }
            }
            Console.WriteLine($"Коннект ОТКРЫТ: {KeyTransport}");
            IsCycleReopened = false;
            return true;
        }


        public void CycleReOpenedCancelation()
        {
            if (IsCycleReopened)
             _ctsCycleReOpened.Cancel();
        }


        private bool ReOpenWithDispose()
        {
            Dispose();
            IsOpen = false;
            StatusDataExchange = StatusDataExchange.None;
            try
            {
                _port.Open();
            }
            catch (Exception ex)
            {
                IsOpen = false;
                StatusString = $"Ошибка открытия порта: {_port.PortName}. ОШИБКА: {ex}";
                return false;
            }

            IsOpen = true;
            StatusString = $"Порт открыт: {_port.PortName}.";
            return true;
        }


        public async Task<bool> ReOpen()
        {
            try
            {
                if (_port.IsOpen)
                {
                    _port.Close();
                    IsOpen = false;
                }

                if (!_port.IsOpen)
                {
                    _port.Open();
                    IsOpen = true;
                    return true;
                }
            }
            catch (Exception ex)
            {
                StatusString = $"Ошибка ReOpen порта: {_port.PortName}. ОШИБКА: {ex}";
                await CycleReOpened();
            }
            return false;
        }


        /// <summary>
        /// Функция обмена по порту. Запрос-ожидание-ответ.
        /// Возвращает true если результат обмена успешен.
        /// </summary>
        public async Task<StatusDataExchange> DataExchangeAsync(int timeRespoune, ITransportDataProvider dataProvider, CancellationToken ct)
        {
            if (!IsOpen)
                return StatusDataExchange.None;

            if (dataProvider == null)
                return StatusDataExchange.None;

            StatusDataExchange = StatusDataExchange.Start;
            try
            {
                byte[] writeBuffer = dataProvider.GetDataByte();
                if (writeBuffer != null && writeBuffer.Any())
                {
                    StatusDataExchange = StatusDataExchange.Process;
                    var readBuff = await RequestAndRespawnConstPeriodAsync(writeBuffer, dataProvider.CountSetDataByte, timeRespoune, ct);
                    dataProvider.SetDataByte(readBuff);
                }
            }
            catch (OperationCanceledException)
            {
                StatusDataExchange = StatusDataExchange.EndWithCanceled;
                return StatusDataExchange.EndWithCanceled;
            }
            catch (TimeoutException)
            {
                //ReOpen();
                StatusDataExchange = StatusDataExchange.EndWithTimeout;
                return StatusDataExchange.EndWithTimeout;
            }
            StatusDataExchange = StatusDataExchange.End; 
            return StatusDataExchange.End;
        }


        /// <summary>
        /// Функция посылает запрос в порт, потом отсчитывает время readTimeout и проверяет буфер порта на чтение.
        /// Таким образом обеспечивается одинаковый промежуток времени между запросами в порт.
        /// </summary>
        private async Task<byte[]> RequestAndRespawnConstPeriodAsync(byte[] writeBuffer, int nBytesRead, int readTimeout, CancellationToken ct)
        {
            if (!_port.IsOpen)
                return await Task<byte[]>.Factory.StartNew(() => null, ct);

            //очистили буферы порта
            _port.DiscardInBuffer();
            _port.DiscardOutBuffer();

            //отправили данные в порт
            _port.WriteTimeout = 500;
            _port.Write(writeBuffer, 0, writeBuffer.Length);

            //ждем ответа....
            await Task.Delay(readTimeout, ct);

            //проверяем ответ
            var buffer = new byte[nBytesRead];
            if (_port.BytesToRead == nBytesRead)
            {
                _port.Read(buffer, 0, nBytesRead);
                return buffer;
            }
            throw new TimeoutException("Время на ожидание ответа вышло");
        }


        /// <summary>
        /// Функция посылает запрос в порт, и как только в буфер порта приходят данные сразу же проверяет их кол-во.
        /// Как только накопится нужное кол-во байт сразу же будет возвращен ответ не дожедаясь времени readTimeout.
        /// Таким образом период опроса не фиксированный, а определяется скоростью ответа slave устройства.
        /// </summary>
        private bool _isBysuRequestAndRespawn;
        private async Task<byte[]> RequestAndRespawnInstantlyAsync(byte[] writeBuffer, int nBytesRead, int readTimeout, CancellationToken ct)
        {
            if (!_isBysuRequestAndRespawn)
            {
                _isBysuRequestAndRespawn = true;

                if (!_port.IsOpen)
                    return null;

                var tcs = new TaskCompletionSource<byte[]>();
                SerialDataReceivedEventHandler handler = null;
                try
                {
                    //очистили буферы порта
                    _port.DiscardInBuffer();
                    _port.DiscardOutBuffer();

                    //_port.WriteTimeout = 500; //??????
                    _port.Write(writeBuffer, 0, writeBuffer.Length);     //отправили данные в порт

                    //ждем ответа....
                    handler = (o, e) =>
                    {
                        if (_port.BytesToRead >= nBytesRead)
                        {
                            var buffer = new byte[nBytesRead];
                            _port.Read(buffer, 0, nBytesRead);
                            tcs.TrySetResult(buffer);
                        }
                    };
                    _port.DataReceived += handler;

                    var buff = await tcs.Task.WithTimeout2HandleException(readTimeout, ct);
                    return buff;
                }
                catch (TimeoutException)
                {
                    tcs.TrySetCanceled();
                    throw;
                }
                catch (Exception ex)
                {
                    StatusString = $"Ошибка работы с портом (RequestAndRespawnInstantlyAsync): {_port.PortName}. ОШИБКА: {ex}  InnerException: {ex.InnerException?.Message ?? string.Empty}";
                    //ReOpen();
                    return null;
                }
                finally
                {
                    _port.DataReceived -= handler;
                    _isBysuRequestAndRespawn = false;
                }
            }

            StatusString = "Ошибка работы с портом (ПОПЫТКА ОБРАЩЕНИЯ К ЗАНЯТОМУ ПОРТУ)";
            return null;
        }

        #endregion




        #region Disposable

        public void Dispose()
        {
            if (_port == null)
                return;

            if (_port.IsOpen)
            {
                _port.Close();
            }

            _port.Dispose();
        }

        #endregion
    }
}