using Case.TransferenciaAPI.DTOs;
using Case.TransferenciaAPI.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Case.TransferenciaAPI.Controllers
{
	[Route("api/v1/[controller]")]
	[ApiController]
	public class TransferenciaController : ControllerBase
	{
		private readonly ITransferenciaService _transferenciaService;
		public TransferenciaController(ITransferenciaService transferenciaService)
		{
			_transferenciaService = transferenciaService ?? throw new ArgumentNullException(nameof(transferenciaService));
		}
		[HttpPost]
		public async Task<IActionResult> Post([FromBody] TransferenciaDTO request)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			var transferencia = await _transferenciaService.RealizarTransferencia(request);
			return Ok(transferencia);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(Guid id)
		{
			var transferencia = await _transferenciaService.ObterTransferenciaPorIdAsync(id);
			return Ok(transferencia);
		}

		[HttpGet("historico/{numeroConta}")]
		public async Task<IActionResult> GetHistorico(string numeroConta)
		{
			if (string.IsNullOrWhiteSpace(numeroConta))
			{
				return BadRequest("Número da conta não pode ser nulo ou vazio.");
			}
			var historico = await _transferenciaService.ObterHistoricoAsync(numeroConta);
			return Ok(historico);
		}
	}
}
