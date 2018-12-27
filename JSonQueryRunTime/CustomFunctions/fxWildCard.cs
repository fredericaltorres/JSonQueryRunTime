using HiSystems.Interpreter;
using WildCardExercice.net;

namespace JSonQueryRunTime
{
    class fxWildCard : Function
    {
        public override string Name 
        {
            get {
	            return "Wildcard";
            }
        }

        public override Literal Execute(IConstruct[] arguments)
        {
            base.EnsureArgumentCountIs(arguments, 2);

    		string value = base.GetTransformedArgument<Text>(arguments, argumentIndex: 0);
            string pattern = base.GetTransformedArgument<Text>(arguments, argumentIndex: 1);

            //var r = LikeOperator.LikeString(value,pattern, Microsoft.VisualBasic.CompareMethod.Binary);
            
            IWildCard wc = new RecursiveWildCard();
            var r = wc.IsMatch(value, pattern);
            
            return new HiSystems.Interpreter.Boolean(r);
        }
    }
}
