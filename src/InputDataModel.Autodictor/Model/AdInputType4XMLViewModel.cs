using System;
using InputDataModel.Autodictor.Entities;

namespace InputDataModel.Autodictor.Model
{
    public class AdInputType4XmlViewModel
    {
        public int Id { get; set; }
        public int  ScheduleId { get; set; }           //ID поезда в годовом расписании (из ЦИС)
        public int TrnId { get; set; }                 //уникальный ID отправления поезда из ЦИС

        public string TrainNumber { get; set; }        //номер поезда в формате Num1/Num2 если 2 номера и Num1 если 1 номер

        public int TrainType { get; set; }       //Тип поезда в цифровом виде
        public string TypeName { get; set; }           //Тип поезда СТРОКА
        public string TypeNameAlias { get; set; }      //Тип поезда СТРОКА алиас

        public string DirectionStation  { get; set; }                     // направление движения (Тверское, Ленинградское и т.д.)
        public Station StartStation { get; set; }                         //начальная станция
        public Station EndStation { get; set; }                           //конечная станция
        public string  WhereFrom { get; set; }                           //ближайшая станция после текущей
        public string  WhereTo { get; set; }                             //ближайшая станция после текущей

        public DateTime  RecDateTime { get; set; }                       //время прибытия
        public DateTime  SndDateTime { get; set; }                      //время отправления

        public DateTime? DelayDateTime { get; set; }                     //Время задержки (прибытия или отправления поезда)
        public int LateTime { get; set; }                                //Время задержки в минутах

        public DateTime ExpectedDateTime { get; set; }                   //Ожидаемое время (Время + Время задержки)
        public int ExpectedTime { get; set; }                           //Ожидаемое время в минутах

        public TimeSpan? StopTime { get; set; }                         //время стоянки (для транзитов: Время отпр - время приб)
        public int HereDateTime { get; set; }                           //Время стоянки в минутах


        public string DaysOfGoing  { get; set; }                       //Дни следования
        public string DaysOfGoingAlias  { get; set; }                  //Дни следования алиас
        public string DaysOfGoingAliasEng  { get; set; }               //Дни следования алиас

        public string TrackNumber { get; set; }                        //Путь   
        public string TrackNumberWithoutAutoReset { get; set; }        //Путь до автосброса

        public string Platform { get; set; }                           //платформа
        public int Direction  { get; set; }                     //0,1 - прибытие, отправление
        public VagonDirection VagonDirection { get; set; }            //Нумерация вагонов в поезде. 0,1,2 - не задано, нумерация с головы, с хвоста
        public EmergencySituation EmergencySituation { get; set; }    //Нештатки

        public string Addition { get; set; }                         //Дополнение
        public string AdditionEng { get; set; }                      //Дополнение ENG

        public string Note { get; set; }                            //Остановки
        public string NoteEng { get; set; }                         //Остановки ENG

        public int KolVag { get; set; }                             //количество вагонов в повагонке, по умолчанию 0
        public int KolLok { get; set; }                             //кол-во локомотивов, по умолчанию 0
        public int UslDlPoezd { get; set; }                         //длина поезда, по умолчанию 0
        //Vagons - вагоны в повагонке, пример надо будет поискать. Ну и сектора всякие.


        #region Refactoring
        public DateTime  EvRecTime { get; set; }    //время прибытия (как RecDateTime)
        public DateTime  EvSndTime { get; set; }    //время отправления (как SndDateTime)
        public string EvTrackNumber { get; set; }   //Путь (как TrackNumber)
        #endregion
    }

}