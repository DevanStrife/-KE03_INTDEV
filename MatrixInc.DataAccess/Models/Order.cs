using System.ComponentModel.DataAnnotations;

namespace MatrixInc.DataAccess.Models;

public class Order
{
    public int Id { get; set; }

    [Required]
    public int CustomerId { get; set; }

    public Customer Customer { get; set; } = null!;

    public DateTime OrderDate { get; set; } = DateTime.Now;

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal TotalAmount { get; set; }

    [Required]
    [StringLength(50)]
    public string Status { get; set; } = OrderStatus.InBewerking;

    [StringLength(500)]
    public string? Notes { get; set; }

    // Tracking voor courier
    public int? AssignedCourierId { get; set; } // Kan later uitgebreid worden met Courier user

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}

// Order status constants
public static class OrderStatus
{
    public const string InBewerking = "In Bewerking";           // Net geplaatst
    public const string KlaarAanHetMaken = "Klaar aan het Maken"; // Wordt ingepakt
    public const string KlaarVoorLevering = "Klaar voor Levering"; // Klaar, wacht op courier
    public const string Onderweg = "Onderweg";                  // Courier is bezig met levering
    public const string Afgeleverd = "Afgeleverd";            // Succesvol afgeleverd
    public const string Geannuleerd = "Geannuleerd";          // Geannuleerd
}

