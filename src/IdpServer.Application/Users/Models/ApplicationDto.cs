using System;
using System.Collections.Generic;

namespace IdpServer.Application.User.Model
{
    public class ApplicationDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public string Endpoint { get; set; }
        public IEnumerable<ProjectDto> Projects { get; set; }
        public ApplicationDto()
        {
            Projects = new List<ProjectDto>();
        }
    }
}