using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PuebloBonitoApi.Domain
{
    public abstract class BaseEntity
    {
        [Key]
        public Guid Id { get; private set; } = Guid.NewGuid();

        [Column("created_on")]
        public DateTimeOffset CreatedOn { get; private set; }

        [Column("created_by")]
        public string? CreatedBy { get; private set; }

        [Column("last_modified_on")]
        public DateTimeOffset? LastModifiedOn { get; private set; }

        [Column("last_modified_by")]
        public string? LastModifiedBy { get; private set; }

        [Column("is_deleted")]
        public bool IsDeleted { get; private set; }

        public void UpdateCreationInfo(DateTimeOffset createdOn)
        {
            CreatedOn = createdOn;
        }

        public void UpdateModificationInfo(DateTimeOffset lastModifiedOn)
        {
            LastModifiedOn = lastModifiedOn;
        }
    }
}
