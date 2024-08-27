using System.Collections.Generic;
using IdentityServer4.Quickstart.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System.Linq;

namespace IdentityServer4.Quickstart.UI
{
    public class ImportModelStateAttribute : ModelStateTransfer
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var controller = filterContext.Controller as Controller;
            var modelErrorJson = controller.TempData[Key] as string;
            if (!string.IsNullOrEmpty(modelErrorJson))
            {
                var modelStateErrors = JsonConvert.DeserializeObject<List<ModelStateError>>(modelErrorJson);
                //Only Import if we are viewing
                if (filterContext.Result is ViewResult)
                {
                    foreach (var error in modelStateErrors) 
                    {
                        var duplicated = controller.ViewData.ModelState[error.Key];
                        if (duplicated == null || !duplicated.Errors.Any(e => e.ErrorMessage == error.Message))
                        {
                            controller.ViewData.ModelState.AddModelError(error.Key, error.Message);
                        }
                    }
                }
                else
                {
                    //Otherwise remove it.
                    controller.TempData.Remove(Key);
                }
            }

            base.OnActionExecuted(filterContext);
        }
    }
}
