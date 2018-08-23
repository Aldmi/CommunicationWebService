using System;
using AutoMapper;
using DAL.Abstract.Entities.Options.Transport;
using DAL.EFCore.Entities;

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
                });
            Mapper = config.CreateMapper();
        }
    }
}