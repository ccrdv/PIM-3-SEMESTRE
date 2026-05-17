using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComercialMorro.API.Models
{
    public class Venda
    {
        [Key]
        [Column("ID_VENDA")]
        public int IdVenda { get; set; }

        [Column("DATA_HORA")]
        public DateTime DataHora { get; set; } = DateTime.Now;

        [Column("TOTAL_VENDA")]
        [Precision(12, 2)]
        public decimal TotalVenda { get; set; }

        [Column("TOTAL_DESCONTO")]
        [Precision(12, 2)]
        public decimal TotalDesconto { get; set; } = 0;

        [Column("ID_CLIENTE")]
        public int? ClienteIdCliente { get; set; }

        [Column("ID_FUNCIONARIO")]
        public int? FuncionarioIdFuncionario { get; set; } 

        // Navegação
        [ForeignKey("ClienteIdCliente")]
        public Cliente? Cliente { get; set; }

        public ICollection<ItemVenda> Itens { get; set; } = new List<ItemVenda>();
    }
}