namespace Locadora_JEM_20.Models;
using System.ComponentModel.DataAnnotations;
public class Filme
{
  public Filme(){
    CriadoEm = DateTime.Now;
    Disponivel = true;
  }
  [Key]
  public int FilmeId { get; set; }
  public string? Titulo { get; set; }
  public int Ano { get; set; }
  public string? Genero { get; set; }
  public string? Sinopse { get; set; }
  public string? Capa { get; set; }
  public string? Descricao { get; set; }
  public bool Disponivel { get; set; }
  public float Valor { get; set; }
  public DateTime CriadoEm { get; set; }
}
