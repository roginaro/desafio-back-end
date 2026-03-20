using System.Net;
using System.Net.Http.Json;
using Desafio.Application.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Desafio.Testes.Integration.Controllers;

public class StatusControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public StatusControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    #region Regra 1: CODIGO_PEDIDO_INVALIDO

    [Fact]
    public async Task ProcessarStatus_DeveRetornarCodigoPedidoInvalido_QuandoPedidoNaoExistir()
    {
        // Arrange
        var request = new StatusRequest
        {
            Pedido = "PED-INEXISTENTE",
            Status = "APROVADO",
            ValorAprovado = 100,
            ItensAprovados = 2
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/status", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var resultado = await response.Content.ReadFromJsonAsync<StatusResponse>();
        resultado.Should().NotBeNull();
        resultado!.Pedido.Should().Be("PED-INEXISTENTE");
        resultado.Status.Should().HaveCount(1);
        resultado.Status.Should().Contain("CODIGO_PEDIDO_INVALIDO");
    }

    #endregion

    #region Regra 2: REPROVADO

    [Theory]
    [InlineData("REPROVADO")]
    [InlineData("reprovado")]
    [InlineData("Reprovado")]
    [InlineData("RePrOvAdO")]
    public async Task ProcessarStatus_DeveRetornarReprovado_QuandoStatusForReprovado(string status)
    {
        // Arrange - Criar pedido primeiro
        var pedidoRequest = new PedidoRequest
        {
            Pedido = $"PED-REPROVADO-{Guid.NewGuid()}",
            Itens = new List<ItemPedidoModel>
            {
                new() { Descricao = "Produto A", PrecoUnitario = 50.00m, Qtd = 2 }
            }
        };
        await _client.PostAsJsonAsync("/api/pedido", pedidoRequest);

        var statusRequest = new StatusRequest
        {
            Pedido = pedidoRequest.Pedido,
            Status = status,
            ValorAprovado = 100,
            ItensAprovados = 2
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/status", statusRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var resultado = await response.Content.ReadFromJsonAsync<StatusResponse>();
        resultado.Should().NotBeNull();
        resultado!.Status.Should().HaveCount(1);
        resultado.Status.Should().Contain("REPROVADO");
    }

    #endregion

    #region Regra 3: APROVADO (Completo)

    [Theory]
    [InlineData("APROVADO")]
    [InlineData("aprovado")]
    [InlineData("Aprovado")]
    [InlineData("ApRoVaDo")]
    public async Task ProcessarStatus_DeveRetornarAprovado_QuandoValorEQuantidadeForemExatos(string status)
    {
        // Arrange - Criar pedido
        var pedidoRequest = new PedidoRequest
        {
            Pedido = $"PED-APROVADO-{Guid.NewGuid()}",
            Itens = new List<ItemPedidoModel>
            {
                new() { Descricao = "Produto A", PrecoUnitario = 50.00m, Qtd = 2 }
            }
        };
        await _client.PostAsJsonAsync("/api/pedido", pedidoRequest);

        var statusRequest = new StatusRequest
        {
            Pedido = pedidoRequest.Pedido,
            Status = status,
            ValorAprovado = 100.00m,  // 50 * 2 = 100
            ItensAprovados = 2
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/status", statusRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var resultado = await response.Content.ReadFromJsonAsync<StatusResponse>();
        resultado.Should().NotBeNull();
        resultado!.Status.Should().HaveCount(1);
        resultado.Status.Should().Contain("APROVADO");
    }

    #endregion

    #region Regra 4: APROVADO_VALOR_A_MENOR

    [Fact]
    public async Task ProcessarStatus_DeveRetornarAprovadoValorAMenor_QuandoValorAprovadoForMenor()
    {
        // Arrange - Criar pedido
        var pedidoRequest = new PedidoRequest
        {
            Pedido = $"PED-VALOR-MENOR-{Guid.NewGuid()}",
            Itens = new List<ItemPedidoModel>
            {
                new() { Descricao = "Produto A", PrecoUnitario = 50.00m, Qtd = 2 }
            }
        };
        await _client.PostAsJsonAsync("/api/pedido", pedidoRequest);

        var statusRequest = new StatusRequest
        {
            Pedido = pedidoRequest.Pedido,
            Status = "APROVADO",
            ValorAprovado = 80.00m,  // Menor que 100
            ItensAprovados = 2
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/status", statusRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var resultado = await response.Content.ReadFromJsonAsync<StatusResponse>();
        resultado.Should().NotBeNull();
        resultado!.Status.Should().Contain("APROVADO_VALOR_A_MENOR");
    }

    #endregion

    #region Regra 5: APROVADO_VALOR_A_MAIOR

    [Fact]
    public async Task ProcessarStatus_DeveRetornarAprovadoValorAMaior_QuandoValorAprovadoForMaior()
    {
        // Arrange - Criar pedido
        var pedidoRequest = new PedidoRequest
        {
            Pedido = $"PED-VALOR-MAIOR-{Guid.NewGuid()}",
            Itens = new List<ItemPedidoModel>
            {
                new() { Descricao = "Produto A", PrecoUnitario = 50.00m, Qtd = 2 }
            }
        };
        await _client.PostAsJsonAsync("/api/pedido", pedidoRequest);

        var statusRequest = new StatusRequest
        {
            Pedido = pedidoRequest.Pedido,
            Status = "APROVADO",
            ValorAprovado = 120.00m,  // Maior que 100
            ItensAprovados = 2
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/status", statusRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var resultado = await response.Content.ReadFromJsonAsync<StatusResponse>();
        resultado.Should().NotBeNull();
        resultado!.Status.Should().Contain("APROVADO_VALOR_A_MAIOR");
    }

    #endregion

    #region Regra 6: APROVADO_QTD_A_MENOR

    [Fact]
    public async Task ProcessarStatus_DeveRetornarAprovadoQtdAMenor_QuandoQuantidadeAprovadaForMenor()
    {
        // Arrange - Criar pedido
        var pedidoRequest = new PedidoRequest
        {
            Pedido = $"PED-QTD-MENOR-{Guid.NewGuid()}",
            Itens = new List<ItemPedidoModel>
            {
                new() { Descricao = "Produto A", PrecoUnitario = 50.00m, Qtd = 2 }
            }
        };
        await _client.PostAsJsonAsync("/api/pedido", pedidoRequest);

        var statusRequest = new StatusRequest
        {
            Pedido = pedidoRequest.Pedido,
            Status = "APROVADO",
            ValorAprovado = 100.00m,
            ItensAprovados = 1  // Menor que 2
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/status", statusRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var resultado = await response.Content.ReadFromJsonAsync<StatusResponse>();
        resultado.Should().NotBeNull();
        resultado!.Status.Should().Contain("APROVADO_QTD_A_MENOR");
    }

    #endregion

    #region Regra 7: APROVADO_QTD_A_MAIOR

    [Fact]
    public async Task ProcessarStatus_DeveRetornarAprovadoQtdAMaior_QuandoQuantidadeAprovadaForMaior()
    {
        // Arrange - Criar pedido
        var pedidoRequest = new PedidoRequest
        {
            Pedido = $"PED-QTD-MAIOR-{Guid.NewGuid()}",
            Itens = new List<ItemPedidoModel>
            {
                new() { Descricao = "Produto A", PrecoUnitario = 50.00m, Qtd = 2 }
            }
        };
        await _client.PostAsJsonAsync("/api/pedido", pedidoRequest);

        var statusRequest = new StatusRequest
        {
            Pedido = pedidoRequest.Pedido,
            Status = "APROVADO",
            ValorAprovado = 100.00m,
            ItensAprovados = 3  // Maior que 2
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/status", statusRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var resultado = await response.Content.ReadFromJsonAsync<StatusResponse>();
        resultado.Should().NotBeNull();
        resultado!.Status.Should().Contain("APROVADO_QTD_A_MAIOR");
    }

    #endregion

    #region Múltiplos Status

    [Fact]
    public async Task ProcessarStatus_DeveRetornarMultiplosStatus_QuandoHouverDivergenciaEmValorEQuantidade()
    {
        // Arrange - Criar pedido
        var pedidoRequest = new PedidoRequest
        {
            Pedido = $"PED-MULTIPLOS-1-{Guid.NewGuid()}",
            Itens = new List<ItemPedidoModel>
            {
                new() { Descricao = "Produto A", PrecoUnitario = 50.00m, Qtd = 2 }
            }
        };
        await _client.PostAsJsonAsync("/api/pedido", pedidoRequest);

        var statusRequest = new StatusRequest
        {
            Pedido = pedidoRequest.Pedido,
            Status = "APROVADO",
            ValorAprovado = 80.00m,   // Menor que 100
            ItensAprovados = 3        // Maior que 2
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/status", statusRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var resultado = await response.Content.ReadFromJsonAsync<StatusResponse>();
        resultado.Should().NotBeNull();
        resultado!.Status.Should().HaveCount(2);
        resultado.Status.Should().Contain("APROVADO_VALOR_A_MENOR");
        resultado.Status.Should().Contain("APROVADO_QTD_A_MAIOR");
    }

    [Fact]
    public async Task ProcessarStatus_DeveRetornarMultiplosStatus_QuandoValorEQuantidadeForemMaiores()
    {
        // Arrange - Criar pedido
        var pedidoRequest = new PedidoRequest
        {
            Pedido = $"PED-MULTIPLOS-2-{Guid.NewGuid()}",
            Itens = new List<ItemPedidoModel>
            {
                new() { Descricao = "Produto A", PrecoUnitario = 50.00m, Qtd = 2 }
            }
        };
        await _client.PostAsJsonAsync("/api/pedido", pedidoRequest);

        var statusRequest = new StatusRequest
        {
            Pedido = pedidoRequest.Pedido,
            Status = "APROVADO",
            ValorAprovado = 120.00m,  // Maior que 100
            ItensAprovados = 3        // Maior que 2
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/status", statusRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var resultado = await response.Content.ReadFromJsonAsync<StatusResponse>();
        resultado.Should().NotBeNull();
        resultado!.Status.Should().HaveCount(2);
        resultado.Status.Should().Contain("APROVADO_VALOR_A_MAIOR");
        resultado.Status.Should().Contain("APROVADO_QTD_A_MAIOR");
    }

    [Fact]
    public async Task ProcessarStatus_DeveRetornarMultiplosStatus_QuandoValorEQuantidadeForemMenores()
    {
        // Arrange - Criar pedido
        var pedidoRequest = new PedidoRequest
        {
            Pedido = $"PED-MULTIPLOS-3-{Guid.NewGuid()}",
            Itens = new List<ItemPedidoModel>
            {
                new() { Descricao = "Produto A", PrecoUnitario = 50.00m, Qtd = 2 }
            }
        };
        await _client.PostAsJsonAsync("/api/pedido", pedidoRequest);

        var statusRequest = new StatusRequest
        {
            Pedido = pedidoRequest.Pedido,
            Status = "APROVADO",
            ValorAprovado = 80.00m,   // Menor que 100
            ItensAprovados = 1        // Menor que 2
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/status", statusRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var resultado = await response.Content.ReadFromJsonAsync<StatusResponse>();
        resultado.Should().NotBeNull();
        resultado!.Status.Should().HaveCount(2);
        resultado.Status.Should().Contain("APROVADO_VALOR_A_MENOR");
        resultado.Status.Should().Contain("APROVADO_QTD_A_MENOR");
    }

    #endregion

    #region Edge Cases

    [Fact]
    public async Task ProcessarStatus_DeveRetornarListaVazia_QuandoStatusNaoForAprovadoNemReprovado()
    {
        // Arrange - Criar pedido
        var pedidoRequest = new PedidoRequest
        {
            Pedido = $"PED-PENDENTE-{Guid.NewGuid()}",
            Itens = new List<ItemPedidoModel>
            {
                new() { Descricao = "Produto A", PrecoUnitario = 50.00m, Qtd = 2 }
            }
        };
        await _client.PostAsJsonAsync("/api/pedido", pedidoRequest);

        var statusRequest = new StatusRequest
        {
            Pedido = pedidoRequest.Pedido,
            Status = "PENDENTE",
            ValorAprovado = 100.00m,
            ItensAprovados = 2
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/status", statusRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var resultado = await response.Content.ReadFromJsonAsync<StatusResponse>();
        resultado.Should().NotBeNull();
        resultado!.Status.Should().BeEmpty();
    }

    [Fact]
    public async Task ProcessarStatus_DeveRetornarAprovado_QuandoPedidoTiverMultiplosItens()
    {
        // Arrange - Criar pedido com múltiplos itens
        var pedidoRequest = new PedidoRequest
        {
            Pedido = $"PED-MULTIPLOS-ITENS-{Guid.NewGuid()}",
            Itens = new List<ItemPedidoModel>
            {
                new() { Descricao = "Produto A", PrecoUnitario = 50.00m, Qtd = 2 },   // 100
                new() { Descricao = "Produto B", PrecoUnitario = 50.00m, Qtd = 3 },   // 150
                new() { Descricao = "Produto C", PrecoUnitario = 100.00m, Qtd = 1 }   // 100
            }
        };
        await _client.PostAsJsonAsync("/api/pedido", pedidoRequest);

        var statusRequest = new StatusRequest
        {
            Pedido = pedidoRequest.Pedido,
            Status = "APROVADO",
            ValorAprovado = 350.00m,  // 100 + 150 + 100
            ItensAprovados = 6        // 2 + 3 + 1
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/status", statusRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var resultado = await response.Content.ReadFromJsonAsync<StatusResponse>();
        resultado.Should().NotBeNull();
        resultado!.Status.Should().HaveCount(1);
        resultado.Status.Should().Contain("APROVADO");
    }

    #endregion

    #region Cenários do Documento

    [Fact]
    public async Task ProcessarStatus_Cenario1_PedidoInexistente()
    {
        // Arrange
        var request = new StatusRequest
        {
            Pedido = "1",
            Status = "APROVADO",
            ValorAprovado = 150.00m,
            ItensAprovados = 1
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/status", request);

        // Assert
        var resultado = await response.Content.ReadFromJsonAsync<StatusResponse>();
        resultado!.Status.Should().Contain("CODIGO_PEDIDO_INVALIDO");
    }

    [Fact]
    public async Task ProcessarStatus_Cenario2_Reprovado()
    {
        // Arrange - Criar pedido
        var pedidoRequest = new PedidoRequest
        {
            Pedido = $"PED-CENARIO-2-{Guid.NewGuid()}",
            Itens = new List<ItemPedidoModel>
            {
                new() { Descricao = "Camiseta", PrecoUnitario = 35.00m, Qtd = 1 },
                new() { Descricao = "Calça", PrecoUnitario = 115.00m, Qtd = 1 }
            }
        };
        await _client.PostAsJsonAsync("/api/pedido", pedidoRequest);

        var statusRequest = new StatusRequest
        {
            Pedido = pedidoRequest.Pedido,
            Status = "REPROVADO",
            ValorAprovado = 150.00m,
            ItensAprovados = 2
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/status", statusRequest);

        // Assert
        var resultado = await response.Content.ReadFromJsonAsync<StatusResponse>();
        resultado!.Status.Should().Contain("REPROVADO");
    }

    #endregion
}