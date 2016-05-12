using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
namespace Core.Transcoder.DataAccess.ViewModels
{
    public class ShortEditUserViewModel
    {
        [Required]
        public int PK_ID_USER { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Courrier électronique")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Prénom")]
        public string Firstname { get; set; }

        [Required]
        [Display(Name = "Nom de famille")]
        public string Lastname { get; set; }

        [Required]
        [Display(Name = "Nom d'utilisateur")]
        public string Username { get; set; }

        [Required]
        [Display(Name = "Mot de passe")]
        public string Password { get; set; }

        public ShortEditUserViewModel()
        {

        }

        public ShortEditUserViewModel(USER user)
        {
            PK_ID_USER = user.PK_ID_USER;
            Email = user.EMAIL;
            Firstname = user.FIRSTNAME;
            Lastname = user.LASTNAME;
        }

    }
}
