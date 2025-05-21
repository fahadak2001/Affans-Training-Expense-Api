using System.ComponentModel.DataAnnotations;

namespace ExpenseBackend.Models
{
    public class Expense
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Value is required.")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Value is required.")]
        public double Value { get; set; }

        public string? Description { get; set; }
    }
}