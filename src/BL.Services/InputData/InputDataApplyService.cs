using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public async Task<IEnumerable<string>> ApplyInputData(IEnumerable<InputData<TIn>> inputDatas)
        {
            //найти Device по имени и передать ему данные 
            var errors= new List<string>();
            var tasks = new List<Task>();
            foreach (var inData in inputDatas)
            {
                var device = _mediatorForStorages.GetDevice(inData.DeviceName);
                if (device == null)
                {
                    errors.Add($"устройство не найденно: {inData.DeviceName}");
                    continue;
                }

                if(_mediatorForStorages.GetExchange(inData.ExchangeName) == null)
                {
                  errors.Add($"Обмен не найденн: {inData.ExchangeName}");
                  continue;
                }

                if (string.IsNullOrEmpty(inData.ExchangeName))
                {
                    tasks.Add(device.Send2AllExchanges(inData.DataAction, inData.Data, inData.Command));
                }
                else
                {
                    tasks.Add(device.Send2ConcreteExchanges(inData.ExchangeName, inData.DataAction, inData.Data, inData.Command));
                }
            }

            await Task.WhenAll(tasks);
            return errors;
        }

        #endregion
    }
}