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

                    cfg.CreateMap<DeviceOption, EfDeviceOption>()
                        .ForMember(dest => dest.DeviceId, opt => opt.MapFrom(src => src.Id))
                        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => 0))
                        .ReverseMap();

                    cfg.CreateMap<ExchangeOption, EfExchangeOption>()
                    .ForMember(dest => dest.ExchangeId, opt => opt.MapFrom(src => src.Id))
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => 0))
                    .ReverseMap();

                    cfg.CreateMap<Provider, EfProvider>()
                        .ForMember(dest => dest.ProviderId, opt => opt.MapFrom(src => src.Id))
                        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => 0))
                        .ReverseMap();
                });
            Mapper = config.CreateMapper();
        }
    }
}