using Locadora_JEM_20.Data;
using Locadora_JEM_20.Models;
using Microsoft.AspNetCore.Mvc;

namespace Locadora_JEM_20.Controllers;

[ApiController]
[Route("api/filme")]
public class FilmeController : ControllerBase
{
    private readonly AppDataContext _ctx;
    public FilmeController(AppDataContext ctx) => _ctx = ctx;
    
    [HttpPost]
    [Route("cadastrar")]
    public IActionResult Cadastrar([FromBody] Filme filme)
    {
        try
        {
            _ctx.Filmes.Add(filme);
            _ctx.SaveChanges();
            return Created("", filme);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpGet]
    [Route("buscar/{titulo}")]
    public IActionResult Buscar([FromRoute] string titulo)
    {
        try
        {
            foreach (Filme filmeCadastrado in _ctx.Filmes.ToList())
            {
                if(filmeCadastrado.Titulo == titulo)
                {
                    return Ok(filmeCadastrado);
                }
            }
            return NotFound();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }

    }

    [HttpGet]
    [Route("listar")]
    public IActionResult Listar()
    {
        try
        {
            List<Filme> filmes = _ctx.Filmes.ToList();
            return filmes.Count == 0 ? NotFound() : Ok(filmes);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut]
    [Route("alterar/{id}")]
    public IActionResult Alterar([FromRoute] int id, [FromBody] Filme filme)
    {
        try
        {
            var filmeExistente = _ctx.Filmes.FirstOrDefault(f => f.FilmeId == id);

            if (filmeExistente == null)
            {
                return NotFound("Filme não encontrado");
            }

            filmeExistente.Titulo = filme.Titulo;
            filmeExistente.Ano = filme.Ano;
            filmeExistente.Genero = filme.Genero;
            filmeExistente.Sinopse = filme.Sinopse;
            filmeExistente.Capa = filme.Capa;
            filmeExistente.Descricao = filme.Descricao;
            filmeExistente.Disponivel = filme.Disponivel;
            filmeExistente.Valor = filme.Valor;

            _ctx.SaveChanges();

            return Ok(filmeExistente);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete]
    [Route("deletar/{id}")]
    public IActionResult Deletar([FromRoute] int id)
    {
        try
        {
            var filmeExistente = _ctx.Filmes.FirstOrDefault(f => f.FilmeId == id);

            if (filmeExistente == null)
            {
                return NotFound("Filme não encontrado");
            }

            // Verifique se o filme está em locação
            var locacaoComFilme = _ctx.Locacoes.Any(l => l.FilmeId == id && !l.Devolvido);
            if (locacaoComFilme)
            {
                return BadRequest("Não é possível deletar o filme enquanto estiver em locação");
            }

            _ctx.Filmes.Remove(filmeExistente);
            _ctx.SaveChanges();

            return NoContent(); // Retorna 204 No Content em caso de sucesso
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    // Método para verificar se um filme com o nome está disponível
    [HttpGet]
    [Route("disponivel/titulo/{titulo}")]
    public IActionResult VerificarDisponibilidadePorTitulo([FromRoute] string titulo)
    {
        try
        {
            var filme = _ctx.Filmes.FirstOrDefault(f => f.Titulo == titulo);

            if (filme != null && filme.Disponivel)
            {
                return Ok(filme);
            }
            return NotFound();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    //-------------->>           Refazer Método para verificar se um filme com o gênero está disponível    (e passar um nome por url tbm para acha-lo) (talvez criar uma classe genêroController para pesquisar por nome dentro de gênero)

    [HttpGet]
    [Route("disponivel/genero/{genero}")]
    public IActionResult VerificarDisponibilidadePorGenero([FromRoute] string genero)
    {
        try
        {
            // Filtrar filmes pelo gênero e disponibilidade
            var filmes = _ctx.Filmes.Where(f => f.Genero == genero && f.Disponivel).ToList();

            if (filmes.Count > 0)
            {
                return Ok(filmes);
            }

            return NotFound("Nenhum filme disponível desse gênero encontrado.");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

}
