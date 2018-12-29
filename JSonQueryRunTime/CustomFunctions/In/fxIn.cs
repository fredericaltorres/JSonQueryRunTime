using HiSystems.Interpreter;
using WildCardExercice.net;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace JSonQueryRunTime
{
    class fxIn: Function
    {
        public override string Name 
        {
            get {
	            return "In";
            }
        }

        public override Literal Execute(IConstruct[] arguments)
        {
            base.EnsureArgumentCountIs(arguments, 2);

            var jsonType = fxPath.ConvertInterpreterTypeIntoJTokenType(arguments[0]);

            if(jsonType == JTokenType.String)
            {
                string value = base.GetTransformedArgument<Text>(arguments, argumentIndex: 0);
                var stringArrays = base.GetTransformedArgumentArray<Text>(arguments, argumentIndex: 1);
                var r = stringArrays.ToList().Contains(value);
                return new HiSystems.Interpreter.Boolean(r);
            }
            else if(jsonType == JTokenType.Integer || jsonType == JTokenType.Float)
            {
                decimal value = base.GetTransformedArgument<Number>(arguments, argumentIndex: 0);
                var decimalArrays = base.GetTransformedArgumentArray<Number>(arguments, argumentIndex: 1);
                var r = decimalArrays.ToList().Contains(value);
                return new HiSystems.Interpreter.Boolean(r);
            }
            else throw new System.ArgumentException($"{jsonType} type not supported by function In()");
        }
    }
}
