namespace Shared.Enums
{
    /// <summary>
    /// статус ПОСЛЕДНЕГО обмена данными
    /// </summary>
    public enum StatusDataExchange
    {
        None,
        Start,
        Process,
        End,
        EndWithTimeout,
        EndWithCanceled,
        EndWithError,
        EndWithTimeoutCritical,
        EndWithErrorCritical
    }
}