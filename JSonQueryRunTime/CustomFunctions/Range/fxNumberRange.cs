using HiSystems.Interpreter;
using WildCardExercice.net;

namespace JSonQueryRunTime
{
    class fxNumberRange : Function
    {
        public override string Name 
        {
            get {
	            return "NumberRange";
            }
        }

        public override Literal Execute(IConstruct[] arguments)
        {
            base.EnsureArgumentCountIs(arguments, 3);

    		decimal decimalValue = base.GetTransformedArgument<HiSystems.Interpreter.Number>(arguments, argumentIndex: 0);
            decimal decimalStart = base.GetTransformedArgument<HiSystems.Interpreter.Number>(arguments, argumentIndex: 1);
            decimal decimalEnd = base.GetTransformedArgument<HiSystems.Interpreter.Number>(arguments, argumentIndex: 2);
           
            var r = decimalValue >= decimalStart && decimalValue <= decimalEnd;
            
            return new HiSystems.Interpreter.Boolean(r);
        }
    }
}
