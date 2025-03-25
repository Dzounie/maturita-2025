namespace Eshop.Frontend.Models;
using System.ComponentModel.DataAnnotations;

public class User
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Jméno je povinné.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email je povinný.")]
    [EmailAddress(ErrorMessage = "Neplatný formát emailu.")]
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = "User";
    public string TempRole { get; set; } = string.Empty;
    public bool IsHardcodedAdmin { get; set; } = false;

    [Required(ErrorMessage = "Telefon je povinný.")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Heslo je povinné.")]
    [StringLength(16, MinimumLength = 8, ErrorMessage = "Heslo musí mít 8-16 znaků.")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Město je povinné.")]
    public string Town { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ulice je povinná.")]
    public string Street { get; set; } = string.Empty;

    [Required(ErrorMessage = "PSČ je povinné.")]
    public string Psc { get; set; } = string.Empty;
}