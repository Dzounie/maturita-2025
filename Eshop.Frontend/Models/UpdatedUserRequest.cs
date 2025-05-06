namespace Eshop.Frontend.Models;
using System.ComponentModel.DataAnnotations;


public class UpdatedUserRequest
{
    [Required(ErrorMessage = "Jméno je povinné.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email je povinný.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Telefon je povinný.")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Město je povinné.")]
    public string Town { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ulice je povinná.")]
    public string Street { get; set; } = string.Empty;

    [Required(ErrorMessage = "PSČ je povinné.")]
    public string Psc { get; set; } = string.Empty;
}