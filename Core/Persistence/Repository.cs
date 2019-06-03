using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Everest.Identity.Core.Persistence
{
    public class Repository<T, TID> : IRepository<T, TID> where T : class
    {
        public DbSet<T> Collections { get; set; }
        protected DbContext context;

        public Repository(DbContext dataContext)
        {
            context = dataContext;
            Collections = context.Set<T>();
        }

        public T Find(TID id) {

            T entity = Collections.Find(id);

            if (entity == null)
            {
                throw new EntityNotFoundException
                    ($"Aucune du type {typeof(T)} avec l'ID {id} n'a été trouvée");
            }

            return entity;
        }


        public T First(Func<T, bool> predicate) => Collections.FirstOrDefault(predicate);

        public IList<T> List()
        {
            return Collections.ToList();
        }


        public IList<T> List(Func<T, bool> predicate)
        {
            return Collections.Where(predicate).ToList();
        }


        public IList<T> List(Func<T, bool> predicate, int page, int size)
        {
            return Collections.Where(predicate).ToList().Skip((page - 1) * size).Take(size).ToList();
        }

        public T Save(T obj)
        {
            EntityEntry<T> entry = Collections.Add(obj);
            context.SaveChanges();
            return entry.Entity;
        }
        
        public void Update(T obj)
        {
            Collections.Update(obj);
            context.SaveChanges();
        }
       
        public void Refresh(T obj)
        {
            context.Entry(obj).Reload();
        }
       
       
        public bool Exists(T obj) => Collections.Any(t => t.Equals(obj));


        public bool Exists(Func<T, bool> predicate) => Collections.Any(predicate);

        public long Count()
        {
            return Collections.Count();
        }

        public long Count(Func<T, bool> predicate)
        {
            return Collections.Count(predicate);
        }
        
        
        

        public void Delete(T obj)
        {
            Collections.Remove(obj);
            context.SaveChanges();
        }


        public void DeleteAll()
        {
            Collections.RemoveRange(Collections.ToArray());
            context.SaveChanges();
        }


        public void Delete(Func<T, bool> predicate)
        {
            IList<T> objects = List(predicate);
            Collections.RemoveRange(objects);
        }
    }
}
