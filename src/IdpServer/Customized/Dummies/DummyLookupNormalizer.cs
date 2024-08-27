using Microsoft.AspNetCore.Identity;

namespace IdpServer.Customized.Dummy
{
    public class DummyLookupNormalizer : ILookupNormalizer
    {
        public string NormalizeEmail(string email)
        {
            return email;
        }

        public string NormalizeName(string name)
        {
            return name;
        }
    }
}