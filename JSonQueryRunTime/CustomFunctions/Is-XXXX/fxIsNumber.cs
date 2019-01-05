using HiSystems.Interpreter;
using WildCardExercice.net;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace JsonQueryRunTime
{
    class fxIsNumber : Function
    {
        public override string Name
        {
            get
            {
                return "IsNumber";
            }
        }

        public override Literal Execute(IConstruct[] arguments)
        {
            base.EnsureArgumentCountIs(arguments, 1);
            try
            {
                decimal value = base.GetTransformedArgument<Number>(arguments, argumentIndex: 0);
                return new HiSystems.Interpreter.Boolean(true);
            }
            catch (System.InvalidOperationException ioEx)
            {
                return new HiSystems.Interpreter.Boolean(false);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
    }
}
