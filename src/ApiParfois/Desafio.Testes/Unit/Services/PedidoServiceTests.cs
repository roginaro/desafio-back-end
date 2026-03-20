using Desafio.Application.Models;
using Desafio.Application.Services;
using Desafio.Domain.Entities;
using Desafio.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace Desafio.Testes.Unit.Services;

public class PedidoServiceTests
{
    private readonly Mock<IPedidoRepository> _repositoryMock;
    private readonly PedidoService _service;

    public PedidoServiceTests()
    {
        _repositoryMock = new Mock<IPedidoRepository>();
        _service = new PedidoService(_repositoryMock.Object);
    }

    #region CriarAsync - Casos de Sucesso

    [Fact]
    public async Task CriarAsync_DeveCriarPedido_QuandoDadosForemValidos()
    {
        // Arrange
        var request = new PedidoRequest
        {
            Pedido = "PED-001",
            Itens = new List<ItemPedidoModel>
            {
                new() { Descricao = "Produto A", PrecoUnitario = 50.00m, Qtd = 2 }
            }
        };

        _repositoryMock.Setup(x => x.ExisteAsync("PED-001")).ReturnsAsync(false);
        _repositoryMock.Setup(x => x.AdicionarAsync(It.IsAny<Pedido>())).Returns(Task.CompletedTask);

        // Act
        var resultado = await _service.CriarAsync(request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Pedido.Should().Be("PED-001");
        resultado.Itens.Should().HaveCount(1);
        resultado.ValorTotal.Should().Be(100.00m);
        resultado.QuantidadeTotalItens.Should().Be(2);

        _repositoryMock.Verify(x => x.ExisteAsync("PED-001"), Times.Once);
        _repositoryMock.Verify(x => x.AdicionarAsync(It.IsAny<Pedido>()), Times.Once);
    }

    [Fact]
    public async Task CriarAsync_DeveCriarPedido_ComMultiplosItens()
    {
        // Arrange
        var request = new PedidoRequest
        {
            Pedido = "PED-002",
            Itens = new List<ItemPedidoModel>
            {
                new() { Descricao = "Produto A", PrecoUnitario = 50.00m, Qtd = 2 },
                new() { Descricao = "Produto B", PrecoUnitario = 30.00m, Qtd = 1 },
                new() { Descricao = "Produto C", PrecoUnitario = 100.00m, Qtd = 3 }
            }
        };

        _repositoryMock.Setup(x => x.ExisteAsync("PED-002")).ReturnsAsync(false);
        _repositoryMock.Setup(x => x.AdicionarAsync(It.IsAny<Pedido>())).Returns(Task.CompletedTask);

        // Act
        var resultado = await _service.CriarAsync(request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Itens.Should().HaveCount(3);
        resultado.ValorTotal.Should().Be(430.00m); // 100 + 30 + 300
        resultado.QuantidadeTotalItens.Should().Be(6); // 2 + 1 + 3
    }

    #endregion

    #region CriarAsync - Validações

    [Fact]
    public async Task CriarAsync_DeveLancarInvalidOperationException_QuandoPedidoJaExistir()
    {
        // Arrange
        var request = new PedidoRequest
        {
            Pedido = "PED-001",
            Itens = new List<ItemPedidoModel>
            {
                new() { Descricao = "Produto A", PrecoUnitario = 50.00m, Qtd = 2 }
            }
        };

        _repositoryMock.Setup(x => x.ExisteAsync("PED-001")).ReturnsAsync(true);

        // Act
        Func<Task> act = async () => await _service.CriarAsync(request);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*PED-001*já existe*");

        _repositoryMock.Verify(x => x.AdicionarAsync(It.IsAny<Pedido>()), Times.Never);
    }

    [Fact]
    public async Task CriarAsync_DeveLancarArgumentException_QuandoNumeroPedidoForVazio()
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
        Func<Task> act = async () => await _service.CriarAsync(request);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    #endregion

    #region ObterPorIdAsync

    [Fact]
    public async Task ObterPorIdAsync_DeveRetornarPedido_QuandoPedidoExistir()
    {
        // Arrange
        var pedido = new Pedido("PED-001");
        pedido.AdicionarItem("Produto A", 50.00m, 2);

        _repositoryMock.Setup(x => x.ObterPorIdAsync(1)).ReturnsAsync(pedido);

        // Act
        var resultado = await _service.ObterPorIdAsync(1);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Pedido.Should().Be("PED-001");
        resultado.Itens.Should().HaveCount(1);
        resultado.ValorTotal.Should().Be(100.00m);
    }

    [Fact]
    public async Task ObterPorIdAsync_DeveRetornarNull_QuandoPedidoNaoExistir()
    {
        // Arrange
        _repositoryMock.Setup(x => x.ObterPorIdAsync(999)).ReturnsAsync((Pedido?)null);

        // Act
        var resultado = await _service.ObterPorIdAsync(999);

        // Assert
        resultado.Should().BeNull();
    }

    #endregion

    #region ObterPorNumeroAsync

    [Fact]
    public async Task ObterPorNumeroAsync_DeveRetornarPedido_QuandoPedidoExistir()
    {
        // Arrange
        var pedido = new Pedido("PED-001");
        pedido.AdicionarItem("Produto A", 50.00m, 2);

        _repositoryMock.Setup(x => x.ObterPorNumeroAsync("PED-001")).ReturnsAsync(pedido);

        // Act
        var resultado = await _service.ObterPorNumeroAsync("PED-001");

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Pedido.Should().Be("PED-001");
        resultado.ValorTotal.Should().Be(100.00m);
    }

    [Fact]
    public async Task ObterPorNumeroAsync_DeveRetornarNull_QuandoPedidoNaoExistir()
    {
        // Arrange
        _repositoryMock.Setup(x => x.ObterPorNumeroAsync("PED-999")).ReturnsAsync((Pedido?)null);

        // Act
        var resultado = await _service.ObterPorNumeroAsync("PED-999");

        // Assert
        resultado.Should().BeNull();
    }

    #endregion

    #region ObterTodosAsync

    [Fact]
    public async Task ObterTodosAsync_DeveRetornarListaPedidos_QuandoExistiremPedidos()
    {
        // Arrange
        var pedidos = new List<Pedido>
        {
            new("PED-001"),
            new("PED-002"),
            new("PED-003")
        };

        pedidos[0].AdicionarItem("Produto A", 50.00m, 2);
        pedidos[1].AdicionarItem("Produto B", 30.00m, 1);
        pedidos[2].AdicionarItem("Produto C", 100.00m, 3);

        _repositoryMock.Setup(x => x.ObterTodosAsync()).ReturnsAsync(pedidos);

        // Act
        var resultado = await _service.ObterTodosAsync();

        // Assert
        resultado.Should().HaveCount(3);
        resultado.Should().Contain(p => p.Pedido == "PED-001");
        resultado.Should().Contain(p => p.Pedido == "PED-002");
        resultado.Should().Contain(p => p.Pedido == "PED-003");
    }

    [Fact]
    public async Task ObterTodosAsync_DeveRetornarListaVazia_QuandoNaoExistiremPedidos()
    {
        // Arrange
        _repositoryMock.Setup(x => x.ObterTodosAsync()).ReturnsAsync(new List<Pedido>());

        // Act
        var resultado = await _service.ObterTodosAsync();

        // Assert
        resultado.Should().BeEmpty();
    }

    #endregion

    #region AtualizarAsync - Casos de Sucesso

    [Fact]
    public async Task AtualizarAsync_DeveAtualizarPedido_QuandoPedidoExistir()
    {
        // Arrange
        var pedidoExistente = new Pedido("PED-001");
        pedidoExistente.AdicionarItem("Produto A", 50.00m, 2);

        var request = new PedidoRequest
        {
            Pedido = "PED-001-UPDATED",
            Itens = new List<ItemPedidoModel>
            {
                new() { Descricao = "Produto X", PrecoUnitario = 100.00m, Qtd = 1 }
            }
        };

        _repositoryMock.Setup(x => x.ObterPorIdComItensAsync(1)).ReturnsAsync(pedidoExistente);
        _repositoryMock.Setup(x => x.SalvarAlteracoesAsync()).Returns(Task.CompletedTask);

        // Act
        var resultado = await _service.AtualizarAsync(1, request);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Pedido.Should().Be("PED-001-UPDATED");
        resultado.Itens.Should().HaveCount(1);
        resultado.Itens.First().Descricao.Should().Be("Produto X");
        resultado.ValorTotal.Should().Be(100.00m);

        _repositoryMock.Verify(x => x.SalvarAlteracoesAsync(), Times.Once);
    }

    [Fact]
    public async Task AtualizarAsync_DeveSubstituirItens_QuandoAtualizarPedido()
    {
        // Arrange
        var pedidoExistente = new Pedido("PED-001");
        pedidoExistente.AdicionarItem("Produto A", 50.00m, 2);
        pedidoExistente.AdicionarItem("Produto B", 30.00m, 1);

        var request = new PedidoRequest
        {
            Pedido = "PED-001",
            Itens = new List<ItemPedidoModel>
            {
                new() { Descricao = "Produto X", PrecoUnitario = 100.00m, Qtd = 1 }
            }
        };

        _repositoryMock.Setup(x => x.ObterPorIdComItensAsync(1)).ReturnsAsync(pedidoExistente);
        _repositoryMock.Setup(x => x.SalvarAlteracoesAsync()).Returns(Task.CompletedTask);

        // Act
        var resultado = await _service.AtualizarAsync(1, request);

        // Assert
        resultado!.Itens.Should().HaveCount(1);
        resultado.Itens.Should().NotContain(i => i.Descricao == "Produto A");
        resultado.Itens.Should().NotContain(i => i.Descricao == "Produto B");
        resultado.Itens.Should().Contain(i => i.Descricao == "Produto X");
    }

    [Fact]
    public async Task AtualizarAsync_NaoDeveAtualizarNumero_QuandoNumeroForIgual()
    {
        // Arrange
        var pedidoExistente = new Pedido("PED-001");
        pedidoExistente.AdicionarItem("Produto A", 50.00m, 2);

        var request = new PedidoRequest
        {
            Pedido = "PED-001", // Mesmo número
            Itens = new List<ItemPedidoModel>
            {
                new() { Descricao = "Produto X", PrecoUnitario = 100.00m, Qtd = 1 }
            }
        };

        _repositoryMock.Setup(x => x.ObterPorIdComItensAsync(1)).ReturnsAsync(pedidoExistente);
        _repositoryMock.Setup(x => x.SalvarAlteracoesAsync()).Returns(Task.CompletedTask);

        // Act
        var resultado = await _service.AtualizarAsync(1, request);

        // Assert
        resultado!.Pedido.Should().Be("PED-001");
        _repositoryMock.Verify(x => x.SalvarAlteracoesAsync(), Times.Once);
    }

    [Fact]
    public async Task AtualizarAsync_DeveRetornarNull_QuandoPedidoNaoExistir()
    {
        // Arrange
        var request = new PedidoRequest
        {
            Pedido = "PED-001",
            Itens = new List<ItemPedidoModel>
            {
                new() { Descricao = "Produto A", PrecoUnitario = 50.00m, Qtd = 2 }
            }
        };

        _repositoryMock.Setup(x => x.ObterPorIdComItensAsync(999)).ReturnsAsync((Pedido?)null);

        // Act
        var resultado = await _service.AtualizarAsync(999, request);

        // Assert
        resultado.Should().BeNull();
        _repositoryMock.Verify(x => x.SalvarAlteracoesAsync(), Times.Never);
    }

    #endregion

    #region RemoverAsync

    [Fact]
    public async Task RemoverAsync_DeveRemoverPedido_QuandoPedidoExistir()
    {
        // Arrange
        var pedido = new Pedido("PED-001");

        _repositoryMock.Setup(x => x.ObterPorIdAsync(1)).ReturnsAsync(pedido);
        _repositoryMock.Setup(x => x.RemoverAsync(1)).Returns(Task.CompletedTask);

        // Act
        var resultado = await _service.RemoverAsync(1);

        // Assert
        resultado.Should().BeTrue();
        _repositoryMock.Verify(x => x.RemoverAsync(1), Times.Once);
    }

    [Fact]
    public async Task RemoverAsync_DeveRetornarFalse_QuandoPedidoNaoExistir()
    {
        // Arrange
        _repositoryMock.Setup(x => x.ObterPorIdAsync(999)).ReturnsAsync((Pedido?)null);

        // Act
        var resultado = await _service.RemoverAsync(999);

        // Assert
        resultado.Should().BeFalse();
        _repositoryMock.Verify(x => x.RemoverAsync(It.IsAny<int>()), Times.Never);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public async Task CriarAsync_DeveCalcularValorTotalCorretamente_ComPrecisaoDecimal()
    {
        // Arrange
        var request = new PedidoRequest
        {
            Pedido = "PED-003",
            Itens = new List<ItemPedidoModel>
            {
                new() { Descricao = "Produto A", PrecoUnitario = 33.33m, Qtd = 3 },
                new() { Descricao = "Produto B", PrecoUnitario = 66.67m, Qtd = 2 }
            }
        };

        _repositoryMock.Setup(x => x.ExisteAsync("PED-003")).ReturnsAsync(false);
        _repositoryMock.Setup(x => x.AdicionarAsync(It.IsAny<Pedido>())).Returns(Task.CompletedTask);

        // Act
        var resultado = await _service.CriarAsync(request);

        // Assert
        resultado.ValorTotal.Should().Be(233.33m); // 99.99 + 133.34
    }

    [Fact]
    public async Task ObterTodosAsync_DeveMapearCorretamente_TodosOsCampos()
    {
        // Arrange
        var pedido = new Pedido("PED-001");
        pedido.AdicionarItem("Produto A", 50.00m, 2);

        _repositoryMock.Setup(x => x.ObterTodosAsync()).ReturnsAsync(new List<Pedido> { pedido });

        // Act
        var resultado = (await _service.ObterTodosAsync()).ToList();

        // Assert
        resultado.Should().HaveCount(1);
        var pedidoResponse = resultado.First();
        pedidoResponse.Pedido.Should().Be("PED-001");
        pedidoResponse.DataCriacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
        pedidoResponse.Itens.Should().HaveCount(1);
        pedidoResponse.Itens.First().Descricao.Should().Be("Produto A");
        pedidoResponse.Itens.First().PrecoUnitario.Should().Be(50.00m);
        pedidoResponse.Itens.First().Qtd.Should().Be(2);
    }

    #endregion
}