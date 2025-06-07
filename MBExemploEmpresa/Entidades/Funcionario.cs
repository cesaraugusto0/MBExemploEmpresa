using System.ComponentModel.DataAnnotations;

namespace MBExemploEmpresa.Entidades
{
    public class Funcionario
    {
        // ========================================
        // IDENTIDADE E RELACIONAMENTOS
        // ========================================
        public int Id { get; set; }
        public int DepartamentoId { get; set; }
        public int CargoId { get; set; }
        public int? CidadeId { get; set; }  // Nullable - opcional

        // ========================================
        // DADOS PESSOAIS
        // ========================================
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(200, ErrorMessage = "Nome deve ter no máximo 200 caracteres")]
        public string Nome { get; set; } = string.Empty;

        [StringLength(200)]
        public string NomeMae { get; set; } = string.Empty;

        [StringLength(200)]
        public string NomePai { get; set; } = string.Empty;

        [Required(ErrorMessage = "Data de nascimento é obrigatória")]
        public DateTime DataNascimento { get; set; }

        [Required(ErrorMessage = "CPF é obrigatório")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "CPF deve ter exatamente 11 dígitos")]
        public string CPF { get; set; } = string.Empty;

        [StringLength(20)]
        public string RG { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        // ========================================
        // DADOS PROFISSIONAIS
        // ========================================
        [Required(ErrorMessage = "Salário é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Salário deve ser maior que zero")]
        public decimal Salario { get; set; }

        [Required(ErrorMessage = "Data de admissão é obrigatória")]
        public DateTime DataAdmissao { get; set; } = DateTime.Today;

        public bool Ativo { get; set; } = true;

        // ========================================
        // CAMPOS PREENCHIDOS VIA JOIN (ADO.NET)
        // Estes campos são populados pelas queries SQL
        // ========================================
        public string NomeDepartamento { get; set; } = string.Empty;
        public string NomeCargo { get; set; } = string.Empty;
        public string NomeCidade { get; set; } = string.Empty;

        // ========================================
        // OBJETOS DE NAVEGAÇÃO (Para uso futuro)
        // ========================================
        public Departamento? Departamento { get; set; }
        public Cargo? Cargo { get; set; }
    }
}
