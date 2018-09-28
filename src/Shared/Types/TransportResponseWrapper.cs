using System;
using System.Collections.Generic;
using Shared.Enums;

namespace Shared.Types
{
    public class TransportResponseWrapper
    {  
        public Exception ExceptionExchangePipline { get; set; }
        public List<TransportResponse> TransportResponses { get; set; } = new List<TransportResponse>();
               
        public Dictionary<string, dynamic> DataBag { get; set; }     //Не типизированный контейнер для передачи любых данных
    }


    public class TransportResponse
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