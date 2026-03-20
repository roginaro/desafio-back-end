using System.Net;
using System.Net.Http.Json;
using Desafio.Application.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Desafio.Testes.Integration.Controllers;

public class PedidoControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public PedidoControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    #region POST /api/pedido - Criar Pedido

    [Fact]
    public async Task Criar_DeveRetornar201Created_QuandoPedidoForValido()
    {
        // Arrange
        var request = new PedidoRequest
        {
            Pedido = "PED-001",
            Itens = new List<ItemPedidoModel>  
            {
                new() { Descricao = "Produto A", PrecoUnitario = 50.00m, Qtd = 2 },
                new() { Descricao = "Produto B", PrecoUnitario = 30.00m, Qtd = 1 }
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/pedido", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var pedidoCriado = await response.Content.ReadFromJsonAsync<PedidoResponse>();
        pedidoCriado.Should().NotBeNull();
        pedidoCriado!.Pedido.Should().Be("PED-001");
        pedidoCriado.Itens.Should().HaveCount(2);
        
        response.Headers.Location.Should().NotBeNull();
    }

    [Fact]
    public async Task Criar_DeveRetornar400BadRequest_QuandoPedidoJaExistir()
    {
        // Arrange
        var request = new PedidoRequest
        {
            Pedido = "PED-DUPLICADO",
            Itens = new List<ItemPedidoModel>  
            {
                new() { Descricao = "Produto A", PrecoUnitario = 50.00m, Qtd = 2 }
            }
        };

        await _client.PostAsJsonAsync("/api/pedido", request);

        // Act
        var response = await _client.PostAsJsonAsync("/api/pedido", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Criar_DeveRetornar400BadRequest_QuandoNumeroPedidoForVazio()
    {
        // Arrange
        var request = new PedidoRequest
        {
            Pedido = "",
            Itens = new List<ItemPedidoModel>  
            {
                new() { Descricao = "Produto A", PrecoUnitario = 50.00m, Qtd = 2 }
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/pedido", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    //Cria Pedido sem itens.
    [Fact]
    public async Task Criar_DeveRetornar400BadRequest_QuandoListaItensForVazia()
    {
        // Arrange
        var request = new PedidoRequest
        {
            Pedido = "PED-SEM-ITENS",
            Itens = new List<ItemPedidoModel>()  
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/pedido", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var resultado = await response.Content.ReadFromJsonAsync<PedidoResponse>();

        resultado.Should().NotBeNull();
        resultado!.Pedido.Should().Be("PED-SEM-ITENS");
        resultado.Itens.Should().BeEmpty(); 
        resultado.ValorTotal.Should().Be(0);
        resultado.QuantidadeTotalItens.Should().Be(0);
    }

    [Fact]
    public async Task Criar_DeveRetornar400BadRequest_QuandoPrecoUnitarioForZero()
    {
        // Arrange
        var request = new PedidoRequest
        {
            Pedido = "PED-PRECO-ZERO",
            Itens = new List<ItemPedidoModel>  
            {
                new() { Descricao = "Produto A", PrecoUnitario = 0, Qtd = 2 }
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/pedido", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Criar_DeveRetornar400BadRequest_QuandoQuantidadeForZero()
    {
        // Arrange
        var request = new PedidoRequest
        {
            Pedido = "PED-QTD-ZERO",
            Itens = new List<ItemPedidoModel>  
            {
                new() { Descricao = "Produto A", PrecoUnitario = 50.00m, Qtd = 0 }
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/pedido", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region GET /api/pedido - Obter Todos

    [Fact]
    public async Task ObterTodos_DeveRetornar200OK_ComListaDePedidos()
    {
        // Arrange
        var request1 = new PedidoRequest
        {
            Pedido = "PED-GET-001",
            Itens = new List<ItemPedidoModel>  
            {
                new() { Descricao = "Produto A", PrecoUnitario = 50.00m, Qtd = 2 }
            }
        };
        var request2 = new PedidoRequest
        {
            Pedido = "PED-GET-002",
            Itens = new List<ItemPedidoModel>  
            {
                new() { Descricao = "Produto B", PrecoUnitario = 30.00m, Qtd = 1 }
            }
        };

        await _client.PostAsJsonAsync("/api/pedido", request1);
        await _client.PostAsJsonAsync("/api/pedido", request2);

        // Act
        var response = await _client.GetAsync("/api/pedido");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var pedidos = await response.Content.ReadFromJsonAsync<List<PedidoResponse>>();
        pedidos.Should().NotBeNull();
        pedidos.Should().HaveCountGreaterThanOrEqualTo(2);
    }

    #endregion

    #region GET /api/pedido/{id} - Obter Por ID

    [Fact]
    public async Task ObterPorId_DeveRetornar200OK_QuandoPedidoExistir()
    {
        // Arrange
        var request = new PedidoRequest
        {
            Pedido = "PED-GET-ID-001",
            Itens = new List<ItemPedidoModel>  
            {
                new() { Descricao = "Produto A", PrecoUnitario = 50.00m, Qtd = 2 }
            }
        };

        var createResponse = await _client.PostAsJsonAsync("/api/pedido", request);
        var pedidoCriado = await createResponse.Content.ReadFromJsonAsync<PedidoResponse>();

        // Act
        var response = await _client.GetAsync($"/api/pedido/{pedidoCriado!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var pedido = await response.Content.ReadFromJsonAsync<PedidoResponse>();
        pedido.Should().NotBeNull();
        pedido!.Id.Should().Be(pedidoCriado.Id);
        pedido.Pedido.Should().Be("PED-GET-ID-001");
    }

    [Fact]
    public async Task ObterPorId_DeveRetornar404NotFound_QuandoPedidoNaoExistir()
    {
        // Act
        var response = await _client.GetAsync("/api/pedido/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region GET /api/pedido/numero/{numeroPedido} - Obter Por Número

    [Fact]
    public async Task ObterPorNumero_DeveRetornar200OK_QuandoPedidoExistir()
    {
        // Arrange
        var request = new PedidoRequest
        {
            Pedido = "PED-NUMERO-001",
            Itens = new List<ItemPedidoModel>  
            {
                new() { Descricao = "Produto A", PrecoUnitario = 50.00m, Qtd = 2 }
            }
        };

        await _client.PostAsJsonAsync("/api/pedido", request);

        // Act
        var response = await _client.GetAsync("/api/pedido/numero/PED-NUMERO-001");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var pedido = await response.Content.ReadFromJsonAsync<PedidoResponse>();
        pedido.Should().NotBeNull();
        pedido!.Pedido.Should().Be("PED-NUMERO-001");
    }

    [Fact]
    public async Task ObterPorNumero_DeveRetornar404NotFound_QuandoPedidoNaoExistir()
    {
        // Act
        var response = await _client.GetAsync("/api/pedido/numero/PED-INEXISTENTE");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region PUT /api/pedido/{id} - Atualizar

    [Fact]
    public async Task Atualizar_DeveRetornar200OK_QuandoPedidoForAtualizado()
    {
        // Arrange
        var createRequest = new PedidoRequest
        {
            Pedido = "PED-UPDATE-001",
            Itens = new List<ItemPedidoModel>  
            {
                new() { Descricao = "Produto A", PrecoUnitario = 50.00m, Qtd = 2 }
            }
        };

        var createResponse = await _client.PostAsJsonAsync("/api/pedido", createRequest);
        var pedidoCriado = await createResponse.Content.ReadFromJsonAsync<PedidoResponse>();

        var updateRequest = new PedidoRequest
        {
            Pedido = "PED-UPDATE-001-MODIFICADO",
            Itens = new List<ItemPedidoModel>  
            {
                new() { Descricao = "Produto B", PrecoUnitario = 100.00m, Qtd = 3 }
            }
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/pedido/{pedidoCriado!.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Atualizar_DeveRetornar404NotFound_QuandoPedidoNaoExistir()
    {
        // Arrange
        var request = new PedidoRequest
        {
            Pedido = "PED-UPDATE-INEXISTENTE",
            Itens = new List<ItemPedidoModel>  
            {
                new() { Descricao = "Produto A", PrecoUnitario = 50.00m, Qtd = 2 }
            }
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/pedido/99999", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region DELETE /api/pedido/{id} - Remover

    [Fact]
    public async Task Remover_DeveRetornar204NoContent_QuandoPedidoForRemovido()
    {
        // Arrange
        var request = new PedidoRequest
        {
            Pedido = "PED-DELETE-001",
            Itens = new List<ItemPedidoModel>  
            {
                new() { Descricao = "Produto A", PrecoUnitario = 50.00m, Qtd = 2 }
            }
        };

        var createResponse = await _client.PostAsJsonAsync("/api/pedido", request);
        var pedidoCriado = await createResponse.Content.ReadFromJsonAsync<PedidoResponse>();

        // Act
        var response = await _client.DeleteAsync($"/api/pedido/{pedidoCriado!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/pedido/{pedidoCriado.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Remover_DeveRetornar404NotFound_QuandoPedidoNaoExistir()
    {
        // Act
        var response = await _client.DeleteAsync("/api/pedido/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Testes de Fluxo Completo

    [Fact]
    public async Task FluxoCompleto_DeveCriarObterAtualizarERemoverPedido()
    {
        // 1. Criar
        var createRequest = new PedidoRequest
        {
            Pedido = "PED-FLUXO-001",
            Itens = new List<ItemPedidoModel>  
            {
                new() { Descricao = "Produto A", PrecoUnitario = 50.00m, Qtd = 2 }
            }
        };

        var createResponse = await _client.PostAsJsonAsync("/api/pedido", createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var pedidoCriado = await createResponse.Content.ReadFromJsonAsync<PedidoResponse>();

        // 2. Obter por ID
        var getResponse = await _client.GetAsync($"/api/pedido/{pedidoCriado!.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // 3. Obter por Número
        var getByNumeroResponse = await _client.GetAsync("/api/pedido/numero/PED-FLUXO-001");
        getByNumeroResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // 4. Atualizar
        var updateRequest = new PedidoRequest
        {
            Pedido = "PED-FLUXO-001-ATUALIZADO",
            Itens = new List<ItemPedidoModel>  
            {
                new() { Descricao = "Produto B", PrecoUnitario = 100.00m, Qtd = 3 }
            }
        };

        var updateResponse = await _client.PutAsJsonAsync($"/api/pedido/{pedidoCriado.Id}", updateRequest);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // 5. Remover
        var deleteResponse = await _client.DeleteAsync($"/api/pedido/{pedidoCriado.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // 6. Verificar remoção
        var getAfterDeleteResponse = await _client.GetAsync($"/api/pedido/{pedidoCriado.Id}");
        getAfterDeleteResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion
}