using Case.TransferenciaAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace Case.TransferenciaAPI.Data
{
	public class AppDbContext: DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
		{
		}

		public DbSet<Cliente> Clientes { get; set; }
		public DbSet<Transferencia> Transferencias { get; set; }
	}
}
