﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sass
{
    public static class Extensions
    {
        /// <summary>
        /// Reduces all consecutive spaces and tabs to one space.
        /// </summary>
        public static string RemoveExcessWhitespace(this string value)
        {
            string newvalue = "";
            value = value.Trim().Replace('\t', ' ');
            bool inString = false, inChar = false, previousWhitespace = false;
            for (int i = 0; i < value.Length; i++)
            {
                if (!(char.IsWhiteSpace(value[i]) && previousWhitespace) || inString || inChar)
                    newvalue += value[i];
                if (char.IsWhiteSpace(value[i]))
                    previousWhitespace = true;
                else
                    previousWhitespace = false;
                if (value[i] == '"' && !inChar)
                    inString = !inString;
                if (value[i] == '\'' && !inString)
                    inChar = !inChar;
            }
            return newvalue.Trim();
        }

        /// <summary>
        /// Trims comments from the end of a line. Uses ';' as the comment
        /// delimiter.
        /// </summary>
        public static string TrimComments(this string value)
        {
            value = value.Trim();
            bool inString = false, inChar = false;
            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] == ';' && !inString && !inChar)
                    return value.Remove(i).Trim();
                if (value[i] == '"' && !inChar)
                    inString = !inString;
                if (value[i] == '\'' && !inString)
                    inChar = !inChar;
            }
            return value.Trim();
        }

        public static int SafeIndexOf(this string value, char needle)
        {
            value = value.Trim();
            bool inString = false, inChar = false;
            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] == needle && !inString && !inChar)
                    return i;
                if (value[i] == '"' && !inChar)
                    inString = !inString;
                if (value[i] == '\'' && !inString)
                    inChar = !inChar;
            }
            return -1;
        }

        public static int SafeIndexOf(this string value, string needle)
        {
            value = value.Trim();
            bool inString = false, inChar = false;
            for (int i = 0; i < value.Length; i++)
            {
                if (value.Substring(i).StartsWith(needle) && !inString && !inChar)
                    return i;
                if (value[i] == '"' && !inChar)
                    inString = !inString;
                if (value[i] == '\'' && !inString)
                    inChar = !inChar;
            }
            return -1;
        }

        /// <summary>
        /// Checks for value contained within a string, outside of '' and ""
        /// </summary>
        public static bool SafeContains(this string value, char needle)
        {
            value = value.Trim();
            bool inString = false, inChar = false;
            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] == needle && !inString && !inChar)
                    return true;
                if (value[i] == '"' && !inChar)
                    inString = !inString;
                if (value[i] == '\'' && !inString)
                    inChar = !inChar;
            }
            return false;
        }

        /// <summary>
        /// Checks for value contained within a string, outside of '' and ""
        /// </summary>
        public static bool SafeContains(this string value, string needle)
        {
            value = value.Trim();
            bool inString = false, inChar = false;
            for (int i = 0; i < value.Length; i++)
            {
                if (value.Substring(i).StartsWith(needle) && !inString && !inChar)
                    return true;
                if (value[i] == '"' && !inChar)
                    inString = !inString;
                if (value[i] == '\'' && !inString)
                    inChar = !inChar;
            }
            return false;
        }

        /// <summary>
        /// Works the same as String.Split, but will not split if the requested characters are within
        /// a character or string literal.
        /// </summary>
        public static string[] SafeSplit(this string value, params char[] characters)
        {
            string[] result = new string[1];
            result[0] = "";
            bool inString = false, inChar = false;
            foreach (char c in value)
            {
                bool foundChar = false;
                if (!inString && !inChar)
                {
                    foreach (char haystack in characters)
                    {
                        if (c == haystack)
                        {
                            foundChar = true;
                            result = result.Concat(new string[] { "" }).ToArray();
                            break;
                        }
                    }
                }
                if (!foundChar)
                {
                    result[result.Length - 1] += c;
                    if (c == '"' && !inChar)
                        inString = !inString;
                    if (c == '\'' && !inString)
                        inChar = !inChar;
                }
            }
            return result;
        }
    }
}
