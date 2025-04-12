using System;

namespace MyEmployeeProject.Models
{
    public class Departamento
    {
        public int Id { get; set; }
        public required string Nome { get; set; }
        public string? Descricao { get; set; }
        public DateTime DataCriacao { get; set; }
    }

    public class Cargo
    {
        public int Id { get; set; }
        public required string Nome { get; set; }
        public required string Nivel { get; set; }
        public decimal SalarioBase { get; set; }
        public DateTime DataCriacao { get; set; }
    }

    public enum StatusFuncionario
    {
        Ativo,
        Inativo,
        Ferias,
        Licenca
    }

    public class Funcionario
    {
        public int Id { get; set; }
        public required string Nome { get; set; }
        public required string Email { get; set; }
        public required string Telefone { get; set; }
        
        // Relacionamentos
        public int CargoId { get; set; }
        public Cargo? Cargo { get; set; }
        public int DepartamentoId { get; set; }
        public Departamento? Departamento { get; set; }
        
        public decimal Salario { get; set; }
        public DateTime DataContratacao { get; set; }
        public DateTime? DataNascimento { get; set; }
        public StatusFuncionario Status { get; set; }
        public string? Observacoes { get; set; }
        
        // Auditoria
        public DateTime DataCriacao { get; set; }
        public DateTime DataAtualizacao { get; set; }

        public Funcionario()
        {
            Status = StatusFuncionario.Ativo;
            DataCriacao = DateTime.Now;
            DataAtualizacao = DateTime.Now;
        }

        public int Idade
        {
            get
            {
                if (!DataNascimento.HasValue) return 0;
                var idade = DateTime.Today.Year - DataNascimento.Value.Year;
                if (DataNascimento.Value.Date > DateTime.Today.AddYears(-idade))
                    idade--;
                return idade;
            }
        }

        public decimal SalarioAnual
        {
            get { return Salario * 12; }
        }

        public bool EstaAtivo
        {
            get { return Status == StatusFuncionario.Ativo; }
        }
    }
}
