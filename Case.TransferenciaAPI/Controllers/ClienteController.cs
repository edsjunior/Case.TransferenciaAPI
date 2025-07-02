using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Case.TransferenciaAPI.DTOs;
using Case.TransferenciaAPI.Interfaces;

namespace Case.TransferenciaAPI.Controllers;

[ApiController]
[Route("api/v1/clientes")]
public class ClienteController : ControllerBase
{
	private readonly IClienteService _clienteService;

	public ClienteController(IClienteService clienteService)
	{
		_clienteService = clienteService ?? throw new ArgumentNullException(nameof(clienteService));
	}

	[HttpPost]
	public async Task<IResult> Post([FromBody] ClienteDTO request)
	{
		if (!ModelState.IsValid)
		{
			return TypedResults.BadRequest(ModelState);
		}


		var cliente = await _clienteService.CadastrarClienteAsync(request);
		return TypedResults.CreatedAtRoute(
				routeName: "GetByNumeroConta",
				routeValues: new { numeroConta = cliente.NumeroConta },
				value: cliente
			);

	}

	[HttpGet]
	public async Task<IResult> Get()
	{
		var clientes = await _clienteService.ListarClientesAsync();
		return TypedResults.Ok(clientes);
	}

	[HttpGet("{numeroConta}", Name = "GetByNumeroConta")]
	public async Task<IResult> GetByNumeroConta(string numeroConta)
	{
		try
		{
			var cliente = await _clienteService.BuscarPorNumeroContaAsync(numeroConta);
			return TypedResults.Ok(cliente);
		}
		catch (KeyNotFoundException)
		{
			return TypedResults.NotFound($"Cliente não encontrado pela conta {numeroConta}.");
		}
		catch (Exception ex)
		{
			return TypedResults.Problem(
				detail: $"Erro ao obter Cliente: {ex.Message}",
				statusCode: StatusCodes.Status500InternalServerError
			);
		}
	}

	[HttpPut("{id}")]
	public async Task<IResult> Put(Guid id, [FromBody] ClienteDTO request)
	{
		if (!ModelState.IsValid)
		{
			return TypedResults.BadRequest(ModelState);
		}

		try
		{
			var clienteAtualizado = await _clienteService.AtualizarClienteAsync(id, request);
			return TypedResults.Ok(clienteAtualizado);
		}
		catch (KeyNotFoundException)
		{
			return TypedResults.NotFound($"Cliente com ID {id} não encontrado.");
		}
		catch (Exception ex)
		{
			return TypedResults.Problem(
				detail: $"Erro ao atualizar cliente: {ex.Message}",
				statusCode: StatusCodes.Status500InternalServerError
			);
		}
	}

	[HttpDelete("{id}")]
	public async Task<IResult> Delete(Guid id)
	{
		var excluido = await _clienteService.ExcluirClienteAsync(id);
		return excluido ? TypedResults.NoContent() : TypedResults.NotFound($"Cliente com ID {id} não encontrado.");
	}


}
