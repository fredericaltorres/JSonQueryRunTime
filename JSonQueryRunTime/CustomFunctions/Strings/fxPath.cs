using HiSystems.Interpreter;
using WildCardExercice.net;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using JsonQueryRunTimeNS;

namespace JsonQueryRunTime
{
    class fxPath : Function
    {
        public override string Name
        {
            get
            {
                return "Path";
            }
        }

        public Literal ExecuteWithUnknown(IConstruct[] arguments)
        {
            base.EnsureArgumentCountIs(arguments, 2);
            try
            {
                string pathExpression = base.GetTransformedArgument<Text>(arguments, argumentIndex: 0);
                var jsonExpectedType = fxUtils.ConvertInterpreterTypeIntoJTokenType(arguments[1]);

                var currentPath = "";
                var possiblePaths = fxUtils.DerivePathWithUnknown(pathExpression, ref currentPath);

                foreach (var possiblePath in possiblePaths)
                {
                    JToken lastValue = fxUtils.EvalJsonDotNetPath(possiblePath);
                    if (lastValue.Type == jsonExpectedType || (lastValue.Type == JTokenType.Integer && jsonExpectedType == JTokenType.Float))
                    {
                        switch (jsonExpectedType)
                        {
                            case JTokenType.Float: return base.GetTransformedArgument<Number>(arguments, argumentIndex: 1) == (decimal)lastValue;
                            case JTokenType.String: return base.GetTransformedArgument<Text>(arguments, argumentIndex: 1) == (string)lastValue;
                            case JTokenType.Boolean: return base.GetTransformedArgument<Boolean>(arguments, argumentIndex: 1) == (bool)lastValue;
                            case JTokenType.Null: return base.GetTransformedArgument<Null>(arguments, argumentIndex: 1) == null;
                            case JTokenType.Date: return base.GetTransformedArgument<DateTime>(arguments, argumentIndex: 1) == new HiSystems.Interpreter.DateTime((System.DateTime)lastValue);
                        }
                    }
                }
                return new HiSystems.Interpreter.Boolean(false);
            }
            catch (System.InvalidOperationException ioEx)
            {
                return new HiSystems.Interpreter.Boolean(false);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public override Literal Execute(IConstruct[] arguments)
        {
            if (arguments.Length == 2) // Path() with pattern matching
                return ExecuteWithUnknown(arguments);

            base.EnsureArgumentCountIs(arguments, 1);
            try
            {
                string pathExpression = base.GetTransformedArgument<Text>(arguments, argumentIndex: 0);
                if(pathExpression == string.Empty) 
                    return new HiSystems.Interpreter.Null(null);

                // Evaluate the expression using JSON.NET Path Api
                JToken lastValue = fxUtils.EvalJsonDotNetPath("$." + pathExpression);
                return fxUtils.ResolveValueFromJToken(lastValue);
            }
            //catch (System.InvalidOperationException ioEx)
            //{
            //    return new HiSystems.Interpreter.Boolean(false);
            //}
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        

       


      
    }
}
