using System;
using AutoMapper;
using DAL.Abstract.Entities.Options.Device;
using DAL.Abstract.Entities.Options.Exchange;
using DAL.Abstract.Entities.Options.Transport;
using InputDataModel.Autodictor.Entities;
using InputDataModel.Autodictor.Model;
using WebServer.DTO.JSON.OptionsDto.DeviceOption;
using WebServer.DTO.JSON.OptionsDto.ExchangeOption;
using WebServer.DTO.JSON.OptionsDto.TransportOption;
using WebServer.DTO.XML;


namespace WebServer.AutoMapperConfig
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //Option mapping-----------------------------------------------------------------------------------------
            CreateMap<DeviceOption, DeviceOptionDto>().ReverseMap();
            CreateMap<ExchangeOption, ExchangeOptionDto>().ReverseMap();
            CreateMap<SerialOption, SerialOptionDto>().ReverseMap();
            CreateMap<TcpIpOption, TcpIpOptionDto>().ReverseMap();
            CreateMap<HttpOption, HttpOptionDto>().ReverseMap();
            CreateMap<TransportOption, TransportOptionsDto>().ReverseMap();

            //AdInputType xml in Data mapping-------------------------------------------------------------------------
            //CreateMap<AdInputType4XmlDto, AdInputType>()
            //    .ForMember(dest => dest.ScheduleId, opt => opt.MapFrom(src => src.ScheduleId))
            //    .ForMember(dest => dest.Lang, opt => opt.MapFrom(src => Lang.Ru))
            //    .ForMember(dest => dest.NumberOfTrain, opt => opt.MapFrom(src => src.TrainNumber))
            //    .ForMember(dest => dest.PathNumber, opt => opt.MapFrom(src => src.TrackNumber))
            //    .ForMember(dest => dest.Event, opt => opt.MapFrom(src => new EventTrain(src.Direction)))
            //    .ForMember(dest => dest.TrainType, opt => opt.MapFrom(src => new TypeTrain
            //    {
            //        NameRu = src.TypeName,
            //        NameAliasRu = src.TypeAlias,
            //        Num = src.TrainType
            //    }))
            //    .ForMember(dest => dest.VagonDirection, opt => opt.MapFrom(src => new VagonDirection(src.VagonDirection)))
            //    .ForMember(dest => dest.StationArrival, opt => opt.MapFrom(src => new Station
            //    {
            //        NameRu = src.StartStation,
            //        NameEng = src.StartStationENG
            //    }))
            //    .ForMember(dest => dest.StationDeparture, opt => opt.MapFrom(src => new Station
            //    {
            //        NameRu = src.EndStation,
            //        NameEng = src.EndStationENG
            //    }))
            //    .ForMember(dest => dest.StationWhereFrom, opt => opt.MapFrom(src => new Station
            //    {
            //        NameRu = src.WhereFrom
            //    }))
            //    .ForMember(dest => dest.StationWhereTo, opt => opt.MapFrom(src => new Station
            //    {
            //        NameRu = src.WhereTo
            //    }))
            //    .ForMember(dest => dest.DirectionStation, opt => opt.MapFrom(src => new DirectionStation
            //    {
            //        NameRu = src.DirectionStation
            //    }))
            //    .ForMember(dest => dest.ArrivalTime, opt => opt.MapFrom(src => ConvertString2DataTime(src.RecDateTime)))
            //    .ForMember(dest => dest.DepartureTime, opt => opt.MapFrom(src => ConvertString2DataTime(src.SndDateTime)))
            //    .ForMember(dest => dest.DelayTime, opt => opt.MapFrom(src => ConvertString2DataTimeMinute(src.LateTime)))
            //    .ForMember(dest => dest.ExpectedTime, opt => opt.MapFrom(src => ConvertString2DataTimeMinute(src.ExpectedTime) ?? DateTime.MinValue))
            //    .ForMember(dest => dest.StopTime, opt => opt.MapFrom(src => ConvertString2TimeSpan(src.HereDateTime)))
            //    .ForMember(dest => dest.Addition, opt => opt.MapFrom(src => new Addition
            //    {
            //        NameRu = src.Addition,
            //        NameEng = src.AdditionEng
            //    }))
            //    .ForMember(dest => dest.Note, opt => opt.MapFrom(src => new Note
            //    {
            //        NameRu = src.Note,
            //        NameEng = src.NoteEng
            //    }))
            //    .ForMember(dest => dest.DaysFollowing, opt => opt.MapFrom(src => new DaysFollowing
            //    {
            //        NameRu = src.DaysOfGoing,
            //        NameAliasRu = src.DaysOfGoingAlias,
            //        NameAliasEng = src.DaysOfGoingAliasEng
            //    }));
        }


        public DateTime? ConvertString2DataTime(string str)
        {
            if (DateTime.TryParse(str, out DateTime val))
            {
                return val;
            }
            return null;
        }


        public DateTime? ConvertString2DataTimeMinute(string minute)
        {
            if (int.TryParse(minute, out var val))
            {
                var minuteRes = new DateTime(1998, 04, 30, 0, val, 0);
                return new DateTime();
            }
            return null;
        }


        public TimeSpan? ConvertString2TimeSpan(string str)
        {
            if (TimeSpan.TryParse(str, out var val))
            {
                return val;
            }
            return null;
        }
    }




}