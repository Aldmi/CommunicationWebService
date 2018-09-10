using AutoMapper;
using DAL.Abstract.Entities.Options.Device;
using DAL.Abstract.Entities.Options.Exchange;
using DAL.Abstract.Entities.Options.Transport;
using DAL.EFCore.Entities.Device;
using DAL.EFCore.Entities.Exchange;
using DAL.EFCore.Entities.Transport;

namespace DAL.EFCore.Mappers
{
    public class AutoMapperConfig
    {
        public static IMapper Mapper { get; set; }

        public static void Register()
        {
            var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<SerialOption, EfSerialOption>().ReverseMap();
                    cfg.CreateMap<TcpIpOption, EfTcpIpOption>().ReverseMap();
                    cfg.CreateMap<HttpOption, EfHttpOption>().ReverseMap();
                    cfg.CreateMap<DeviceOption, EfDeviceOption>().ReverseMap();
                    cfg.CreateMap<ExchangeOption, EfExchangeOption>().ReverseMap();
                });
            Mapper = config.CreateMapper();
        }
    }
}