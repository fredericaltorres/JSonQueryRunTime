/* _________________________________________________

  (c) Hi-Integrity Systems 2012. All rights reserved.
  www.hisystems.com.au - Toby Wicks
  github.com/hisystems/Interpreter
 
  Licensed under the Apache License, Version 2.0 (the "License");
  you may not use this file except in compliance with the License.
  You may obtain a copy of the License at

      http://www.apache.org/licenses/LICENSE-2.0

  Unless required by applicable law or agreed to in writing, software
  distributed under the License is distributed on an "AS IS" BASIS,
  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
  See the License for the specific language governing permissions and
  limitations under the License.
 ___________________________________________________ */

using System;
using System.ComponentModel;
using HiSystems.Interpreter.Converters;

namespace HiSystems.Interpreter
{
    [TypeConverter(typeof(TextTypeConverter))]
    public class Null : Literal
    {
        private string value = null;

        public Null(string value)
        {
        }
        
        public static implicit operator string(Null text)
        {
            return null;
        }

        public static implicit operator Null(string text)
        {
            return new Null(text);
        }

        public static Boolean operator==(Null value1, Null value2)
        {
            return AreEqual(value1, value2);
        }
        
        public static Boolean operator!=(Null value1, Null value2)
        {
            return !AreEqual(value1, value2);
        }

        public static Text operator+(Null value1, Null value2)
        {
            return new Text(value1.value + value2.value);
        }


        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Null))
                return false;
            else 
                return AreEqual(this, (Null)obj);
        }
        
        private static Boolean AreEqual(Null value1, Null value2)
        {
            if (ReferenceEquals(value1, null) || ReferenceEquals(value2, null))
                return new Boolean(false);
            else
            {
                if(value1.value == null)
                {
                    if(value2.value == null)
                        return new Boolean(true);
                    else
                        return new Boolean(false);
                }
                return new Boolean(value1.value.Equals(value2.value, StringComparison.InvariantCulture));
            }
                
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        } 
        
        public override string ToString()
        {
            return "null";
        }
    }
}

