using HiSystems.Interpreter;
using WildCardExercice.net;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace JSonQueryRunTime
{
    class fxContains: Function
    {
        public override string Name 
        {
            get {
	            return "Contains";
            }
        }

        public override Literal Execute(IConstruct[] arguments)
        {
            base.EnsureArgumentCountIs(arguments, 2);

            // Verify that the first parameter is an array
            var jsonType = fxPath.ConvertInterpreterTypeIntoJTokenType(arguments[0]);
            if (jsonType != JTokenType.Array)
                throw new System.ArgumentException($"Function Contains() requires an array as the first parameter");

            // Extract the type of the array based on the type of the first value
            jsonType = fxPath.ConvertInterpreterArrayTypeIntoJTokenType(arguments[0]);

            if (jsonType == JTokenType.String)
            {
    		    var outStringArrays = base.GetTransformedArgumentArray<Text>(arguments, argumentIndex: 0);
                var inStringArrays  = base.GetTransformedArgumentArray<Text>(arguments, argumentIndex: 1);
                var r = true;
                foreach(var inDec in inStringArrays)
                    if(!outStringArrays.Contains(inDec))
                    {
                        r = false;
                        break;
                    }
                return new HiSystems.Interpreter.Boolean(r);
            }
            else if (jsonType == JTokenType.Integer || jsonType == JTokenType.Float)
            {
    		    var outStringArrays = base.GetTransformedArgumentArray<Number>(arguments, argumentIndex: 0);
                var inStringArrays  = base.GetTransformedArgumentArray<Number>(arguments, argumentIndex: 1);
                var r = true;
                foreach(var inDec in inStringArrays)
                    if(!outStringArrays.Contains(inDec))
                    {
                        r = false;
                        break;
                    }
                return new HiSystems.Interpreter.Boolean(r);
            }
            else if (jsonType == JTokenType.Boolean)
            {
    		    var outBooleanArrays = base.GetTransformedArgumentArray<Boolean>(arguments, argumentIndex: 0);
                var inBooleanArrays  = base.GetTransformedArgumentArray<Boolean>(arguments, argumentIndex: 1);
                var r = true;
                foreach(var inDec in inBooleanArrays)
                    if(!outBooleanArrays.Contains(inDec))
                    {
                        r = false;
                        break;
                    }
                return new HiSystems.Interpreter.Boolean(r);
            }
            else throw new System.ArgumentException($"{jsonType} type not supported by function Contains()");
        }
    }
}
