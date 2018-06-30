using Shared.Enums;

namespace Exchange.Base
{
    public interface IExhangeBehavior
    {
        TypeExchange TypeExchange { get; }
        void StartCycleExchange();
        void StopCycleExchange();

    }
}