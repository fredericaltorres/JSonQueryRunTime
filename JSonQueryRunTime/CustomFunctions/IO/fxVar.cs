using HiSystems.Interpreter;
using WildCardExercice.net;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace JsonQueryRunTime
{
    class fxVar : Function
    {
        public override string Name
        {
            get
            {
                return "Var";
            }
        }

        public override Literal Execute(IConstruct[] arguments)
        {
            base.EnsureArgumentCountIs(arguments, 2);

            string varName = base.GetTransformedArgument<Text>(arguments, argumentIndex: 0);

            var jsonType = fxUtils.ConvertInterpreterTypeIntoJTokenType(arguments[1]);
            if(jsonType == JTokenType.String)
            {
                var value = base.GetTransformedArgument<Text>(arguments, argumentIndex: 1);
                JsonQueryRunTimeNS.JsonQueryRuntime.SingletonInstance.AddVariable(varName, value);
                return new HiSystems.Interpreter.Boolean(true);
            }
            else if(jsonType == JTokenType.Integer || jsonType == JTokenType.Float)
            {
                var value = base.GetTransformedArgument<Number>(arguments, argumentIndex: 1);
                JsonQueryRunTimeNS.JsonQueryRuntime.SingletonInstance.AddVariable(varName, value);
                return new HiSystems.Interpreter.Boolean(true);
            }
            else if(jsonType == JTokenType.Date)
            {
                var value = base.GetTransformedArgument<DateTime>(arguments, argumentIndex: 1);
                JsonQueryRunTimeNS.JsonQueryRuntime.SingletonInstance.AddVariable(varName, value);
                return new HiSystems.Interpreter.Boolean(true);
            }
            else if(jsonType == JTokenType.Boolean)
            {
                var value = base.GetTransformedArgument<Boolean>(arguments, argumentIndex: 1);
                JsonQueryRunTimeNS.JsonQueryRuntime.SingletonInstance.AddVariable(varName, value);
                return new HiSystems.Interpreter.Boolean(true);
            }
            else throw new System.ArgumentException($"type {jsonType} not supported by WriteLine()");
        }
    }
}
