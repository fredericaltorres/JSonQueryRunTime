using HiSystems.Interpreter;
using Newtonsoft.Json.Linq;
using WildCardExercice.net;

namespace JSonQueryRunTime
{
    class fxRange : Function
    {
        public override string Name
        {
            get
            {
                return "Range";
            }
        }

        public override Literal Execute(IConstruct[] arguments)
        {
            base.EnsureArgumentCountIs(arguments, 3);

            var jsonType = fxPath.ConvertInterpreterTypeIntoJTokenType(arguments[0]);

            if (jsonType == JTokenType.Date)
            {
                DateTime dateValue = base.GetTransformedArgument<HiSystems.Interpreter.DateTime>(arguments, argumentIndex: 0);
                DateTime dateStart = base.GetTransformedArgument<HiSystems.Interpreter.DateTime>(arguments, argumentIndex: 1);
                DateTime dateEnd = base.GetTransformedArgument<HiSystems.Interpreter.DateTime>(arguments, argumentIndex: 2);
                var r = dateValue >= dateStart && dateValue <= dateEnd;
                return new HiSystems.Interpreter.Boolean(r);
            }
            if (jsonType == JTokenType.String)
            {
                string dateValue = base.GetTransformedArgument<HiSystems.Interpreter.Text>(arguments, argumentIndex: 0);
                string dateStart = base.GetTransformedArgument<HiSystems.Interpreter.Text>(arguments, argumentIndex: 1);
                string dateEnd = base.GetTransformedArgument<HiSystems.Interpreter.Text>(arguments, argumentIndex: 2);
                var e0 = string.Compare(dateValue, dateStart); 
                var e1 = string.Compare(dateValue, dateEnd);
                var r = (e0 == 1 || e0 == 0)
                        && 
                        (e1 == -1 || e1 == 0);
                return new HiSystems.Interpreter.Boolean(r);
            }
            else if (jsonType == JTokenType.Integer || jsonType == JTokenType.Float)
            {
                decimal numValue = base.GetTransformedArgument<HiSystems.Interpreter.Number>(arguments, argumentIndex: 0);
                decimal numStart = base.GetTransformedArgument<HiSystems.Interpreter.Number>(arguments, argumentIndex: 1);
                decimal numEnd = base.GetTransformedArgument<HiSystems.Interpreter.Number>(arguments, argumentIndex: 2);
                var r = numValue >= numStart && numValue <= numEnd;
                return new HiSystems.Interpreter.Boolean(r);
            }
            else throw new System.ArgumentException($"{jsonType} is not supported in function Range()");
        }
    }
}
