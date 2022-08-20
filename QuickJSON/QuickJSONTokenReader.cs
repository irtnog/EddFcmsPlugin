﻿/*
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
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace QuickJSON
{
    public partial class JToken
    {
        /// <summary>Exception when the token reader fails </summary>
        public class TokenException : System.Exception
        {
            /// <summary> Error text </summary>
            public string Error { get; set; }
            /// <summary> Constructor </summary>
            public TokenException(string s) { Error = s; }
        }

        /// <summary> Read a token string and return one by one the JTokens. 
        /// Will return JToken EndArray and JToken EndObject to indicate end of those objects
        /// </summary>
        /// <param name="text">JSON text</param>
        /// <param name="flags">JSON Parser flags</param>
        /// <param name="charbufsize">Maximum length of a JSON element</param>
        /// <exception cref="QuickJSON.JToken.TokenException">Exception when token reader fails
        /// </exception>
        /// <returns>Next token, or Null at end of text</returns>
        public static IEnumerable<JToken> ParseToken(string text, JToken.ParseOptions flags = JToken.ParseOptions.None, int charbufsize = 16384)
        {
            using (StringReader sr = new StringReader(text))         // read directly from file..
            {
                var parser = new StringParserQuickTextReader(sr, 16384);
                return ParseToken(parser, flags, charbufsize);
            }
        }

        /// <summary> Read a token string and return one by one the JTokens. 
        /// Will return JToken EndArray and JToken EndObject to indicate end of those objects
        /// </summary>
        /// <param name="tr">A text reader to get the text from</param>
        /// <param name="flags">JSON Parser flags</param>
        /// <param name="charbufsize">Maximum length of a JSON element</param>
        /// <exception cref="QuickJSON.JToken.TokenException">Exception when token reader fails
        /// </exception>
        /// <returns>Next token, or Null at end of text</returns>

        public static IEnumerable<JToken> ParseToken(TextReader tr, JToken.ParseOptions flags = JToken.ParseOptions.None, int charbufsize = 16384)
        {
            var parser = new StringParserQuickTextReader(tr, 16384);
            return ParseToken(parser, flags, charbufsize);
        }

        /// <summary> Read a token string and return one by one the JTokens. 
        /// Will return JToken EndArray and JToken EndObject to indicate end of those objects
        /// </summary>
        /// <param name="parser">A string parser based on IStringParserQuick</param>
        /// <param name="flags">JSON Parser flags</param>
        /// <param name="charbufsize">Maximum length of a JSON element</param>
        /// <exception cref="QuickJSON.JToken.TokenException">Exception when token reader fails
        /// </exception>
        /// <returns>Next token, or Null at end of text</returns>
        public static IEnumerable<JToken> ParseToken(IStringParserQuick parser, JToken.ParseOptions flags = JToken.ParseOptions.None, int charbufsize = 16384)
        {
            var res = ParseTokenInt(parser, flags, charbufsize);
            if ((flags & ParseOptions.CheckEOL) != 0 && !parser.IsEOL())
            {
                throw new TokenException(GenErrorString(parser, "Extra Chars after JSON"));
            }
            return res;
        }

        private static IEnumerable<JToken> ParseTokenInt(IStringParserQuick parser, JToken.ParseOptions flags = JToken.ParseOptions.None, int maxstringlen = 16384)
        {
            char[] textbuffer = new char[maxstringlen];
            JToken[] stack = new JToken[256];
            int sptr = 0;
            bool comma = false;
            JArray curarray = null;
            JObject curobject = null;

            {
                parser.SkipSpace();

                JToken o = DecodeValue(parser, textbuffer, false);       // grab new value, not array end

                if (o == null)
                {
                    throw new TokenException(GenErrorString(parser, "No Obj/Array"));
                }
                else if (o.TokenType == JToken.TType.Array)
                {
                    stack[++sptr] = o;                      // push this one onto stack
                    curarray = o as JArray;                 // this is now the current array
                    yield return o;
                }
                else if (o.TokenType == JToken.TType.Object)
                {
                    stack[++sptr] = o;                      // push this one onto stack
                    curobject = o as JObject;               // this is now the current object
                    yield return o;
                }
                else
                {
                    yield return o;                               // value only
                    yield break;
                }
            }

            while (true)
            {
                if (curobject != null)      // if object..
                {
                    while (true)
                    {
                        char next = parser.GetChar();

                        if (next == '}')    // end object
                        {
                            parser.SkipSpace();

                            if (comma == true && (flags & JToken.ParseOptions.AllowTrailingCommas) == 0)
                            {
                                throw new TokenException(GenErrorString(parser, "Comma"));
                            }
                            else
                            {
                                yield return new JToken(JToken.TType.EndObject);

                                JToken prevtoken = stack[--sptr];
                                if (prevtoken == null)      // if popped stack is null, we are back to beginning, return this
                                {
                                    yield break;
                                }
                                else
                                {
                                    comma = parser.IsCharMoveOn(',');
                                    curobject = prevtoken as JObject;
                                    if (curobject == null)
                                    {
                                        curarray = prevtoken as JArray;
                                        break;
                                    }
                                }
                            }
                        }
                        else if (next == '"')   // property name
                        {
                            int textlen = parser.NextQuotedString(next, textbuffer, true);

                            if (textlen < 1 || (comma == false && curobject.Count > 0) || !parser.IsCharMoveOn(':'))
                            {
                                throw new TokenException(GenErrorString(parser, "Object missing property name"));
                            }
                            else
                            {
                                string name = new string(textbuffer, 0, textlen);

                                JToken o = DecodeValue(parser, textbuffer, false);      // get value

                                if (o == null)
                                {
                                    if ((flags & ParseOptions.IgnoreBadObjectValue) != 0)       // if we get a bad value, and flag set, try and move to the next start point
                                    {
                                        while (true)
                                        {
                                            char nc = parser.PeekChar();
                                            if (nc == char.MinValue || nc == '"' || nc == '}')  // looking for next property " or } or eol
                                                break;
                                            else
                                                parser.GetChar();
                                        }
                                    }
                                    else
                                    {
                                        throw new TokenException(GenErrorString(parser, "Object bad value"));
                                    }
                                }
                                else
                                {
                                    o.Name = name;

                                    yield return o;

                                    if (o.TokenType == JToken.TType.Array) // if array, we need to change to this as controlling object on top of stack
                                    {
                                        if (sptr == stack.Length - 1)
                                        {
                                            throw new TokenException(GenErrorString(parser, "Stack overflow"));
                                        }

                                        stack[++sptr] = o;          // push this one onto stack
                                        curarray = o as JArray;                 // this is now the current object
                                        curobject = null;
                                        comma = false;
                                        break;
                                    }
                                    else if (o.TokenType == JToken.TType.Object)   // if object, this is the controlling object
                                    {
                                        if (sptr == stack.Length - 1)
                                        {
                                            throw new TokenException(GenErrorString(parser, "Stack overflow"));
                                        }

                                        stack[++sptr] = o;          // push this one onto stack
                                        curobject = o as JObject;                 // this is now the current object
                                        comma = false;
                                    }
                                    else
                                    {
                                        comma = parser.IsCharMoveOn(',');
                                    }
                                }
                            }
                        }
                        else
                        {
                            throw new TokenException(GenErrorString(parser, "Bad format in object"));
                        }
                    }
                }
                else
                {
                    while (true)
                    {
                        JToken o = DecodeValue(parser, textbuffer, true);       // grab new value

                        if (o == null)
                        {
                            throw new TokenException(GenErrorString(parser, "Bad array value"));
                        }
                        else if (o.TokenType == JToken.TType.EndArray)          // if end marker, jump back
                        {
                            if (comma == true && (flags & JToken.ParseOptions.AllowTrailingCommas) == 0)
                            {
                                throw new TokenException(GenErrorString(parser, "Comma"));
                            }
                            else
                            {
                                yield return new JToken(JToken.TType.EndArray);

                                JToken prevtoken = stack[--sptr];
                                if (prevtoken == null)      // if popped stack is null, we are back to beginning, return this
                                {
                                    yield break;
                                }
                                else
                                {
                                    comma = parser.IsCharMoveOn(',');
                                    curobject = prevtoken as JObject;
                                    if (curobject == null)
                                    {
                                        curarray = prevtoken as JArray;
                                    }
                                    else
                                        break;
                                }
                            }
                        }
                        else if ((comma == false && curarray.Count > 0))   // missing comma
                        {
                            throw new TokenException(GenErrorString(parser, "Comma"));
                        }
                        else
                        {
                            yield return o;

                            if (o.TokenType == JToken.TType.Array) // if array, we need to change to this as controlling object on top of stack
                            {
                                if (sptr == stack.Length - 1)
                                {
                                    throw new TokenException(GenErrorString(parser, "Stack overflow"));
                                }

                                stack[++sptr] = o;              // push this one onto stack
                                curarray = o as JArray;         // this is now the current array
                                comma = false;
                            }
                            else if (o.TokenType == JToken.TType.Object) // if object, this is the controlling object
                            {
                                if (sptr == stack.Length - 1)
                                {
                                    throw new TokenException(GenErrorString(parser, "Stack overflow"));
                                }

                                stack[++sptr] = o;              // push this one onto stack
                                curobject = o as JObject;       // this is now the current object
                                curarray = null;
                                comma = false;
                                break;
                            }
                            else
                            {
                                comma = parser.IsCharMoveOn(',');
                            }
                        }
                    }
                }

            }
        }

        /// <summary> Read the token stream at the current heirarchy level into the current enumerator JToken </summary>
        /// <param name="enumerator">Current enumerator position. Will load the item at the enumerator will all fields found and then stop</param>
        /// <returns>true if loaded correctly</returns>
        public static bool LoadTokens(IEnumerator<JToken> enumerator)
        {
            JToken t = enumerator.Current;

            if (t.IsObject)
            {
                JObject jo = t as JObject;

                while (enumerator.MoveNext())
                {
                    JToken i = enumerator.Current;

                    if (i.IsEndObject)
                        return true;
                    else if (i.IsArray || i.IsObject)
                    {
                        if (!LoadTokens(enumerator))
                            return false;
                    }

                    if (i.IsProperty)
                    {
                        jo.Add(i.Name, i);
                    }
                }

                return false;
            }
            else if (t.IsArray)
            {
                JArray ja = t as JArray;

                while (enumerator.MoveNext())
                {
                    JToken i = enumerator.Current;

                    if (i.IsEndArray)
                        return true;
                    else if (i.IsArray || i.IsObject)
                    {
                        if (!LoadTokens(enumerator))
                            return false;
                    }
                    else
                    {
                        ja.Add(i);
                    }
                }

                return false;
            }
            return false;
        }
    }
  
}



