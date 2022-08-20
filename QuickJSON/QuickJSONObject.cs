/*
 * Copyright © 2021 robby & EDDiscovery development team
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

using QuickJSON.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace QuickJSON
{

    /// <summary>
    /// JSON Object, holding a list of JSON JTokens referenced by property names
    /// </summary>

    public partial class JObject : JToken, IEnumerable<KeyValuePair<string, JToken>>
    {
        /// <summary> Makes an empty JObject </summary>
        public JObject()
        {
            TokenType = TType.Object;
            Objects = new Dictionary<string, JToken>(16);   // giving a small initial cap seems to help
        }

        /// <summary> Makes an JObject with property names and values from an IDictionary</summary>
        public JObject(IDictionary dict) : this()           // convert from a dictionary. Key must be string
        {
            foreach (DictionaryEntry x in dict)
            {
                this.Add((string)x.Key, JToken.CreateToken(x.Value));
            }
        }

        /// <summary> Makes an clone of another JObject </summary>
        public JObject(JObject other) : this()              // create with deep copy from another object
        {
            foreach (var kvp in other.Objects)
            {
                Objects[kvp.Key] = kvp.Value.Clone();
            }
        }

        /// <summary> Access JToken by string property key
        /// Returns JToken found by property key, or null if not present or indexer is not a string
        /// </summary>
        /// <exception cref="System.InvalidCastException">If name is not of type string on set
        /// </exception>
        public override JToken this[object key]
        {
            get { if (key is string && Objects.TryGetValue((string)key, out JToken v)) return v; else return null; }
            set { System.Diagnostics.Debug.Assert(key is string); Objects[(string)key] = (value == null) ? JToken.Null() : value; }
        }

        /// <summary> Access JToken by string indexer
        /// Returns JToken found by indexer, or null if not present
        /// </summary>
        public JToken this[string key]
        {
            get { if (Objects.TryGetValue(key, out JToken v)) return v; else return null; }
            set { Objects[key] = (value == null) ? JToken.Null() : value; }
        }

        /// <summary> Get the first JToken </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">If no items are present
        /// </exception>
        public override JToken First() { return Objects.First().Value; }

        /// <summary> Get the last JToken </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">If no items are present
        /// </exception>
        public override JToken Last() { return Objects.Last().Value; }

        /// <summary> Get the first JToken or null if no properties are present</summary>
        public override JToken FirstOrDefault() { return Objects.Count > 0 ? Objects.First().Value : null; }

        /// <summary> Get the last JToken or null if no properties are present</summary>
        public override JToken LastOrDefault() { return Objects.Count > 0 ? Objects.Last().Value : null; }

        /// <summary> Get an array of all property names</summary>
        public string[] PropertyNames() { return Objects.Keys.ToArray(); }

        /// <summary> Does the JObject contain property name</summary>
        public bool Contains(string name) { return Objects.ContainsKey(name); }

        /// <summary> Get a JToken by this property name</summary>
        /// <param name="name">Name of property</param>
        /// <param name="token">Where to store the found JToken</param>
        /// <returns>If property is not found, return false (null in token), else return true (token has found item)</returns>
        public bool TryGetValue(string name, out JToken token) { return Objects.TryGetValue(name, out token); }

        /// <summary> Does the JObject contain any of these properties, if so return first one found, else return null</summary>
        public JToken Contains(string[] ids)     
        {
            foreach (string key in ids)
            {
                if (Objects.ContainsKey(key))
                    return Objects[key];
            }
            return null;
        }

        /// <summary> Get number of properties </summary>
        public override int Count { get { return Objects.Count; } }

        /// <summary> Add a JToken with this property name.  Will overwrite any existing property</summary>
        public void Add(string key, JToken value) { this[key] = value; }
        /// <summary> Remove a JToken with this property name.  True if found, false if not</summary>
        public bool Remove(string key) { return Objects.Remove(key); }
        /// <summary> Remove JTokens with these property names.</summary>
        public void Remove(params string[] key) { foreach (var k in key) Objects.Remove(k); }

        /// <summary> Remove JTokens with these property names.</summary>
        /// <param name="wildcard">Property name to find. May contain * and ? wildcards</param>
        /// <param name="caseinsensitive">True if case insensitive match</param>
        public void RemoveWildcard(string wildcard, bool caseinsensitive = false)       // use * ?
        {
            var list = new List<string>();
            foreach (var kvp in Objects)
            {
                if (kvp.Key.WildCardMatch(wildcard, caseinsensitive))
                {
                    list.Add(kvp.Key);
                }
            }
            foreach (var k in list) Objects.Remove(k);
        }

        /// <summary> Clear all properties</summary>
        public override void Clear() { Objects.Clear(); }

        /// <summary> Key Value Pair Enumerator</summary>
        public new IEnumerator<KeyValuePair<string, JToken>> GetEnumerator() { return Objects.GetEnumerator(); }

        /// <summary> Parse the JSON string presuming it will return an JObject</summary>
        /// <param name="text">Text to parse</param>
        /// <param name="flags">Parsing flags, default None</param>
        /// <returns>JObject or null if parse fails or does not return a JObject</returns>
        /// <exception cref="QuickJSON.JToken.JsonException"> Thrown on error if parse flags indicate exception required. Error will be in exception Error value.  
        /// </exception>
        public new static JObject Parse(string text, ParseOptions flags = ParseOptions.None)        // null if failed.
        {
            var res = JToken.Parse(text, flags);
            if ((flags & ParseOptions.ThrowOnError) != 0 && !(res is JObject))
                throw new JsonException("Parse is not returning a JObject");
            return res as JObject;
        }

        /// <summary> Parse the JSON string presuming it will return an JObject
        /// Parsing flags are set to AllowTrailingCommas | CheckEOL | ThrowOnError </summary>
        /// <param name="text">Text to parse</param>
        /// <returns>JObject or null if parse fails or does not return a JObject</returns>
        /// <exception cref="QuickJSON.JToken.JsonException"> Thrown on error. Error will be in exception Error value.
        /// </exception>
        public new static JObject ParseThrowCommaEOL(string text)        // throws if fails, allows trailing commas and checks EOL
        {
            var res = JToken.Parse(text, JToken.ParseOptions.AllowTrailingCommas | JToken.ParseOptions.CheckEOL | JToken.ParseOptions.ThrowOnError);
            if (!(res is JObject))
                throw new JsonException("Parse is not returning a JObject");
            return res as JObject;
        }

        /// <summary> Parse the JSON string presuming it will return an JObject </summary>
        /// <param name="text">Text to parse</param>
        /// <param name="error">Null if no error, else error string</param>
        /// <param name="flags">Parsing flags</param>
        /// <returns>JObject or null if parse fails or does not return a JObject</returns>
        /// <exception cref="QuickJSON.JToken.JsonException"> Thrown on JSON parse error if parse flags indicate exception required. Error will be in exception Error value
        /// </exception>
        public new static JObject Parse(string text, out string error, ParseOptions flags)
        {
            var res = JToken.Parse(text, out error, flags);
            if ((flags & ParseOptions.ThrowOnError) != 0 && !(res is JObject))
                throw new JsonException("Parse is not returning a JObject");
            return res as JObject;
        }

        internal override IEnumerator<JToken> GetSubClassTokenEnumerator() { return Objects.Values.GetEnumerator(); }
        internal override IEnumerator GetSubClassEnumerator() { return Objects.GetEnumerator(); }
        private Dictionary<string, JToken> Objects { get; set; }
    }
}



