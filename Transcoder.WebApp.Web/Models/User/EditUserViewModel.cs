using Core.Transcoder.DataAccess;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Transcoder.WebApp.Web.Models.User
{
    public class EditUserViewModel
    {
        [Required]
        public int PK_ID_USER { get; set; }

        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Courrier électronique")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Display(Name ="Prénom")]
        public string Firstname { get; set; }

        [Required]
        [Display(Name ="Nom de famille")]
        public string Lastname { get; set; }

        public EditUserViewModel()
        {

        }

        public USER EditFromModel()
        {
            var user = new USER();
            user.PK_ID_USER = this.PK_ID_USER;
            user.USERNAME = this.Username;
            user.EMAIL = this.Email;
            user.PASSWORD = this.Password;
            user.FIRSTNAME = this.Firstname;
            user.LASTNAME = this.Lastname;

            return user;
        }

    }
}