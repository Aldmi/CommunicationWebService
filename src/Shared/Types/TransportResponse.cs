using System.Collections.Generic;
using Shared.Enums;

namespace Shared.Types
{

    public class TransportResponse
    {
        public TransportResponseStatus TransportResponseStatus { get; set; }
        public string Message { get; set; }     //Доп. информация
        public byte[] RowData { get; set; }    //ответ от устройства
        public string Encoding { get; set; }   //кодировка ответа       
                
        public Dictionary<string, dynamic> DataBag { get; set; }     //Не типизированный контейнер для передачи любых данных
    }
}