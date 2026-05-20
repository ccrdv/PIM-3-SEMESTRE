using Microsoft.EntityFrameworkCore;
using ComercialMorro.API.Models;

namespace ComercialMorro.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        public DbSet<Pessoa> Pessoas { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Funcionario> Funcionarios { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Venda> Vendas { get; set; }
        public DbSet<ItemVenda> ItensVenda { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<FormaPagamento> FormaPagamentos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("COMERCIALMORRO");

            // ====================== PESSOA ======================
            modelBuilder.Entity<Pessoa>(entity =>
            {
                entity.ToTable("PESSOA");
                entity.HasKey(p => p.IdPessoa);
                entity.Property(p => p.IdPessoa).HasColumnName("ID_PESSOA");
                entity.Property(p => p.Nome).HasColumnName("NOME").IsRequired().HasMaxLength(100);
                entity.Property(p => p.Cpf).HasColumnName("CPF").IsRequired().HasMaxLength(11);
                entity.Property(p => p.Telefone).HasColumnName("TELEFONE").HasMaxLength(15);
                entity.Property(p => p.Endereco).HasColumnName("ENDERECO").HasMaxLength(150);

                entity.Property(p => p.ClienteIdCliente).HasColumnName("CLIENTE_ID_CLIENTE");
                entity.Property(p => p.FuncionarioIdFuncionario).HasColumnName("FUNCIONARIO_ID_FUNCIONARIO");
                entity.Property(p => p.UsuarioIdUsuario).HasColumnName("USUARIO_ID_USUARIO");
            });

            // ====================== CLIENTE ======================
            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.ToTable("CLIENTE");
                entity.HasKey(c => c.IdCliente);
                entity.Property(c => c.IdCliente).HasColumnName("ID_CLIENTE");
                entity.Property(c => c.TotalFiado).HasColumnName("TOTAL_FIADO").HasPrecision(12, 2).HasDefaultValue(0);
                entity.Property(c => c.Status).HasColumnName("STATUS").HasMaxLength(20);
            });

            // ====================== FUNCIONARIO ======================
            modelBuilder.Entity<Funcionario>(entity =>
            {
                entity.ToTable("FUNCIONARIO");
                entity.HasKey(f => f.IdFuncionario);
                entity.Property(f => f.IdFuncionario).HasColumnName("ID_FUNCIONARIO");
                entity.Property(f => f.Cargo).HasColumnName("CARGO").HasMaxLength(50);
                entity.Property(f => f.Salario).HasColumnName("SALARIO").HasPrecision(12, 2);
            });

            // ====================== CATEGORIA ======================
            modelBuilder.Entity<Categoria>(entity =>
            {
                entity.ToTable("CATEGORIA");
                entity.HasKey(c => c.IdCategoria);
                entity.Property(c => c.IdCategoria).HasColumnName("ID_CATEGORIA");
                entity.Property(c => c.Descricao).HasColumnName("DESCRICAO").IsRequired().HasMaxLength(500);
            });

            // ====================== PRODUTO ======================
            modelBuilder.Entity<Produto>(entity =>
            {
                entity.ToTable("PRODUTO");
                entity.HasKey(p => p.IdProduto);
                entity.Property(p => p.IdProduto).HasColumnName("ID_PRODUTO");
                entity.Property(p => p.Descricao).HasColumnName("DESCRICAO").IsRequired().HasMaxLength(500);
                entity.Property(p => p.PrecoCompra).HasColumnName("PRECO_COMPRA").HasPrecision(12, 2);
                entity.Property(p => p.PrecoVenda).HasColumnName("PRECO_VENDA").HasPrecision(12, 2);
                entity.Property(p => p.Qtde).HasColumnName("QTDE").HasDefaultValue(0);
                entity.Property(p => p.IdCategoria).HasColumnName("ID_CATEGORIA");
            });

            // ====================== VENDA ======================
            modelBuilder.Entity<Venda>(entity =>
            {
                entity.ToTable("VENDA");
                entity.HasKey(v => v.IdVenda);
                entity.Property(v => v.IdVenda).HasColumnName("ID_VENDA");
                entity.Property(v => v.DataHora).HasColumnName("DATA_HORA");
                entity.Property(v => v.TotalVenda).HasColumnName("TOTAL_VENDA").HasPrecision(12, 2);
                entity.Property(v => v.TotalDesconto).HasColumnName("TOTAL_DESCONTO").HasPrecision(12, 2);
                entity.Property(v => v.ClienteIdCliente).HasColumnName("CLIENTE_ID_CLIENTE");
                entity.Property(v => v.FuncionarioIdFuncionario).HasColumnName("FUNCIONARIO_ID_FUNCIONARIO");
            });

            // ====================== ITEM_VENDA ======================
            modelBuilder.Entity<ItemVenda>(entity =>
            {
                entity.ToTable("ITEM_VENDA");
                entity.HasKey(i => i.IdItemVenda);
                entity.Property(i => i.IdItemVenda).HasColumnName("ID_ITEM_VENDA");
                entity.Property(i => i.IdVenda).HasColumnName("ID_VENDA");
                entity.Property(i => i.IdProduto).HasColumnName("ID_PRODUTO");
                entity.Property(i => i.Qdte).HasColumnName("QDTE");
                entity.Property(i => i.PrecoUnitario).HasColumnName("PRECO_UNITARIO").HasPrecision(12, 2);
                entity.Property(i => i.ValorDesconto).HasColumnName("VALOR_DESCONTO").HasPrecision(12, 2);
                entity.Property(i => i.ValorTotal).HasColumnName("VALOR_TOTAL").HasPrecision(12, 2);
            });
            // ====================== FORMA_PAGAMENTO ======================
            modelBuilder.Entity<FormaPagamento>(entity =>
            {
                entity.ToTable("FORMA_PAGAMENTO");
                entity.HasKey(f => f.IdFormaPgto);
                entity.Property(f => f.IdFormaPgto).HasColumnName("ID_FORMA_PGTO");
                entity.Property(f => f.DataHoraPgto).HasColumnName("DATA_HORA_PGTO");
                entity.Property(f => f.TotalPgto).HasColumnName("TOTAL_PGTO").HasPrecision(12, 2);
                entity.Property(f => f.TipoPgto).HasColumnName("TIPO_PGTO").HasMaxLength(20);
                entity.Property(f => f.IdVenda).HasColumnName("ID_VENDA");
                entity.Property(f => f.IdTipoPgto).HasColumnName("ID_TIPO_PGTO");
            });

            // ====================== USUARIO ======================
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("USUARIO");
                entity.HasKey(u => u.IdUsuario);
                entity.Property(u => u.IdUsuario).HasColumnName("ID_USUARIO");
                entity.Property(u => u.Username).HasColumnName("USERNAME");
                entity.Property(u => u.Senha).HasColumnName("SENHA");
                entity.Property(u => u.Status).HasColumnName("STATUS");
                entity.Property(u => u.IdFuncionario).HasColumnName("ID_FUNCIONARIO");
            });
        }
    }
}