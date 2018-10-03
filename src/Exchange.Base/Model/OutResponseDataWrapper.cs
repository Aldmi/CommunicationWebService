using System;
using System.Collections.Generic;
using Shared.Enums;

namespace Exchange.Base.Model
{
    public class OutResponseDataWrapper<TIn>
    {  
        public Exception ExceptionExchangePipline { get; set; }
        public string Message { get; set; }                    //Доп. информация
        public List<ResponseDataItem<TIn>> ResponsesItems { get; set; } = new List<ResponseDataItem<TIn>>();
               
        public Dictionary<string, dynamic> DataBag { get; set; }     //Не типизированный контейнер для передачи любых данных
    }


    public class ResponseDataItem<TIn>
    {
        public string RequestId { get; set; }
        public string Message { get; set; }                   //Доп. информация
        public StatusDataExchange Status { get; set; }

        public InDataWrapper<TIn> RequestData { get; set; }    //Данные запроса
        public Exception TransportException { get; set; }      //Ошибка передачи данных

        public string ResponseData { get; set; }              //Ответ от устройства
        public string Encoding { get; set; }                  //Кодировка ответа    
        public bool IsOutDataValid { get; set; }              //Флаг валидности ответа
    }
}