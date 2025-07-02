using Case.TransferenciaAPI.Data;
using Case.TransferenciaAPI.DTOs;
using Case.TransferenciaAPI.Entities;
using Case.TransferenciaAPI.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Case.TransferenciaAPI.Services
{
	public class ClienteService : IClienteService
	{
		private readonly AppDbContext _context;
		public ClienteService(AppDbContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}
		public async Task<Cliente> AtualizarClienteAsync(Guid id, ClienteDTO request)
		{
			var cliente = await _context.Clientes.FindAsync(id);
			if (cliente == null)
			{
				throw new KeyNotFoundException("Cliente não encontrado.");
			}

			cliente.Nome = request.Nome;
			cliente.NumeroConta = request.NumeroConta;
			cliente.Saldo = request.Saldo;	

			await _context.SaveChangesAsync();
			return cliente;
		}

		public async Task<Cliente> BuscarPorNumeroContaAsync(string numeroConta)
		{
			var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.NumeroConta == numeroConta);	

			if (cliente == null)
			{
				throw new KeyNotFoundException("Não foi encontrado nenhum cliente com a conta informada.");
			}
			return cliente;
		}

		public async Task<Cliente> CadastrarClienteAsync(ClienteDTO request)
		{
			if(await _context.Clientes.AnyAsync(c => c.NumeroConta == request.NumeroConta))
			{
				throw new InvalidOperationException("Já existe um cliente cadastrado com o número de conta informado.");
			}
			if(request.Saldo < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(request.Saldo), "O saldo não pode ser negativo.");
			}

			var cliente = new Cliente
			{
				Id = Guid.NewGuid(),
				Nome = request.Nome,
				NumeroConta = request.NumeroConta,
				Saldo = request.Saldo
			};	
			_context.Clientes.Add(cliente);
			await _context.SaveChangesAsync();

			return cliente;
		}

		public async Task<bool> ExcluirClienteAsync(Guid id)
		{
			var cliente = await _context.Clientes.FindAsync(id);
			if (cliente == null)
			{
				throw new KeyNotFoundException("Cliente não encontrado.");
			}
			_context.Clientes.Remove(cliente);
			return await _context.SaveChangesAsync() > 0;
		}

		public async Task<IEnumerable<Cliente>> ListarClientesAsync()
		{
			return await _context.Clientes.ToListAsync();
		}
	}
}
