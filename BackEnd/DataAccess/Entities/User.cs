using B2B_Proje.DataAccess.Enums;

namespace B2B_Proje.DataAccess.Entities
{
    public class User : BaseEntity
    {
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty; 
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        
        public UserRole Permissions { get; set; } = UserRole.ViewProducts; 
        
        public DateTime? LastLoginDate { get; set; } 
    }
}