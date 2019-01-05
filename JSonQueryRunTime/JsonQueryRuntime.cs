using System;
using System.Linq;
using System.Collections.Generic;
using HiSystems.Interpreter;
using JsonQueryRunTime;
using Newtonsoft.Json.Linq;

namespace JsonQueryRunTimeNS
{
    public class JsonQueryRuntime
    {
        /// <summary>
        /// Parsed expression, ready to execute
        /// </summary>
        private Expression _expression;

        /// <summary>
        /// The HiSystems.Interpreter engine
        /// </summary>
        private Engine _engineSingleton;

        /// <summary>
        /// Contain the current JSON parsed object in process
        /// that can be access by function Parse()
        /// </summary>
        public static JObject _currentJsonObject;

        /// <summary>
        /// Make a current instance of JsonQueryRuntime available
        /// used by custom function Var()
        /// </summary>
        public static JsonQueryRuntime SingletonInstance;

        public JsonQueryRuntime(string whereClause)
        {
            // Custom function Var() need access to the instance
            JsonQueryRuntime.SingletonInstance = this;

            _engineSingleton = new Engine();
            _engineSingleton.Register(new fxWildCard());
            _engineSingleton.Register(new fxRange());
            _engineSingleton.Register(new fxIn());
            _engineSingleton.Register(new fxContains());
            _engineSingleton.Register(new fxIsString());
            _engineSingleton.Register(new fxIsObject());
            _engineSingleton.Register(new fxPath());
            _engineSingleton.Register(new fxRegex());
            _engineSingleton.Register(new fxIsNull());
            _engineSingleton.Register(new fxNot());
            _engineSingleton.Register(new fxEqualArray());
            _engineSingleton.Register(new fxIsNumber());
            _engineSingleton.Register(new fxIsDate());
            _engineSingleton.Register(new fxIsBoolean());
            _engineSingleton.Register(new fxIsArray());
            _engineSingleton.Register(new fxWriteLine());
            _engineSingleton.Register(new fxVar());

            this._expression = _engineSingleton.Parse(whereClause.Replace(Environment.NewLine, ""));
        }

        /// <summary>
        /// Return one where clause which is the combination of all passed boolean expressions
        /// </summary>
        /// <param name="expressions"></param>
        /// <returns></returns>
        public static string CombineWhereClauseExpressions(IEnumerable<string> expressions, string booleanOperators = "AND")
        {
            // Create on giant where clause based on all above where clauses
            var allString = new System.Text.StringBuilder(4096);
            foreach (var expression in expressions)
                allString.Append($"( {expression} ) {booleanOperators} ").AppendLine();
            allString.Append($"1=1 ");
            return allString.ToString();
        }

        /// <summary>
        /// Apply the where clause to list of JSON object defined in the file
        /// </summary>
        /// <param name="fileName">The name of the JSON file</param>
        /// <param name="isJsonLine">If true the file contains JSON-LINES else the file must contain an array of JSON objects</param>
        /// <returns>The list of JSON string that match the where clause</returns>
        public IEnumerable<string> ExecuteFile(string fileName, bool isJsonLine)
        {
            var json = System.IO.File.ReadAllText(fileName);
            if (isJsonLine)
            {
                return this.Execute(json.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries));
            }
            else
            {
                // Expect a JSON array of object
                if (json.Trim().StartsWith("["))
                {
                    var l = new List<string>();
                    JArray a = JArray.Parse(json);
                    foreach (JObject jObject in a)
                        if (this.Execute(jObject))
                            l.Add(jObject.ToString());
                    return l;
                }
                else throw new ArgumentException($"{fileName} does not contains an JSON array of object and is not a JSON-LINE file");
            }
        }

        /// <summary>
        /// Apply the where clause to list of JSON strings
        /// </summary>
        /// <param name="jsonStrings">A list of JSON string</param>
        /// <returns>The list of JSON string that match the where clause</returns>
        public IEnumerable<string> Execute(IEnumerable<string> jsonStrings)
        {
            var l = new List<string>();
            foreach (var jsonString in jsonStrings)
                if (this.Execute(jsonString))
                    l.Add(jsonString);
            return l;
        }
        /// <summary>
        /// Apply the where clause to list of JSON objects
        /// </summary>
        /// <param name="jObjects"></param>
        /// <returns>The list of JSON string that match the where clause</returns>
        public IEnumerable<string> Execute(List<JObject> jObjects)
        {
            var l = new List<string>();
            foreach (var jo in jObjects)
                if (this.Execute(jo))
                    l.Add(jo.ToString());
            return l;
        }

        /// <summary>
        /// Apply the where clause to the JSON string
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns>true if the where clause apply to the JSON string</returns>
        public bool Execute(string jsonString)
        {
            return this.Execute(JObject.Parse(jsonString));
        }

        /// <summary>
        /// Apply the where clause to the JSON object
        /// </summary>
        /// <param name="o"></param>
        /// <returns> true if the where clause apply to the JSON object</returns>
        public bool Execute(JObject o)
        {
            JsonQueryRuntime._currentJsonObject = o;

            this.SetVariables();
            this.SetVariablesDefinedAsJsonNestedPath();

            return this._expression.Execute<HiSystems.Interpreter.Boolean>();
        }

        /// <summary>
        /// Set the variables of the interpreter with the values extracted from the JSON object
        /// The variable name are composed of nested JSON name, for example a.b.c
        /// </summary>
        private void SetVariablesDefinedAsJsonNestedPath()
        {
            // Look for variables name which are json path
            foreach (var v in this._expression.Variables)
            {
                if (v.Key.Contains("."))
                {
                    // Evaluate the expression using JSON.NET Path Api
                    JToken lastValue = fxUtils.EvalJsonDotNetPath("$." + v.Key, JsonQueryRuntime._currentJsonObject);
                    this._expression.Variables[v.Key].Value = fxUtils.ResolveValueFromJToken(lastValue);
                }
            }
        }

        /// <summary>
        /// Set the variables of the interpreter with the values extracted from the JSON object
        /// </summary>
        private void SetVariables()
        {
            foreach (JProperty prop in JsonQueryRuntime._currentJsonObject.Properties())
            {
                var n = prop.Name;
                if (this._expression.Variables.ContainsKey(n))
                {
                    var v = prop.Value;
                    switch (v.Type)
                    {
                        case JTokenType.String:
                            this._expression.Variables[n].Value = new HiSystems.Interpreter.Text(v.ToString());
                            break;
                        case JTokenType.Integer:
                        case JTokenType.Float:
                            this._expression.Variables[n].Value = new HiSystems.Interpreter.Number((Decimal)v);
                            break;
                        case JTokenType.Boolean:
                            this._expression.Variables[n].Value = new HiSystems.Interpreter.Boolean((bool)v);
                            break;
                        case JTokenType.Date:
                            this._expression.Variables[n].Value = new HiSystems.Interpreter.DateTime((System.DateTime)v);
                            break;
                        case JTokenType.Null:
                            this._expression.Variables[n].Value = new HiSystems.Interpreter.Null(null);
                            break;
                        case JTokenType.Object:
                            // Pass the sub object are the JSON represenation
                            this._expression.Variables[n].Value = new HiSystems.Interpreter.Text(v.ToString(Newtonsoft.Json.Formatting.None));
                            break;
                        // Assume that array contains the same item type Number, String or Boolean for all items
                        // Can only be used with Contains() and EqualArray()
                        case JTokenType.Array:
                            JArray a = prop.Value as JArray;
                            if (a.Count == 0)
                            {
                                // Remark: I think empty HiSystems.Interpreter.Array throw an exception
                                this._expression.Variables[n].Value = new HiSystems.Interpreter.Array(new List<decimal>().ToArray());
                            }
                            else
                            {
                                var isArrayOfTypeNumeric = a[0].Type == JTokenType.Float || a[0].Type == JTokenType.Integer;
                                var isArrayOfTypeBooleam = a[0].Type == JTokenType.Boolean;
                                if (isArrayOfTypeNumeric)
                                {
                                    var decimalList = a.Children().Select( d => new HiSystems.Interpreter.Number(d.Value<decimal>() ) );
                                    this._expression.Variables[n].Value = new HiSystems.Interpreter.Array(decimalList.ToArray());
                                }
                                else if (isArrayOfTypeBooleam)
                                {
                                    var boolList = a.Children().Select( d => new HiSystems.Interpreter.Boolean(d.Value<bool>() ) );
                                    this._expression.Variables[n].Value = new HiSystems.Interpreter.Array(boolList.ToArray());
                                }
                                else // Fall back on text/string type
                                {
                                    var stringList = a.Children().Select( d => new HiSystems.Interpreter.Text(d.ToString() ) );
                                    this._expression.Variables[n].Value = new HiSystems.Interpreter.Array(stringList.ToArray());
                                }
                            }
                            break;
                    }
                }
            }
        }
        public void AddUpdateVariable(string name, Literal value)
        {
            if (this._expression.Variables.ContainsKey(name))
                this._expression.Variables[name].Value = value;
            else
                this._expression.Variables.Add(name, new Variable(name, value));
        }
    }
}
