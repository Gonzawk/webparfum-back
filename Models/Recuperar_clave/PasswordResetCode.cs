// Models/Recuperar_clave/PasswordResetCode.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebParfum.API.Models; // Para la relación con Usuario

namespace WebParfum.API.Models.Recuperar_clave
{
    public class PasswordResetCode
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ResetCodeId { get; set; } // Clave primaria

        [Required]
        public int UsuarioId { get; set; }   // Clave foránea a Usuario

        [Required]
        [MaxLength(10)]
        public string Code { get; set; }     // Código de 4 dígitos

        [Required]
        public DateTime ExpiryDate { get; set; } // Fecha y hora de expiración

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Propiedad de navegación opcional, si deseas la relación con Usuario
        public virtual Usuario Usuario { get; set; }
    }
}
