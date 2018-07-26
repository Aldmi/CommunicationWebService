using AutoMapper;
using DAL.Abstract.Entities.Device;
using DAL.Abstract.Entities.Exchange;
using DAL.Abstract.Entities.Transport;
using WebServer.DTO.JSON.OptionsDto.DeviceOption;
using WebServer.DTO.JSON.OptionsDto.ExchangeOption;
using WebServer.DTO.JSON.OptionsDto.TransportOption;

namespace WebServer.AutoMapperConfig
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<DeviceOption, DeviceOptionDto>().ReverseMap();
            CreateMap<ExchangeOption, ExchangeOptionDto>().ReverseMap();
            CreateMap<SerialOption, SerialOptionDto>().ReverseMap();
            CreateMap<TcpIpOption, TcpIpOptionDto>().ReverseMap();
            CreateMap<HttpOption, HttpOptionDto>().ReverseMap();
            CreateMap<TransportOption, TransportOptionsDto>().ReverseMap();

            // CreateMap<User, UserDto>();
        }
    }




}