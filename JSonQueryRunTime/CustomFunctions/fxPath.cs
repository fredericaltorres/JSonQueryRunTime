using HiSystems.Interpreter;
using WildCardExercice.net;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using JSonQueryRunTimeNS;

namespace JSonQueryRunTime
{
    class fxPath : Function
    {
        public override string Name
        {
            get
            {
                return "Path";
            }
        }


        public static JTokenType ConvertInterpreterTypeIntoJTokenType(IConstruct l)
        {

            string expectedValueType = l.GetType().Name;
            if (expectedValueType == "Variable")
            {
                Variable v = l as Variable;
                return ConvertInterpreterTypeIntoJTokenType(v.Value);
            }

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

        public Literal ExecuteWithUnknown(IConstruct[] arguments)
        {
            base.EnsureArgumentCountIs(arguments, 2);
            try
            {
                string pathExpression = base.GetTransformedArgument<Text>(arguments, argumentIndex: 0);
                var jsonExpectedType = ConvertInterpreterTypeIntoJTokenType(arguments[1]);

                var currentPath = "";
                var possiblePaths = DerivePathWithUnknown(pathExpression, ref currentPath);

                foreach (var possiblePath in possiblePaths)
                {
                    JToken lastValue = EvalJsonDotNetPath(possiblePath);
                    if (lastValue.Type == jsonExpectedType || (lastValue.Type == JTokenType.Integer && jsonExpectedType == JTokenType.Float))
                    {
                        switch (jsonExpectedType)
                        {
                            case JTokenType.Float: return base.GetTransformedArgument<Number>(arguments, argumentIndex: 1) == (decimal)lastValue;
                            case JTokenType.String: return base.GetTransformedArgument<Text>(arguments, argumentIndex: 1) == (string)lastValue;
                            case JTokenType.Boolean: return base.GetTransformedArgument<Boolean>(arguments, argumentIndex: 1) == (bool)lastValue;
                            case JTokenType.Null: return base.GetTransformedArgument<Null>(arguments, argumentIndex: 1) == null;
                            case JTokenType.Date: return base.GetTransformedArgument<DateTime>(arguments, argumentIndex: 1) == new HiSystems.Interpreter.DateTime((System.DateTime)lastValue);
                        }
                    }
                }
                return new HiSystems.Interpreter.Boolean(false);
            }
            catch (System.InvalidOperationException ioEx)
            {
                return new HiSystems.Interpreter.Boolean(false);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public override Literal Execute(IConstruct[] arguments)
        {
            if (arguments.Length == 2) // Path() with pattern matching
                return ExecuteWithUnknown(arguments);

            base.EnsureArgumentCountIs(arguments, 1);
            try
            {
                string pathExpression = base.GetTransformedArgument<Text>(arguments, argumentIndex: 0);
                if(pathExpression == string.Empty) 
                    return new HiSystems.Interpreter.Null(null);

                // Evaluate the expression using JSON.NET Path Api
                JToken lastValue = EvalJsonDotNetPath("$." + pathExpression);

                //if(pathExpression.StartsWith("$"))
                //{
                //    lastValue = EvalJsonDotNetPath(pathExpression);
                //}
                //else
                //{
                //    // Eval
                //    lastValue = EvalPath(pathExpression);
                //}
                return ResolveValueFromJToken(lastValue);
            }
            catch (System.InvalidOperationException ioEx)
            {
                return new HiSystems.Interpreter.Boolean(false);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        private static Literal ResolveValueFromJToken(JToken lastValue)
        {

            if (lastValue == null)
                return new HiSystems.Interpreter.Null(null);

            if (lastValue.Type == JTokenType.Float || lastValue.Type == JTokenType.Integer)
                return new HiSystems.Interpreter.Number((decimal)lastValue);

            if (lastValue.Type == JTokenType.Boolean)
                return new HiSystems.Interpreter.Boolean((bool)lastValue);

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
