using HiSystems.Interpreter;
using WildCardExercice.net;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using JSonQueryRunTimeNS;

namespace JSonQueryRunTime
{
    class fxIsObject: Function
    {
        public override string Name 
        {
            get {
	            return "IsObject";
            }
        }

        public override Literal Execute(IConstruct[] arguments)
        {
            base.EnsureArgumentCountIs(arguments, 1);
            try{
    		    string jsonString = base.GetTransformedArgument<Text>(arguments, argumentIndex: 0);

                // Check if it is a json string
                if(jsonString.TrimStart().StartsWith("{"))
                {
                    JObject jsonObj = JObject.Parse(jsonString);
                    return new HiSystems.Interpreter.Boolean(true);
                }                
                else // Should be json path
                {
                    JToken lastValue = fxPath.EvalPath(jsonString);
                    if(lastValue == null) // The jsonString does not contains a path, but a property name that eval to something which is not an object
                        return new HiSystems.Interpreter.Boolean(false);
                    else
                        return new HiSystems.Interpreter.Boolean(lastValue.Type == JTokenType.Object);                    
                }
            }
            catch(JsonReaderException jrEx)
            {
                return new HiSystems.Interpreter.Boolean(false);
            }
            catch(System.InvalidOperationException ioEx)
            {
                return new HiSystems.Interpreter.Boolean(false);
            }
            catch(System.Exception ex)
            {
                throw ex;
            }
        }
    }
}
