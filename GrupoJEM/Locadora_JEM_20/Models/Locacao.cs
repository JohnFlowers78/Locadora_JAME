using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Locadora_JEM_20.Models;
public class Locacao
{
  public Locacao()
      {
        LocadoEm = DateTime.Now;
        DataInicio = LocadoEm;
        DataFim = LocadoEm.AddDays(3);
        Devolvido = false;
        VerificarDevolucao();
      }
  [Key]
  public int LocacaoId { get; set; }    //PK
  public DateTime LocadoEm { get; set; }
  public DateTime DataInicio { get; set; }
  public DateTime DataFim { get; set; }
  public bool Devolvido { get; set; }

  public string? Observacoes { get; set; }
  public float Multa { get; set; }

  public int FilmeId { get; set; } // FK
  [ForeignKey("FilmeId")]
  public Filme? Filme { get; set; }
  
  public int ClienteId { get; set; } // FK
  [ForeignKey("ClienteId")]
  public Cliente? Cliente { get; set; }

  public float Valor
  {
      get
      {
          float valorFilme = 0;

          if (Filme != null)
          {
              valorFilme = Filme.Valor;
          }
          return valorFilme;
      }
  }

  public void VerificarDevolucao()
      {
        if (DateTime.Now > DataFim && !Devolvido)
        {
          Devolvido = true;

        // Calcule o número de dias de atraso
        int diasAtraso = (int)(DateTime.Now - DataFim).TotalDays;

        // Calcula a multa para o primeiro mês (R$ 2 por dia)
        float multa = 0;
        if (diasAtraso <= 30)
        {
            multa = diasAtraso * 2; // R$ 2 por dia no primeiro mês
        }
        else
        {
            // A partir do segundo mês, multa mensal fixa (R$ 10 por mês)
            int mesesAtraso = ((DateTime.Now.Year - DataFim.Year) * 12) + DateTime.Now.Month - DataFim.Month;
            multa = 30 * 2 + (mesesAtraso - 1) * 10; // R$ 30 do primeiro mês + R$ 10 por mês a partir do segundo
        }
        Multa = multa + Valor;
    }   
  }
}