using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.EFCore.Entities.Device
{
    public class EfDeviceOption
    {
        [Key]
        public int Id { get; set; }

        public int DeviceId { get; set; }                

        [Required]
        [MaxLength(256)]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }
        public bool AutoBuild { get; set; }                         //Автоматичекое создание Deivice на базе DeviceOption, при запуске сервиса.
        public bool AutoStart { get; set; }                         //Автоматичекий запук Deivice в работу (после AutoBuild), при запуске сервиса.


        private string _exchangeKeys;
        [NotMapped]
        public string[] ExchangeKeys
        {
            get => _exchangeKeys.Split(';');
            set => _exchangeKeys = string.Join($"{';'}", value);
        }
    }
}