namespace IdpServer.Domain.Entity
{
    public class ClientRedirectUris
    {
        public int Id { get; set; }
        public string RedirectUri { get; set; }
        public int ClientId { get; set; }
    }
}
