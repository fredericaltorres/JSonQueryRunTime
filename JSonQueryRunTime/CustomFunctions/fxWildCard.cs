using HiSystems.Interpreter;
using WildCardExercice.net;

namespace JsonQueryRunTime
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

            IWildCard wc = new RecursiveWildCard();
            var r = wc.IsMatch(value, pattern);
            
            return new HiSystems.Interpreter.Boolean(r);
        }
    }
}
