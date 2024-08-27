namespace IdpServer.Domain.Entity
{
    public class ClientGrantType
    {
        public int Id { get; set; }
        public string GrantType { get; set; }
        public int ClientId { get; set; }
    }
}
