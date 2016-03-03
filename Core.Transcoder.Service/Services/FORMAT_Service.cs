using Core.Transcoder.DataAccess;
using Core.Transcoder.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Core.Transcoder.Service.Services;

namespace Core.Transcoder.Service
{
    public class FORMAT_Service : BaseService
    {

        public FORMAT GetFormatById(int Id)
        {
           return UoW.FORMAT_Repository.GetByID(Id);
            
        }
        public FORMAT GetFormatByName(string Name)
        {
            return UoW.FORMAT_Repository.Get(x => x.FORMAT_NAME == Name).FirstOrDefault();
        }

        public List<FORMAT> GetAll()
        {
            return UoW.FORMAT_Repository.Get(null, x => x.OrderBy(s => s.PK_ID_FORMAT),"").ToList();
        }

        public List<SelectListItem> GetSelectListFormatByFormatTypeIdAndFormatBase(int formatTypeId, int formatBase)
        {
            var listFormat =  UoW.FORMAT_Repository.Get(x => x.FK_ID_FORMAT_TYPE == formatTypeId && x.FK_ID_FORMAT_BASE == formatBase).ToList();
            List<SelectListItem> slFormat = listFormat.Select(x => new SelectListItem()
            {
                Text = x.FORMAT_NAME,
                Value = x.PK_ID_FORMAT.ToString()
            }).ToList();

            return slFormat;

        }
    }
}
