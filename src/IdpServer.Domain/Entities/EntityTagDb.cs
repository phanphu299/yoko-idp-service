using System.ComponentModel.DataAnnotations.Schema;
using AHI.Infrastructure.Service.Tag.Model;

namespace IdpServer.Domain.Entity
{
    public class EntityTagDb : EntityTag
    {
        public Client Client { get; set; }
    }
}