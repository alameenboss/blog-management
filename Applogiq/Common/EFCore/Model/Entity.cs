using Newtonsoft.Json;

namespace Applogiq.Common.EFCore.Model
{
    public class Entity : IEntity
    {
        public int Id { get; set; }

        [JsonIgnore]
        public DateTime CreateDate { get; set; }
        [JsonIgnore]
        public string? CreatedBy { get; set; }
        [JsonIgnore]
        public DateTime ModifiedDate { get; set; }
        [JsonIgnore]
        public string? ModifiedBy { get; set; }
    }
}