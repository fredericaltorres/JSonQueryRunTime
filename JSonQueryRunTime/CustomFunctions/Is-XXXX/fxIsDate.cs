using HiSystems.Interpreter;
using WildCardExercice.net;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace JsonQueryRunTime
{
    class fxIsDate: Function
    {
        public override string Name 
        {
            get {
	            return "IsDate";
            }
        }

        public override Literal Execute(IConstruct[] arguments)
        {
            base.EnsureArgumentCountIs(arguments, 1);
            try {
                    DateTime value = base.GetTransformedArgument<DateTime>(arguments, argumentIndex: 0);
                    return new HiSystems.Interpreter.Boolean(true);
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
