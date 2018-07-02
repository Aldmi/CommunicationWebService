namespace Shared.Enums
{
    /// <summary>
    /// статус обмена данными
    /// </summary>
    public enum StatusDataExchange
    {
        None,
        Start,
        Process,
        End,
        EndWithTimeout,
        EndWithCanceled,
        EndWithError
    }
}