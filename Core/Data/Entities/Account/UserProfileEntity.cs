using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Data.Entities {
    [Table(name: "UserProfiles")]
    public class UserProfileEntity: AuditableEntity<long> {
        [MaxLength(64)]
        public string Name { get; set; }
        /// <summary>
        /// Фамилия
        /// </summary>
        [MaxLength(64)]
        public string SurName { get; set; }

        /// <summary>
        /// Отчество
        /// </summary>
        [MaxLength(64)]
        public string MiddleName { get; set; }

        [MaxLength(2048)]
        public string Description { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }
    }
}
