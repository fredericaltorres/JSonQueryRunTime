using HiSystems.Interpreter;
using WildCardExercice.net;
using System.Linq;

namespace JSonQueryRunTime
{
    class fxContainArrayBoolean: Function
    {
        public override string Name 
        {
            get {
	            return "ContainArrayBoolean";
            }
        }

        public override Literal Execute(IConstruct[] arguments)
        {
            base.EnsureArgumentCountIs(arguments, 2);
    		var outDecimalArrays = base.GetTransformedArgumentArray<Boolean>(arguments, argumentIndex: 0);
            var inDecimalArrays = base.GetTransformedArgumentArray<Boolean>(arguments, argumentIndex: 1);
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
