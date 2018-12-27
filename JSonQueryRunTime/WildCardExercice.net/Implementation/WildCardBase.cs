namespace WildCardExercice.net
{
    public class WildCardBase 
    {
        protected const char WILDCARD_ANY_CHAR_ZERO_OR_MORE = '*';
        protected const char WILDCARD_ANY_CHAR_ONE_OR_MORE  = '+';
        protected const char WILDCARD_ANY_CHAR              = '?';

        protected string WILDCARD_ANY_CHAR_ZERO_OR_MORE_TWICE = WILDCARD_ANY_CHAR_ZERO_OR_MORE.ToString()+WILDCARD_ANY_CHAR_ZERO_OR_MORE.ToString();
        protected string WILDCARD_ANY_CHAR_ONE_OR_MORE_TWICE = WILDCARD_ANY_CHAR_ONE_OR_MORE.ToString()+WILDCARD_ANY_CHAR_ONE_OR_MORE.ToString();
    }
}
