namespace InputDataModel.Autodictor.Entities
{
    /// <summary>
    ///    None = 0,               // Нет аварий
    ///    Canceled = 1,           // ОТМЕНЕН
    ///    DelayedArrival = 2,     // Задержка приб.
    ///    DelayedDeparture = 4,   // Задержка отпр.
    ///    TrainDeparture = 8,     // Отправка по готовности
    ///    DelayedLanding = 16     // Задержка под посадку
    /// </summary>
    public class EmergencySituation : TrainBase
    {
        public int? Num { get; set; }                            //нештатка цифровом виде

        #region ctor

        public EmergencySituation(int num)
        {
            Num = num;
            switch (Num.Value)
            {
                case 0:
                    NameRu = "Нет аварий";
                    NameAliasRu = "НЕТ";
                    NameEng = "None";
                    break;

                case 1:
                    NameRu = "Отменен";
                    NameAliasRu = "Отм.";
                    NameEng = "Canceled";
                    break;

                case 2:
                    NameRu = "Задержка приб.";
                    NameAliasRu = "Зад. приб.";
                    NameEng = "Delayed Arrival";
                    break;

                case 4:
                    NameRu = "Задержка отпр.";
                    NameAliasRu = "Зад. отпр.";
                    NameEng = "Delayed Departure";
                    break;

                case 8:
                    NameRu = "Отправка по готовности";
                    NameAliasRu = "Отпр. по готов";
                    NameEng = "Train Departure";
                    break;

                case 16:
                    NameRu = "Задержка под посадку";
                    NameAliasRu = "Зад. пос.";
                    NameEng = "Delayed Landing";
                    break;
            }
        }

        public EmergencySituation()
        {
        }

        #endregion
    }
}