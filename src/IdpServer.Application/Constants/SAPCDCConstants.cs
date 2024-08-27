namespace IdpServer.Application.Constant
{
    public class SAPCDCConstants
    {
        public const string ENDPOINT_LOGIN = "accounts.login";
        public const string ENDPOINT_RESET_PASSWORD = "accounts.resetPassword";
        public const string ENDPOINT_FINALIZE = "accounts.finalizeRegistration";
        public const string CONTENT_API_KEY = "apiKey";
        public const string CONTENT_USER_KEY = "userKey";
        public const string CONTENT_SECRET = "secret";
        public const string CONTENT_HTTP_STATUS_CODE = "httpStatusCodes";
        public const string CONTENT_FORMAT = "format";
        public const string ENDPOINT_UPDATE = "accounts.setAccountInfo";
        public const string ENDPOINT_REGISTER = "accounts.register";
        public const string ENDPOINT_SEARCH = "accounts.search";
        public const int ERROR_CODE_PASSWORD_INVALID = 400006;
        public const int ERROR_CODE_EMAIL_ALREADY_EXSITS = 400003;
    }
}
