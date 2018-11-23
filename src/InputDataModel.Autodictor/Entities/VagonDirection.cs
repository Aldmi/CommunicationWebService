namespace InputDataModel.Autodictor.Entities
{
    public class VagonDirection : TrainBase
    {
        public int? Num { get; set; }                            //Нумерация вагонов в цифровом виде


        #region ctor

        public VagonDirection(string numStr)
        {
            var res= int.TryParse(numStr, out int num);
            if (!res) return;
            Num = num;
            switch (Num.Value)
            {
                case 0:
                    NameRu = "С головы";
                    NameAliasRu = "гол.";
                    break;

                case 1:
                    NameRu = "С хвоста";
                    NameAliasRu = "хв.";
                    break;
            }
        }


        public VagonDirection(int num)
        {
            Num = num;
            switch (Num.Value)
            {
                case 0:
                    NameRu = "С головы";
                    NameAliasRu = "гол.";
                    break;

                case 1:
                    NameRu = "С хвоста";
                    NameAliasRu = "хв.";
                    break;
            }
        }

        public VagonDirection()
        {
        }

        #endregion
    }
}