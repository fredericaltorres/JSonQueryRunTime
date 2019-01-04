using System;
using System.Collections.Generic;
using HiSystems.Interpreter;
using JsonQueryRunTime;
using Newtonsoft.Json.Linq;

namespace JsonQueryRunTimeNS
{
    public class JsonQueryRuntime
    {
        private Expression _expression;
        private Engine _engine;

        /// <summary>
        /// Contain the current JSON parsed object in process
        /// that can be access by function Parse()
        /// </summary>
        public static JObject _currentJsonObject;

        public JsonQueryRuntime(string whereClause)
        {
            _engine = new Engine();
            this._engine.Register(new fxWildCard());
            this._engine.Register(new fxRange());
            this._engine.Register(new fxIn());
            this._engine.Register(new fxIsString());
            this._engine.Register(new fxContains());
            this._engine.Register(new fxIsObject());
            this._engine.Register(new fxPath());
            this._engine.Register(new fxRegex());
            this._engine.Register(new fxIsNull());
            this._engine.Register(new fxNot());
            this._engine.Register(new fxEqualArray());
            this._engine.Register(new fxIsNumber());
            this._engine.Register(new fxIsDate());
            this._engine.Register(new fxIsBoolean());
            this._engine.Register(new fxIsArray());

            this._expression = this._engine.Parse(whereClause.Replace(Environment.NewLine, ""));
        }

        /// <summary>
        /// Return one where clause which is the combination of all passed boolean expressions
        /// </summary>
        /// <param name="expressions"></param>
        /// <returns></returns>
        public static string CombineWhereClauseExpressions(IEnumerable<string> expressions, string booleanOperators = "AND") {
            
          // Create on giant where clause based on all above where clauses
            var allString = new System.Text.StringBuilder(4096);
            foreach(var expression in expressions)
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
            var json  = System.IO.File.ReadAllText(fileName);
            if(isJsonLine)
            {
                return this.Execute(json.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries));
            }
            else
            {
                // expect a JSON array of object
                if(json.Trim().StartsWith("["))
                {
                    var l = new List<string>();
                    JArray a = JArray.Parse(json);
                    foreach(JObject jObject in a)
                        if(this.Execute(jObject))
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
            _currentJsonObject = o;

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
                    JToken lastValue = fxUtils.EvalJsonDotNetPath("$." + v.Key, _currentJsonObject);
                    this._expression.Variables[v.Key].Value = fxUtils.ResolveValueFromJToken(lastValue);
                }
            }
        }

        /// <summary>
        /// Set the variables of the interpreter with the values extracted from the JSON object
        /// </summary>
        private void SetVariables()
        {
            foreach (JProperty prop in _currentJsonObject.Properties())
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
                                    var decimalList = new List<decimal>();
                                    foreach (var tok in a.Children())
                                        decimalList.Add(tok.Value<decimal>());
                                    this._expression.Variables[n].Value = new HiSystems.Interpreter.Array(decimalList.ToArray());
                                }
                                else if (isArrayOfTypeBooleam)
                                {
                                    var booleanList = new List<IConstruct>();
                                    foreach (var tok in a.Children())
                                        booleanList.Add(new HiSystems.Interpreter.Boolean(tok.Value<bool>()));
                                    this._expression.Variables[n].Value = new HiSystems.Interpreter.Array(booleanList.ToArray());
                                }
                                else
                                {
                                    var stringList = new List<IConstruct>();
                                    foreach (var tok in a.Children())
                                        stringList.Add(new HiSystems.Interpreter.Text(tok.ToString()));
                                    this._expression.Variables[n].Value = new HiSystems.Interpreter.Array(stringList.ToArray());
                                }
                            }
                            break;
                    }
                }
            }
        }
    }
}
