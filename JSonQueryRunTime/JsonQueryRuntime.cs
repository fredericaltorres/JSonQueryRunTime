using System;
using System.Collections.Generic;
using HiSystems.Interpreter;
using JSonQueryRunTime;
using Newtonsoft.Json.Linq;

namespace JSonQueryRunTime_UnitTests
{
    public class JsonQueryRuntime
    {
        Expression _expression;
        Engine _engine;
        public JsonQueryRuntime(string whereClause)
        {
            _engine = new Engine();
            _engine.Register(new fxWildCard());
            _engine.Register(new fxDateRange());
            _engine.Register(new fxInString());
            _engine.Register(new fxNumberRange());
            _engine.Register(new fxInNumber());
            _engine.Register(new fxIsString());
            _engine.Register(new fxContainArrayNumber());
            _engine.Register(new fxContainArrayString());
            _engine.Register(new fxContainArrayBoolean());
            
            
            _expression = _engine.Parse(whereClause.Replace(Environment.NewLine, ""));
            
        }
        public IEnumerable<string> Eval( IEnumerable<string> jsonStrings)
        {
            var l = new List<string>();
            foreach(var jsonString in jsonStrings)
            {
                if(this.Eval(jsonString))
                    l.Add(jsonString);
            }
            return l;
        }

        public bool Eval(string jsonString)
        {
            JObject jsonObj = JObject.Parse(jsonString);

            foreach (JProperty prop in jsonObj.Properties())
            {
                var n = prop.Name;
                if(_expression.Variables.ContainsKey(n)) {
                    var v = prop.Value;
                    switch(v.Type)
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
                        // Assume array contains the same item type Number or String
                        // Can only be used with ContainArrayNumber() and ContainArrayString()
                        case JTokenType.Array:
                            JArray a = prop.Value as JArray;
                            if(a.Count == 0)
                            {
                                _expression.Variables[n].Value = new HiSystems.Interpreter.Array(new List<decimal>().ToArray());
                            }
                            else
                            {
                                var isArrayOfTypeNumeric = a[0].Type == JTokenType.Float || a[0].Type == JTokenType.Integer;
                                var isArrayOfTypeBooleam = a[0].Type == JTokenType.Boolean;
                                if(isArrayOfTypeNumeric)
                                {
                                    var decList = new List<decimal>();
                                    foreach(var tok in a.Children())
                                        decList.Add(tok.Value<decimal>());
                                    _expression.Variables[n].Value = new HiSystems.Interpreter.Array(decList.ToArray());
                                }
                                else if(isArrayOfTypeBooleam)
                                {
                                    var booList = new List<IConstruct>();
                                    foreach(var tok in a.Children())
                                        booList.Add(new HiSystems.Interpreter.Boolean(tok.Value<bool>()));
                                    _expression.Variables[n].Value = new HiSystems.Interpreter.Array(booList.ToArray());
                                }
                                else {
                                    var strList = new List<IConstruct>();
                                    foreach(var tok in a.Children())
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
