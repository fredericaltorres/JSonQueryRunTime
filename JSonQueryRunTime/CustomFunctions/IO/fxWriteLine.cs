using HiSystems.Interpreter;
using WildCardExercice.net;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace JsonQueryRunTime
{
    class fxWriteLine : Function
    {
        public override string Name
        {
            get
            {
                return "WriteLine";
            }
        }

        public override Literal Execute(IConstruct[] arguments)
        {
            base.EnsureArgumentCountIs(arguments, 1);
            string value = base.GetTransformedArgument<Text>(arguments, argumentIndex: 0);
            System.Console.WriteLine(value);
            System.Diagnostics.Debug.WriteLine(value);
            return new HiSystems.Interpreter.Boolean(true);
        }
    }
}
