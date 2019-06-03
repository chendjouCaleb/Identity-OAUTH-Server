using System;

namespace Everest.Identity.Core.Models
{
    public abstract class Entity<TID> : IEntity<TID>
    {
        public TID Id { get; set; }
        
        public DateTime RegistrationDate { get; set; } = DateTime.Now;


        public override bool Equals(object obj)
        {
            Entity<TID> o = obj as Entity<TID>;
            if (o == null)
            {
                return false;
            }
            if (!o.GetType().Equals(this.GetType()))
                return false;
            return Id.Equals(o.Id);
        }

        /// <summary>
        /// Pour vérifier l'égalité en deux objects explicitement de type Entity.
        /// </summary>
        /// <param name="obj">Object de comparaison.</param>
        /// <returns>true si les les objects sont égaux.</returns>
        public bool Equal(Entity<TID> obj)
        {
            return Equals(obj);
        }

        public bool NotEquals(Entity<TID> obj) => !Equals(obj);

        public override int GetHashCode()
        {
            return (base.GetHashCode().ToString() + Id.ToString()).GetHashCode();
        }
    }
}
