using System;

namespace WildCardExercice.net
{
    /// <summary>
    /// Wildcard implementation using recursion
    /// </summary>
    public class RecursiveWildCard : WildCardBase, IWildCard
    {
        public static int MaxRecursionLevel = 0;

        public bool IsMatch(String s, String p)
        {
            return helper(s, p, 0, 0, 0);
        }
        bool helper(String s, String pattern, int sX, int patternX, int recursionLevel)
        {
            MaxRecursionLevel = Math.Max(MaxRecursionLevel, recursionLevel);

            // If we reached the end of the pattern and at the same time
            // we also reached the end of the string, then we are done, 
            // the string is a match
            if(patternX == pattern.Length)
                return sX == s.Length;

            if(pattern[patternX] == WILDCARD_ANY_CHAR_ZERO_OR_MORE || pattern[patternX] == WILDCARD_ANY_CHAR_ONE_OR_MORE)
            {
                var isPlusChar = pattern[patternX] == WILDCARD_ANY_CHAR_ONE_OR_MORE;
                if(isPlusChar) // With the + we must at least mach one char, accept the + to match the current char
                    sX += 1;

                // Skip the * or any *** or + or any +++
                while ( patternX < pattern.Length && 
                        (pattern[patternX] == WILDCARD_ANY_CHAR_ZERO_OR_MORE || pattern[patternX] == WILDCARD_ANY_CHAR_ONE_OR_MORE) )
                {
                    patternX += 1;
                }

                while(sX < s.Length)
                {
                    if(helper(s, pattern, sX, patternX, recursionLevel+1))
                        return true;
                    sX += 1; // If we failed matching, skip one char try to skip two...
                }
                return helper(s, pattern, sX, patternX, recursionLevel+1); // Do not forget the final evaluation in case we reach the end of the string
            }
            else if(sX < s.Length) {
                // Direct char to char or char to ? match
                if( (s[sX] == pattern[patternX]) || (pattern[patternX] == WILDCARD_ANY_CHAR) )
                    return helper(s, pattern, sX+1, patternX+1, recursionLevel); // Move to next char
            }
            return false;
        }
    }
}
