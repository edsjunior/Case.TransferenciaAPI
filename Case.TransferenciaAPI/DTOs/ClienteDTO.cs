using System.ComponentModel.DataAnnotations;

namespace Case.TransferenciaAPI.DTOs
{
	public class ClienteDTO
	{
		[Required, StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
		public required String Nome { get; set; }

		[Required, RegularExpression(@"^\d{5}-\d$", ErrorMessage = "Número da conta deve estar no formato 12345-1.")]
		public required string NumeroConta { get; set; }
		
		[Range(0, double.MaxValue, ErrorMessage = "O saldo não pode ser negativo.")]
		public decimal Saldo { get; set; }
	}
}
