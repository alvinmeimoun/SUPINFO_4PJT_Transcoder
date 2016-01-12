using Core.Transcoder.Repository;
using Core.Transcoder.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Transcoder.Repository
{
    public class UnitOfWork : IDisposable
    {
        private TRANSCODEREntities context = new TRANSCODEREntities();
        private TRACE_Repository traceRepository;
        private TASK_Repository taskRepository;
        private PARAM_TASK_STATUS_Repository paramTaskRepository;
        private FORMAT_Repository formatRepository;
        private USER_Repository userRepository;


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

