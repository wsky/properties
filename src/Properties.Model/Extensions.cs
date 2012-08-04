//  Copyright 2012 wsky. wskyhx@gmail.com
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//        http://www.apache.org/licenses/LICENSE-2.0
//  Unless required by applicable law or agreed to in writing, 
//  software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Properties.Model
{
    internal static class Extensions
    {
    }
    internal class Assert : NUnit.Framework.Assert
    {
        public static void IsNotNullOrWhiteSpace(string input)
        {
            Assert.IsNotNullOrEmpty(input);
            Assert.IsNotNullOrEmpty(input.Trim());
        }
        /// <summary>IsNotNullOrWhiteSpace, LessOrEqual 255
        /// </summary>
        /// <param name="input"></param>
        public static void IsValidKey(string input)
        {
            Assert.IsNotNullOrWhiteSpace(input);
            Assert.LessOrEqual(input.Length, 255);
        }
        public static void IsValidDescription(string input)
        {
            if (!string.IsNullOrWhiteSpace(input))
                Assert.LessOrEqual(input.Length, 500);
        }
    }
}
