using System;
using System.Collections.Generic;
using Shared.Enums;

namespace Exchange.Base.Model
{
    public class OutResponseDataWrapper
    {  
        public Exception ExceptionExchangePipline { get; set; }
        public List<ResponseDataItem> ResponsesItems { get; set; } = new List<ResponseDataItem>();
               
        public Dictionary<string, dynamic> DataBag { get; set; }     //Не типизированный контейнер для передачи любых данных
    }


    public class ResponseDataItem
    {
        public string RequestId { get; set; }
        public string Message { get; set; }                   //Доп. информация
        public StatusDataExchange Status { get; set; }


        public string RequestData { get; set; }               //данные запроса
        public Exception TransportException { get; set; }     //Ошибка передачи данных

        //TODO: можно в DataBag записать и запрос и ответ, словарь должен ериализоваться верно в json

        public string ResponseData { get; set; }              //ответ от устройства
        public string Encoding { get; set; }                  //кодировка ответа    
        public bool IsOutDataValid { get; set; }    
    }
}