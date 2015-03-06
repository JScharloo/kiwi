﻿using System.ComponentModel.DataAnnotations;
using Lisa.Kiwi.Web.App_GlobalResources.Resources;

namespace Lisa.Kiwi.Web.ViewModels
{
    public class LoginModel
    {
        // TODO: add error messages

        [Required]
        [Display(Name = "UserName", ResourceType = typeof(DisplayNames))]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password", ResourceType = typeof(DisplayNames))]
        public string Password { get; set; }
    }
}