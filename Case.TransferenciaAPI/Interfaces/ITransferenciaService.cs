using Case.TransferenciaAPI.DTOs;
using Case.TransferenciaAPI.Entities;

namespace Case.TransferenciaAPI.Interfaces
{
	public interface ITransferenciaService
	{
		Task<Transferencia> RealizarTransferencia(TransferenciaDTO request);
		Task<IEnumerable<Transferencia>> ObterHistoricoAsync(string numeroConta);
		Task<Transferencia> ObterTransferenciaPorIdAsync(Guid id);
		
	}
}
