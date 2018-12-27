using HiSystems.Interpreter;
using WildCardExercice.net;
using System.Linq;

namespace JSonQueryRunTime
{
    class fxInString: Function
    {
        public override string Name 
        {
            get {
	            return "InString";
            }
        }

        public override Literal Execute(IConstruct[] arguments)
        {
            base.EnsureArgumentCountIs(arguments, 2);

    		string value = base.GetTransformedArgument<Text>(arguments, argumentIndex: 0);
            var stringArrays = base.GetTransformedArgumentArray<Text>(arguments, argumentIndex: 1);
            var r = stringArrays.ToList().Contains(value);
            return new HiSystems.Interpreter.Boolean(r);
        }
    }
}
