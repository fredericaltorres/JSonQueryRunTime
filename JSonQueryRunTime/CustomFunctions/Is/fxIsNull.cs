using HiSystems.Interpreter;
using WildCardExercice.net;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace JSonQueryRunTime
{
    class fxIsNull: Function
    {
        public override string Name 
        {
            get {
	            return "IsNull";
            }
        }

        public override Literal Execute(IConstruct[] arguments)
        {
            base.EnsureArgumentCountIs(arguments, 1);
            try{
    		    var nullValue = base.GetTransformedArgument<Null>(arguments, argumentIndex: 0);
                return new HiSystems.Interpreter.Boolean(true);
            }
            catch(JsonReaderException jrEx)
            {
                return new HiSystems.Interpreter.Boolean(false);
            }
            catch(System.InvalidOperationException ioEx)
            {
                return new HiSystems.Interpreter.Boolean(false);
            }
            catch(System.Exception ex)
            {
                throw ex;
            }
        }
    }
}
