using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Core.Transcoder.DataAccess.ViewModels
{
    public class CreateTaskViewModel
    {
        [Required]
        [Display(Name = "Url du fichier")]
        public string FILE_URL { get; set; }

        [Required]
        [Display(Name = "Utilisateur")]
        public int FK_ID_USER { get; set; }
        [Required]
        [Display(Name = "Type de conversion")]
        public int FK_ID_FORMAT_TYPE { get; set; }
        [Required]
        [Display(Name = "Format de base")]
        public int FK_ID_FORMAT_BASE { get; set; }
        [Required]
        [Display(Name = "Format de conversion")]
        public int FK_ID_FORMAT_TO_CONVERT { get; set; }
        //public Nullable<bool> IS_PAID { get; set; }
        [Required]
        [Display(Name = "Taille du fichier")]
        public int LENGTH { get; set; }

        public ShortEditUserViewModel ShortEditUserViewModel { get; set; }

        public List<FORMAT> ListAvailableFormats { get; set; }

        public IEnumerable<SelectListItem> slAvailableFormats
        {
            get
            {
                return ListAvailableFormats.Select(x => new SelectListItem()
                {
                    Text = x.FORMAT_NAME,
                    Value = x.PK_ID_FORMAT.ToString()
                });
            }
        }

        public List<FORMAT_TYPE> ListAvailableFormatTypes { get; set; }

        public IEnumerable<SelectListItem> slAvailableFormatsTypes
        {
            get
            {
                return ListAvailableFormatTypes.Select(x => new SelectListItem()
                {
                    Text = x.FORMAT_TYPE_NAME,
                    Value = x.PK_ID_FORMAT_TYPE.ToString()
                });
            }
        }

        public CreateTaskViewModel()
        {

        }
        public CreateTaskViewModel(int userId,List<FORMAT_TYPE> listFormatsTypes, List<FORMAT> listFormats, ShortEditUserViewModel userModel)
        {
            this.FK_ID_USER = userId;
            this.ListAvailableFormatTypes = listFormatsTypes;
            this.ListAvailableFormats = listFormats;
            this.ShortEditUserViewModel = userModel;
        }

    }
}