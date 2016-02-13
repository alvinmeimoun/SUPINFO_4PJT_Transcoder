using Core.Transcoder.DataAccess;
using Core.Transcoder.Repository;
using Core.Transcoder.Service.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Core.Transcoder.Service
{
    public class FORMAT_TYPE_Service
    {
        public UnitOfWork UoW = new UnitOfWork();


        public List<SelectListItem> GetSelectListFormatTypeByFormat(int fkFormatId)
        {
            var listFormat = new List<FORMAT_TYPE>();
            if (fkFormatId == (int)EnumManager.FORMAT_TYPE.VIDEO)
            {
                listFormat = UoW.FORMAT_TYPE_Repository.Get(x => x.PK_ID_FORMAT_TYPE == fkFormatId || x.PK_ID_FORMAT_TYPE == (int)EnumManager.FORMAT_TYPE.VIDEOTOAUDIO).ToList();
            }
            else
            {
                listFormat = UoW.FORMAT_TYPE_Repository.Get(x => x.PK_ID_FORMAT_TYPE == fkFormatId).ToList();
            }
            List<SelectListItem> slFormat = listFormat.Select(x => new SelectListItem()
            {
                Text = x.FORMAT_TYPE_NAME,
                Value = x.PK_ID_FORMAT_TYPE.ToString()
            }).ToList();

            return slFormat;
        }
    }
}
