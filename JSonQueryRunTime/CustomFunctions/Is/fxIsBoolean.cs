using HiSystems.Interpreter;
using WildCardExercice.net;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace JSonQueryRunTime
{
    class fxIsBoolean: Function
    {
        public override string Name 
        {
            get {
	            return "IsBoolean";
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
                    bool value = base.GetTransformedArgument<Boolean>(arguments, argumentIndex: 0);
                    return new HiSystems.Interpreter.Boolean(true);
                }
                else
                {
                    bool value = base.GetTransformedArgument<Boolean>(arguments, argumentIndex: 0);
                    return new HiSystems.Interpreter.Boolean(true);
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
