namespace IdpServer.Application.User.Model
{
    public class ProjectDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsMigrated { get; set; }
        public bool Deleted { get; set; }
    }
}