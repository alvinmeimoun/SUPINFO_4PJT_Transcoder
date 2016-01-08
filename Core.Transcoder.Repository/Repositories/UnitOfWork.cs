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
       
        public TRACE_Repository TraceRepository
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
        public TASK_Repository TaskRepository
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
        //public GenericRepository<Course> CourseRepository
        //{
        //    get
        //    {

        //        if (this.courseRepository == null)
        //        {
        //            this.courseRepository = new GenericRepository<Course>(context);
        //        }
        //        return courseRepository;
        //    }
        //}

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

