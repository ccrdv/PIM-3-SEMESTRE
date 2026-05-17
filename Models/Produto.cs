using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComercialMorro.API.Models
{
    public class Produto
    {
        [Key]
        [Column("ID_PRODUTO")]
        public int IdProduto { get; set; }

        [Column("DESCRICAO")]
        [Required]
        [MaxLength(500)]
        public string Descricao { get; set; } = string.Empty;

        [Column("PRECO_COMPRA")]
        [Precision(12, 2)]
        public decimal PrecoCompra { get; set; }

        [Column("PRECO_VENDA")]
        [Precision(12, 2)]
        public decimal PrecoVenda { get; set; }

        [Column("QTDE")]
        public int Qtde { get; set; }

        [Column("ID_CATEGORIA")]
        [ForeignKey("Categoria")]
        public int IdCategoria { get; set; }

        [ForeignKey("IdCategoria")]
        public virtual Categoria? Categoria { get; set; }
    }
}