using Case.TransferenciaAPI.Entities;

namespace Case.TransferenciaAPI.Data;

public static class DbInit
{
	public static void Seed(AppDbContext context)
	{
		if (context.Clientes.Any())
			return;

		var clientes = new List<Cliente>
		{
			new() { Id = Guid.NewGuid(), Nome = "João", NumeroConta = "11111-1", Saldo = 1000 },
			new() { Id = Guid.NewGuid(), Nome = "Maria", NumeroConta = "22222-1", Saldo = 500 },
			new() { Id = Guid.NewGuid(), Nome = "Carlos", NumeroConta = "33333-1", Saldo = 300 }
		};

		context.Clientes.AddRange(clientes);
		context.SaveChanges();
	}
}
