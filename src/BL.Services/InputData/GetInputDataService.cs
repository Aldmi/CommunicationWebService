using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BL.Services.Mediators;
using InputDataModel.Base;

namespace BL.Services.InputData
{
    public class GetInputDataService<TIn>
    {
        private readonly MediatorForStorages<TIn> _mediatorForStorages;



        #region ctor

        public GetInputDataService(MediatorForStorages<TIn> mediatorForStorages)
        {
            _mediatorForStorages = mediatorForStorages;
        }

        #endregion




        #region Methode
        /// <summary>
        /// Найти все ус-ва по имени и передать им данные.
        /// </summary>
        /// <param name="inputDatas"></param>
        public void ApplyInputData(IEnumerable<InputData<TIn>> inputDatas)
        {
            //найти Device по имени и передать ему данные 
            foreach (var inData in inputDatas)
            {
                var device = _mediatorForStorages.GetDevice(inData.DeviceName);
                if (device == null)
                {
                    //LOG
                    continue;
                }
                //Передать данные по конкретным Exchanges на конкретное действие
                var exch = device.Exchanges.FirstOrDefault();
                exch.SendOneTimeData(inData.Data.FirstOrDefault());
            }
        }

        #endregion
    }
}