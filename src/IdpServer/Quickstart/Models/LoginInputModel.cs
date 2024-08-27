// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Quickstart.Model
{
    public class LoginInputModel
    {

        [Display(Name = "COMMON.FIELD.EMAIL")]
        [CustomRequired]
        public string Username { get; set; }

        [CustomRequired]
        public string Password { get; set; }
        public string ReturnUrl { get; set; }
        public bool LockedOut { get; set; } = false;
    }
}
