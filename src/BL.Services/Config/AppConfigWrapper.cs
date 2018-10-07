using Infrastructure.MessageBroker.Options;
using Microsoft.Extensions.Configuration;

namespace BL.Services.Config
{
    public class AppConfigWrapper
    {
        private readonly IConfiguration _configuration;


        #region ctor

        public AppConfigWrapper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #endregion



        #region prop

        private ProduserOption _produser4DeviceOption;
        public ProduserOption GetProduser4DeviceOption
        {
            get
            {
                _produser4DeviceOption = _produser4DeviceOption ??
                                         new ProduserOption
                                         {
                                             BrokerEndpoints = _configuration["MessageBrokerProduser4DeviceResp:BrokerEndpoints"],
                                         };
                return _produser4DeviceOption;
            }
        }

        #endregion
    }
}