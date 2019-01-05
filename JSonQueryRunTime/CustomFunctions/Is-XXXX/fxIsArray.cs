using HiSystems.Interpreter;
using WildCardExercice.net;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace JsonQueryRunTime
{
    class fxIsArray: Function
    {
        public override string Name 
        {
            get {
	            return "IsArray";
            }
        }

        public override Literal Execute(IConstruct[] arguments)
        {
            base.EnsureArgumentCountIs(arguments, 1);
            try {
                if(fxUtils.ConvertInterpreterTypeIntoJTokenType(arguments[0]) == JTokenType.Array)
                    return new HiSystems.Interpreter.Boolean(true);
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
