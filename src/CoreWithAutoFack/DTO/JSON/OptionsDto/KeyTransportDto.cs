using System.ComponentModel.DataAnnotations;

namespace WebServer.DTO.JSON.OptionsDto
{
    public class KeyTransportDto
    {
        #region prop

        [Required(ErrorMessage = "KeyTransport.Key не может быть NULL")]
        public string Key { get; set; }

        [Required(ErrorMessage = "KeyTransport.TransportType не может быть NULL")]
        public string TransportType { get; set; }

        #endregion
    }
}