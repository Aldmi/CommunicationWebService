namespace InputDataModel.Autodictor.Entities
{
    public class VagonDirection
    {
        public int? Num { get; set; }                            //Тип поезда в цифровом виде

        public string NameRu { get; set; }                      //Тип поезда RU
        public string NameAliasRu { get; set; }                 //Тип поезда RU алиас

        public string NameEng { get; set; }                      //Тип поезда ENG
        public string NameAliasEng { get; set; }                 //Тип поезда ENG алиас
    }
}