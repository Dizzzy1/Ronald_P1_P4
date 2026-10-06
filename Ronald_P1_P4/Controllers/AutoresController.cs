using Microsoft.AspNetCore.Mvc;
using Ronald_P1_P4.Models;
using Ronald_P1_P4.Services;

namespace Ronald_P1_P4.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AutoresController(AutorService autoresService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var autores = await autoresService.GetListAsync();

        return Ok(autores);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var autor = await autoresService.GetByIdAsync(id);

        if (autor == null)
        {
            return NotFound(new
            {
                mensaje = "Autor no encontrado"
            });
        }

        return Ok(autor);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Autor autor)
    {
        var id = await autoresService.SaveAsync(autor);

        autor.AutorId = id;

        return CreatedAtAction(
            nameof(GetById),
            new { id = autor.AutorId },
            autor
        );
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] Autor autor)
    {
        var existente = await autoresService.GetByIdAsync(id);

        if (existente == null)
        {
            return NotFound(new
            {
                mensaje = "Autor no encontrado"
            });
        }

        autor.AutorId = id;

        await autoresService.UpdateAsync(autor);

        return Ok(autor);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existente = await autoresService.GetByIdAsync(id);

        if (existente == null)
        {
            return NotFound(new
            {
                mensaje = "Autor no encontrado"
            });
        }

        await autoresService.DeleteAsync(id);

        return NoContent();
    }
}