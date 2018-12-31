using HiSystems.Interpreter;
using WildCardExercice.net;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using JSonQueryRunTimeNS;

namespace JSonQueryRunTime
{
    public class fxUtils
    {
        public static HiSystems.Interpreter.Array BuildStringArray(List<string> l) {
            var consList = new List<IConstruct>();
            foreach (var s in l)
                consList.Add(new HiSystems.Interpreter.Text(s.ToString()));
            return new HiSystems.Interpreter.Array(consList.ToArray());
        }
        public static HiSystems.Interpreter.Array BuildNumberArray(List<decimal> l) {
            var consList = new List<IConstruct>();
            foreach (var s in l)
                consList.Add(new HiSystems.Interpreter.Number(s));
            return new HiSystems.Interpreter.Array(consList.ToArray());
        }
        public static HiSystems.Interpreter.Array BuildBooleanArray(List<bool> l) {
            var consList = new List<IConstruct>();
            foreach (var s in l)
                consList.Add(new HiSystems.Interpreter.Boolean(s));
            return new HiSystems.Interpreter.Array(consList.ToArray());
        }
        public static HiSystems.Interpreter.Array BuildDateArray(List<DateTime> l) {
            var consList = new List<IConstruct>();
            foreach (var s in l)
                consList.Add(new HiSystems.Interpreter.DateTime(s));
            return new HiSystems.Interpreter.Array(consList.ToArray());
        }
        public static JTokenType ConvertInterpreterArrayTypeIntoJTokenType(IConstruct l)
        {
            string expectedValueType = l.GetType().Name;
            if (expectedValueType == "Variable")
            {
                Variable v = l as Variable;
                return ConvertInterpreterArrayTypeIntoJTokenType(v.Value);
            }
            else if (expectedValueType == "Array")
            {
                var a = (l as HiSystems.Interpreter.Array).ToList();
                if(a.Count > 0)
                {
                    return ConvertInterpreterTypeIntoJTokenType(a[0]);
                }
                else throw new System.ArgumentException($"ConvertInterpreterArrayTypeIntoJTokenType() cannot infer the type of the array because the array is empty, l:{l.ToString()} ");
            }
            else
                throw new System.ArgumentException($"ConvertInterpreterArrayTypeIntoJTokenType() requires an array as first parameter, received:${l.ToString()}");
        }

        public static JTokenType ConvertInterpreterTypeIntoJTokenType(IConstruct l)
        {

            string expectedValueType = l.GetType().Name;
            if (expectedValueType == "Variable")
            {
                Variable v = l as Variable;
                return ConvertInterpreterTypeIntoJTokenType(v.Value);
            }
            if (expectedValueType == "FunctionOperation")
                return ConvertInterpreterTypeIntoJTokenType(l.Transform());
            if (expectedValueType == "Array")
                return JTokenType.Array;
            if (expectedValueType == "Number")
                return JTokenType.Float;
            if (expectedValueType == "Text")
                return JTokenType.String;
            if (expectedValueType == "DateTime")
                return JTokenType.Date;
            if (expectedValueType == "Boolean")
                return JTokenType.Boolean;
            if (expectedValueType == "Null")
                return JTokenType.Null;
            if (expectedValueType == "Array")
                return JTokenType.Array;
            return JTokenType.Undefined;
        }

        public static Literal ResolveValueFromJToken(JToken lastValue)
        {

            if (lastValue == null)
                return new HiSystems.Interpreter.Null(null);

            if (lastValue.Type == JTokenType.Float || lastValue.Type == JTokenType.Integer)
                return new HiSystems.Interpreter.Number((decimal)lastValue);

            if (lastValue.Type == JTokenType.Boolean)
                return new HiSystems.Interpreter.Boolean((bool)lastValue);

            // Return object as string that start with a '{'
            if (lastValue.Type == JTokenType.Object)
                return new HiSystems.Interpreter.Text(lastValue.ToString());

            // As Default return as string
            return new HiSystems.Interpreter.Text(lastValue.ToString());
        }

        public static string RemoveFirstToken(string s, char tokenSeparator = '.')
        {
            var p = s.IndexOf(tokenSeparator);
            if (p == -1)
            {
                return s;
            }
            else
            {
                return s.Substring(p + 1);
            }
        }

        public static List<string> DerivePathWithUnknown(string pathExpression, ref string currentPath, JObject rootObj = null)
        {
            if (rootObj == null) // Start at with the top object
                rootObj = JsonQueryRuntime._currentJsonObject;

            var l = new List<string>();
            JToken lastValue = null;
            var tokens = pathExpression.Split('.');
            var token = tokens[0]; // Check there is at least one value
            if (token == "?")
            {
                List<JProperty> props = rootObj.Properties().Where(p => true).ToList();
                foreach (var prop in props)
                {
                    var dot = currentPath == "" ? "" : ".";
                    l.Add($"{currentPath}{dot}{prop.Name}");
                }
            }
            else
            {
                var prop = rootObj.Properties().FirstOrDefault(p => p.Name == token);
                currentPath += currentPath == "" ? $"{prop.Name}" : $".{prop.Name}";
                var newPathExpression = RemoveFirstToken(pathExpression);
                var nextObj = prop.Value as JObject;
                var ll = DerivePathWithUnknown(newPathExpression, ref currentPath, nextObj);
                l.AddRange(ll);
            }
            return l;
        }

        /// <summary>
        /// https://www.newtonsoft.com/json/help/html/QueryJsonSelectTokenJsonPath.htm
        /// </summary>
        /// <param name="pathExpression"></param>
        /// <returns></returns>
        public static JToken EvalJsonDotNetPath(string pathExpression)
        {
            JObject rootObj = JsonQueryRuntime._currentJsonObject;
            JToken r = rootObj.SelectToken(pathExpression);
            return r;
        }

        //public static JToken EvalPath(string pathExpression)
        //{
        //    var tokens = pathExpression.Split('.');
        //    JObject rootObj = JsonQueryRuntime._currentJsonObject;
        //    JToken lastValue = null;
        //    foreach (var token in tokens)
        //    {
        //        var prop = rootObj.Properties().FirstOrDefault(p => p.Name == token);
        //        if (prop == null)
        //        {
        //            return null;
        //        }
        //        else
        //        {
        //            var jToken = prop.Value;
        //            rootObj = jToken as JObject;
        //            lastValue = jToken;
        //        }
        //    }
        //    return lastValue;
        //}
    }
}
