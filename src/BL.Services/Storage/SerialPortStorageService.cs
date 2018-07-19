using System.Collections.Generic;
using BL.Services.Storage;
using Shared.Enums;
using Shared.Types;
using Transport.SerialPort.Abstract;

namespace BL.Services
{
    /// <summary>
    /// Сервис содержит словарь портов и словарь бекграунда для них. Ключ словаря - TransportType
    /// Сервис предоставдяет методы для добавления/удаления портов и связанного с ним бекграунда, также запуск/останов бекграунда
    /// </summary>
    public class SerialPortStorageService : StorageService<ISerailPort>
    {

    }
}