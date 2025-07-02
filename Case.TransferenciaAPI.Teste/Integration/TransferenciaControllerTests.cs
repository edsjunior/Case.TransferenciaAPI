using Case.TransferenciaAPI;
using Case.TransferenciaAPI.DTOs;
using Case.TransferenciaAPI.Entities;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Case.TransferenciaAPI.Teste.Integration;
public class TransferenciaControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
	private readonly WebApplicationFactory<Program> _factory;

	public TransferenciaControllerTests(WebApplicationFactory<Program> factory)
	{
		_factory = factory.WithWebHostBuilder(builder =>
		{
			builder.ConfigureServices(services =>
			{
				// Remove o contexto real
				var descriptor = services.SingleOrDefault(
					d => d.ServiceType == typeof(DbContextOptions<Data.AppDbContext>));
				if (descriptor != null)
					services.Remove(descriptor);

				// Substitui por InMemory
				services.AddDbContext<Data.AppDbContext>(options =>
				{
					options.UseInMemoryDatabase("TestDb");
				});

				// Popular dados para o teste
				var sp = services.BuildServiceProvider();
				using var scope = sp.CreateScope();
				var context = scope.ServiceProvider.GetRequiredService<Data.AppDbContext>();
				context.Database.EnsureCreated();

				context.Clientes.AddRange(
					new Cliente { Id = Guid.NewGuid(), Nome = "Origem", NumeroConta = "77777-1", Saldo = 1000 },
					new Cliente { Id = Guid.NewGuid(), Nome = "Destino", NumeroConta = "88888-1", Saldo = 500 }
				);
				context.SaveChanges();
			});
		});
	}

	[Fact]
	public async Task Post_Transferencia_EntreContasValidas_DeveRetornarCreated()
	{
		// Arrange
		var client = _factory.CreateClient();

		var dto = new TransferenciaDTO
		{
			NumeroContaOrigem = "77777-1",
			NumeroContaDestino = "88888-1",
			Valor = 200
			
		};

		// Act
		var response = await client.PostAsJsonAsync("/api/v1/transferencias", dto);

		var content = await response.Content.ReadAsStringAsync();

		// Assert
		Assert.Equal(HttpStatusCode.Created, response.StatusCode);

		var body = await response.Content.ReadFromJsonAsync<Transferencia>();
		Assert.NotNull(body);
		Assert.Equal("sucesso", body!.Status);
		Assert.Equal("Transferência efetivada com sucesso.", body.MensagemStatus);
	}

	[Fact]
	public async Task Post_Transferencia_ComSaldoInsuficiente_DeveRetornarUnprocessableEntity()
	{
		// Arrange
		var client = _factory.CreateClient();

		var dto = new TransferenciaDTO
		{
			NumeroContaOrigem = "77777-1", 
			NumeroContaDestino = "88888-1",
			Valor = 5000
		};

		// Act
		var response = await client.PostAsJsonAsync("/api/v1/transferencias", dto);

		var content = await response.Content.ReadFromJsonAsync<Transferencia>();

		// Assert
		Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
		Assert.NotNull(content);
		Assert.Equal("falha", content!.Status);
		Assert.Equal("Saldo insuficiente na conta de origem.", content.MensagemStatus);
	}

	[Fact]
	public async Task Post_Transferencia_ContaOrigemInvalida_DeveRetornarUnprocessableEntity()
	{
		var client = _factory.CreateClient();

		var dto = new TransferenciaDTO
		{
			NumeroContaOrigem = "99999-1",
			NumeroContaDestino = "22222-1",
			Valor = 100
		};

		var response = await client.PostAsJsonAsync("/api/v1/transferencias", dto);

		var content = await response.Content.ReadFromJsonAsync<Transferencia>();

		Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
		Assert.NotNull(content);
		Assert.Equal("falha", content!.Status);
		Assert.Equal("Conta de origem ou destino não encontrada.", content.MensagemStatus);
	}

	[Fact]
	public async Task Post_Transferencia_ContaDestinoInvalida_DeveRetornarUnprocessableEntity()
	{
		var client = _factory.CreateClient();

		var dto = new TransferenciaDTO
		{
			NumeroContaOrigem = "11111-1",
			NumeroContaDestino = "99999-1", // inexistente
			Valor = 100
		};

		var response = await client.PostAsJsonAsync("/api/v1/transferencias", dto);

		var content = await response.Content.ReadFromJsonAsync<Transferencia>();

		Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
		Assert.NotNull(content);
		Assert.Equal("falha", content!.Status);
		Assert.Equal("Conta de origem ou destino não encontrada.", content.MensagemStatus);
	}

	[Fact]
	public async Task Post_Transferencia_MesmaConta_DeveRetornarUnprocessableEntity()
	{
		var client = _factory.CreateClient();

		var dto = new TransferenciaDTO
		{
			NumeroContaOrigem = "22222-1",
			NumeroContaDestino = "22222-1",
			Valor = 100
		};

		var response = await client.PostAsJsonAsync("/api/v1/transferencias", dto);

		var content = await response.Content.ReadFromJsonAsync<Transferencia>();

		Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
		Assert.NotNull(content);
		Assert.Equal("falha", content!.Status);
		Assert.Equal("Transferência não pode ser feita para a mesma conta.", content.MensagemStatus);
	}

	[Fact]
	public async Task Post_Transferencia_ValorExcedente_DeveRetornarUnprocessableEntity()
	{
		var client = _factory.CreateClient();

		var dto = new TransferenciaDTO
		{
			NumeroContaOrigem = "11111-1",
			NumeroContaDestino = "22222-1",
			Valor = -1
		};

		var response = await client.PostAsJsonAsync("/api/v1/transferencias", dto);

		var content = await response.Content.ReadFromJsonAsync<Transferencia>();

		Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
		Assert.NotNull(content);
		Assert.Equal("falha", content!.Status);
		Assert.Equal("Valor da transferência deve estar entre 0.01 e 10.000.", content.MensagemStatus);
	}
}

