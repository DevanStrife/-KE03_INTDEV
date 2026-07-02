using System.ComponentModel.DataAnnotations;

namespace MatrixInc.DataAccess.Models;

public class Address
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Street { get; set; } = string.Empty;

    [Required]
    [MaxLength(10)]
    public string HouseNumber { get; set; } = string.Empty;

    [MaxLength(10)]
    public string? ApartmentNumber { get; set; } // Optioneel

    [Required]
    [MaxLength(20)]
    public string PostalCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string City { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Province { get; set; } // Provincie voor courier navigatie

    [Required]
    [MaxLength(100)]
    public string Country { get; set; } = "Nederland";

    // Voor display
    public string FullAddress => 
        $"{Street} {HouseNumber}{(string.IsNullOrEmpty(ApartmentNumber) ? "" : $" ({ApartmentNumber})")}, {PostalCode} {City}, {Country}";
}
