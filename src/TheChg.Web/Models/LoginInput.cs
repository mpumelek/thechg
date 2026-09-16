using System.ComponentModel.DataAnnotations;

namespace TheChg.Web.Models;

public sealed class LoginInput
{
    [Required, EmailAddress, StringLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    public string? ReturnUrl { get; set; }
}
