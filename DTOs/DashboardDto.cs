namespace ComercialMorro.API.DTOs
{
    public class DashboardDto
    {
        public VendasHojeDto VendasHoje { get; set; } = new();
        public decimal TotalFiado { get; set; }
        public int ProdutosBaixoEstoque { get; set; }
        public int TotalClientes { get; set; }
        public List<VendaSimplesDto> UltimasVendas { get; set; } = new();
    }

    public class VendasHojeDto
    {
        public int QuantidadeVendas { get; set; }
        public decimal TotalValor { get; set; }
    }

    public class VendaSimplesDto
    {
        public int IdVenda { get; set; }
        public DateTime DataHora { get; set; }
        public string NomeCliente { get; set; } = "Venda à Vista";
        public decimal ValorTotal { get; set; }
    }
}