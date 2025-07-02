using Case.TransferenciaAPI.DTOs;
using Case.TransferenciaAPI.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Case.TransferenciaAPI.Controllers
{
	[ApiController]
	[Route("api/v1/transferencias")]
	public class TransferenciaController : ControllerBase
	{
		private readonly ITransferenciaService _transferenciaService;
		public TransferenciaController(ITransferenciaService transferenciaService)
		{
			_transferenciaService = transferenciaService ?? throw new ArgumentNullException(nameof(transferenciaService));
		}
		[HttpPost]
		public async Task<IResult> Post([FromBody] TransferenciaDTO request)
		{
			if (!ModelState.IsValid)
			{
				return TypedResults.BadRequest(ModelState);
			}
			var transferencia = await _transferenciaService.RealizarTransferencia(request);

			if (transferencia.Status == "sucesso")
			{
				return TypedResults.CreatedAtRoute(
					routeName: "GetByTransferenciaId",
					routeValues: new { id = transferencia.Id },
					value: transferencia
				);
			} else
			{
				return TypedResults.UnprocessableEntity(transferencia);

			}

		}

		[HttpGet("{id}", Name = "GetByTransferenciaId")]
		public async Task<IResult> GetById(Guid id)
		{
			var transferencia = await _transferenciaService.ObterTransferenciaPorIdAsync(id);
			return TypedResults.Ok(transferencia);
		}

		[HttpGet("historico/{numeroConta}")]
		public async Task<IResult> GetHistorico(string numeroConta)
		{
			try
			{
				var historico = await _transferenciaService.ObterHistoricoAsync(numeroConta);
				return TypedResults.Ok(historico);
			}
			catch (KeyNotFoundException)
			{
				return TypedResults.NotFound($"Histórico não encontrado para a conta {numeroConta}.");
			}
			catch (ArgumentNullException)
			{
				return TypedResults.BadRequest("Número da conta não pode ser nulo ou vazio.");
			}
			catch (Exception ex)
			{
				return TypedResults.Problem(
					detail: $"Erro ao obter histórico: {ex.Message}",
					statusCode: StatusCodes.Status500InternalServerError
				);
			}
		}
	}
}
