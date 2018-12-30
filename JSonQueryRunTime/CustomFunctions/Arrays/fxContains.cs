﻿using HiSystems.Interpreter;
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
            return ContainsArrayOrEqualArray(this, arguments, true);
        }

        public static Literal ContainsArrayOrEqualArray(Function baseClass, IConstruct[] arguments, bool containsArray)
        {
            // Verify that the first parameter is an array
            var jsonType = fxPath.ConvertInterpreterTypeIntoJTokenType(arguments[0]);
            if (jsonType != JTokenType.Array)
                throw new System.ArgumentException($"Function Contains() requires an array as the first parameter");

            // Extract the type of the array based on the type of the first value
            jsonType = fxPath.ConvertInterpreterArrayTypeIntoJTokenType(arguments[0]);
            if (jsonType == JTokenType.String)
            {
                var outStringArrays = baseClass.GetTransformedArgumentArray<Text>(arguments, argumentIndex: 0);
                var inStringArrays = baseClass.GetTransformedArgumentArray<Text>(arguments, argumentIndex: 1);
                var r = true;
                if(containsArray)
                {
                    foreach (var inDec in inStringArrays)
                    {
                        if (!outStringArrays.Contains(inDec))
                        {
                            r = false;
                            break;
                        }
                    }
                }
                else
                {
                    if(inStringArrays.Length == outStringArrays.Length)
                    {
                        for(var i=0; i<inStringArrays.Length; i++)
                        {
                            if(inStringArrays[i] != outStringArrays[i])
                            {
                                r = false;
                                break;
                            }
                        }
                    }
                    else r = false;
                }
                return new HiSystems.Interpreter.Boolean(r);
            }
            else if (jsonType == JTokenType.Integer || jsonType == JTokenType.Float)
            {
                var outNumberArrays = baseClass.GetTransformedArgumentArray<Number>(arguments, argumentIndex: 0);
                var inNumberArrays = baseClass.GetTransformedArgumentArray<Number>(arguments, argumentIndex: 1);
                var r = true;
                if(containsArray)
                {
                    foreach (var inDec in inNumberArrays)
                    {
                        if (!outNumberArrays.Contains(inDec))
                        {
                            r = false;
                            break;
                        }
                    }
                }
                else
                {
                    if(inNumberArrays.Length == outNumberArrays.Length)
                    {
                        for(var i=0; i<inNumberArrays.Length; i++)
                        {
                            if(inNumberArrays[i] != outNumberArrays[i])
                            {
                                r = false;
                                break;
                            }
                        }
                    }
                    else r = false;
                }
                return new HiSystems.Interpreter.Boolean(r);
            }
            else if (jsonType == JTokenType.Boolean)
            {
                var outBooleanArrays = baseClass.GetTransformedArgumentArray<Boolean>(arguments, argumentIndex: 0);
                var inBooleanArrays = baseClass.GetTransformedArgumentArray<Boolean>(arguments, argumentIndex: 1);
                var r = true;
                if(containsArray)
                {
                    foreach (var inDec in inBooleanArrays)
                    {
                        if (!outBooleanArrays.Contains(inDec))
                        {
                            r = false;
                            break;
                        }
                    }
                }
                else
                {
                    if(inBooleanArrays.Length == outBooleanArrays.Length)
                    {
                        for(var i=0; i<inBooleanArrays.Length; i++)
                        {
                            if(inBooleanArrays[i] != outBooleanArrays[i])
                            {
                                r = false;
                                break;
                            }
                        }
                    }
                    else r = false;
                }
                return new HiSystems.Interpreter.Boolean(r);
            }
            else throw new System.ArgumentException($"{jsonType} type not supported by function Contains()");
        }
    }
}
