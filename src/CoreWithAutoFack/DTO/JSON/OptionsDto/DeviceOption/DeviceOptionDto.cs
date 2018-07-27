﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebServer.DTO.JSON.OptionsDto.DeviceOption
{
    public class DeviceOptionDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Укажите название устройства")]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Список ключей ExchangeKeys не может быть пуст")]
        public List<string> ExchangeKeys { get; set; }
    }
}