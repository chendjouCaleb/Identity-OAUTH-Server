using Everest.Identity.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Everest.Identity.Core.Persistence
{
    public class ListRepository<T, TID> : IRepository<T, TID> where T : class, IEntity<TID>
    {
        
        private List<T> Collections { get; set; }

        public ListRepository()
        {
            Collections = new List<T>();
        }

        public ListRepository(List<T> collections)
        {
            Collections = collections;
        }

        public T Find(TID id)
        {

            T entity = Collections.Find(t => t.Id.Equals(id));

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
            Collections.Add(obj);
            return obj;
        }

        public void Update(T obj)
        {
           
        }

        public void Refresh(T obj)
        {
            
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
        }


        public void DeleteAll()
        {
            Collections.Clear();
        }


        public void Delete(Func<T, bool> predicate)
        {
            IList<T> objects = List(predicate);
            foreach(T o in objects)
            {
                Collections.Remove(o);
            }
        }
    }
}
