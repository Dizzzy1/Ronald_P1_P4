using Dapper;
using Microsoft.Data.Sqlite;
using Ronald_P1_P4.Models;

namespace Ronald_P1_P4.Services;

public class AutorService(IConfiguration configuration)
{
    private readonly string _connectionString =
        configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException(
            "No se encontró la cadena de conexión DefaultConnection.");

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
                Sueldo NUMERIC NOT NULL
            );
            """;

        await connection.ExecuteAsync(sql);
    }

    public async Task<int> SaveAsync(Autor autor)
    {
        using var connection = CreateConnection();

        const string sql = """
            INSERT INTO Autores
                (Nombres, Nacionalidad, FechaNacimiento, Sueldo)
            VALUES
                (@Nombres, @Nacionalidad, @FechaNacimiento, @Sueldo);

            SELECT last_insert_rowid();
            """;

        return await connection.ExecuteScalarAsync<int>(sql, autor);
    }
    public async Task UpdateAsync(Autor autor)
    {
        using var connection = CreateConnection();

        const string sql = """
        UPDATE Autores
        SET Nombres = @Nombres,
            Nacionalidad = @Nacionalidad,
            FechaNacimiento = @FechaNacimiento,
            Sueldo = @Sueldo
        WHERE IdAutor = @AutorId;
        """;

        var parametros = new
        {
            Nombres = autor.Nombres,
            Nacionalidad = autor.Nacionalidad,
            FechaNacimiento = autor.FechaNacimiento,
            Sueldo = autor.Sueldo,
            AutorId = autor.IdAutor
        };

        await connection.ExecuteAsync(sql, parametros);
    }

    public async Task<Autor?> GetByIdAsync(int id)
    {
        using var connection = CreateConnection();

        const string sql = """
        SELECT
            IdAutor,
            Nombres,
            Nacionalidad,
            FechaNacimiento,
            Sueldo
        FROM Autores
        WHERE IdAutor = @Id;
        """;

        var autor = await connection.QueryFirstOrDefaultAsync<dynamic>(
            sql,
            new { Id = id });

        if (autor == null)
            return null;

        return new Autor(
            (int)autor.IdAutor,
            (string)autor.Nombres,
            (string)autor.Nacionalidad,
            DateTime.Parse((string)autor.FechaNacimiento),
            Convert.ToDecimal(autor.Sueldo,
                System.Globalization.CultureInfo.InvariantCulture)
        );
    }

    public async Task<IEnumerable<Autor>> GetListAsync()
    {
        using var connection = CreateConnection();

        const string sql = """
        SELECT
            IdAutor,
            Nombres,
            Nacionalidad,
            FechaNacimiento,
            Sueldo
        FROM Autores
        ORDER BY IdAutor;
        """;

        var resultados = await connection.QueryAsync<dynamic>(sql);

        return resultados.Select(autor => new Autor(
            (int)autor.IdAutor,
            (string)autor.Nombres,
            (string)autor.Nacionalidad,
            DateTime.Parse((string)autor.FechaNacimiento),
            Convert.ToDecimal(autor.Sueldo,
                System.Globalization.CultureInfo.InvariantCulture)
        ));
    }

    public async Task DeleteAsync(int id)
    {
        using var connection = CreateConnection();

        const string sql = """
            DELETE FROM Autores
            WHERE IdAutor = @AutorId;
            """;

        await connection.ExecuteAsync(
            sql,
            new { AutorId = id });
    }
}