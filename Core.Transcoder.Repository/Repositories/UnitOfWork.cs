using Core.Transcoder.Repository;
using Core.Transcoder.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Transcoder.Repository.Repositories;

namespace Core.Transcoder.Repository
{
    public class UnitOfWork : IDisposable
    {
        
        private static TRANSCODEREntities _context;

        public static TRANSCODEREntities context
        {
            get
            {
                if (_context == null)
                {
                    _context = new TRANSCODEREntities();
                }
                return _context;
            }
        }

        private TRACE_Repository traceRepository;
        private TASK_Repository taskRepository;
        private PARAM_TASK_STATUS_Repository paramTaskRepository;
        private PARAM_SPLIT_Repository paramSplitRepository;
        private PARAM_LENGTH_Repository paramLengthRepository;
        private FORMAT_Repository formatRepository;
        private FORMAT_TYPE_Repository formatTypeRepository;
        private USER_Repository userRepository;
        private CONFIG_Repository configRepository;

        public TRACE_Repository TRACE_Repository
        {
            get
            {
                if (this.traceRepository == null)
                {
                    this.traceRepository = new TRACE_Repository(context);
                }
                return traceRepository;
            }
        }
        public TASK_Repository TASK_Repository
        {
            get
            {
                if (this.taskRepository == null)
                {
                    this.taskRepository = new TASK_Repository(context);
                }
                return taskRepository;
            }
        }
        public PARAM_TASK_STATUS_Repository PARAM_TASK_STATUS_Repository
        {
            get
            {
                if (this.paramTaskRepository == null)
                {
                    this.paramTaskRepository = new PARAM_TASK_STATUS_Repository(context);
                }
                return paramTaskRepository;
            }
        }
        public PARAM_SPLIT_Repository PARAM_SPLIT_Repository
        {
            get
            {
                if (this.paramSplitRepository == null)
                {
                    this.paramSplitRepository = new PARAM_SPLIT_Repository(context);
                }
                return paramSplitRepository;
            }
        }
        public PARAM_LENGTH_Repository PARAM_LENGTH_Repository
        {
            get
            {
                if (this.paramLengthRepository == null)
                {
                    this.paramLengthRepository = new PARAM_LENGTH_Repository(context);
                }
                return paramLengthRepository;
            }
        }
        public FORMAT_Repository FORMAT_Repository
        {
            get
            {
                if (this.formatRepository == null)
                {
                    this.formatRepository = new FORMAT_Repository(context);
                }
                return formatRepository;
            }
        }
        public FORMAT_TYPE_Repository FORMAT_TYPE_Repository
        {
            get
            {
                if (this.formatTypeRepository == null)
                {
                    this.formatTypeRepository = new FORMAT_TYPE_Repository(context);
                }
                return formatTypeRepository;
            }
        }
        public USER_Repository USER_Repository
        {
            get
            {
                if (this.userRepository == null)
                {
                    this.userRepository = new USER_Repository(context);
                }
                return userRepository;
            }
        }
        public CONFIG_Repository CONFIG_Repository
        {
            get
            {
                if (this.configRepository == null)
                {
                    this.configRepository = new CONFIG_Repository(context);
                }
                return configRepository;
            }
        }


        public void Save()
        {
            context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

