﻿using Core.Transcoder.DataAccess;
using Core.Transcoder.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Transcoder.Service.Services;

namespace Core.Transcoder.Service
{
    public class TRANSACTION_Service : BaseService
    {

        public bool AddOrUpdateTransaction(TRANSACTION transaction)
        {
            if (transaction.PK_ID_TRANSACTION != 0)
            {

                UoW.TRANSACTION_Repository.Update(transaction);
                UoW.Save();
                return true;
            }
            else
            {
                UoW.TRANSACTION_Repository.Insert(transaction);
                UoW.Save();
                return true;
            }
        }
    }
}
