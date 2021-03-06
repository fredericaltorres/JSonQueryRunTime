﻿using HiSystems.Interpreter;
using WildCardExercice.net;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace JsonQueryRunTime
{
    class fxEqualArray: Function
    {
        public override string Name 
        {
            get {
	            return "EqualArray";
            }
        }

        public override Literal Execute(IConstruct[] arguments)
        {
            base.EnsureArgumentCountIs(arguments, 2);
            return fxContains.ContainsArrayOrEqualArrayOrContainsString(this, arguments, false);
        }
    }
}
