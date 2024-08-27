namespace IdpServer.Application.Constant
{
    public static class DefaultOptions
    {
        public const string PASSWORD_LENGTH_ENABLED = "true";
        public const string PASSWORD_LENGTH_MIN = "8";
        public const string PASSWORD_LENGTH_MAX = "128";
        public const string PASSWORD_HISTORY_ENABLED = "false";
        public const string PASSWORD_HISTORY_NUMBER = "5";
        public const string PASSWORD_HISTORY_LETTER_CHANGE_NUMBER = "2";
        public const string PASSWORD_EXPIRATION_ENABLED = "false";
        public const string PASSWORD_EXPIRATION_DAY = "90";
        public const string ACCOUNT_LOCKOUT_ENABLED = "false";
        public const string ACCOUNT_LOCKOUT_ATTEMPT = "5";
        public const string ACCOUNT_LOCKOUT_DURATION = "15";
        public const string PASSWORD_COMPLEXITY_ENABLED = "true";
        public const string PASSWORD_COMPLEXITY_LOWERCASE_MIN = "1";
        public const string PASSWORD_COMPLEXITY_UPPERCASE_MIN = "1";
        public const string PASSWORD_COMPLEXITY_SPECIAL_CHAR_MIN = "1";
        public const string PASSWORD_COMPLEXITY_NUMERIC_CHAR_MIN = "1";
    }
}
