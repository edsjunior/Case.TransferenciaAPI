using Case.TransferenciaAPI.Data;
using Case.TransferenciaAPI.DTOs;
using Case.TransferenciaAPI.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Case.TransferenciaAPI.Teste.Unit.Services;

public class ClienteServiceTests
{
	private AppDbContext GetContext()
	{
		var options = new DbContextOptionsBuilder<AppDbContext>()
			.UseInMemoryDatabase(databaseName: "TestDb")
			.Options;
		
		return new AppDbContext(options);
	}

	[Fact]
	public async Task Add_DadosValidos_CadastrarCliente()
	{
		// Arrange
		var context = GetContext();
		var service = new ClienteService(context);
		
		var dto = new ClienteDTO
		{
			Nome = "João",
			NumeroConta = "12345-1",
			Saldo = 1000
		};
		// Act
		var cliente = await service.CadastrarClienteAsync(dto);
		
		// Assert
		Assert.NotNull(cliente);
		Assert.Equal("João", cliente.Nome);
		Assert.Equal("12345-1", cliente.NumeroConta);
		Assert.Equal(1000, cliente.Saldo);

	}

	[Fact]
	public async Task Add_ContaDuplicada_CadastrarClienteException()
	{
		// Arrange
		var context = GetContext();
		context.Clientes.Add(new Entities.Cliente
		{
			Nome = "João",
			NumeroConta = "12345-1",
			Saldo = 1000
		});
		await context.SaveChangesAsync();

		var service = new ClienteService(context);

		var dto = new ClienteDTO
		{
			Nome = "Pedro",
			NumeroConta = "12345-1",
			Saldo = 2000
		};
		await context.SaveChangesAsync();


		//Act & Assert
		await Assert.ThrowsAsync<InvalidOperationException>(() => service.CadastrarClienteAsync(dto));

	}

	[Fact]
	public async Task Add_SaldoNegativo_CadastrarClienteException()
	{
		// Arrange
		var context = GetContext();
		var service = new ClienteService(context);

		var dto = new ClienteDTO
		{
			Nome = "Carlos",
			NumeroConta = "99999-1",
			Saldo = -50 // inválido
		};

		// Act & Assert
		await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.CadastrarClienteAsync(dto));
	}

}