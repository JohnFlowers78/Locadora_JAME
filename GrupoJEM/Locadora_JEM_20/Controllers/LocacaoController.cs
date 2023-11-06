using Locadora_JEM_20.Data;
using Locadora_JEM_20.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Locadora_JEM_20.Controllers
{
    [ApiController]
    [Route("api/locacao")]
    public class LocacaoController : ControllerBase
    {
        private readonly AppDataContext _ctx;
        public LocacaoController(AppDataContext context)
        {
            _ctx = context;
        }

        [HttpGet]
        [Route("listar/em_aberto")]
        public IActionResult ListarEmAberto()
        {
            List<Locacao> locacoesEmAberto = _ctx.Locacoes
                .Where(l => !l.Devolvido) // Filtra as locações com Devolvido igual a false
                .Include(l => l.Cliente)
                .Include(l => l.Filme)
                .ToList();

            if (locacoesEmAberto.Count == 0)
            {
                return NotFound("Nenhuma locação em aberto encontrada.");
            }

            var responseModel = locacoesEmAberto.Select(locacao => new
            {
                Locacao = locacao,
                Cliente = locacao.Cliente,
                Filme = locacao.Filme,
                Valor = locacao.Valor
            });

            return Ok(responseModel);
        }

        [HttpGet]
        [Route("buscar/{id}")]
        public IActionResult Buscar(int id)
        {
            var locacao = _ctx.Locacoes.Include(l => l.Cliente).Include(l => l.Filme).FirstOrDefault(l => l.LocacaoId == id);

            if (locacao != null)
            {
                var responseModel = new
                {
                    Locacao = locacao,
                    Cliente = locacao.Cliente,
                    Filme = locacao.Filme
                };

                return Ok(responseModel);
            }

            return NotFound();
        }

        [HttpPost]
        [Route("emprestar/{fId}/{cId}")]   //CADASTAR
        public IActionResult EmprestarFilme([FromRoute] int fId, [FromRoute] int cId)
        {
            try
            {
                var cliente = _ctx.Clientes.FirstOrDefault(c => c.ClienteId == cId);
                var filme = _ctx.Filmes.FirstOrDefault(f => f.FilmeId == fId);

                if (cliente != null)
                {
                    if (filme != null && filme.Disponivel)
                    {
                        filme.Disponivel = false;
                        _ctx.SaveChanges();

                        // Crie uma nova locação relacionando o cliente e o filme
                        var locacao = new Locacao
                        {
                            // ClienteId = cliente.ClienteId,
                            Cliente = cliente,
                            // FilmeId = filme.FilmeId,
                            Filme = filme
                        };

                        _ctx.Locacoes.Add(locacao);
                        _ctx.SaveChanges();

                        return Created("", locacao);
                    }
                }
                return NotFound();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

                // Método para devolver um filme (alterar o atributo "Disponivel" para true)
        [HttpPost]
        [Route("devolver/{id}")]
        public IActionResult DevolverFilme([FromRoute] int id)
        {
            try
            {
                var filme = _ctx.Filmes.FirstOrDefault(f => f.FilmeId == id);

                if (filme != null && !filme.Disponivel)
                {
                    filme.Disponivel = true;

                    var locacao = _ctx.Locacoes.FirstOrDefault(l => l.FilmeId == id && !l.Devolvido);

                    if (locacao != null)
                    {
                        locacao.Devolvido = true;
                        locacao.VerificarDevolucao(); // Atualiza a multa, se aplicável

                        _ctx.SaveChanges();

                        return Ok(new { Mensagem = "Filme devolvido com sucesso! \n\n Registro de devolução já foi guardado no sistema!" });
                    }
                }

                return NotFound();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut]
        [Route("alterar/{id}")]
        public IActionResult Alterar([FromRoute] int id, [FromBody] Locacao locacao)
        {
            try
            {
                var locacaoExistente = _ctx.Locacoes.Include(l => l.Cliente).Include(l => l.Filme).FirstOrDefault(l => l.LocacaoId == id);

                if (locacaoExistente == null)
                {
                    return NotFound("Locação não encontrada");
                }

                locacaoExistente.Observacoes = locacao.Observacoes;


                _ctx.SaveChanges();

                return Ok(locacaoExistente);
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
                var locacaoExistente = _ctx.Locacoes.FirstOrDefault(l => l.LocacaoId == id);

                if (locacaoExistente == null)
                {
                    return NotFound("Locação não encontrada");
                }

                // Implemente regras adicionais, se necessário, para evitar a exclusão de locações em determinadas situações.
                
                var filme = _ctx.Filmes.FirstOrDefault(f => f.FilmeId == locacaoExistente.FilmeId);

                if (filme != null)
                {
                    filme.Disponivel = true;
                }

                _ctx.Locacoes.Remove(locacaoExistente);
                _ctx.SaveChanges();

                return NoContent(); // Retorna 204 No Content em caso de sucesso
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        public IActionResult Listar()
        {
            List<Locacao> locacoes = _ctx.Locacoes.Include(l => l.Cliente).Include(l => l.Filme).ToList();

            if (locacoes.Count == 0)
            {
                return NotFound();
            }

            var responseModel = locacoes.Select(locacao => new
            {
                Locacao = locacao,
                Cliente = locacao.Cliente,
                Filme = locacao.Filme
            });

            return Ok(responseModel);
        }
    }
}