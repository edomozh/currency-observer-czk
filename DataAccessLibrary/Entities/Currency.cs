using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLibrary.Entities
{
    public class Currency
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public ulong Id { get; set; }

        [Required]
        [MinLength(3)]
        [StringLength(3)]
        public string Code { get; set; } = string.Empty;

        public int Multiplier { get; set; }
    }
}
