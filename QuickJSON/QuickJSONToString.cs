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

using QuickJSON.Utils;
using System;
using System.Collections;
using System.Collections.Generic;

namespace QuickJSON
{
    public partial class JToken 
    {
        /// <summary> Convert to string default settings </summary>
        /// <returns>JSON string representation</returns>
        public override string ToString()   
        {
            return ToString(this, "", "", "", false);
        }
        /// <summary> Convert to string with strings themselves being unquoted or escaped.
        /// Useful for data extraction purposes</summary>
        /// <returns>JSON string representation</returns>
        public string ToStringLiteral()     
        {
            return ToString(this, "", "", "", true);
        }

        /// <summary> Convert to string </summary>
        /// <param name="verbose">If verbose, pad the structure out</param>
        /// <param name="oapad">Pad before objects or arrays are outputted (only for verbose=true) mode</param>
        /// <returns>JSON string representation</returns>
        public string ToString(bool verbose = false, string oapad = "  ")           
        {
            return verbose ? ToString(this, "", "\r\n", oapad, false) : ToString(this, "", "", "", false);
        }

        /// <summary> Convert to string with ability to control the array/output pad</summary>
        /// <param name="oapad">Pad before objects or arrays are outputted</param>
        /// <returns>JSON string representation</returns>
        public string ToString(string oapad)            // not verbose, but prefix in from of obj/array is configuration
        {
            return ToString(this, "", "", oapad, false);
        }

        /// <summary> Convert to string </summary>
        /// <param name="token">Token to convert</param>
        /// <param name="prepad">Pad before token is outputted</param>
        /// <param name="postpad">Pad after token is outputted</param>
        /// <param name="oapad">Pad before objects or arrays are outputted</param>
        /// <param name="stringliterals">true to output strings without escaping or quoting</param>
        /// <returns>JSON string representation</returns>
        public static string ToString(JToken token, string prepad, string postpad, string oapad, bool stringliterals)
        {
            if (token.TokenType == TType.String)
            {
                if (stringliterals)       // used if your extracting the value of the data as a string, and not turning it back to json.
                    return prepad + (string)token.Value + postpad;
                else
                    return prepad + "\"" + ((string)token.Value).EscapeControlCharsFull() + "\"" + postpad;
            }
            else if (token.TokenType == TType.Double)
            {
                string sd = ((double)token.Value).ToStringInvariant("R");       // round trip it - use 'R' since minvalue won't work very well. See https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings#RFormatString
                if (!(sd.Contains("E") || sd.Contains(".")))                // needs something to indicate its a double, and if it does not have a dot or E, it needs a .0
                    sd += ".0";
                return prepad + sd + postpad;
            }
            else if (token.TokenType == TType.Long)
                return prepad + ((long)token.Value).ToStringInvariant() + postpad;
            else if (token.TokenType == TType.ULong)
                return prepad + ((ulong)token.Value).ToStringInvariant() + postpad;
#if JSONBIGINT
            else if (token.TokenType == TType.BigInt)
                return prepad + ((System.Numerics.BigInteger)token.Value).ToString(System.Globalization.CultureInfo.InvariantCulture) + postpad;
#endif
            else if (token.TokenType == TType.Boolean)
                return prepad + ((bool)token.Value).ToString().ToLower() + postpad;
            else if (token.TokenType == TType.Null)
                return prepad + "null" + postpad;
            else if (token.TokenType == TType.Array)
            {
                string s = prepad + "[" + postpad;
                string arrpad = prepad + oapad;
                JArray ja = token as JArray;
                for (int i = 0; i < ja.Count; i++)
                {
                    bool notlast = i < ja.Count - 1;
                    s += ToString(ja[i], arrpad, postpad, oapad, stringliterals);
                    if (notlast)
                    {
                        s = s.Substring(0, s.Length - postpad.Length) + "," + postpad;
                    }
                }
                s += prepad + "]" + postpad;
                return s;
            }
            else if (token.TokenType == TType.Object)
            {
                string s = prepad + "{" + postpad;
                string objpad = prepad + oapad;
                int i = 0;
                JObject jo = ((JObject)token);
                foreach (var e in jo)
                {
                    bool notlast = i++ < jo.Count - 1;
                    if (e.Value is JObject || e.Value is JArray)
                    {
                        if (stringliterals)
                            s += objpad + e.Key.EscapeControlCharsFull() + ":" + postpad;
                        else
                            s += objpad + "\"" + e.Key.EscapeControlCharsFull() + "\":" + postpad;

                        s += ToString(e.Value, objpad, postpad, oapad, stringliterals);
                        if (notlast)
                        {
                            s = s.Substring(0, s.Length - postpad.Length) + "," + postpad;
                        }
                    }
                    else
                    {
                        if (stringliterals)
                            s += objpad + e.Key.EscapeControlCharsFull() + ":";
                        else
                            s += objpad + "\"" + e.Key.EscapeControlCharsFull() + "\":";

                        s += ToString(e.Value, "", "", oapad, stringliterals) + (notlast ? "," : "") + postpad;
                    }
                }
                s += prepad + "}" + postpad;
                return s;
            }
            else if (token.TokenType == TType.Error)
                return "ERROR:" + (string)token.Value;
            else
                return null;
        }
    }
}



