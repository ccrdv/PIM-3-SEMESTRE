using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComercialMorro.API.Models
{
    public class ItemVenda
    {
        [Key]
        [Column("ID_ITEM_VENDA")]
        public int IdItemVenda { get; set; }

        [Column("ID_VENDA")]
        public int IdVenda { get; set; }

        [Column("ID_PRODUTO")]
        public int IdProduto { get; set; }

        [Column("QTDE")]
        public int Qdte { get; set; }

        [Column("PRECO_UNITARIO")]
        [Precision(12, 2)]
        public decimal PrecoUnitario { get; set; }

        [Column("VALOR_DESCONTO")]
        [Precision(12, 2)]
        public decimal ValorDesconto { get; set; } = 0;

        [Column("VALOR_TOTAL")]
        [Precision(12, 2)]
        public decimal ValorTotal { get; set; }

        // Navegação
        [ForeignKey("IdVenda")]
        public Venda? Venda { get; set; }

        [ForeignKey("IdProduto")]
        public Produto? Produto { get; set; }
    }
}