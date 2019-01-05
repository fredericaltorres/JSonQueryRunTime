using HiSystems.Interpreter;
using WildCardExercice.net;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace JsonQueryRunTime
{
    class fxWriteLine : Function
    {
        public override string Name
        {
            get
            {
                return "WriteLine";
            }
        }

        public override Literal Execute(IConstruct[] arguments)
        {
            base.EnsureArgumentCountIs(arguments, 1);

            var jsonType = fxUtils.ConvertInterpreterTypeIntoJTokenType(arguments[0]);
            if(jsonType == JTokenType.String)
            {
                string value = base.GetTransformedArgument<Text>(arguments, argumentIndex: 0);
                System.Console.WriteLine(value);
                System.Diagnostics.Debug.WriteLine(value);
                return new HiSystems.Interpreter.Boolean(true);
            }
            else if(jsonType == JTokenType.Integer || jsonType == JTokenType.Float)
            {
                decimal value = base.GetTransformedArgument<Number>(arguments, argumentIndex: 0);
                System.Console.WriteLine(value);
                System.Diagnostics.Debug.WriteLine(value);
                return new HiSystems.Interpreter.Boolean(true);
            }
            else if(jsonType == JTokenType.Date)
            {
                System.DateTime value = base.GetTransformedArgument<DateTime>(arguments, argumentIndex: 0);
                System.Console.WriteLine(value);
                System.Diagnostics.Debug.WriteLine(value);
                return new HiSystems.Interpreter.Boolean(true);
            }
            else throw new System.ArgumentException($"type {jsonType} not supported by WriteLine()");
        }
    }
}
