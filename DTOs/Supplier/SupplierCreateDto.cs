using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.DTOs.Supplier;
public class SupplierCreateDto
{
    [Required]
    [StringLength(200)]
    public string? Name { get; set; }

    [Required]
    [EmailAddress]
    [StringLength(200)]
    public string? ContactEmail { get; set; }

    [Required]
    [StringLength(50)]
    public string? PhoneNumber { get; set; }

    [StringLength(500)]
    public string? Address { get; set; }
}   