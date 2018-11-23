namespace InputDataModel.Autodictor.Entities
{
    public class TypeTrain : TrainBase
    {
        public int? Num { get; set; }                            //Тип поезда в цифровом виде


        #region ctor

        public TypeTrain(int num)
        {
            Num = num;
            switch (Num.Value)
            {
                case 0:
                    NameRu = "Скорый";
                    NameAliasRu = "Скор.";
                    break;

                case 1:
                    NameRu = "Ласточка";
                    NameAliasRu = "ласт.";
                    break;

                case 2:
                    //TODO: добавить типы
                    break;

                case 3:
                    break;

                case 4:
                    break;
            }
        }

        public TypeTrain()
        {
        }

        #endregion
    }
}