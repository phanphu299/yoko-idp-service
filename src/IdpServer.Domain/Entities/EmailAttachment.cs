using System;
using AHI.Infrastructure.Repository.Model.Generic;

namespace IdpServer.Domain.Entity
{
    public class EmailAttachment : IEntity<Guid>
    {
        public Guid Id { get; set; }
        public Guid EmailTemplateId { get; set; }

        /// <summary>
        ///     Gets or sets the content-disposition of the attachment specifying how you would
        ///     like the attachment to be displayed. For example, "inline" results in the attached
        ///     file being displayed automatically within the message while "attachment" results
        ///     in the attached file requiring some action to be taken before it is displayed
        ///     (e.g. opening or downloading the file). Defaults to "attachment". Can be either
        ///     "attachment" or "inline".
        /// </summary>
        public string Disposition { get; set; }
        public string FilePath { get; set; }
    }
}
