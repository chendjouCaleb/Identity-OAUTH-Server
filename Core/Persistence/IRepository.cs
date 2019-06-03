using System;
using System.Collections.Generic;

namespace Everest.Identity.Core.Persistence
{
    public interface IRepository<T, I> 
    {
        T Find(I id);
        T First(Func<T, bool> predicate);

        T Save(T obj);

        void Update(T obj);

        void Refresh(T obj);

        void Delete(T obj);
        void DeleteAll();
        void Delete(Func<T, bool> predicate);

        bool Exists(T obj);
        bool Exists(Func<T, bool> predicate);

        long Count();
        long Count(Func<T, bool> predicate);
   

        IList<T> List();
        IList<T> List(Func<T, bool> predicate);
        IList<T> List(Func<T, bool> predicate, int page, int size);
    }
}
