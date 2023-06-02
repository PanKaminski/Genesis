namespace Genesis.Common
{
    public class ManagedEntity : AuditableEntity
    {
        public int? ChangedByAccount { get; set; }
    }
}
