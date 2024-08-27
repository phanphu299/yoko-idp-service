namespace IdpServer.Application.Constant
{
    public static class SystemConfigKeys
    {
        public const string PASSWORD_LENGTH_ENABLED = "password.policy.length.enabled";
        public const string PASSWORD_LENGTH_MIN = "password.policy.length.min";
        public const string PASSWORD_LENGTH_MAX = "password.policy.length.max";
        public const string PASSWORD_HISTORY_ENABLED = "password.policy.history.enabled";
        public const string PASSWORD_HISTORY_NUMBER = "password.policy.history.number";
        public const string PASSWORD_HISTORY_LETTER_CHANGE_NUMBER = "password.policy.history_letter_change";
        public const string PASSWORD_EXPIRATION_ENABLED = "password.policy.expiration.enabled";
        public const string PASSWORD_EXPIRATION_DAY = "password.policy.expiration.day";
        public const string ACCOUNT_LOCKOUT_ENABLED = "password.policy.lockout.enabled";
        public const string ACCOUNT_LOCKOUT_ATTEMPT = "password.policy.lockout.attempt";
        public const string ACCOUNT_LOCKOUT_DURATION = "password.policy.lockout.duration";
        public const string PASSWORD_COMPLEXITY_ENABLED = "password.policy.complexity.enabled";
        public const string PASSWORD_COMPLEXITY_LOWERCASE_MIN = "password.policy.complexity.lowercase_min";
        public const string PASSWORD_COMPLEXITY_UPPERCASE_MIN = "password.policy.complexity.uppercase_min";
        public const string PASSWORD_COMPLEXITY_SPECIAL_CHAR_MIN = "password.policy.complexity.special_char_min";
        public const string PASSWORD_COMPLEXITY_NUMERIC_CHAR_MIN = "password.policy.complexity.numeric_char_min";
    }
}
