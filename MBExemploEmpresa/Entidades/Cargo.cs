using System.ComponentModel.DataAnnotations;

namespace MBExemploEmpresa.Entidades
{
    public class Cargo
    {
        // ========================================
        // IDENTIDADE E RELACIONAMENTOS
        // ========================================
        public int Id { get; set; }
        public int DepartamentoId { get; set; }

        // ========================================
        // DADOS DO CARGO
        // ========================================
        [Required(ErrorMessage = "Nome do cargo é obrigatório")]
        [StringLength(200, ErrorMessage = "Nome deve ter no máximo 200 caracteres")]
        public string Nome { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Descrição deve ter no máximo 500 caracteres")]
        public string Descricao { get; set; } = string.Empty;

        [Required(ErrorMessage = "Salário base é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Salário base deve ser maior que zero")]
        public decimal SalarioBase { get; set; }

        // ========================================
        // CAMPO PREENCHIDO VIA JOIN (ADO.NET)
        // ========================================
        public string NomeDepartamento { get; set; } = string.Empty;

        // ========================================
        // OBJETO DE NAVEGAÇÃO (Para uso futuro)
        // ========================================
        public Departamento? Departamento { get; set; }
        public List<Funcionario> Funcionarios { get; set; } = new();
    }

}
