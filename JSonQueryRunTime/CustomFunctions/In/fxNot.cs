using HiSystems.Interpreter;
using WildCardExercice.net;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace JSonQueryRunTime
{
    class fxNot: Function
    {
        public override string Name 
        {
            get {
	            return "Not";
            }
        }

        public override Literal Execute(IConstruct[] arguments)
        {
            base.EnsureArgumentCountIs(arguments, 1);
            try
            {
                bool value = base.GetTransformedArgument<Boolean>(arguments, argumentIndex: 0);
                return new HiSystems.Interpreter.Boolean(!value);
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
