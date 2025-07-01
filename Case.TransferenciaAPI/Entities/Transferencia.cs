namespace Case.TransferenciaAPI.Entities
{
	public class Transferencia
	{
		public Guid Id { get; set; }
		public required String NumeroContaOrigem { get; set; }
		public required String NumeroContaDestino { get; set; }
		public decimal Valor { get; set; }
		public DateTime DataTransferencia { get; set; }
		public required string Status { get; set; }
		public required string MensagemStatus { get; set; }

	}
}
