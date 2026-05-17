namespace ComercialMorro.API.Models
{
    public class Pessoa
    {
        public int IdPessoa { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public string Endereco { get; set; } = string.Empty;

        // Relacionamentos (nullable para não dar erro)
        public int? ClienteIdCliente { get; set; }
        public int? FuncionarioIdFuncionario { get; set; }
        public int? UsuarioIdUsuario { get; set; }     // pode ser null

        // Navegação
        public Cliente? Cliente { get; set; }
        public Funcionario? Funcionario { get; set; }
        public Usuario? Usuario { get; set; }
    }
}