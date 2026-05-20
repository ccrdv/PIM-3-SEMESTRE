using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComercialMorro.API.Models
{
    [Table("USUARIO")]
    public class Usuario
    {
        [Key]
        [Column("ID_USUARIO")]
        public int IdUsuario { get; set; }

        [Column("USERNAME")]
        public string Username { get; set; } = string.Empty;

        [Column("SENHA")]
        public string Senha { get; set; } = string.Empty;

        [Column("STATUS")]
        public string Status { get; set; } = "Ativo";

        [Column("ID_FUNCIONARIO")]
        public int? IdFuncionario { get; set; }

        // Navegação
        [ForeignKey("IdFuncionario")]
        public virtual Funcionario? Funcionario { get; set; }
    }
}