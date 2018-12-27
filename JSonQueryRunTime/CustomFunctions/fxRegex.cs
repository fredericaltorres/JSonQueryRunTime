using HiSystems.Interpreter;
using WildCardExercice.net;

namespace JSonQueryRunTime
{
    class fxRegex: Function
    {
        public override string Name 
        {
            get {
	            return "Regex";
            }
        }

        public override Literal Execute(IConstruct[] arguments)
        {
            base.EnsureArgumentCountIs(arguments, 2);

    		string value = base.GetTransformedArgument<Text>(arguments, argumentIndex: 0);
            string pattern = base.GetTransformedArgument<Text>(arguments, argumentIndex: 1);

            var regex = new System.Text.RegularExpressions.Regex(pattern);
            var r = regex.IsMatch(value);
            
            return new HiSystems.Interpreter.Boolean(r);
        }
    }
}
