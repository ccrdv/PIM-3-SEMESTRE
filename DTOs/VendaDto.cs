using System.Text.Json.Serialization;

namespace ComercialMorro.API.DTOs
{
    public class NovaVendaRequestDto
    {
        [JsonPropertyName("clienteId")]
        public int? ClienteId { get; set; }

        [JsonPropertyName("itens")]
        public List<ItemVendaDto> Itens { get; set; } = new();

        [JsonPropertyName("formaPagamento")]
        public string FormaPagamento { get; set; } = "Dinheiro";

        public bool IsFiado => FormaPagamento?.ToUpper() == "FIADO";
    }

    public class ItemVendaDto
    {
        [JsonPropertyName("produtoId")]
        public int ProdutoId { get; set; }

        [JsonPropertyName("quantidade")]
        public int Quantidade { get; set; }

        [JsonPropertyName("desconto")]
        public decimal? Desconto { get; set; }
    }

    public class VendaResponseDto
    {
        public int IdVenda { get; set; }
        public DateTime DataHora { get; set; }
        public decimal TotalVenda { get; set; }
        public decimal TotalDesconto { get; set; }
        public string? NomeCliente { get; set; }  
        public List<ItemVendaResponseDto> Itens { get; set; } = new();
    }

    public class ItemVendaResponseDto
    {
        public int ProdutoId { get; set; }
        public string NomeProduto { get; set; } = string.Empty;
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        public decimal ValorDesconto { get; set; }
        public decimal ValorTotal { get; set; }
    }
}