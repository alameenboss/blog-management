namespace Applogiq.Common.EFCore.Model
{
    public interface IEntity
    {
        public int Id { get; set; }
        DateTime CreateDate { get; set; }
        string CreatedBy { get; set; }
        string ModifiedBy { get; set; }
        DateTime ModifiedDate { get; set; }
    }
}