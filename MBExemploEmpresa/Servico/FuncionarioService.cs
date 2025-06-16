using MBExemploEmpresa.Entidades;
using Microsoft.Data.SqlClient;
using System.Data;

namespace MBExemploEmpresa.Servico
{
    public class FuncionarioService
    {
        private readonly string _connectionString;

        public FuncionarioService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<Funcionario>> ObterTodosAsync()
        {
            var funcionarios = new List<Funcionario>();

            const string queryFuncionario = @"
                SELECT 
                    f.Id, f.Nome, f.CPF, f.Email, f.Salario, f.Ativo, f.DataAdmissao, f.DataNascimento, f.RG, f.NomeMae, f.NomePai,
                    f.DepartamentoId, d.Nome AS NomeDepartamento,
                    f.CargoId, c.Nome AS NomeCargo
                FROM Funcionarios f
                INNER JOIN Departamentos d ON f.DepartamentoId = d.Id
                INNER JOIN Cargos c ON f.CargoId = c.Id
                ORDER BY f.Nome;";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand(queryFuncionario, connection);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        funcionarios.Add(new Funcionario
                        {
                            // ========================================
                            // MAPEAMENTO DIRETO (COLUNAS NOT NULL)
                            // ========================================
                            Id = reader.GetInt32("Id"),
                            DepartamentoId = reader.GetInt32("DepartamentoId"),
                            CargoId = reader.GetInt32("CargoId"),
                            Nome = reader.GetString("Nome"),
                            DataNascimento = reader.GetDateTime("DataNascimento"),
                            CPF = reader.GetString("CPF"),
                            Salario = reader.GetDecimal("Salario"),
                            DataAdmissao = reader.GetDateTime("DataAdmissao"),
                            Ativo = reader.GetBoolean("Ativo"),

                            // ========================================
                            // MAPEAMENTO DE COLUNAS NULLABLE (QUE PODEM SER NULAS)
                            // Usamos o padrão: reader.IsDBNull("Coluna") ? valor_padrao : reader.Get...("Coluna")
                            // ========================================

                            // Para strings, o padrão é retornar string vazia se for nulo.
                            NomeMae = reader.IsDBNull("NomeMae") ? string.Empty : reader.GetString("NomeMae"),
                            NomePai = reader.IsDBNull("NomePai") ? string.Empty : reader.GetString("NomePai"),
                            RG = reader.IsDBNull("RG") ? string.Empty : reader.GetString("RG"),
                            Email = reader.IsDBNull("Email") ? string.Empty : reader.GetString("Email"),

                            // Para o CidadeId, que é um 'int?', o valor padrão se for nulo é 'null'.
                            // Descomente a linha abaixo se você reativar o CidadeId na sua entidade e na query.
                            // CidadeId = reader.IsDBNull("CidadeId") ? (int?)null : reader.GetInt32("CidadeId"),

                            // ========================================
                            // MAPEAMENTO DE CAMPOS VINDOS DO JOIN
                            // Estes campos dependem de uma query SQL com JOIN e alias (AS).
                            // Ex: SELECT d.Nome AS NomeDepartamento FROM ...
                            // ========================================
                            NomeDepartamento = reader.GetString("NomeDepartamento"),
                            NomeCargo = reader.GetString("NomeCargo")

                            // Descomente a linha abaixo se você incluir o JOIN com a tabela Cidades.
                            // NomeCidade = reader.IsDBNull("NomeCidade") ? string.Empty : reader.GetString("NomeCidade"),
                        });
                    }
                }
            }

            return funcionarios;
        }

        public async Task<List<Funcionario>> BuscarPorNomeAsync(string nome)
        {
            var funcionarios = new List<Funcionario>();

            const string queryFuncionario = @"
                SELECT 
                    f.Id, f.Nome, f.CPF, f.Email, f.Salario, f.Ativo, f.DataAdmissao, f.DataNascimento, f.RG, f.NomeMae, f.NomePai,
                    f.DepartamentoId, d.Nome AS NomeDepartamento,
                    f.CargoId, c.Nome AS NomeCargo
                FROM Funcionarios f
                INNER JOIN Departamentos d ON f.DepartamentoId = d.Id
                INNER JOIN Cargos c ON f.CargoId = c.Id
                WHERE f.Nome LIKE @nome
                ORDER BY f.Nome;";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand(queryFuncionario, connection);
                command.Parameters.AddWithValue("@nome", $"%{nome}%"); // Busca por nomes que contenham o termo

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        funcionarios.Add(new Funcionario
                        {
                            Id = reader.GetInt32("Id"),
                            DepartamentoId = reader.GetInt32("DepartamentoId"),
                            CargoId = reader.GetInt32("CargoId"),
                            Nome = reader.GetString("Nome"),
                            DataNascimento = reader.GetDateTime("DataNascimento"),
                            CPF = reader.GetString("CPF"),
                            Salario = reader.GetDecimal("Salario"),
                            DataAdmissao = reader.GetDateTime("DataAdmissao"),
                            Ativo = reader.GetBoolean("Ativo"),
                            NomeMae = reader.IsDBNull("NomeMae") ? string.Empty : reader.GetString("NomeMae"),
                            NomePai = reader.IsDBNull("NomePai") ? string.Empty : reader.GetString("NomePai"),
                            RG = reader.IsDBNull("RG") ? string.Empty : reader.GetString("RG"),
                            Email = reader.IsDBNull("Email") ? string.Empty : reader.GetString("Email"),
                            NomeDepartamento = reader.GetString("NomeDepartamento"),
                            NomeCargo = reader.GetString("NomeCargo")
                        });
                    }
                }
            }
            return funcionarios;
        }

        public async Task<Funcionario?> ObterPorIdAsync(int id)
        {
            Funcionario? funcionario = null;

            const string queryFuncionario = @"
                    SELECT 
                        f.Id, f.Nome, f.CPF, f.Email, f.Salario, f.Ativo, f.DataAdmissao, f.DataNascimento, f.RG, f.NomeMae, f.NomePai,
                        f.DepartamentoId, d.Nome AS NomeDepartamento,
                        f.CargoId, c.Nome AS NomeCargo
                    FROM Funcionarios f
                    INNER JOIN Departamentos d ON f.DepartamentoId = d.Id
                    INNER JOIN Cargos c ON f.CargoId = c.Id
                    WHERE f.Id = @id;";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand(queryFuncionario, connection);
                command.Parameters.AddWithValue("@id", id);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        // Aqui podemos reutilizar a lógica de mapeamento
                        funcionario = new Funcionario
                        {
                            Id = reader.GetInt32("Id"),
                            DepartamentoId = reader.GetInt32("DepartamentoId"),
                            CargoId = reader.GetInt32("CargoId"),
                            Nome = reader.GetString("Nome"),
                            DataNascimento = reader.GetDateTime("DataNascimento"),
                            CPF = reader.GetString("CPF"),
                            Salario = reader.GetDecimal("Salario"),
                            DataAdmissao = reader.GetDateTime("DataAdmissao"),
                            Ativo = reader.GetBoolean("Ativo"),
                            NomeMae = reader.IsDBNull("NomeMae") ? string.Empty : reader.GetString("NomeMae"),
                            NomePai = reader.IsDBNull("NomePai") ? string.Empty : reader.GetString("NomePai"),
                            RG = reader.IsDBNull("RG") ? string.Empty : reader.GetString("RG"),
                            Email = reader.IsDBNull("Email") ? string.Empty : reader.GetString("Email"),
                            NomeDepartamento = reader.GetString("NomeDepartamento"),
                            NomeCargo = reader.GetString("NomeCargo")
                        };
                    }
                }
            }
            return funcionario;
        }

        public async Task AdicionarFuncionarioAsync(Funcionario funcionario)
        {
            const string queryFuncionario = @"
                INSERT INTO Funcionarios 
                    (DepartamentoId, CargoId, Nome, NomeMae, NomePai, DataNascimento, CPF, RG, Email, Salario, DataAdmissao, Ativo)
                VALUES 
                    (@DepartamentoId, @CargoId, @Nome, @NomeMae, @NomePai, @DataNascimento, @CPF, @RG, @Email, @Salario, @DataAdmissao, @Ativo);";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand(queryFuncionario, connection);

                command.Parameters.AddWithValue("@DepartamentoId", funcionario.DepartamentoId);
                command.Parameters.AddWithValue("@CargoId", funcionario.CargoId);
                command.Parameters.AddWithValue("@Nome", funcionario.Nome);
                command.Parameters.AddWithValue("@CPF", funcionario.CPF);
                command.Parameters.AddWithValue("@Salario", funcionario.Salario);
                command.Parameters.AddWithValue("@DataAdmissao", funcionario.DataAdmissao);
                command.Parameters.AddWithValue("@DataNascimento", funcionario.DataNascimento);
                command.Parameters.AddWithValue("@Ativo", funcionario.Ativo);
                command.Parameters.AddWithValue("@NomeMae", (object)funcionario.NomeMae ?? DBNull.Value);
                command.Parameters.AddWithValue("@NomePai", (object)funcionario.NomePai ?? DBNull.Value);
                command.Parameters.AddWithValue("@RG", (object)funcionario.RG ?? DBNull.Value);
                command.Parameters.AddWithValue("@Email", (object)funcionario.Email ?? DBNull.Value);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task AtualizarFuncionarioAsync(Funcionario funcionario)
        {
            const string queryFuncionario = @"
                UPDATE Funcionarios SET
                    DepartamentoId = @DepartamentoId,
                    CargoId = @CargoId,
                    Nome = @Nome,
                    NomeMae = @NomeMae,
                    NomePai = @NomePai,
                    DataNascimento = @DataNascimento,
                    CPF = @CPF,
                    RG = @RG,
                    Email = @Email,
                    Salario = @Salario,
                    DataAdmissao = @DataAdmissao,
                    Ativo = @Ativo
                WHERE Id = @Id;";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand(queryFuncionario, connection);

                command.Parameters.AddWithValue("@Id", funcionario.Id);
                command.Parameters.AddWithValue("@DepartamentoId", funcionario.DepartamentoId);
                command.Parameters.AddWithValue("@CargoId", funcionario.CargoId);
                command.Parameters.AddWithValue("@Nome", funcionario.Nome);
                command.Parameters.AddWithValue("@CPF", funcionario.CPF);
                command.Parameters.AddWithValue("@Salario", funcionario.Salario);
                command.Parameters.AddWithValue("@DataAdmissao", funcionario.DataAdmissao);
                command.Parameters.AddWithValue("@DataNascimento", funcionario.DataNascimento);
                command.Parameters.AddWithValue("@Ativo", funcionario.Ativo);
                command.Parameters.AddWithValue("@NomeMae", (object)funcionario.NomeMae ?? DBNull.Value);
                command.Parameters.AddWithValue("@NomePai", (object)funcionario.NomePai ?? DBNull.Value);
                command.Parameters.AddWithValue("@RG", (object)funcionario.RG ?? DBNull.Value);
                command.Parameters.AddWithValue("@Email", (object)funcionario.Email ?? DBNull.Value);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task DeletarFuncionarioAsync(int id)
        {
            const string queryFuncionario = "DELETE FROM Funcionarios WHERE Id = @Id;";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand(queryFuncionario, connection);
                command.Parameters.AddWithValue("@Id", id);
                await command.ExecuteNonQueryAsync();
            }
        }
    }
}
