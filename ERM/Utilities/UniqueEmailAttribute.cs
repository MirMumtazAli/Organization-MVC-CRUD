using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Organization.Data; // your DbContext namespace

public class UniqueEmailAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value == null)
            return ValidationResult.Success;

        var context = (OrganizationDbContext)validationContext.GetService(typeof(OrganizationDbContext));
        var email = value.ToString();

        // Check if email already exists
        bool exists = context.Emploees.Any(e => e.Email == email);

        if (exists)
        {
            return new ValidationResult("Email already exists");
        }

        return ValidationResult.Success;
    }
}
