namespace Genesis.Common;

public class AuditableEntity
{
    public int Id { get; set; }

    public DateTime CreatedTime { get; set; }
    
    public DateTime? UpdatedTime { get; set; }
}