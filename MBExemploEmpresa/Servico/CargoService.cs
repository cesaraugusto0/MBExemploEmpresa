using MBExemploEmpresa.Entidades;
using Microsoft.Data.SqlClient;
using System.Data;

namespace MBExemploEmpresa.Servico
{
    public class CargoService
    {
        private readonly string _connectionString;

        public CargoService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<Cargo>> ObterTodosAsync()
        {
            var cargos = new List<Cargo>();

            const string queryCargo = @"
                SELECT 
                    c.Id, c.Nome, c.Descricao, c.SalarioBase, 
                    c.DepartamentoId, d.Nome AS NomeDepartamento
                FROM Cargos c
                INNER JOIN Departamentos d ON c.DepartamentoId = d.Id
                ORDER BY c.Nome;";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand(queryCargo, connection);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        cargos.Add(new Cargo
                        {
                            Id = reader.GetInt32("Id"),
                            Nome = reader.GetString("Nome"),
                            Descricao = reader.IsDBNull("Descricao") ? string.Empty : reader.GetString("Descricao"),
                            SalarioBase = reader.GetDecimal("SalarioBase"),
                            DepartamentoId = reader.GetInt32("DepartamentoId"),
                            NomeDepartamento = reader.GetString("NomeDepartamento") // Campo do JOIN
                        });
                    }
                }
            }
            return cargos;
        }

        public async Task<Cargo?> ObterPorIdAsync(int id)
        {
            Cargo? cargo = null;

            const string queryCargo = @"
                SELECT 
                    c.Id, c.Nome, c.Descricao, c.SalarioBase, 
                    c.DepartamentoId, d.Nome AS NomeDepartamento
                FROM Cargos c
                INNER JOIN Departamentos d ON c.DepartamentoId = d.Id
                WHERE c.Id = @id;";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand(queryCargo, connection);
                command.Parameters.AddWithValue("@id", id);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        cargo = new Cargo
                        {
                            Id = reader.GetInt32("Id"),
                            Nome = reader.GetString("Nome"),
                            Descricao = reader.IsDBNull("Descricao") ? string.Empty : reader.GetString("Descricao"),
                            SalarioBase = reader.GetDecimal("SalarioBase"),
                            DepartamentoId = reader.GetInt32("DepartamentoId"),
                            NomeDepartamento = reader.GetString("NomeDepartamento")
                        };
                    }
                }
            }
            return cargo;
        }

        public async Task AdicionarCargoAsync(Cargo cargo)
        {
            const string queryCargo = @"
                INSERT INTO Cargos (DepartamentoId, Nome, Descricao, SalarioBase) 
                VALUES (@DepartamentoId, @Nome, @Descricao, @SalarioBase);";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand(queryCargo, connection);

                command.Parameters.AddWithValue("@DepartamentoId", cargo.DepartamentoId);
                command.Parameters.AddWithValue("@Nome", cargo.Nome);
                command.Parameters.AddWithValue("@SalarioBase", cargo.SalarioBase);
                command.Parameters.AddWithValue("@Descricao", (object)cargo.Descricao ?? DBNull.Value);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task AtualizarCargoAsync(Cargo cargo)
        {
            const string queryCargo = @"
                UPDATE Cargos SET 
                    DepartamentoId = @DepartamentoId, 
                    Nome = @Nome, 
                    Descricao = @Descricao, 
                    SalarioBase = @SalarioBase 
                WHERE Id = @Id;";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand(queryCargo, connection);

                command.Parameters.AddWithValue("@Id", cargo.Id);
                command.Parameters.AddWithValue("@DepartamentoId", cargo.DepartamentoId);
                command.Parameters.AddWithValue("@Nome", cargo.Nome);
                command.Parameters.AddWithValue("@SalarioBase", cargo.SalarioBase);
                command.Parameters.AddWithValue("@Descricao", (object)cargo.Descricao ?? DBNull.Value);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task DeletarCargoAsync(int id)
        {
            const string queryCargo = "DELETE FROM Cargos WHERE Id = @Id;";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand(queryCargo, connection);
                command.Parameters.AddWithValue("@Id", id);
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<bool> PossuiFuncionariosAtivosAsync(int cargoId)
        {
            const string queryCargo = "SELECT COUNT(1) FROM Funcionarios WHERE CargoId = @cargoId AND Ativo = 1;";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand(queryCargo, connection);
                command.Parameters.AddWithValue("@cargoId", cargoId);

                // ExecuteScalarAsync é ideal para queries que retornam um único valor.
                var count = (int)await command.ExecuteScalarAsync();

                return count > 0;
            }
        }

    }
}
