using HiSystems.Interpreter;
using WildCardExercice.net;

namespace JSonQueryRunTime
{
    class fxDateRange : Function
    {
        public override string Name 
        {
            get {
	            return "DateRange";
            }
        }

        public override Literal Execute(IConstruct[] arguments)
        {
            base.EnsureArgumentCountIs(arguments, 3);

    		DateTime dateValue = base.GetTransformedArgument<HiSystems.Interpreter.DateTime>(arguments, argumentIndex: 0);
            DateTime dateStart = base.GetTransformedArgument<HiSystems.Interpreter.DateTime>(arguments, argumentIndex: 1);
            DateTime dateEnd = base.GetTransformedArgument<HiSystems.Interpreter.DateTime>(arguments, argumentIndex: 2);
           
            var r = dateValue >= dateStart && dateValue <= dateEnd;
            
            return new HiSystems.Interpreter.Boolean(r);
        }
    }
}
