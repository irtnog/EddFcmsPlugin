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

using System.Collections.Generic;

namespace QuickJSON
{
    public partial class JObject : JToken, IEnumerable<KeyValuePair<string, JToken>>
    {

        /// <summary>
        /// Filter this object into a return object. 
        /// Allowed fields are a set of object key/value pairs with the value set to:
        /// ```
        ///    Boolean true/false: direct allow/disallow of any type with this field name
        ///    JArray containing one JOBJECT: item is an array of objects, with the allowed fields in that object listed:
        ///        ["name"] = new JArray
        ///            {   new JObject
        ///                {
        ///                    ["Null"] = true,
        ///                    ["Star"] = true,
        ///                    ["Planet"] = true,
        ///                    ["Ring"] = true
        ///                } }
        ///         or
        ///         ["name"] = "[]" - Allow anything in an array
        ///    JObject: allow an object with these fields present if boolean is true.
        ///          ["name"] = new JObject
        ///          {
        ///              ["Rock"] = true,
        ///              ["Metal"] = true,
        ///          },
        /// ```
        /// </summary>
        /// <param name="allowedFields">A JObject of allowed fields to keep</param>
        /// <param name="path">path is used to track the object names through the heirachy for diagnostics purposes only. Do not set.</param>
        /// <returns>Clone of JToken with allowed fields only</returns>
        public JObject Filter(JObject allowedFields, string path = "")     
        {
            JObject ret = new JObject();

            foreach (var kvp in this)
            {
                string mpath = $"{path}.{kvp.Key}";

                if (allowedFields.Contains(kvp.Key))
                {
                    JToken allowedField = allowedFields[kvp.Key];

                    if (kvp.Value.HasValue)
                    {
                        if (allowedField.BoolNull() == true)      // if straight value and allowed
                        {
                            ret[kvp.Key] = kvp.Value;
                        }
                        else
                        {
                            //System.Diagnostics.Trace.WriteLine($"Object value {mpath} rejected");
                        }
                    }                                                               // if Jarray, allowed is Jarray, and one JOBJECT underneath
                    else if (kvp.Value.IsArray && allowedField.IsArray && allowedField.Count == 1 && allowedField[0] is JObject)
                    {
                        JObject allowed = (JObject)allowedField[0];
                        JArray vals = new JArray();

                        foreach (JObject val in kvp.Value)      // go thru array
                        {
                            vals.Add(val.Filter(allowed, $"{mpath}[]"));
                        }

                        ret[kvp.Key] = vals;
                    }
                    else if (kvp.Value.IsArray && allowedField.StrNull() == "[]")     //  if Jarray, and allowed fields is a special [] string marker
                    {
                        JArray vals = new JArray();

                        foreach (JToken val in kvp.Value)       // just add all values
                        {
                            if (val.HasValue)
                            {
                                vals.Add(val);
                            }
                            else
                            {
                                System.Diagnostics.Trace.WriteLine($"Array value {mpath}[] is not a value: {val?.ToString()}");
                            }
                        }

                        ret[kvp.Key] = vals;
                    }
                    else if (kvp.Value.IsObject && allowedField.IsObject)       // if object, and allowed is object
                    {
                        JObject allowed = (JObject)allowedField;
                        JObject val = (JObject)kvp.Value;

                        ret[kvp.Key] = val.Filter(allowed, mpath);     // recurse add
                    }
                    else
                    {
                        System.Diagnostics.Trace.WriteLine($"Object value {mpath} {kvp.Value.TokenType} is not of expected type: {kvp.Value?.ToString()}");
                    }
                }
                else
                {
                    //System.Diagnostics.Trace.WriteLine($"Object value {mpath} not in allowed list: {kvp.Value?.ToString()}");
                }
            }

            return ret;
        }

    }
}



