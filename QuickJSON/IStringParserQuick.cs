/*
 * Copyright © 2021 robbyxp1 @ github.com
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
 */

namespace QuickJSON.Utils
{
    /// <summary>
    /// This namespace contains the Quick JSON Utilities
    /// </summary>
    internal static class NamespaceDoc { } // just for documentation purposes

    /// <summary>
    /// Interface for parsers for QuickJSON
    /// </summary>
    public interface IStringParserQuick
    {
        /// <summary> Current parse position </summary>
        int Position { get; }
        /// <summary> Text of line in whole</summary>
        string Line { get; }
        /// <summary> Skip all white space</summary>
        void SkipSpace();
        /// <summary> Is at End of Line</summary>
        bool IsEOL();       // function as it can have side effects
        /// <summary> Get next character. Space if at EOL. </summary>
        char GetChar(); 
        /// <summary> Peek next character. Space if at EOL. </summary>
        char PeekChar();
        /// <summary> Is this string at the current position, if so, skip it and skip space</summary>
        bool IsStringMoveOn(string s);
        /// <summary> Is this character at the current position, if so, skip it. Optionally skip space afterwards</summary>
        bool IsCharMoveOn(char t, bool skipspace = true);
        /// <summary> Backup one position </summary>
        void BackUp();
        /// <summary> Get the next quoted string into buffer. Quote has already been removed. </summary>
        /// <param name="quote">Quote character to stop on </param>
        /// <param name="buffer">Buffer to place string into </param>
        /// <param name="replaceescape">True to replace escape sequences \\, \/, \b, \f, \n, \r, \t, uNNNN</param>
        /// <returns>Number of characters in buffe. -1 if it runs out of buffer space</returns>
        int NextQuotedString(char quote, char[] buffer, bool replaceescape = false);
        /// <summary> Read next number: long, ulong, bigint or double.</summary>
        /// <param name="sign">True if negative.  Sign has been removed</param>
        /// <returns>New JToken of number, Long, BigInt or Double. Null if failed</returns>
        JToken JNextNumber(bool sign);
    }
}
