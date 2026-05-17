namespace ComercialMorro.API.Models
{
    public class Cliente
    {
        public int IdCliente { get; set; }
        public decimal TotalFiado { get; set; } = 0;
        public string? Status { get; set; } = "Ativo"; 

        public Pessoa? Pessoa { get; set; }
    }
}