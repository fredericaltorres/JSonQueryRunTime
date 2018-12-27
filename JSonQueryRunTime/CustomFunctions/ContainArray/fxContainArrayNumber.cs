using HiSystems.Interpreter;
using WildCardExercice.net;
using System.Linq;

namespace JSonQueryRunTime
{
    class fxContainArrayNumber: Function
    {
        public override string Name 
        {
            get {
	            return "ContainArrayNumber";
            }
        }

        public override Literal Execute(IConstruct[] arguments)
        {
            base.EnsureArgumentCountIs(arguments, 2);
    		var outDecimalArrays = base.GetTransformedArgumentArray<Number>(arguments, argumentIndex: 0);
            var inDecimalArrays = base.GetTransformedArgumentArray<Number>(arguments, argumentIndex: 1);
            var r = true;
            foreach(var inDec in inDecimalArrays)
            {
                if(!outDecimalArrays.Contains(inDec))
                {
                    r = false;
                    break;
                }
            }
            return new HiSystems.Interpreter.Boolean(r);
        }
    }
}
