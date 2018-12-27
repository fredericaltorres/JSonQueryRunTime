using HiSystems.Interpreter;
using WildCardExercice.net;
using System.Linq;

namespace JSonQueryRunTime
{
    class fxInNumber: Function
    {
        public override string Name 
        {
            get {
	            return "InNumber";
            }
        }

        public override Literal Execute(IConstruct[] arguments)
        {
            base.EnsureArgumentCountIs(arguments, 2);

    		decimal value = base.GetTransformedArgument<Number>(arguments, argumentIndex: 0);
            var decimalArrays = base.GetTransformedArgumentArray<Number>(arguments, argumentIndex: 1);
            var r = decimalArrays.ToList().Contains(value);
            return new HiSystems.Interpreter.Boolean(r);
        }
    }
}
