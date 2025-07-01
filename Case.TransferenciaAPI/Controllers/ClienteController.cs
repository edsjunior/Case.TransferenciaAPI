using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Case.TransferenciaAPI.DTOs;
using Case.TransferenciaAPI.Interfaces;

namespace Case.TransferenciaAPI.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ClienteController : ControllerBase
{
	private readonly IClienteService _clienteService;

	public ClienteController(IClienteService clienteService)
	{
		_clienteService = clienteService ?? throw new ArgumentNullException(nameof(clienteService));
	}

	[HttpPost]
	public async Task<IActionResult> Post([FromBody] ClienteDTO request)
	{
		if (!ModelState.IsValid)
		{
			return BadRequest(ModelState);
		}


		var cliente = await _clienteService.CadastrarClienteAsync(request);
		return CreatedAtAction(nameof(GetByNumeroConta), new { numeroConta = cliente.NumeroConta }, cliente);
	}

	[HttpGet]
	public async Task<IActionResult> Get()
	{
		var clientes = await _clienteService.ListarClientesAsync();
		return Ok(clientes);
	}

	[HttpGet("{numeroConta}")]
	public async Task<IActionResult> GetByNumeroConta(string numeroConta)
	{
		if (string.IsNullOrWhiteSpace(numeroConta))
		{
			return BadRequest("Número da conta não pode ser nulo ou vazio.");
		}
		var cliente = await _clienteService.BuscarPorNumeroContaAsync(numeroConta);
		if (cliente == null)
		{
			return NotFound($"Cliente com número de conta {numeroConta} não encontrado.");
		}
		return Ok(cliente);
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> Put(Guid id, [FromBody] ClienteDTO request)
	{
		if (!ModelState.IsValid)
		{
			return BadRequest(ModelState);
		}

		var clienteAtualizado = await _clienteService.AtualizarClienteAsync(id, request);
		if (clienteAtualizado == null)
		{
			return NotFound($"Cliente com ID {id} não encontrado.");
		}
		return Ok(clienteAtualizado);
	}

	[HttpDelete("{id}")]
	public async Task<IActionResult> Delete(Guid id)
	{
		var excluiu = await _clienteService.ExcluirClienteAsync(id);
		if (!excluiu)
		{
			return NotFound($"Cliente com ID {id} não encontrado.");
		}
		return NoContent();
	}


}
