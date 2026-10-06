using Dapper;
using Microsoft.Data.Sqlite;
using Ronald_P1_P4.Models;

namespace Ronald_P1_P4.Services;

public class AutorService(IConfiguration configuration)
{
    private readonly string? _connectionString =
        configuration.GetConnectionString("DefaultConnection");

    private SqliteConnection CreateConnection()
    {
        return new SqliteConnection(_connectionString);
    }

    public async Task InitializeAsync()
    {
        using var connection = CreateConnection();

        await connection.OpenAsync();

        const string sql = """
            CREATE TABLE IF NOT EXISTS Autores
            (
                IdAutor INTEGER PRIMARY KEY AUTOINCREMENT,
                Nombres TEXT NOT NULL,
                Nacionalidad TEXT NOT NULL,
                FechaNacimiento DATE NOT NULL,
                Sueldo INT NOT NULL
            );
            """;

        await connection.ExecuteAsync(sql);
    }

    public async Task<int> SaveAsync(Autor autor)
    {
        using var connection = CreateConnection();

        const string sql = """
            INSERT INTO Autores (Nombres, Nacionalidad, FechaNacimiento, Sueldo)
            VALUES (@Nombre, @Nacionalidad, @FechaNacimiento, @Sueldo);

            SELECT last_insert_rowid();
            """;

        return await connection.ExecuteScalarAsync<int>(sql, autor);
    }

    public async Task UpdateAsync(Autor autor)
    {
        using var connection = CreateConnection();

        const string sql = """
            UPDATE Autores
            SET Nombre = @Nombre,
                Nacionalidad = @Nacionalidad,
                FechaNacimiento = @FechaNacimiento,
                Sueldo = @Suedo,
            WHERE IdAutor = @Id;
            """;

        await connection.ExecuteAsync(sql, autor);
    }

    public async Task<Autor?> GetByIdAsync(int id)
    {
        using var connection = CreateConnection();

        const string sql = """
            SELECT IdAutor, Nombre, Nacionalidad, FechaNacimiento, Sueldo
            FROM Autor
            WHERE Id = @Id;
            """;

        return await connection.QueryFirstOrDefaultAsync<Autor>(
            sql,
            new { Id = id });
    }

    public async Task<IEnumerable<Autor>> GetListAsync()
    {
        using var connection = CreateConnection();

        const string sql = """
            SELECT IdAutor, Nombre, Nacionalidad, FechaNacimiento, Sueldo
            FROM Autores
            ORDER BY IdAutor;
            """;

        return await connection.QueryAsync<Autor>(sql);
    }

    public async Task DeleteAsync(int id)
    {
        using var connection = CreateConnection();

        const string sql = """
            DELETE FROM Autores
            WHERE IdAutor = @IdAutor;
            """;

        await connection.ExecuteAsync(
            sql,
            new { Id = id });
    }
}