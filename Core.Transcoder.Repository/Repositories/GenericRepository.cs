using Core.Transcoder.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Data.Entity;
using System.Diagnostics;

namespace Core.Transcoder.Repository
{
    public class GenericRepository<TEntity> where TEntity : class
    {
        internal TRANSCODEREntities context;
        internal DbSet<TEntity> dbSet;


        public GenericRepository(TRANSCODEREntities context)
        {
            this.context = context;
            this.dbSet = context.Set<TEntity>();
        }

        public virtual IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "" , bool asNoTracking = false)
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                if (asNoTracking)
                {
                    return orderBy(query).AsNoTracking().ToList();
                }
                else
                {
                    return orderBy(query).ToList();
                }
            }
            else
            {
                if (asNoTracking)
                {
                    return query.AsNoTracking().ToList();
                }
                else
                {
                    return query.ToList();
                }
            }
        }


        public virtual TEntity GetByID(object id)
        {
            return dbSet.Find(id);
        }

        public virtual bool Insert(TEntity entity)
        {
            try
            {
                dbSet.Add(entity);
                
                return true;
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                return false;
            }
          
        }

        public virtual void Delete(object id)
        {
            TEntity entityToDelete = dbSet.Find(id);
            Delete(entityToDelete);
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            if (context.Entry(entityToDelete).State == EntityState.Detached)
            {
                dbSet.Attach(entityToDelete);
            }
            dbSet.Remove(entityToDelete);
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            
            
            dbSet.Attach(entityToUpdate);
            context.Entry(entityToUpdate).State = EntityState.Modified;


        }
        public virtual void UpdateOnlyTask(TASK entityToUpdate)
        {
            var local = context.Set<TASK>().Local.FirstOrDefault(f => f.PK_ID_TASK == entityToUpdate.PK_ID_TASK);
            if(local != null)
            {
                context.Entry(local).State = EntityState.Detached;
            }
            context.Entry(entityToUpdate).State = EntityState.Modified;


        }
    }
}
