using System;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using DAL.Abstract.Entities.Options.Transport;
using Shared.Enums;
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
                //IsOpenChangeRx.OnNext(new IsOpenChangeRxModel { IsOpen = _isOpen, TransportName = Option.Port });
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
                //StatusStringChangeRx.OnNext(new StatusStringChangeRxModel { Status = _statusString, TransportName = Option.Port });
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
                //StatusDataExchangeChangeRx.OnNext(new StatusDataExchangeChangeRxModel { StatusDataExchange = _statusDataExchange, TransportName = Option.Port });
            }
        }

        public bool IsCycleReopened { get; private set; }

        #endregion




        #region ctor

        public TcpIpTransport(TcpIpOption option,  KeyTransport keyTransport)
        {
            Option = option;
            KeyTransport = keyTransport;
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
            await Task.CompletedTask;
            IsOpen = true;
            return true; //DEBUG
        }


        public void CycleReOpenedCancelation()
        {
            if (IsCycleReopened)
                _ctsCycleReOpened.Cancel();
        }


        public async Task ReOpen()
        {
            try
            {
                _terminalClient = new TcpClient { NoDelay = false };  //true - пакет будет отправлен мгновенно (при NetworkStream.Write). false - пока не собранно значительное кол-во данных отправки не будет.
                IPAddress ipAddress = IPAddress.Parse(Option.IpAddress);
                StatusString = $"Conect to {ipAddress} : {Option.IpPort} ...";

                await _terminalClient.ConnectAsync(ipAddress, Option.IpPort);
                _terminalNetStream = _terminalClient.GetStream();
                IsOpen = true;
                return;
            }
            catch (Exception ex)
            {
                IsOpen = false;
                StatusString = $"Ошибка инициализации соединения: \"{ex.Message}\"";
                //LogException.WriteLog("Инициализация: ", ex, LogException.TypeLog.TcpIp);
                Dispose();
            }
        }


        public async Task<StatusDataExchange> DataExchangeAsync(int timeRespoune, ITransportDataProvider dataProvider, CancellationToken ct)
        {
            if (!IsOpen)
                return StatusDataExchange.None;

            if (dataProvider == null)
                return StatusDataExchange.None;

            StatusDataExchange = StatusDataExchange.Start;

            var buffer = dataProvider.GetDataByte();


            await Task.CompletedTask;

            return StatusDataExchange;
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
        }

        #endregion
    }
}