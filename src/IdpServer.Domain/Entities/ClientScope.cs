namespace IdpServer.Domain.Entity
{
    public class ClientScope
    {
        public int Id { get; set; }
        public string Scope { get; set; }
        public int ClientId { get; set; }
    }
}
