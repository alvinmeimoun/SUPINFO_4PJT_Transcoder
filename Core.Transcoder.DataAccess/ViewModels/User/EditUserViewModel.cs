using Core.Transcoder.DataAccess;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Core.Transcoder.DataAccess.ViewModels
{
    public class EditUserViewModel
    {
      
        public int Pk_id_user { get; set; }
        
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Courrier électronique")]
        public string Email { get; set; }
        
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

        public EditUserViewModel(USER user)
        {
            Pk_id_user = user.PK_ID_USER;
            Username = user.USERNAME;
            Email = user.EMAIL;
            Password = user.PASSWORD;
            Firstname = user.FIRSTNAME;
            Lastname = user.LASTNAME;
        }

    }
}