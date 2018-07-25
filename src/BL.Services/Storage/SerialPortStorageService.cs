using Shared.Types;
using Transport.SerialPort.Abstract;

namespace BL.Services.Storage
{
    /// <summary>
    /// Сервис содержит словарь портов и словарь бекграунда для них. Ключ словаря - TransportType
    /// Сервис предоставдяет методы для добавления/удаления портов и связанного с ним бекграунда, также запуск/останов бекграунда
    /// </summary>
    public class SerialPortStorageService : StorageServiceBase<KeyTransport, ISerailPort>
    {

    }
}