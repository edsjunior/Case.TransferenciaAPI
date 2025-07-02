using Case.TransferenciaAPI.Data;
using Case.TransferenciaAPI.DTOs;
using Case.TransferenciaAPI.Entities;
using Case.TransferenciaAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace Case.TransferenciaAPI.Teste.Unit.Services;


public class TransferenciaServiceTests
{
	private AppDbContext GetContext()
	{
		var options = new DbContextOptionsBuilder<AppDbContext>()
			.UseInMemoryDatabase(databaseName: "TestDb")
			.Options;
		return new AppDbContext(options);
	}

	[Fact]
	public async Task Transferencia_DadosValidos_RealizarTransferencia()
	{
		// Arrange
		var context = GetContext();

		context.Clientes.AddRange(
			new Cliente { Id = Guid.NewGuid(), Nome = "Origem", NumeroConta = "11111-1", Saldo = 1000 },
			new Cliente { Id = Guid.NewGuid(), Nome = "Destino", NumeroConta = "22222-1", Saldo = 500 }
		);
		await context.SaveChangesAsync();

		var service = new TransferenciaService(context);

		var dto = new TransferenciaDTO
		{
			NumeroContaOrigem = "11111-1",
			NumeroContaDestino = "22222-1",
			Valor = 200
		};

		// Act
		var resultado = await service.RealizarTransferencia(dto);

		// Assert
		Assert.NotNull(resultado);
		Assert.Equal("11111-1", resultado.NumeroContaOrigem);
		Assert.Equal("22222-1", resultado.NumeroContaDestino);
		Assert.Equal(200, resultado.Valor);

		var origem = await context.Clientes.FirstAsync(c => c.NumeroConta == "11111-1");
		var destino = await context.Clientes.FirstAsync(c => c.NumeroConta == "22222-1");

		Assert.Equal(800, origem.Saldo);
		Assert.Equal(700, destino.Saldo);
	}

	[Fact]
	public async Task Transferencia_OrigemNaoExiste_MensagemFalhaOrigemInvalido()
	{
		// Arrange
		var context = GetContext();

		context.Clientes.Add(new Cliente { Id = Guid.NewGuid(), Nome = "Destino", NumeroConta = "33333-1", Saldo = 500 });
		await context.SaveChangesAsync();

		var service = new TransferenciaService(context);

		var dto = new TransferenciaDTO
		{
			NumeroContaOrigem = "00000-1",
			NumeroContaDestino = "33333-1",
			Valor = 100
		};

		// Act
		var resultado = await service.RealizarTransferencia(dto);

		// Assert
		Assert.Equal("falha", resultado.Status);
		Assert.Equal("Conta de origem ou destino não encontrada.", resultado.MensagemStatus);
	}

	[Fact]
	public async Task Transferencia_DestinoNaoExiste_MensagemFalhaDesinoInvalido()
	{
		// Arrange
		var context = GetContext();

		context.Clientes.Add(new Cliente { Id = Guid.NewGuid(), Nome = "Origem", NumeroConta = "44444-1", Saldo = 1000 });
		await context.SaveChangesAsync();

		var service = new TransferenciaService(context);

		var dto = new TransferenciaDTO
		{
			NumeroContaOrigem = "44444-1",
			NumeroContaDestino = "99999-1",
			Valor = 50
		};

		// Act
		var resultado = await service.RealizarTransferencia(dto);

		// Assert
		Assert.Equal("falha", resultado.Status);
		Assert.Equal("Conta de origem ou destino não encontrada.", resultado.MensagemStatus);
	}

	[Fact]
	public async Task Transferencia_QuandoSaldoInsuficiente_ExceptionSaldo()
	{
		// Arrange
		var context = GetContext();

		context.Clientes.AddRange(
			new Cliente { Id = Guid.NewGuid(), Nome = "Origem", NumeroConta = "55555-1", Saldo = 50 },
			new Cliente { Id = Guid.NewGuid(), Nome = "Destino", NumeroConta = "66666-1", Saldo = 300 }
		);
		await context.SaveChangesAsync();

		var service = new TransferenciaService(context);

		var dto = new TransferenciaDTO
		{
			NumeroContaOrigem = "55555-1",
			NumeroContaDestino = "66666-1",
			Valor = 100 // maior que saldo
		};

		// Act
		var resultado = await service.RealizarTransferencia(dto);

		// Assert
		Assert.Equal("falha", resultado.Status);
		Assert.Equal("Saldo insuficiente na conta de origem.", resultado.MensagemStatus);

	}

	[Fact]
	public async Task Transferencias_DeveBloquearConcorrencia_UsandoLock()
	{
		// Arrange
		var context = GetContext();
		context.Clientes.AddRange(
					new Cliente { Id = Guid.NewGuid(), Nome = "ContaA", NumeroConta = "12345-1", Saldo = 1000 },
					new Cliente { Id = Guid.NewGuid(), Nome = "ContaB", NumeroConta = "67890-1", Saldo = 1000 },
					new Cliente { Id = Guid.NewGuid(), Nome = "ContaC", NumeroConta = "54321-1", Saldo = 0 }
				);
		context.SaveChanges();

		var service = new TransferenciaService(context);
		var dto1 = new TransferenciaDTO
		{
			NumeroContaOrigem = "12345-1",
			NumeroContaDestino = "67890-1",
			Valor = 800
		};
		var dto2 = new TransferenciaDTO
		{
			NumeroContaOrigem = "12345-1",
			NumeroContaDestino = "54321-1",
			Valor = 500
		};

		// Act (duas chamadas simultâneas)
		var task1 = Task.Run(() =>
		{
			//Thread.Sleep(50); // Simula atraso proposital para criar competição
			return service.RealizarTransferencia(dto1);
		});

		var task2 = Task.Run(() =>
		{
			Thread.Sleep(50);
			return service.RealizarTransferencia(dto2);
		});

		await Task.WhenAll(task1, task2);

		var resultado1 = task1.Result;
		var resultado2 = task2.Result;

		// Assert: Apenas uma deve ter sucesso
		Assert.True(
			(resultado1.Status == "sucesso" && resultado2.Status == "falha") ||
			(resultado1.Status == "falha" && resultado2.Status == "sucesso")
		);

		// E saldo final deve ser correto
		var conta = context.Clientes.First(c => c.NumeroConta == "12345-1");
		Assert.True(conta.Saldo == 200);
	}

}
