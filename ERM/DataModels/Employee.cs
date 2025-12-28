using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Metrics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Organization.DataModels
{
    [Table("Employee")]
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        // 1. Profile Picture (store as Base64 or file path)
        [Display(Name = "Profile Picture")]
        public string? ProfilePicture { get; set; }

        // 2. employee Name (first, middle, last)
        [Required(ErrorMessage = "First name is required")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [StringLength(50, ErrorMessage = "Middle name cannot exceed 50 characters")]
        [Display(Name = "Middle Name")]
        public string? MiddleName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        // 3. Father Name
        [Required(ErrorMessage = "Father's name is required")]
        [StringLength(100, ErrorMessage = "Father's name cannot exceed 100 characters")]
        [Display(Name = "Father's Name")]
        public string? FatherName { get; set; }

        // 4. Gender
        [Required(ErrorMessage = "Gender is required")]
        [Display(Name = "Gender")]
        public string? Gender { get; set; } // values: "Male" or "Female"

        // 5. Date of Birth
        [Required(ErrorMessage = "Date of Birth is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime? DOB { get; set; }

        // 6. Date of Joining
        [Required(ErrorMessage = "Date of Joining is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Joining")]
        public DateTime? DOJ { get; set; }

        // 7. Date of Expiry (Contract End)
        
        [DataType(DataType.Date)]
        [Display(Name = "Date of Contract Expiry")]
        public DateTime? DOE { get; set; } //Nullable, because employee may not have exited yet

        // 8. Email
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [UniqueEmail]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        // 9. Salary
        [Required(ErrorMessage = "Salary is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Salary cannot be negative")]
        [Display(Name = "Salary")]
        [Column(TypeName = "decimal(18,2)")] // optional: specifies precision
        public decimal Salary { get; set; }

        // 10. Phone Number (with country code)
        [Display(Name = "Phone Number")]
        //add country code list
        [Required(ErrorMessage = "Country code is required")]
        public string? CountryCode { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Phone number must contain only digits")]
        public string? PhoneNumber { get; set; }

        // 11. Description / Remark
        [StringLength(250, ErrorMessage = "Description cannot exceed 250 characters")]
        [RegularExpression(@"^[a-zA-Z\s,\.]+$", ErrorMessage = "Description can contain only letters, spaces, commas, and periods")]
        [Display(Name = "Remarks / Description")]
        public string? Description { get; set; }

        // 12. Document Upload (up to 4 files)

        [Display(Name = "Document 1")]
        public string? Document1 { get; set; }

        [Display(Name = "Document 2")]
        public string? Document2 { get; set; }

        [Display(Name = "Document 3")]
        public string? Document3 { get; set; }

        [Display(Name = "Document 4")]
        public string? Document4 { get; set; }

        //country code and phone number separated by a space instead of a hyphen
        [NotMapped]
        public string? FullPhoneNumber =>
            string.IsNullOrWhiteSpace(CountryCode) ? PhoneNumber : $"{CountryCode} {PhoneNumber}";

        [NotMapped]
        public string? TempProfilePicture { get; set; }
    }
}
