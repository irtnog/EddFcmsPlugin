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
using static QuickJSON.JToken;

namespace QuickJSON
{
    /// <summary>
    /// Class with extensions for JToken
    /// </summary>
    public static class JTokenExtensions
    {
        /// <summary> Convert the JToken tree to an object of type T </summary>
        /// <typeparam name="T">Convert to this type</typeparam>
        /// <param name="token">JToken to convert from</param>
        /// <returns>New object T containing fields filled by JToken, or default(T) on error</returns>
        public static T ToObjectQ<T>(this JToken token)            // quick version, with checkcustomattr off
        {
            return ToObject<T>(token, false, false);
        }

        /// <summary> Convert the JToken tree to an object of type T </summary>
        /// <typeparam name="T">Convert to this type</typeparam>
        /// <param name="token">JToken to convert from</param>
        /// <param name="ignoretypeerrors">Ignore any errors in type between the JToken type and the member type.</param>
        /// <param name="checkcustomattr">Check custom attribute JsonNameAttribute and JsonIgnoreAttribute.  Setting this false improved performance</param>
        /// <returns>New object T containing fields filled by JToken, or default(T) (null) on error. </returns>
        public static T ToObject<T>(this JToken token, bool ignoretypeerrors = false, bool checkcustomattr = true) 
        {
            Type tt = typeof(T);
            try
            {
                Object ret = token.ToObject(tt, ignoretypeerrors, checkcustomattr);        // paranoia, since there are a lot of dynamics, trap any exceptions
                if (ret is ToObjectError)
                {
                    System.Diagnostics.Debug.WriteLine("To Object error:" + ((ToObjectError)ret).ErrorString + ":" + ((ToObjectError)ret).PropertyName);
                    return default(T);
                }
                else if (ret != null)      // or null
                    return (T)ret;          // must by definition have returned tt.
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception JSON ToObject " + ex.Message + " " + ex.StackTrace);
            }

            return default(T);
        }

        /// <summary> Convert the JToken tree to an object of type. Protected from all exception sources</summary>
        /// <param name="token">JToken to convert from</param>
        /// <param name="converttype">Type to convert to</param>
        /// <param name="ignoretypeerrors">Ignore any errors in type between the JToken type and the member type.</param>
        /// <param name="checkcustomattr">Check custom attribute JsonNameAttribute and JsonIgnoreAttribute.  Setting this false improved performance</param>
        /// <param name="membersearchflags">Member search flags, to select what types of members are serialised</param>
        /// <param name="initialobject">If null, object is newed. If given, start from this object. Will except if it and the converttype is not compatible.</param>
        /// <returns>Object containing fields filled by JToken, or a object of ToObjectError on named error, or null if no tokens</returns>

        public static Object ToObjectProtected(this JToken token, Type converttype, bool ignoretypeerrors, bool checkcustomattr,
                                    System.Reflection.BindingFlags membersearchflags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static,
                                    Object initialobject = null)
        {
            try
            {
                return ToObject(token, converttype, ignoretypeerrors, checkcustomattr, membersearchflags, initialobject);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception JSON ToObject " + ex.Message + " " + ex.StackTrace);
            }
            return null;
       
        }

        /// <summary> Class to hold an conversion error </summary>
        public class ToObjectError
        {
            /// <summary> Error string </summary>
            public string ErrorString;
            /// <summary> Property name causing the error, if applicable. Null otherwise</summary>
            public string PropertyName;
            /// <summary> Constructor </summary>
            public ToObjectError(string s) { ErrorString = s; PropertyName = ""; }
        };

        /// <summary> Convert the JToken tree to an object of type.  This member will except.</summary>
        /// <param name="token">JToken to convert from</param>
        /// <param name="converttype">Type to convert to</param>
        /// <param name="ignoretypeerrors">Ignore any errors in type between the JToken type and the member type.</param>
        /// <param name="checkcustomattr">Check custom attribute JsonNameAttribute and JsonIgnoreAttribute.  Setting this false improved performance</param>
        /// <param name="membersearchflags">Member search flags, to select what types of members are serialised.
        /// Use the default plus DeclaredOnly for only members of the top class only </param>
        /// <param name="initialobject">If null, object is newed. If given, start from this object. Will except if it and the converttype is not compatible.</param>
        /// <returns>Object containing fields filled by JToken, or a object of ToObjectError on named error, or null if no tokens</returns>
        /// <exception cref="System.InvalidCastException"> If a type failure occurs (TBD - not sure this is still applicable.)
        /// </exception>
        public static Object ToObject(this JToken token, Type converttype, bool ignoretypeerrors, bool checkcustomattr,
                System.Reflection.BindingFlags membersearchflags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static,
                Object initialobject = null)
        {
            if (token == null)
            {
                return null;
            }
            else if (token.IsArray)
            {
                JArray jarray = (JArray)token;

                if (converttype.IsArray)
                {
                    dynamic instance = initialobject != null ? initialobject : Activator.CreateInstance(converttype, token.Count);   // dynamic holder for instance of array[]

                    for (int i = 0; i < token.Count; i++)
                    {
                        Object ret = ToObject(token[i], converttype.GetElementType(), ignoretypeerrors, checkcustomattr);      // get the underlying element, must match array element type

                        if (ret != null && ret.GetType() == typeof(ToObjectError))      // arrays must be full, any errors means an error
                        {
                            ((ToObjectError)ret).PropertyName = converttype.Name + "." + i.ToString() + "." + ((ToObjectError)ret).PropertyName;
                            return ret;
                        }
                        else
                        {
                            dynamic d = converttype.GetElementType().ChangeTo(ret);
                            instance[i] = d;
                        }
                    }

                    return instance;
                }
                else if (typeof(System.Collections.IList).IsAssignableFrom(converttype))
                {
                    dynamic instance = initialobject != null ? initialobject : Activator.CreateInstance(converttype);        // create the List
                    var types = converttype.GetGenericArguments();

                    for (int i = 0; i < token.Count; i++)
                    {
                        Object ret = ToObject(token[i], types[0], ignoretypeerrors, checkcustomattr);      // get the underlying element, must match types[0] which is list type

                        if (ret != null && ret.GetType() == typeof(ToObjectError))  // lists must be full, any errors are errors
                        {
                            ((ToObjectError)ret).PropertyName = converttype.Name + "." + i.ToString() + "." + ((ToObjectError)ret).PropertyName;
                            return ret;
                        }
                        else
                        {
                            dynamic d = types[0].ChangeTo(ret);
                            instance.Add(d);
                        }
                    }

                    return instance;
                }
                else
                    return new ToObjectError($"JSONToObject: Not array {converttype.Name}");
            }
            else if (token.TokenType == JToken.TType.Object)                   // objects are best efforts.. fills in as many fields as possible
            {
                if (typeof(System.Collections.IDictionary).IsAssignableFrom(converttype))       // if its a Dictionary<x,y> then expect a set of objects
                {
                    dynamic instance = initialobject != null ? initialobject : Activator.CreateInstance(converttype);        // create the class, so class must has a constructor with no paras
                    var types = converttype.GetGenericArguments();

                    foreach (var kvp in (JObject)token)
                    {
                        Object ret = ToObject(kvp.Value, types[1], ignoretypeerrors, checkcustomattr);        // get the value as the dictionary type - it must match type or it get OE

                        if (ret != null && ret.GetType() == typeof(ToObjectError))
                        {
                            ((ToObjectError)ret).PropertyName = converttype.Name + "." + kvp.Key + "." + ((ToObjectError)ret).PropertyName;

                            if (ignoretypeerrors)
                            {
                                System.Diagnostics.Debug.WriteLine("Ignoring Object error:" + ((ToObjectError)ret).ErrorString + ":" + ((ToObjectError)ret).PropertyName);
                            }
                            else
                            {
                                return ret;
                            }
                        }
                        else
                        {
                            dynamic k = types[0].ChangeTo(kvp.Key);             // convert kvp.Key, to the dictionary type. May except if not compatible
                            dynamic d = types[1].ChangeTo(ret);                 // convert value to the data type.
                            instance[k] = d;
                        }
                    }

                    return instance;
                }
                else if (converttype.IsClass ||      // if class
                         (converttype.IsValueType && !converttype.IsPrimitive && !converttype.IsEnum && converttype != typeof(DateTime)))   // or struct, but not datetime (handled below)
                {
                    var instance = initialobject != null ? initialobject : Activator.CreateInstance(converttype);        // create the class, so class must has a constructor with no paras

                    System.Reflection.MemberInfo[] fi = converttype.GetFields(membersearchflags);        // get field list..
                    object[] finames = null;        // alternate names of all fields, loaded only if checkcustom is on,  Object array of string[]

                    System.Reflection.MemberInfo[] pi = null;   // lazy load the property list
                    object[] pinames = null;       // alternate names of all the properties,  Object array of string[]

                    if (checkcustomattr)
                    {
                        finames = new object[fi.Length];
                        for (int i = 0; i < fi.Length; i++)     // thru all the fi fields, see if they have a custom attribute of JsonNameAttribute, if so, pick up the names list
                        {
                            var attrlist = fi[i].GetCustomAttributes(typeof(JsonNameAttribute), false);
                            if (attrlist.Length == 1)
                                finames[i] = ((dynamic)attrlist[0]).Names;
                            else
                                finames[i] = new string[] { fi[i].Name };
                        }
                    }

                    foreach (var kvp in (JObject)token)
                    {
                        System.Reflection.MemberInfo mi = null;

                        if (finames == null)
                        {
                            var fipos = System.Array.FindIndex(fi, x => x.Name.Equals(kvp.Key, StringComparison.InvariantCultureIgnoreCase));     // straight name lookup
                            if (fipos >= 0)
                                mi = fi[fipos];
                        }
                        else
                        {
                            for (int fipos = 0; fipos < finames.Length; fipos++)
                            {
                                if (System.Array.IndexOf((string[])finames[fipos], kvp.Key) >= 0)
                                {
                                    mi = fi[fipos];
                                    break;
                                }
                            }
                        }

                        if ( mi == null )
                        {
                            if (pi == null)     // lazy load pick up, only load these if fields not found
                            {
                                pi = converttype.GetProperties(membersearchflags);

                                if (checkcustomattr)
                                {
                                    pinames = new object[pi.Length];
                                    for (int i = 0; i < pi.Length; i++)
                                    {
                                        var attrlist = pi[i].GetCustomAttributes(typeof(JsonNameAttribute), false);
                                        if ( attrlist.Length == 1 )
                                            pinames[i] = ((dynamic)attrlist[0]).Names;
                                        else
                                            pinames[i] = new string[] { pi[i].Name };
                                    }
                                }
                            }

                            if (pinames == null)
                            {
                                var pipos = System.Array.FindIndex(pi, x => x.Name.Equals(kvp.Key, StringComparison.InvariantCultureIgnoreCase));
                                if (pipos >= 0)
                                    mi = pi[pipos];
                            }
                            else
                            {
                                for (int pipos = 0; pipos < pinames.Length; pipos++)
                                {
                                    if (System.Array.IndexOf((string[])pinames[pipos], kvp.Key) >= 0)
                                    {
                                        mi = pi[pipos];
                                        break;
                                    }
                                }
                            }
                        }

                        if (mi != null)                                   // if we found a class member
                        {
                            var ca = checkcustomattr ? mi.GetCustomAttributes(typeof(JsonIgnoreAttribute), false) : null;

                            if (ca == null || ca.Length == 0)                                              // ignore any ones with JsonIgnore on it.
                            {
                                Type otype = mi.FieldPropertyType();

                                if (otype != null)                          // and its a field or property
                                {
                                    Object ret = ToObject(kvp.Value, otype, ignoretypeerrors, checkcustomattr);    // get the value - must match otype.. ret may be zero for ? types

                                    if (ret != null && ret.GetType() == typeof(ToObjectError))
                                    {
                                        ((ToObjectError)ret).PropertyName = converttype.Name + "." + kvp.Key + "." + ((ToObjectError)ret).PropertyName;

                                        if (ignoretypeerrors)
                                        {
                                            System.Diagnostics.Debug.WriteLine("Ignoring Object error:" + ((ToObjectError)ret).ErrorString + ":" + ((ToObjectError)ret).PropertyName);
                                        }
                                        else
                                        {
                                            return ret;
                                        }
                                    }
                                    else
                                    {
                                        if (!mi.SetValue(instance, ret))         // and set. Set will fail if the property is get only
                                        {
                                            if (ignoretypeerrors)
                                            {
                                                System.Diagnostics.Debug.WriteLine("Ignoring cannot set value on property " + mi.Name);
                                            }
                                            else
                                            {
                                                return new ToObjectError("Cannot set value on property " + mi.Name);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                           // System.Diagnostics.Debug.WriteLine("JSONToObject: No such member " + kvp.Key + " in " + tt.Name);
                        }
                    }

                    return instance;
                }
                else
                    return new ToObjectError($"JSONToObject: Not class {converttype.Name}");
            }
            else
            {
                string name = converttype.Name;                              // compare by name quicker than is

                if (name.Equals("Nullable`1"))                      // nullable types
                {
                    if (token.IsNull)
                        return null;

                    name = converttype.GenericTypeArguments[0].Name;         // get underlying type..
                }

                if (name.Equals("String"))                          // copies of QuickJSON explicit operators in QuickJSON.cs
                {
                    if (token.IsNull)
                        return null;
                    else if (token.IsString)
                        return token.Value;
                }
                else if (name.Equals("Int32"))
                {
                    if (token.TokenType == TType.Long)                  // it won't be a ulong/bigint since that would be too big for an int
                        return (int)(long)token.Value;
                    else if (token.TokenType == TType.Double)           // doubles get trunced.. as per previous system
                        return (int)(double)token.Value;
                }
                else if (name.Equals("Int64"))
                {
                    if (token.TokenType == TType.Long)
                        return token.Value;
                    else if (token.TokenType == TType.Double)
                        return (long)(double)token.Value;
                }
                else if (name.Equals("Boolean"))
                {
                    if (token.TokenType == TType.Boolean)
                        return (bool)token.Value;
                    else if (token.TokenType == TType.Long)
                        return (long)token.Value != 0;
                }
                else if (name.Equals("Double"))
                {
                    if (token.TokenType == TType.Long)
                        return (double)(long)token.Value;
                    else if (token.TokenType == TType.ULong)
                        return (double)(ulong)token.Value;
#if JSONBIGINT
                    else if (token.TokenType == TType.BigInt)
                        return (double)(System.Numerics.BigInteger)token.Value;
#endif
                    else if (token.TokenType == TType.Double)
                        return (double)token.Value;
                }
                else if (name.Equals("Single"))
                {
                    if (token.TokenType == TType.Long)
                        return (float)(long)token.Value;
                    else if (token.TokenType == TType.ULong)
                        return (float)(ulong)token.Value;
#if JSONBIGINT
                    else if (token.TokenType == TType.BigInt)
                        return (float)(System.Numerics.BigInteger)token.Value;
#endif
                    else if (token.TokenType == TType.Double)
                        return (float)(double)token.Value;
                }
                else if (name.Equals("UInt32"))
                {
                    if (token.TokenType == TType.Long && (long)token.Value >= 0)
                        return (uint)(long)token.Value;
                    else if (token.TokenType == TType.Double && (double)token.Value >= 0)
                        return (uint)(double)token.Value;
                }
                else if (name.Equals("UInt64"))
                {
                    if (token.TokenType == TType.ULong)
                        return (ulong)token.Value;
                    else if (token.TokenType == TType.Long && (long)token.Value >= 0)
                        return (ulong)(long)token.Value;
                    else if (token.TokenType == TType.Double && (double)token.Value >= 0)
                        return (ulong)(double)token.Value;
                }
                else if (name.Equals("DateTime"))
                {
                    DateTime? dt = token.DateTime(System.Globalization.CultureInfo.InvariantCulture);
                    if (dt != null)
                        return dt;
                }
                else if (converttype.IsEnum)
                {
                    if (!token.IsString)
                    {
                        System.Diagnostics.Debug.WriteLine($"JSONToObject: Enum Token is not string for {converttype.Name}");
                        return new ToObjectError($"JSONToObject: Enum Token is not string for {converttype.Name}");
                    }

                    try
                    {
                        Object p = Enum.Parse(converttype, token.Str(), true);
                        return Convert.ChangeType(p, converttype);
                    }
                    catch
                    {
                        System.Diagnostics.Debug.WriteLine($"JSONToObject: Unrecognised value '{token.Str()}' for enum {converttype.Name}");
                        return new ToObjectError($"JSONToObject: Unrecognised value '{token.Str()}' for enum {converttype.Name}");
                    }
                }

                return new ToObjectError("JSONToObject: Bad Conversion " + token.TokenType + " to " + converttype.Name);
            }
        }
    }
}



