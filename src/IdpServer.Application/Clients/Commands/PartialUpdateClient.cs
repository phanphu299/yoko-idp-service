using MediatR;
using Microsoft.AspNetCore.JsonPatch;

namespace IdpServer.Application.Client.Command
{
    public class PartialUpdateClient : IRequest<bool>
    {
        public JsonPatchDocument JsonPatchDocument { set; get; }
        public PartialUpdateClient(JsonPatchDocument jsonPatchDocument)
        {
            JsonPatchDocument = jsonPatchDocument;
        }
    }
}
