using HiSystems.Interpreter;
using WildCardExercice.net;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace JsonQueryRunTime
{
    class fxIsString : Function
    {
        public override string Name
        {
            get
            {
                return "IsString";
            }
        }

        public override Literal Execute(IConstruct[] arguments)
        {
            base.EnsureArgumentCountIs(arguments, 1);
            try
            {
                string value = base.GetTransformedArgument<Text>(arguments, argumentIndex: 0);
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
