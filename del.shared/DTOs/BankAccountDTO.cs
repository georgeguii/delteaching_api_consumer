using System.ComponentModel.DataAnnotations;

namespace del.shared.DTOs;
public class BankAccountDTO
{
    public int Id { get; set; }

    [Required(ErrorMessage = "campo obrigatório.")]
    [MinLength(4, ErrorMessage = "o campo deve ter exatamente 4 caracteres")]
    [MaxLength(4, ErrorMessage = "o campo deve ter exatamente 4 caracteres")]
    public string Branch { get; set; }

    [Required(ErrorMessage = "campo obrigatório.")]
    [MinLength(4, ErrorMessage = "o campo deve ter exatamente 4 caracteres")]
    [MaxLength(14, ErrorMessage = "o campo deve ter exatamente 4 caracteres")]
    public string Number { get; set; }

    [Required(ErrorMessage = "campo obrigatório.")]
    [StringLength(50, ErrorMessage = "o campo deve ter exatamente 4 caracteres")]
    public string Type { get; set; }

    [Required(ErrorMessage = "campo obrigatório.")]
    [StringLength(50, ErrorMessage = "o campo deve ter exatamente 4 caracteres")]
    public string HolderName { get; set; }

    [Required(ErrorMessage = "campo obrigatório.")]
    [StringLength(50, ErrorMessage = "o campo deve ter exatamente 4 caracteres")]
    public string HolderEmail { get; set; }

    [Required(ErrorMessage = "campo obrigatório.")]
    [StringLength(50, ErrorMessage = "o campo deve ter exatamente 4 caracteres")]
    public string HolderDocument { get; set; }

    public bool HolderType { get; set; }
}
