using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Core.Transcoder.DataAccess.ViewModels
{
    public class ListTaskViewModel
    {
        [Required]
        [Display(Name = "Id")]
        public int PK_ID_TASK { get; set; }
        [Required]
        [Display(Name = "Url du fichier")]
        public string FILE_URL_ACCESS { get; set; }
        [Required]
        [Display(Name = "Utilisateur")]
        public int FK_ID_USER { get; set; }
        //[Required]
        //[Display(Name = "Type de conversion")]
        //public string FORMAT_TYPE { get; set; }
        [Required]
        [Display(Name = "Format")]
        public string FORMAT_BASE { get; set; }
        [Required]
        [Display(Name = "Format de conversion")]
        public string FORMAT_CONVERT { get; set; }


        [Required]
        [Display(Name = "Taille du fichier")]
        public int LENGTH { get; set; }

        [Required]
        [Display(Name = "Statut de la conversion")]
        public string STATUS { get; set; }


        public ListTaskViewModel()
        {


        }
    }
}