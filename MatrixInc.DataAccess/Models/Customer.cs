using System.ComponentModel.DataAnnotations;

namespace MatrixInc.DataAccess.Models;

public class Customer
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [Phone]
    [StringLength(20)]
    public string? PhoneNumber { get; set; }

    // Nieuw adres systeem
    public int? AddressId { get; set; }
    public Address? Address { get; set; }

    // Oude Address field - behouden voor backwards compatibility
    [StringLength(200)]
    [Obsolete("Use Address navigation property instead")]
    public string? AddressOld { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.Now;

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}

