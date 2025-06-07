using MBExemploEmpresa.Entidades;
using Microsoft.Data.SqlClient;
using System.Data;

namespace MBExemploEmpresa.Servico
{
    public class DepartamentoService
    {
        private readonly string _connectionString;

        public DepartamentoService(string connectionString)
        {
            _connectionString = connectionString;
        }
        public async Task<List<Departamento>> ObterTodosAsync()
        {
            var departamentos = new List<Departamento>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                // CORRIGIDO: Usar o nome correto da tabela
                var command = new SqlCommand("SELECT Id, Nome, Sigla, Email, Telefone FROM departamentos ORDER BY Nome", connection);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        departamentos.Add(new Departamento
                        {
                            Id = reader.GetInt32("Id"),
                            Nome = reader.GetString("Nome"),
                            Sigla = reader.GetString("Sigla"),
                            Email = reader.IsDBNull("Email") ? string.Empty : reader.GetString("Email"),
                            Telefone = reader.IsDBNull("Telefone") ? string.Empty : reader.GetString("Telefone")
                        });
                    }
                }
            }

            return departamentos;
        }

        public async Task<Departamento> ObterPorIdAsync(int id)
        {
            Departamento departamento = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                // CORRIGIDO: Usar o nome correto da tabela
                var command = new SqlCommand("SELECT Id, Nome, Sigla, Email, Telefone " +
                    "FROM departamentos " +
                    "WHERE  Id = @id", connection);
                command.Parameters.AddWithValue("@id", id);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        departamento = new Departamento
                        {
                            Id = reader.GetInt32("Id"),
                            Nome = reader.GetString("Nome"),
                            Sigla = reader.GetString("Sigla"),
                            Email = reader.IsDBNull("Email") ? string.Empty : reader.GetString("Email"),
                            Telefone = reader.IsDBNull("Telefone") ? string.Empty : reader.GetString("Telefone")
                        };
                    }
                }
            }

            return departamento;
        }

        public async Task<Departamento> BuscarPorNomeAsync(string nome)
        {
            var departamentos = new List<Departamento>();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                // CORRIGIDO: Usar o nome correto da tabela
                var command = new SqlCommand("SELECT Id, Nome, Sigla, Email, Telefone " +
                    "FROM departamentos " +
                    "WHERE Nome LIKE @nome" +
                    "ORDER BY Nome", connection);

                command.Parameters.AddWithValue("@nome", $"%{nome}%");

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        departamentos.Add(new Departamento
                        {
                            Id = reader.GetInt32("Id"),
                            Nome = reader.GetString("Nome"),
                            Sigla = reader.GetString("Sigla"),
                            Email = reader.IsDBNull("Email") ? string.Empty : reader.GetString("Email"),
                            Telefone = reader.IsDBNull("Telefone") ? string.Empty : reader.GetString("Telefone")
                        });
                    }
                }
            }

            return departamentos;
        }

        public async Task AdicionarDepartamentoAsync(Departamento departamento)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand("INSERT INTO Departamentos (Nome,Sigla, Email, Telefone)" +
                    "VALUES (@Nome,@Sigla,@Email,@Telefone",connection);
                command.Parameters.AddWithValue("@Nome",departamento.Nome);
                command.Parameters.AddWithValue("@Sigla", departamento.Sigla);
                command.Parameters.AddWithValue("@Email", departamento.Email ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Telefone", departamento.Telefone ?? (object)DBNull.Value);
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task AtualizarDepartamentoAsync(Departamento departamento)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand("UPDATE Departamentos SET " +
                    "Nome = @Nome, Sigla = @Sigla, Email = @Email, Telefone = @Telefone " +
                    "WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", departamento.Id);
                command.Parameters.AddWithValue("@Nome", departamento.Nome);
                command.Parameters.AddWithValue("@Sigla", departamento.Sigla);
                command.Parameters.AddWithValue("@Email", departamento.Email ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Telefone", departamento.Telefone ?? (object)DBNull.Value);
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task DeletarDepartamentoAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand("DELETE FROM Departamento WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", id);
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<bool> PossuiFuncionarioAsync(int departamentoId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand("SELECT COUNT(*) " +
                    "FROM Funcionarios " +
                    "WHERE  DepartamentoId = @id AND" +
                    "Ativo = 1", connection);
                command.Parameters.AddWithValue("@id", departamentoId);
                var count = (int) await command.ExecuteScalarAsync();
                return count > 0;
            }
        }

        public async Task<bool> PossuiCargoAsync(int departamentoId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand("SELECT COUNT(*) " +
                    "FROM Cargos " +
                    "WHERE  DepartamentoId = @id", connection);
                command.Parameters.AddWithValue("@id", departamentoId);
                var count = (int)await command.ExecuteScalarAsync();
                return count > 0;
            }
        }
    }
}
