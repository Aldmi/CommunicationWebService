using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BL.Services.Exceptions;
using BL.Services.Mediators;
using DeviceForExchange;

namespace BL.Services.Actions
{
    /// <summary>
    /// Сервис делает БИЛД ус-ва. 
    /// После успешного билда ус-ва, Обмены, транспорт попадают в Storages
    /// </summary>
    /// <typeparam name="TIn"></typeparam>
    public class BuildDeviceService<TIn>
    {
        #region field

        private readonly MediatorForOptions _mediatorForOptionsRep;
        private readonly MediatorForStorages<TIn> _mediatorForStorages;

        #endregion




        #region ctor

        public BuildDeviceService(MediatorForOptions mediatorForOptionsRep, MediatorForStorages<TIn> mediatorForStorages)
        {
            _mediatorForOptionsRep = mediatorForOptionsRep;
            _mediatorForStorages = mediatorForStorages;
        }

        #endregion




        #region Methode

        /// <summary>
        /// Сделать БИЛД для всех устройств хранящихся в репозитории.
        /// </summary>
        /// <returns>Спсисок созданных ус-в</returns>
        /// <exception cref="AggregateException"></exception>
        public async Task<IEnumerable<Device<TIn>>> BuildAllDevices()
        {
            var newDevices = new List<Device<TIn>>();
            var exceptions = new List<Exception>();
            var devices = await _mediatorForOptionsRep.GetDeviceOptionsWithAutoBuildAsync();
            foreach (var device in devices)
            {
                try
                {
                   var dev= await BuildDevice(device.Name);
                   newDevices.Add(dev);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }           
            }

            if(exceptions.Any())
                throw new AggregateException(exceptions);

            return newDevices;
        }


        /// <summary>
        /// ПОЗАБОТИТСЯ ОБ ОБРАБОТКЕ ИСКЛЮЧЕНИЙ. (StorageHandlerException, OptionHandlerException, Exception)
        /// </summary>
        /// <param name="deviceName"></param>
        /// <returns></returns>
        /// <exception cref="StorageHandlerException"></exception>
        /// <exception cref="OptionHandlerException"></exception>
        /// <exception cref="Exception"></exception> 
        public async Task<Device<TIn>> BuildDevice(string deviceName)
        {        
            if (!await _mediatorForOptionsRep.IsExistDeviceAsync(deviceName))
            {
                return null;
            }
            var optionAgregator = await _mediatorForOptionsRep.GetOptionAgregatorForDeviceAsync(deviceName);
            var newDevice = _mediatorForStorages.BuildAndAddDevice(optionAgregator);
            return newDevice;
        }

        #endregion
    }
}