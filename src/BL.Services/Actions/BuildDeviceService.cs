using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BL.Services.Mediators;
using DeviceForExchange;

namespace BL.Services.Actions
{
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
        /// ПОЗАБОТИТСЯ ОБ ОБРАБОТКЕ ИСКЛЮЧЕНИЙ. (StorageHandlerException, Exception)
        /// </summary>
        /// <param name="deviceName"></param>
        /// <returns></returns>
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


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Device<TIn>>> BuildAllDevices()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}