namespace Function.Service.Message
{
    public class CreateUserMessage
    {
        public string UserName { get; set; }
        public string TenantId { get; set; }
        public string TenantName { get; set; }
    }
}