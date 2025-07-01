namespace Case.TransferenciaAPI.Entities
{
	public class Cliente
	{
		public Guid Id { get; set; }
		public required String Nome { get; set; }
		public required String NumeroConta { get; set; }
		public decimal Saldo { get; set; }
	}
}
