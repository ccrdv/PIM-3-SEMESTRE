namespace ComercialMorro.API.Models
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty; // Em produção deve ser hash
        public string Status { get; set; } = "Ativo";
        public int IdFuncionario { get; set; }

        public Funcionario? Funcionario { get; set; }
    }
}