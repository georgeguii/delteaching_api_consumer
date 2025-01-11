using del.shared.DTOs;
using System.ComponentModel.DataAnnotations;

namespace delteaching_dotnet.Domain;

public class BankAccount
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

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }


    public static BankAccount FromDTO(BankAccountDTO dto)
    {
        return new BankAccount
        {
            Branch = dto.Branch,
            Number = dto.Number,
            Type = dto.Type,
            HolderName = dto.HolderName,
            HolderEmail = dto.HolderEmail,
            HolderDocument = dto.HolderDocument,
            HolderType = dto.HolderType
        };
    }

    public void Update(BankAccountDTO dto)
    {
        if (!string.IsNullOrEmpty(dto.Branch))
            Branch = dto.Branch;

        if (!string.IsNullOrEmpty(dto.Number))
            Number = dto.Number;

        if (!string.IsNullOrEmpty(dto.Type))
            Type = dto.Type;

        if (!string.IsNullOrEmpty(dto.HolderName))
            HolderName = dto.HolderName;

        if (!string.IsNullOrEmpty(dto.HolderEmail))
            HolderEmail = dto.HolderEmail;

        if (!string.IsNullOrEmpty(dto.HolderDocument))
            HolderDocument = dto.HolderDocument;

        if (dto.HolderType != null)
            HolderType = dto.HolderType;

        UpdatedAt = DateTime.UtcNow;
    }
}
