using System.ComponentModel.DataAnnotations;

namespace Metavystic.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Range(0, 1000000)]
        public decimal Salary { get; set; }

        [Required]
        [StringLength(50)]
        public string Department { get; set; }

        // Returned from DB (view + function)
        public decimal AnnualSalary { get; set; }
    }
}

