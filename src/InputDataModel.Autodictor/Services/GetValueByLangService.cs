using System;
using InputDataModel.Autodictor.Entities;
using InputDataModel.Autodictor.Model;

namespace InputDataModel.Autodictor.Services
{
    public class GetValueByLangService
    {
        public string GetEventName(AdInputType adInputType)
        {
            switch (adInputType.Lang)
            {
                case Lang.Ru:
                    return adInputType.
                    break;
                case Lang.Eng:
                    break;
                case Lang.Fin:
                    break;
                case Lang.Ch:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

    }
}