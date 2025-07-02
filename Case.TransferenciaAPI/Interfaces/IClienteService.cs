using Case.TransferenciaAPI.DTOs;
using Case.TransferenciaAPI.Entities;

namespace Case.TransferenciaAPI.Interfaces
{
	public interface IClienteService
	{
		Task<Cliente> CadastrarClienteAsync(ClienteDTO request);
		Task<IEnumerable<Cliente>> ListarClientesAsync();
		Task<Cliente> BuscarPorNumeroContaAsync(string numeroConta);
		Task<Cliente> AtualizarClienteAsync(Guid id, ClienteDTO request);
		Task<bool> ExcluirClienteAsync(Guid id);

	}
}
