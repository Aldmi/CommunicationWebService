﻿using System;
using System.Collections.Generic;
using InputDataModel.Autodictor.Entities;

namespace InputDataModel.Autodictor.Model
{
    public enum TypeTrain
    {
        None,                    //НеОпределен
        Passenger,               //Пассажирский
        Suburban,                //Пригородный
        Corporate,               //Фирменный
        Express,                 //Скорый
        HighSpeed,               //Скоростной
        Swallow,                 //Ласточка
        Rex,                     //РЭКС
    }

    public enum VagonDirection { None, FromTheHead, FromTheTail }


    public class AdInputType
    {
        public int Id { get; set; }
        public TypeTrain TypeTrain { get; set; }                     //тип поезда
        public VagonDirection VagonDirection { get; set; }           //Нумерация вагона (с головы, с хвоста)
        public string NumberOfTrain { get; set; }                    //Номер поезда
        public string PathNumber { get; set; }                       //Номер пути
        public string Event { get; set; }                            //Событие (ОТПР./ПРИБ./СТОЯНКА)
        public string Addition { get; set; }                         //Дополнение (свободная строка)
        public string DirectionStation { get; set; }                 //Направление.
        public Station StationDeparture { get; set; }
        public Station StationArrival { get; set; }
        public string Note { get; set; }                             //Примечание.
        public string DaysFollowing { get; set; }                    //Дни следования
        public string DaysFollowingAlias { get; set; }               //Дни следования, заданные в строке в нужном формате
        public DateTime? DelayTime { get; set; }                     //Время задержки (прибытия или отправления поезда)
        public DateTime ExpectedTime { get; set; }                   //Ожидаемое время (Время + Время задержки)
        public TimeSpan? StopTime { get; set; }                      //время стоянки (для транзитов: Время отпр - время приб)
    }
}