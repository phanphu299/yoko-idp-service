using Microsoft.AspNetCore.Mvc.Filters;

namespace IdentityServer4.Quickstart.UI
{
    public abstract class ModelStateTransfer : ActionFilterAttribute
    {
        protected static readonly string Key = typeof(ModelStateTransfer).FullName;
    }
}
