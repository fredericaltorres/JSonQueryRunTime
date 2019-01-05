using HiSystems.Interpreter;
using WildCardExercice.net;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using JsonQueryRunTimeNS;

namespace JsonQueryRunTime
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
                else return new HiSystems.Interpreter.Boolean(false);
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
