namespace ComercialMorro.API.Models
{
    public class Funcionario
    {
        public int IdFuncionario { get; set; }
        public string Cargo { get; set; } = "Vendedor";
        public decimal Salario { get; set; } = 0;

        public Pessoa? Pessoa { get; set; }
    }
}