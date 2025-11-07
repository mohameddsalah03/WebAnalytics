namespace Analytics.Core.Domain.Entities.Base
{
    public abstract class BaseEntity<TKey>
            where TKey : IEquatable<TKey>
    {
        public TKey Id { get; set; } = default!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
