using System.Collections.Generic;
using System.Linq;
using BL.Services.Mediators;
using InputDataModel.Base;

namespace BL.Services.InputData
{
    public class InputDataApplyService<TIn>
    {
        private readonly MediatorForStorages<TIn> _mediatorForStorages;



        #region ctor

        public InputDataApplyService(MediatorForStorages<TIn> mediatorForStorages)
        {
            _mediatorForStorages = mediatorForStorages;
        }

        #endregion




        #region Methode
        /// <summary>
        /// Найти все ус-ва по имени и передать им данные.
        /// </summary>
        /// <param name="inputDatas">Данные для нескольких ус-в</param>
        /// <returns>Список ОШИБОК</returns>
        public IEnumerable<string> ApplyInputData(IEnumerable<InputData<TIn>> inputDatas)
        {
            //найти Device по имени и передать ему данные 
            foreach (var inData in inputDatas)
            {
                var device = _mediatorForStorages.GetDevice(inData.DeviceName);
                if (device == null)
                {
                    yield return $"устройство не найденно: {inData.DeviceName}";
                    continue;
                }

                if (string.IsNullOrEmpty(inData.ExchangeName))
                {
                    device.Send2AllExchanges(inData.DataAction, inData.Data, inData.Command);
                }
                else
                {
                    device.Send2ConcreteExchanges(inData.ExchangeName, inData.DataAction, inData.Data, inData.Command);
                }
            }
        }

        #endregion
    }
}