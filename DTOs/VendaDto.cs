namespace ComercialMorro.API.DTOs
{
    public class NovaVendaRequestDto
    {
        public int? ClienteId { get; set; }           // Obrigatório se for fiado
        public List<ItemVendaDto> Itens { get; set; } = new();
        public string FormaPagamento { get; set; } = "Dinheiro"; // Dinheiro, Pix, Fiado
        public bool IsFiado => FormaPagamento?.ToUpper() == "FIADO";
    }

    public class ItemVendaDto
    {
        public int ProdutoId { get; set; }
        public int Quantidade { get; set; }
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