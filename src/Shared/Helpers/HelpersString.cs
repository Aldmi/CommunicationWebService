using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NCalc;

namespace Shared.Helpers
{
    public static class HelpersString
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
                string res;
                var key = match.Groups[1].Value;
                if (dict.ContainsKey(key))
                {
                    var replacement = dict[key];
                    var formatValue = match.Groups[2].Value;
                    if (replacement is DateTime time)
                    {
                        res = DateTimeStrHandler(time, formatValue);
                    }
                    else
                    {
                        var format = "{0" + formatValue + "}";
                        res = string.Format(format, replacement);
                    }
                }
                else
                if (key.Contains("rowNumber"))
                {
                    var replacement = dict["rowNumber"];
                    var calcVal = CalculateMathematicFormat(key, (int)replacement);
                    var formatValue = match.Groups[2].Value;
                    var format = "{0" + formatValue + "}";
                    res = string.Format(format, calcVal);
                }
                else
                {
                    res = match.Value;
                }
                return res;
            }

            var result = Regex.Replace(template, pattern, Evaluator);
            return result;
        }


        /// <summary>
        /// Обработчик времени по формату
        /// </summary>
        private static string DateTimeStrHandler(DateTime val, string formatValue)
        {
            const string defaultStr = " ";
            if (val == DateTime.MinValue)
                return defaultStr;

            object resVal;
            if (formatValue.Contains("Sec")) //формат задан в секундах
            {
                resVal = (val.Hour * 3600 + val.Minute * 60);
                formatValue = Regex.Match(formatValue, @"\((.*)\)").Groups[1].Value;
            }
            else
            if (formatValue.Contains("Min")) //формат задан в минутах
            {
                resVal = (val.Hour * 60 + val.Minute);
                formatValue = Regex.Match(formatValue, @"\((.*)\)").Groups[1].Value;
            }
            else
            {
                resVal = val;
            }
            var format = "{0" + formatValue + "}";
            return string.Format(format, resVal);
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


        /// <summary>
        /// Разбиение строки на подстроки.
        /// </summary>
        /// <param name="text">строка</param>
        /// <param name="size">кол-во символов в подстроке</param>
        /// <returns></returns>
        public static IEnumerable<string> Split(this string text, int size) 
        {
            for (var i = 0; i < text.Length; i += size)
                yield return text.Substring(i, Math.Min(size, text.Length - i));
        }
    }
}