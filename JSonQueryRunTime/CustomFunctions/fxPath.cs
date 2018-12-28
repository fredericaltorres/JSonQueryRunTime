using HiSystems.Interpreter;
using WildCardExercice.net;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace JSonQueryRunTime_UnitTests
{
    class fxPath: Function
    {
        public override string Name 
        {
            get {
	            return "Path";
            }
        }

        public override Literal Execute(IConstruct[] arguments)
        {
            base.EnsureArgumentCountIs(arguments, 1);
            try
            {
                string pathExpression = base.GetTransformedArgument<Text>(arguments, argumentIndex: 0);

                JToken lastValue = EvalPath(pathExpression);

                if(lastValue == null)
                    return new HiSystems.Interpreter.Null(null);

                if (lastValue.Type == JTokenType.Float || lastValue.Type == JTokenType.Integer)
                    return new HiSystems.Interpreter.Number((decimal)lastValue);

                if (lastValue.Type == JTokenType.Boolean)
                    return new HiSystems.Interpreter.Boolean((bool)lastValue);

                // As Default return as string
                return new HiSystems.Interpreter.Text(lastValue.ToString());
            }
            catch (System.InvalidOperationException ioEx)
            {
                return new HiSystems.Interpreter.Boolean(false);
            }
            catch(System.Exception ex)
            {
                throw ex;
            }
        }
        public static JToken EvalPath(string pathExpression)
        {
            var tokens = pathExpression.Split('.');
            JObject rootObj = JsonQueryRuntime._currentJsonObject;
            JToken lastValue = null;
            foreach (var token in tokens)
            {
                var prop = rootObj.Properties().FirstOrDefault(p => p.Name == token);
                if(prop == null)
                {
                    return null;
                }
                else
                {
                    var jToken = prop.Value;
                    rootObj = jToken as JObject;
                    lastValue = jToken;
                }
            }
            return lastValue;
        }
    }
}
