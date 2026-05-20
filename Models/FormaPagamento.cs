using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComercialMorro.API.Models
{
    [Table("FORMA_PAGAMENTO")]
    public class FormaPagamento
    {
        [Key]
        [Column("ID_FORMA_PGTO")]
        public int IdFormaPgto { get; set; }

        [Column("DATA_HORA_PGTO")]
        public DateTime DataHoraPgto { get; set; } = DateTime.Now;

        [Column("TOTAL_PGTO")]
        [Precision(12, 2)]
        public decimal? TotalPgto { get; set; }

        [Column("TIPO_PGTO")]
        [MaxLength(20)]
        public string? TipoPgto { get; set; }

        [Column("ID_VENDA")]
        public int? IdVenda { get; set; }

        [Column("ID_TIPO_PGTO")]
        public int? IdTipoPgto { get; set; }

        // Navegação
        [ForeignKey("IdVenda")]
        public virtual Venda? Venda { get; set; }
    }
}