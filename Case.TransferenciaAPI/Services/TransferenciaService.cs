using Case.TransferenciaAPI.Data;
using Case.TransferenciaAPI.DTOs;
using Case.TransferenciaAPI.Entities;
using Case.TransferenciaAPI.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace Case.TransferenciaAPI.Services
{
	public class TransferenciaService : ITransferenciaService
	{
		private readonly AppDbContext _context;
		private static readonly ConcurrentDictionary<string, object> _locks = new();
		public TransferenciaService(AppDbContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}
		public async Task<IEnumerable<Transferencia>> ObterHistoricoAsync(string numeroConta)
		{
			return await _context.Transferencias
				.Where(t => t.NumeroContaOrigem == numeroConta || t.NumeroContaDestino == numeroConta)
				.OrderByDescending(t => t.DataTransferencia)
				.ToListAsync();
		}

		public async Task<Transferencia> ObterTransferenciaPorIdAsync(Guid id)
		{
			var transferencia = await _context.Transferencias.FirstOrDefaultAsync(t =>	t.Id == id);

			if (transferencia == null)
			{
				throw new KeyNotFoundException("Transferência não encontrada.");
			}

			return transferencia;
		}

		public Task<Transferencia> RealizarTransferencia(TransferenciaDTO request)
		{
			var origemLock = _locks.GetOrAdd(request.NumeroContaOrigem, new object());
			var destinoLock = _locks.GetOrAdd(request.NumeroContaDestino, new object());

			var locks = new[] { origemLock, destinoLock }.OrderBy(o => o.GetHashCode()).ToArray();

			lock (locks[0])
			{
				lock (locks[1])
				{
					var origem = _context.Clientes.FirstOrDefault(c => c.NumeroConta == request.NumeroContaOrigem);
					var destino = _context.Clientes.FirstOrDefault(c => c.NumeroConta == request.NumeroContaDestino);

					var transferencia = new Transferencia
					{
						Id = Guid.NewGuid(),
						NumeroContaOrigem = request.NumeroContaOrigem,
						NumeroContaDestino = request.NumeroContaDestino,
						Valor = request.Valor,
						DataTransferencia = DateTime.UtcNow,
						Status = "pendente",
						MensagemStatus = "Transferência pendente."
					};

					if (origem == null || destino == null)
					{
						transferencia.MensagemStatus = "Conta de origem ou destino não encontrada.";
						transferencia.Status = "falha";
					}
					else if (origem.Saldo < request.Valor)
					{
						transferencia.MensagemStatus = "Saldo insuficiente na conta de origem.";
						transferencia.Status = "falha";
					}
					else
					{
						origem.Saldo -= request.Valor;
						destino.Saldo += request.Valor;
						transferencia.MensagemStatus = "Transferência efetivada com sucesso.";
						transferencia.Status = "sucesso";
					}

					_context.Transferencias.Add(transferencia);
					_context.SaveChanges();
					return Task.FromResult(transferencia);
				}
			}
		}

	}
}
