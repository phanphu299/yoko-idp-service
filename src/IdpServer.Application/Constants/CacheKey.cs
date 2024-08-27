namespace IdpServer.Application.Constant
{
    public class CacheKey
    {
        public const string CLIENT = "idp_client:{0}"; //0: clientId
        public const string HASH_FIELD_ROLES = "roles";
        public const string HASH_FIELD_INFO = "info";
        public const string HASH_FIELD_STORE = "store";
        public const string REQUIRED_CONSENT = "{0}_{1}_{2}_required_consent_{3}"; // tenantId, subscriptionId, projectId, upn
    }
}