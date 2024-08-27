using System.Collections.Generic;
using IdentityServer4.Quickstart.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace IdentityServer4.Quickstart.UI
{
    public class ExportModelStateAttribute : ModelStateTransfer
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var controller = filterContext.Controller as Controller;
            //Only export when ModelState is not valid
            if (!controller.ViewData.ModelState.IsValid)
            {
                //Export if we are redirecting
                if ((filterContext.Result is RedirectResult) 
                    || (filterContext.Result is RedirectToRouteResult) 
                    || (filterContext.Result is RedirectToActionResult) 
                    || (filterContext.Result is RedirectToPageResult))
                {

                    var modelStateErrorList = new List<ModelStateError>();
                    foreach (var entry in controller.ViewData.ModelState)
                    {
                        foreach (var error in entry.Value.Errors)
                        {
                            modelStateErrorList.Add(new ModelStateError()
                            {
                                Key = entry.Key,
                                Message = error.ErrorMessage
                            });
                        }
                    }
                    controller.TempData[Key] = JsonConvert.SerializeObject(modelStateErrorList);
                }
            }

            base.OnActionExecuted(filterContext);
        }
    }
}
