using Analytics.Core.Domain.Entities.Base;

namespace Analytics.Core.Domain.Entities
{
    public class User : BaseEntity<int>
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
    }
}
