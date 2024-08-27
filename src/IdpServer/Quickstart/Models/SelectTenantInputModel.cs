using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IdentityServer4.Quickstart.Model
{
    public class SelectTenantInputModel
    {
        [Display(Name = "PAGE.SELECT_TENANT.FIELD.TENANT")]
        [CustomRequired]
        [BindProperty]
        public string SelectedTenant { get; set; }
        public string Username { get; set; }
        public string ReturnUrl { get; set; }
        public SelectList Tenants { get; set; }
    }
}
