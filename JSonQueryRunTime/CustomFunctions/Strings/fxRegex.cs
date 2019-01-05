using HiSystems.Interpreter;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using WildCardExercice.net;

namespace JsonQueryRunTime
{
    public class fxRegex: Function
    {
        public override string Name 
        {
            get {
	            return "Regex";
            }
        }

        /// <summary>
        /// Public compiled Regex cache.
        /// Need to be public to we can clear it if needed
        /// </summary>
        public static Dictionary<string, Regex> RegexCache = new Dictionary<string, Regex>();

        public override Literal Execute(IConstruct[] arguments)
        {
            base.EnsureArgumentCountIs(arguments, 2);

    		string value   = base.GetTransformedArgument<Text>(arguments, argumentIndex: 0);
            string pattern = base.GetTransformedArgument<Text>(arguments, argumentIndex: 1);
            Regex regex    = null;

            if(RegexCache.ContainsKey(pattern))
            {
                regex = RegexCache[pattern];
            }
            else
            {
                regex = new Regex(pattern, RegexOptions.Compiled);
                RegexCache[pattern] = regex;
            }

            var r = regex.IsMatch(value);
            return new HiSystems.Interpreter.Boolean(r);
        }
    }
}
