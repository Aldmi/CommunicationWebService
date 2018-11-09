using System;
using System.Collections.Generic;
using InputDataModel.Base;
using Shared.Enums;

namespace Exchange.Base.Model
{
    /// <summary>
    /// Ответ от устройства на отправку порции данных.
    /// Содержит общую инфу и коллекцию ответов на каждый запрос ResponseDataItem
    /// </summary>
    public class ResponsePieceOfDataWrapper<TIn>
    {
        public string DeviceName { get; set; }                     //Название ус-ва
        public string KeyExchange { get; set; }                    //Ключ обмена
        public DataAction DataAction { get; set; }                 //Действие

        public Exception ExceptionExchangePipline { get; set; }    //Критическая Ошибка обработки данных в конвеере.
        public string Message { get; set; }                        //Доп. информация
        public List<ResponseDataItem<TIn>> ResponsesItems { get; set; } = new List<ResponseDataItem<TIn>>();
               
        public Dictionary<string, dynamic> DataBag { get; set; }     //Не типизированный контейнер для передачи любых данных
    }


    /// <summary>
    /// Единица ответа от устройства на единицу запроса
    /// </summary>
    public class ResponseDataItem<TIn>
    {
        public string RequestId { get; set; }
        public string Message { get; set; }                   //Доп. информация
        public StatusDataExchange Status { get; set; }
        public string StatusStr => Status.ToString();

        public InDataWrapper<TIn> RequestData { get; set; }    //Данные запроса (в сыром виде)
        public Exception TransportException { get; set; }      //Ошибка передачи данных

        public string ResponseData { get; set; }              //Ответ от устройства
        public string Encoding { get; set; }                  //Кодировка ответа    
        public bool IsOutDataValid { get; set; }              //Флаг валидности ответа
    }
}