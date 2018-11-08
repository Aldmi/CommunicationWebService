namespace InputDataModel.Autodictor.Entities
{
    public abstract class TrainBase 
    {
        #region prop

        public string NameRu { get; set; }                     
        public string NameAliasRu { get; set; }               

        public string NameEng { get; set; }                    
        public string NameAliasEng { get; set; }  

        #endregion




        #region Methode

        public string GetName(Lang lang)
        {
            switch (lang)
            {
                case Lang.Ru:  return NameRu;
                case Lang.Eng: return NameEng;
                case Lang.Fin:
                case Lang.Ch:
                default: return string.Empty;
            }
        }

        public string GetNameAlias(Lang lang)
        {
            switch (lang)
            {
                case Lang.Ru:  return NameAliasRu;
                case Lang.Eng: return NameAliasEng;
                case Lang.Fin:
                case Lang.Ch:
                default: return string.Empty;
            }
        }

        #endregion
    }
}