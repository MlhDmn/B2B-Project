namespace B2B_Proje.DataAccess.Entities
{
    public abstract class BaseEntity
    {
        public int Id { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; 
        public bool IsActive { get; set; } = true;
        public int? CreatedByUserId { get; set; }
        public int? UpdatedByUserId { get; set; }
    }
}