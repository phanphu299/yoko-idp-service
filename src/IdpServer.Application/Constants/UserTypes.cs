namespace IdpServer.Application.Constant
{
    public class UserTypes
    {
        public const string LOCAL = "la";
        public static string GUEST = "ga";
        public static readonly string[] VALID_LOCAL_LOGIN_USER_TYPES = new[] { LOCAL, GUEST };
    }
}
