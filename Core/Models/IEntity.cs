

namespace Everest.Identity.Core.Models
{
    /// <summary>
    /// Interface de base pour les entités de l'application.
    /// Toutes les autres entités de l'application
    /// devront implémenter cette interface.
    /// </summary>
    /// <typeparam name="TID">Le type de l'ID de l'entité.</typeparam>
    public interface IEntity<TID>
    {
        TID Id { get; set; }
    }
}
