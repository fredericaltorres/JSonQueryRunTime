using HiSystems.Interpreter;
using WildCardExercice.net;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace JSonQueryRunTime
{
    class fxIsString: Function
    {
        public override string Name 
        {
            get {
	            return "IsString";
            }
        }

        public override Literal Execute(IConstruct[] arguments)
        {
            base.EnsureArgumentCountIs(arguments, 1);
            try {
                // Determine if the argument passed to IsString() is a Json property
                // or a string literal which should contain a path to evaluate
                var isJsonProperty = arguments[0] is Variable;
                if(isJsonProperty)
                {
                    // If we have no exception converting to text the value of the property was of type string
                    string value = base.GetTransformedArgument<Text>(arguments, argumentIndex: 0);
                    return new HiSystems.Interpreter.Boolean(true);
                }
                else
                {
                    // If we passed a Path tp evaluate
                    string jsonPath = base.GetTransformedArgument<Text>(arguments, argumentIndex: 0);
                    JToken lastValue = fxPath.EvalPath(jsonPath);
                    if(lastValue == null) // The jsonString does not contains a path, but a property name that eval to something which is not an object
                        return new HiSystems.Interpreter.Boolean(false);
                    else
                        return new HiSystems.Interpreter.Boolean(lastValue.Type == JTokenType.String);                    
                }
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
