namespace WildCardExercice.net
{
    /*

    Different examples using dynamic programming

    string     :"ab"
    pattern    :"az"

            Pattern
                0   1   2
                    a   z
        -------------------
        0       T   F   F    [0,0] is T  
        1   a   F   T   F    [1,1] is T
        2   b   F   F   F


    string     :"abc"
    pattern    :"ab+"

            Pattern
                0   1   2   3   
                    a   b   +
        -----------------------
        0       T   F   F   F
        1   a   F   T   F   F
        2   b   F   F   T   F
        3   c   F   F   F   T


    string     :"abcd"
    pattern    :"a+bcd" // Should not match

            Pattern
                0   1   2   3   4   5
                    a   +   b   c   d
        ----------------------------------
        0       T   F   F   F   F   F
        1   a   F   T   F   F   F   F
        2   b   F   F   T   F   F   F
        3   c   F   F   F   F   F   F
        4   d   F   F   F   F   F   F


     string     :"xaylmz"
     pattern    :"x?y*z"

            Pattern
                0   1   2   3   4   5
                    x   ?   y   *   z
        ------------------------------
        0       T   F   F   F   F   F
        1   x   F   T   F   F   F   F
        2   a   F   F   T   F   F   F       
        3   y   F   F   F   T   T   F
        4   l   F   F   F   F   T   F
        5   m   F   F   F   F   T   F
        6   z   F   F   F   F   T   T

    ! in [2,2] eval x? match xa use [i-1, j-1] value
    ! in [3,2] eval x?y match xal - No
    ! in [2,3] eval x? match xay - No
    ! in [3,3] eval x?y match xay - Yes
    ! in [4,3] eval x?y* match xay - Yes
    ! in [4,4] eval x?y* match xayl - Yes

    - Final answer is in [5,6] == T
    - Space O(m x n)
    - Time O(m x n)

    Wildcard dynamic programming
        https://duckduckgo.com/?q=wildcard+dynamic+programming&atb=v140-7ap&ia=videos
            
    A possible solution for the +
        https://www.geeksforgeeks.org/wildcard-pattern-matching-three-symbols/

    */
    /// <summary>
    /// Wildcard implementation using dynamic programming
    /// Using dynamic programming: https://www.youtube.com/watch?v=3ZDZ-N0EPV0
    /// 
    /// THE SUPPORT OF '+' IS NOT CORRECTLY WORKING
    /// 
    /// </summary>
    public class DynamicProgrammingWildCard : WildCardBase, IWildCard
    {
        public bool IsMatch(string s, string pp)
        {
            string p         = this.GetRidOfDuplicateOperator(pp);
            var str          = s.ToCharArray();
            var pattern      = p.ToCharArray();
            int maxDim1      = str.Length + 1;
            int maxDim2      = pattern.Length + 1;
            
            bool[,] matrix   = new bool[maxDim1, maxDim2];
            matrix[0, 0]     = true;

            if (pattern.Length > 0 && (pattern[0] == WILDCARD_ANY_CHAR_ZERO_OR_MORE || pattern[0] == WILDCARD_ANY_CHAR_ONE_OR_MORE))
            {
                matrix[0, 1] = true;
            }

            for (var i = 1; i < maxDim1; i++)
            {
                for (var j = 1; j < maxDim2; j++)
                {
                    // Match any char with ? or direct char match
                    if ((pattern[j - 1] == WILDCARD_ANY_CHAR) || (i - 1 < str.Length && str[i - 1] == pattern[j - 1]))
                    {
                        matrix[i, j] = matrix[i - 1, j - 1];
                    }
                    // if pattern is * then use result from left cell or above
                    else if (pattern[j - 1] == WILDCARD_ANY_CHAR_ZERO_OR_MORE)
                    {
                        matrix[i, j] = matrix[i - 1, j] /* left */ || matrix[i, j - 1] /* or above */ ;
                    }
                    // if pattern is + then use result from left cell or above
                    // support of + is not working
                    else if (pattern[j - 1] == WILDCARD_ANY_CHAR_ONE_OR_MORE)
                    {
                        matrix[i, j] = matrix[i - 1, j] /* left */ || matrix[i, j - 1] /* or above */ ;
                    }
                }
            }
            return matrix[str.Length, maxDim2 - 1];
        }

        private string GetRidOfDuplicateOperator(string pp)
        {
            var p = pp;
            while (p.Contains(WILDCARD_ANY_CHAR_ZERO_OR_MORE_TWICE)) // Replace ** with *
                p = p.Replace(WILDCARD_ANY_CHAR_ZERO_OR_MORE_TWICE, WILDCARD_ANY_CHAR_ZERO_OR_MORE.ToString());
            while (p.Contains(WILDCARD_ANY_CHAR_ONE_OR_MORE_TWICE)) // Replace ++ with +
                p = p.Replace(WILDCARD_ANY_CHAR_ONE_OR_MORE_TWICE, WILDCARD_ANY_CHAR_ONE_OR_MORE.ToString());
            return p;
        }
    }
}
