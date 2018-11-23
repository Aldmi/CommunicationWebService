using System.Collections.Generic;
using System.Xml.Serialization;

namespace WebServer.DTO.XML
{

    [XmlRoot("tlist")]
    public class AdInputType4XmlDtoContainer
    {
        [XmlElement("t")]
        public List<AdInputType4XmlDto> Trains { get; set; } = new List<AdInputType4XmlDto>();
    }


    public class AdInputType4XmlDto
    {
        public int Id { get; set; }
        public int  ScheduleId { get; set; }                             //ID поезда в годовом расписании (из ЦИС)
        public int TrnId { get; set; }                                   //уникальный ID отправления поезда из ЦИС
        public string TrainNumber { get; set; }                          //номер поезда в формате Num1/Num2 если 2 номера и Num1 если 1 номер
        public int TrainType { get; set; }                               //Тип поезда в цифровом виде
        public string DirectionStation  { get; set; }                    // направление движения (Тверское, Ленинградское и т.д.)
        public string StartStation { get; set; }                         //начальная станция
        public string EndStation { get; set; }                           //конечная станция
        public string StartStationENG { get; set; }                      //начальная станция
        public string EndStationENG { get; set; }                        //конечная станция
        public string StartStationCH { get; set; }                       //начальная станция
        public string EndStationCH { get; set; }                         //конечная станция
        public string  WhereFrom { get; set; }                           //ближайшая станция после текущей
        public string  WhereTo { get; set; }                             //ближайшая станция после текущей
        public string  RecDateTime { get; set; }                         //время прибытия (parse -> DateTime)
        public string  SndDateTime { get; set; }                         //время отправления (parse -> DateTime)
        public string  EvRecTime { get; set; }                          //время прибытия (как RecDateTime) (parse -> DateTime)
        public string  EvSndTime { get; set; }                        //время отправления (как SndDateTime) (parse -> DateTime)
        public string LateTime { get; set; }                          //Время задержки в минутах  (parse -> int)
        public string HereDateTime { get; set; }                      //Время стоянки в минутах (parse -> TimeSpan)
        public string ExpectedTime { get; set; }                      //Ожидаемое время в минутах (parse -> int)
        public string ExpectedDateTime { get; set; }                  //Ожидаемое время (Время + Время задержки) (parse -> DateTime)
        public string DaysOfGoing  { get; set; }                       //Дни следования
        public string DaysOfGoingAlias  { get; set; }                  //Дни следования алиас
        public string DaysOfGoingAliasEng  { get; set; }               //Дни следования алиас
        public string TrackNumber { get; set; }                        //Путь   
        public string TrackNumberWithoutAutoReset { get; set; }        //Путь до автосброса
        public string Platform { get; set; }                           //платформа
        public int Direction  { get; set; }                           //0,1 - прибытие, отправление
        public string EvTrackNumber { get; set; }                     //Путь (как TrackNumber)
        //<State>0</State>
        public string VagonDirection { get; set; }                    //Нумерация вагонов в поезде. 0,1,2 - не задано, нумерация с головы, с хвоста
        //<Enabled>1</Enabled>
        public string EmergencySituation { get; set; }                //Нештатки
        public string TypeName { get; set; }                          //Тип поезда СТРОКА
        public string TypeAlias { get; set; }                         //Тип поезда СТРОКА алиас
        public string Addition { get; set; }                         //Дополнение
        public string AdditionEng { get; set; }                      //Дополнение ENG
        public string Note { get; set; }                            //Остановки
        public string NoteEng { get; set; }                         //Остановки ENG
        public int KolVag { get; set; }                             //количество вагонов в повагонке, по умолчанию 0
        public int KolLok { get; set; }                             //кол-во локомотивов, по умолчанию 0
        public int UslDlPoezd { get; set; }                         //длина поезда, по умолчанию 0
        //<Vagons>
        //<v/>
        //</Vagons>
        public int UslDlPerrona { get; set; }                      //длина поезда, по умолчанию 0
        public string PlatWhereFrom { get; set; }
        public string PlatWhereTo { get; set; }
        //<Sectors>
        //<sector/>
        //</Sectors>
        public string TimetableType { get; set; }
    }

}