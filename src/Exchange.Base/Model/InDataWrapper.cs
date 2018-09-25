using System.Collections.Generic;
using InputDataModel.Base;

namespace Exchange.Base.Model
{
    /// <summary>
    /// Контейнер-оболочка над входными данными для обменов
    /// </summary>
    /// <typeparam name="TIn"></typeparam>
    public class InDataWrapper<TIn>
    {
        public List<TIn> Datas { get; set; }
        public Command4Device Command { get; set; }
    }
}