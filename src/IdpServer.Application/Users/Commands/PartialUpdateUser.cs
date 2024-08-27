using System;
using MediatR;
using Microsoft.AspNetCore.JsonPatch;

namespace IdpServer.Application.User.Command
{
    public class PartialUpdateUser : IRequest<bool>
    {
        public Guid Id { get; set; }
        public JsonPatchDocument JsonPatchDocument { set; get; }

        public PartialUpdateUser(Guid id, JsonPatchDocument jsonPatchDocument)
        {
            Id = id;
            JsonPatchDocument = jsonPatchDocument;
        }
    }
}
