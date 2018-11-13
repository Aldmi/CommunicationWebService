using System.Collections.Generic;
using System.Text.RegularExpressions;
using NCalc;

namespace Shared.Helpers
{
    public static class HelperString
    {
        /// <summary>
        /// Вставка переменных (по формату) в строку по шаблону. 
        /// </summary>
        /// <param name="template">базовая строка с местозаполнителем</param>
        /// <param name="dict">словарь переменных key= название переменной val= значение</param>
        /// <param name="pattern">Как выдедить переменную и ее формат, по умолчанию {val:format}</param>
        /// <returns></returns>
        public static string StringTemplateInsert(string template, Dictionary<string, object> dict, string pattern = @"\{(.*?)(:.+?)?\}")
        {
            string Evaluator(Match match)
            {
                var key = match.Groups[1].Value;
                if (dict.ContainsKey(key))
                {
                    var replacement = dict[key];
                    var formatValue = match.Groups[2].Value;
                    var format = "{0" + formatValue + "}";
                    return string.Format(format, replacement);
                }
                if (key.Contains("rowNumber"))
                {
                    var replacement = dict["rowNumber"];
                    var calcVal = CalculateMathematicFormat(key, (int)replacement);
                    var formatValue = match.Groups[2].Value;
                    var format = "{0" + formatValue + "}";
                    return string.Format(format, calcVal);
                }
                return match.Value;
            }

            var result = Regex.Replace(template, pattern, Evaluator);
            return result;
        }

        /// <summary>
        /// Математическое вычисление формулы с участием переменной rowNumber
        /// </summary>
        private static int CalculateMathematicFormat(string str, int row)
        {
            var expr = new Expression(str)
            {
                Parameters = { ["rowNumber"] = row }
            };
            var func = expr.ToLambda<int>();
            var arithmeticResult = func();
            return arithmeticResult;
        }
    }
}