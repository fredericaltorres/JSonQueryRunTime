using System;
using System.Collections.Generic;
using HiSystems.Interpreter;
using JSonQueryRunTime;
using Newtonsoft.Json.Linq;

namespace JSonQueryRunTimeNS
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
            _engine.Register(new fxWildCard());
            _engine.Register(new fxRange());
            _engine.Register(new fxIn());
            _engine.Register(new fxIsString());
            _engine.Register(new fxContains());
            _engine.Register(new fxIsObject());
            _engine.Register(new fxPath());
            _engine.Register(new fxRegex());
            _engine.Register(new fxIsNull());
            _engine.Register(new fxNot());
            _engine.Register(new fxEqualArray());
            _engine.Register(new fxIsNumber());
            _engine.Register(new fxIsDate());
            _engine.Register(new fxIsBoolean());
            _engine.Register(new fxIsArray());
            
            

            _expression = _engine.Parse(whereClause.Replace(Environment.NewLine, ""));
        }
        /// <summary>
        /// Evaluate a list of JSON string
        /// </summary>
        /// <param name="jsonStrings"></param>
        /// <returns></returns>
        public IEnumerable<string> Eval(IEnumerable<string> jsonStrings)
        {
            var l = new List<string>();
            foreach (var jsonString in jsonStrings)
                if (this.Eval(jsonString))
                    l.Add(jsonString);
            return l;
        }

        /// <summary>
        /// Evaluate one JSON string
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public bool Eval(string jsonString)
        {
            _currentJsonObject = JObject.Parse(jsonString);

            foreach (JProperty prop in _currentJsonObject.Properties())
            {
                var n = prop.Name;
                if (_expression.Variables.ContainsKey(n))
                {
                    var v = prop.Value;
                    switch (v.Type)
                    {
                        case JTokenType.String:
                            _expression.Variables[n].Value = new HiSystems.Interpreter.Text(v.ToString());
                            break;
                        case JTokenType.Integer:
                        case JTokenType.Float:
                            _expression.Variables[n].Value = new HiSystems.Interpreter.Number((Decimal)v);
                            break;
                        case JTokenType.Boolean:
                            _expression.Variables[n].Value = new HiSystems.Interpreter.Boolean((bool)v);
                            break;
                        case JTokenType.Date:
                            _expression.Variables[n].Value = new HiSystems.Interpreter.DateTime((System.DateTime)v);
                            break;
                        case JTokenType.Null:
                            _expression.Variables[n].Value = new HiSystems.Interpreter.Null(null);
                            break;
                        case JTokenType.Object:
                            {
                                // Pass the sub object are the JSON represenation
                                var s = v.ToString(Newtonsoft.Json.Formatting.None);
                                _expression.Variables[n].Value = new HiSystems.Interpreter.Text(s);
                            }
                            break;
                        // Assume array contains the same item type Number or String
                        // Can only be used with ContainArrayNumber() and ContainArrayString()
                        case JTokenType.Array:
                            JArray a = prop.Value as JArray;
                            if (a.Count == 0)
                            {
                                _expression.Variables[n].Value = new HiSystems.Interpreter.Array(new List<decimal>().ToArray());
                            }
                            else
                            {
                                var isArrayOfTypeNumeric = a[0].Type == JTokenType.Float || a[0].Type == JTokenType.Integer;
                                var isArrayOfTypeBooleam = a[0].Type == JTokenType.Boolean;
                                if (isArrayOfTypeNumeric)
                                {
                                    var decList = new List<decimal>();
                                    foreach (var tok in a.Children())
                                        decList.Add(tok.Value<decimal>());
                                    _expression.Variables[n].Value = new HiSystems.Interpreter.Array(decList.ToArray());
                                }
                                else if (isArrayOfTypeBooleam)
                                {
                                    var booList = new List<IConstruct>();
                                    foreach (var tok in a.Children())
                                        booList.Add(new HiSystems.Interpreter.Boolean(tok.Value<bool>()));
                                    _expression.Variables[n].Value = new HiSystems.Interpreter.Array(booList.ToArray());
                                }
                                else
                                {
                                    var strList = new List<IConstruct>();
                                    foreach (var tok in a.Children())
                                        strList.Add(new HiSystems.Interpreter.Text(tok.ToString()));
                                    _expression.Variables[n].Value = new HiSystems.Interpreter.Array(strList.ToArray());
                                }
                            }
                            break;
                    }
                }
            }
            bool result = _expression.Execute<HiSystems.Interpreter.Boolean>();
            return result;
        }
    }
}
