namespace Shared.Enums
{
    public enum TransportResponseStatus
    {
        Ok,                      //успешно
        Timeout,                 //Время на ответ истекло
        NotImplemented,          //Обработчик не поддерживается
        Error                    //Ошибка
    }
}