using System.ComponentModel.DataAnnotations;

namespace Case.TransferenciaAPI.DTOs
{
	public class TransferenciaDTO
	{

		[Required, RegularExpression(@"^\d{5}-\d$", ErrorMessage = "Número da conta deve estar no formato 12345-1.")]
		public String NumeroContaOrigem { get; set; }

		[Required, RegularExpression(@"^\d{5}-\d$", ErrorMessage = "Número da conta deve estar no formato 12345-1.")]
		public String NumeroContaDestino { get; set; }

		[Range(0.01, 10000, ErrorMessage = "Valor da transferência deve estar entre 0.01 e 10.000.")]
		public decimal Valor { get; set; }
	}
}
