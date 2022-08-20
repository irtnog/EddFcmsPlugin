/*
 * Copyright © 2020 robby & EDDiscovery development team
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not use this
 * file except in compliance with the License. You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software distributed under
 * the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
 * ANY KIND, either express or implied. See the License for the specific language
 * governing permissions and limitations under the License.
 * 
 * EDDiscovery is not affiliated with Frontier Developments plc.
 */

using System;
using System.Collections;
using System.Collections.Generic;

namespace QuickJSON
{
    /// <summary>
    /// JSON Array object, holding a ordered list of JTokens
    /// </summary>
    public class JArray : JToken
    {
        /// <summary> Create an empty JArray </summary>
        public JArray()
        {
            TokenType = TType.Array;
            Elements = new List<JToken>(16);
        }

        /// <summary> Create a JArray with these items as its data </summary>
        /// <param name="data">List of data to store in JArray</param>
        public JArray(params Object[] data) : this()
        {
            foreach (Object o in data)
                this.Add(JToken.CreateToken(o));
        }

        /// <summary> Create a JArray from items in an IEnumerable </summary>
        /// <param name="data">IEnumerable with data itema</param>
        public JArray(IEnumerable data) : this()
        {
            foreach (Object o in data)
                this.Add(JToken.CreateToken(o));
        }

        /// <summary> Create an array with a single item </summary>
        /// <param name="othertoken">JToken to store in array </param>
        public JArray(JToken othertoken) : this()        // construct with this token at start
        {
            Add(othertoken);
        }

        /// <summary> Access JToken by int indexer
        /// Returns JToken found by indexer, or null if not present, indexer out of range or indexer is not an int
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">If indexer is out of range on set
        /// </exception>
        /// <exception cref="System.InvalidCastException">If indexer is not of type int on set
        /// </exception>
        public override JToken this[object key]
        {
            get { if (key is int && (int)key >= 0 && (int)key < Elements.Count) return Elements[(int)key]; else return null; }
            set { System.Diagnostics.Debug.Assert(key is int); Elements[(int)key] = (value == null) ? JToken.Null() : value; }
        }

        /// <summary> Access JToken by int indexer
        /// Returns JToken found by indexer
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">If indexer is out of range
        /// </exception>
        public JToken this[int element] { get { return Elements[element]; } set { Elements[element] = value; } }

        /// <summary> Get the first JToken </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">If no items are present
        /// </exception>
        public override JToken First() { return Elements[0]; }

        /// <summary> Get the last JToken </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">If no items are present
        /// </exception>
        public override JToken Last() { return Elements[Elements.Count-1]; }

        /// <summary> Get the first JToken or null if no elements are in the list</summary>
        public override JToken FirstOrDefault() { return Elements.Count > 0 ? Elements[0] : null; }

        /// <summary> Get the last JToken or null if no elements are in the list</summary>
        public override JToken LastOrDefault() { return Elements.Count > 0 ? Elements[Elements.Count-1] : null; }

        /// <summary> Get a JToken at this index. </summary>
        /// <param name="index">Index of item</param>
        /// <param name="token">Where to store the found JToken</param>
        /// <returns>If index out of range, return false (null in token), else return true (token has found item)</returns>
        public bool TryGetValue(int index, out JToken token) { if (index >= 0 && index < Elements.Count) { token = Elements[index]; return true; } else { token = null; return false; } }

        /// <summary> Get number of JArray items </summary>
        public override int Count { get { return Elements.Count; } }

        /// <summary> Add a JToken to the end of the array </summary>
        public void Add(JToken o) { Elements.Add(o); }
        /// <summary> Add a range of JTokens to the end of the array</summary>
        public void AddRange(IEnumerable<JToken> o) { Elements.AddRange(o); }

        /// <summary> Remove JToken at index</summary>
        /// <exception cref="System.ArgumentOutOfRangeException">If index is out of range
        /// </exception>
        public void RemoveAt(int index) { Elements.RemoveAt(index); }
        /// <summary> </summary>

        /// <summary> Remove JToken at index for count times</summary>
        /// <exception cref="System.ArgumentOutOfRangeException">If index is out of range
        /// </exception>
        public void RemoveRange(int index,int count) { Elements.RemoveRange(index,count); }

        /// <summary> Clear the array</summary>
        public override void Clear() { Elements.Clear(); }

        /// <summary> Find a token in the array according the the predicate.  Return null if not found</summary>
        public JToken Find(System.Predicate<JToken> predicate) { return Elements.Find(predicate); }

        /// <summary> Find a JToken in the array according the the predicate and convert the value to type T. Return null if not found. If found, type must convert</summary>
        /// <typeparam name="T">Type to convert the value to</typeparam>
        /// <exception cref="System.InvalidOperationException">If JToken is not compatible with conversion.
        /// </exception>
        public T Find<T>(System.Predicate<JToken> predicate) { Object r = Elements.Find(predicate); return (T)r; }

        /// <summary> Convert the JTokens in the array to strings and return a list of strings. 
        /// Any non strings are inserted into the list as null
        /// </summary>
        public List<string> String() { return Elements.ConvertAll<string>((o) => { return o.TokenType == TType.String ? ((string)o.Value) : null; }); }
        /// <summary> Convert the JTokens in the array to int and return a list of ints. Truncation of value may occur.</summary>
        /// <exception cref="System.InvalidCastException">If any items are not numbers
        /// </exception> 
        public List<int> Int() { return Elements.ConvertAll<int>((o) => { return (int)((long)o.Value); }); }
        /// <summary> Convert the JTokens in the array to longs and return a list of longs. Truncation of value may occur. </summary>
        /// <exception cref="System.InvalidCastException">If any items are not numbers
        /// </exception> 
        public List<long> Long() { return Elements.ConvertAll<long>((o) => { return ((long)o.Value); }); }
        /// <summary> Convert the JTokens in the array to doubles and return a list of doubles.</summary>
        /// <exception cref="System.InvalidCastException">If any items are not numbers
        /// </exception> 
        public List<double> Double() { return Elements.ConvertAll<double>((o) => { return ((double)o.Value); }); }

        /// <summary> Parse the JSON string presuming it will return an JArray</summary>
        /// <param name="text">Text to parse</param>
        /// <param name="flags">Parsing flags, default None</param>
        /// <returns>JArray or null if parse fails or does not return a JArray</returns>
        /// <exception cref="QuickJSON.JToken.JsonException"> Thrown on error if parse flags indicate exception required. Error will be in exception error value
        /// </exception>
        public new static JArray Parse(string text, ParseOptions flags = ParseOptions.None)
        {
            var res = JToken.Parse(text,flags);
            if ((flags & ParseOptions.ThrowOnError) != 0 && !(res is JArray))
                throw new JsonException("Parse is not returning a JArray");
            return res as JArray;
        }

        /// <summary> Parse the JSON string presuming it will return an JArray
        /// Parsing flags are set to AllowTrailingCommas | CheckEOL | ThrowOnError </summary>
        /// <param name="text">Text to parse</param>
        /// <returns>JArray</returns>
        /// <exception cref="QuickJSON.JToken.JsonException"> Thrown on error. Error will be in exception Error value
        /// </exception>
        public new static JArray ParseThrowCommaEOL(string text)        
        {
            var res = JToken.Parse(text, JToken.ParseOptions.AllowTrailingCommas | JToken.ParseOptions.CheckEOL | JToken.ParseOptions.ThrowOnError);
            if (!(res is JArray))
                throw new JsonException("Parse is not returning a JArray");
            return res as JArray;
        }

        /// <summary> Parse the JSON string presuming it will return an JArray </summary>
        /// <param name="text">Text to parse</param>
        /// <param name="error">Null if no error, else error string</param>
        /// <param name="flags">Parsing flags</param>
        /// <returns>JArray or null if parse fails or does not return a JArray</returns>
        /// <exception cref="QuickJSON.JToken.JsonException"> Thrown on error if parse flags indicate exception required. Error will be in exception Error value
        /// </exception>
        public new static JArray Parse(string text, out string error, ParseOptions flags)
        {
            var res = JToken.Parse(text, out error, flags);
            if ((flags & ParseOptions.ThrowOnError) != 0 && !(res is JArray))
                throw new JsonException("Parse is not returning a JArray");
            return res as JArray;
        }

        internal override IEnumerator<JToken> GetSubClassTokenEnumerator() { return Elements.GetEnumerator(); }
        internal override IEnumerator GetSubClassEnumerator() { return Elements.GetEnumerator(); }

        private List<JToken> Elements { get; set; }

    }

}



