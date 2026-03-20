using Desafio.Application.Models;
using Desafio.Application.Services;
using Desafio.Application.Validators;
using Desafio.Domain.Entities;
using Desafio.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace Desafio.Testes.Unit.Services;

public class StatusServiceTests
{
    private readonly Mock<IPedidoRepository> _repositoryMock;
    private readonly Mock<IStatusValidator> _validatorMock;
    private readonly StatusService _service;

    public StatusServiceTests()
    {
        _repositoryMock = new Mock<IPedidoRepository>();
        _validatorMock = new Mock<IStatusValidator>();
        _service = new StatusService(_repositoryMock.Object, _validatorMock.Object);
    }

    #region ProcessarStatusAsync - Casos de Sucesso

    [Fact]
    public async Task ProcessarStatusAsync_DeveRetornarStatusAprovado_QuandoPedidoExistirEForValido()
    {
        // Arrange
        var request = new StatusRequest
        {
            Pedido = "PED-001",
            Status = "aprovado",
            ItensAprovados = 2,
            ValorAprovado = 100.00m
        };

        var pedido = new Pedido("PED-001");
        pedido.AdicionarItem("Produto A", 50.00m, 2);

        var statusList = new List<string> { "aprovado" };

        _repositoryMock.Setup(x => x.ObterPorNumeroAsync("PED-001")).ReturnsAsync(pedido);
        _validatorMock.Setup(x => x.Validar(request, pedido)).Returns(statusList);

        // Act
        var resultado = await _service.ProcessarStatusAsync(request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Pedido.Should().Be("PED-001");
        resultado.Status.Should().HaveCount(1);
        resultado.Status.Should().Contain("aprovado");

        _repositoryMock.Verify(x => x.ObterPorNumeroAsync("PED-001"), Times.Once);
        _validatorMock.Verify(x => x.Validar(request, pedido), Times.Once);
    }

    [Fact]
    public async Task ProcessarStatusAsync_DeveRetornarStatusReprovado_QuandoPedidoNaoExistir()
    {
        // Arrange
        var request = new StatusRequest
        {
            Pedido = "PED-999",
            Status = "aprovado",
            ItensAprovados = 2,
            ValorAprovado = 100.00m
        };

        var statusList = new List<string> { "reprovado - pedido não encontrado" };

        _repositoryMock.Setup(x => x.ObterPorNumeroAsync("PED-999")).ReturnsAsync((Pedido?)null);
        _validatorMock.Setup(x => x.Validar(request, null)).Returns(statusList);

        // Act
        var resultado = await _service.ProcessarStatusAsync(request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Pedido.Should().Be("PED-999");
        resultado.Status.Should().HaveCount(1);
        resultado.Status.Should().Contain("reprovado - pedido não encontrado");

        _repositoryMock.Verify(x => x.ObterPorNumeroAsync("PED-999"), Times.Once);
        _validatorMock.Verify(x => x.Validar(request, null), Times.Once);
    }

    [Fact]
    public async Task ProcessarStatusAsync_DeveRetornarMultiplosStatus_QuandoHouverMultiplasValidacoes()
    {
        // Arrange
        var request = new StatusRequest
        {
            Pedido = "PED-001",
            Status = "aprovado",
            ItensAprovados = 3,
            ValorAprovado = 150.00m
        };

        var pedido = new Pedido("PED-001");
        pedido.AdicionarItem("Produto A", 50.00m, 2);

        var statusList = new List<string>
        {
            "reprovado - quantidade de itens divergente",
            "reprovado - valor aprovado divergente"
        };

        _repositoryMock.Setup(x => x.ObterPorNumeroAsync("PED-001")).ReturnsAsync(pedido);
        _validatorMock.Setup(x => x.Validar(request, pedido)).Returns(statusList);

        // Act
        var resultado = await _service.ProcessarStatusAsync(request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Pedido.Should().Be("PED-001");
        resultado.Status.Should().HaveCount(2);
        resultado.Status.Should().Contain("reprovado - quantidade de itens divergente");
        resultado.Status.Should().Contain("reprovado - valor aprovado divergente");
    }

    #endregion

    #region ProcessarStatusAsync - Diferentes Cenários de Status

    [Fact]
    public async Task ProcessarStatusAsync_DeveProcessarCorretamente_QuandoStatusForAprovadoParcialmente()
    {
        // Arrange
        var request = new StatusRequest
        {
            Pedido = "PED-002",
            Status = "aprovado",
            ItensAprovados = 1,
            ValorAprovado = 50.00m
        };

        var pedido = new Pedido("PED-002");
        pedido.AdicionarItem("Produto A", 50.00m, 2);

        var statusList = new List<string> { "aprovado parcialmente" };

        _repositoryMock.Setup(x => x.ObterPorNumeroAsync("PED-002")).ReturnsAsync(pedido);
        _validatorMock.Setup(x => x.Validar(request, pedido)).Returns(statusList);

        // Act
        var resultado = await _service.ProcessarStatusAsync(request);

        // Assert
        resultado.Status.Should().Contain("aprovado parcialmente");
    }

    [Fact]
    public async Task ProcessarStatusAsync_DeveProcessarCorretamente_QuandoStatusForReprovado()
    {
        // Arrange
        var request = new StatusRequest
        {
            Pedido = "PED-003",
            Status = "reprovado",
            ItensAprovados = 0,
            ValorAprovado = 0m
        };

        var pedido = new Pedido("PED-003");
        pedido.AdicionarItem("Produto A", 50.00m, 2);

        var statusList = new List<string> { "reprovado" };

        _repositoryMock.Setup(x => x.ObterPorNumeroAsync("PED-003")).ReturnsAsync(pedido);
        _validatorMock.Setup(x => x.Validar(request, pedido)).Returns(statusList);

        // Act
        var resultado = await _service.ProcessarStatusAsync(request);

        // Assert
        resultado.Status.Should().Contain("reprovado");
    }

    #endregion

    #region ProcessarStatusAsync - Validação de Integração

    [Fact]
    public async Task ProcessarStatusAsync_DeveChamarRepositorio_ApenasUmaVez()
    {
        // Arrange
        var request = new StatusRequest
        {
            Pedido = "PED-001",
            Status = "aprovado",
            ItensAprovados = 2,
            ValorAprovado = 100.00m
        };

        var pedido = new Pedido("PED-001");
        var statusList = new List<string> { "aprovado" };

        _repositoryMock.Setup(x => x.ObterPorNumeroAsync("PED-001")).ReturnsAsync(pedido);
        _validatorMock.Setup(x => x.Validar(request, pedido)).Returns(statusList);

        // Act
        await _service.ProcessarStatusAsync(request);

        // Assert
        _repositoryMock.Verify(x => x.ObterPorNumeroAsync("PED-001"), Times.Once);
    }

    [Fact]
    public async Task ProcessarStatusAsync_DeveChamarValidator_ApenasUmaVez()
    {
        // Arrange
        var request = new StatusRequest
        {
            Pedido = "PED-001",
            Status = "aprovado",
            ItensAprovados = 2,
            ValorAprovado = 100.00m
        };

        var pedido = new Pedido("PED-001");
        var statusList = new List<string> { "aprovado" };

        _repositoryMock.Setup(x => x.ObterPorNumeroAsync("PED-001")).ReturnsAsync(pedido);
        _validatorMock.Setup(x => x.Validar(request, pedido)).Returns(statusList);

        // Act
        await _service.ProcessarStatusAsync(request);

        // Assert
        _validatorMock.Verify(x => x.Validar(request, pedido), Times.Once);
    }

    [Fact]
    public async Task ProcessarStatusAsync_DevePassarPedidoCorreto_ParaValidator()
    {
        // Arrange
        var request = new StatusRequest
        {
            Pedido = "PED-001",
            Status = "aprovado",
            ItensAprovados = 2,
            ValorAprovado = 100.00m
        };

        var pedido = new Pedido("PED-001");
        pedido.AdicionarItem("Produto A", 50.00m, 2);

        var statusList = new List<string> { "aprovado" };

        _repositoryMock.Setup(x => x.ObterPorNumeroAsync("PED-001")).ReturnsAsync(pedido);
        _validatorMock.Setup(x => x.Validar(request, pedido)).Returns(statusList);

        // Act
        await _service.ProcessarStatusAsync(request);

        // Assert
        _validatorMock.Verify(x => x.Validar(
            It.Is<StatusRequest>(r => r.Pedido == "PED-001"),
            It.Is<Pedido>(p => p.NumeroPedido == "PED-001")
        ), Times.Once);
    }

    #endregion

    #region ProcessarStatusAsync - Edge Cases

    [Fact]
    public async Task ProcessarStatusAsync_DeveRetornarListaVazia_QuandoValidatorRetornarListaVazia()
    {
        // Arrange
        var request = new StatusRequest
        {
            Pedido = "PED-001",
            Status = "aprovado",
            ItensAprovados = 2,
            ValorAprovado = 100.00m
        };

        var pedido = new Pedido("PED-001");
        var statusList = new List<string>();

        _repositoryMock.Setup(x => x.ObterPorNumeroAsync("PED-001")).ReturnsAsync(pedido);
        _validatorMock.Setup(x => x.Validar(request, pedido)).Returns(statusList);

        // Act
        var resultado = await _service.ProcessarStatusAsync(request);

        // Assert
        resultado.Status.Should().BeEmpty();
    }

    [Fact]
    public async Task ProcessarStatusAsync_DeveManterNumeroPedido_NoResponse()
    {
        // Arrange
        var numeroPedido = "PED-ESPECIAL-123";
        var request = new StatusRequest
        {
            Pedido = numeroPedido,
            Status = "aprovado",
            ItensAprovados = 1,
            ValorAprovado = 50.00m
        };

        var pedido = new Pedido(numeroPedido);
        var statusList = new List<string> { "aprovado" };

        _repositoryMock.Setup(x => x.ObterPorNumeroAsync(numeroPedido)).ReturnsAsync(pedido);
        _validatorMock.Setup(x => x.Validar(request, pedido)).Returns(statusList);

        // Act
        var resultado = await _service.ProcessarStatusAsync(request);

        // Assert
        resultado.Pedido.Should().Be(numeroPedido);
    }

    [Fact]
    public async Task ProcessarStatusAsync_DeveProcessarCorretamente_ComValoresDecimais()
    {
        // Arrange
        var request = new StatusRequest
        {
            Pedido = "PED-001",
            Status = "aprovado",
            ItensAprovados = 3,
            ValorAprovado = 99.99m
        };

        var pedido = new Pedido("PED-001");
        pedido.AdicionarItem("Produto A", 33.33m, 3);

        var statusList = new List<string> { "aprovado" };

        _repositoryMock.Setup(x => x.ObterPorNumeroAsync("PED-001")).ReturnsAsync(pedido);
        _validatorMock.Setup(x => x.Validar(request, pedido)).Returns(statusList);

        // Act
        var resultado = await _service.ProcessarStatusAsync(request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Status.Should().Contain("aprovado");
    }

    #endregion

    #region ProcessarStatusAsync - Múltiplos Itens

    [Fact]
    public async Task ProcessarStatusAsync_DeveProcessarCorretamente_PedidoComMultiplosItens()
    {
        // Arrange
        var request = new StatusRequest
        {
            Pedido = "PED-MULTI",
            Status = "aprovado",
            ItensAprovados = 6,
            ValorAprovado = 430.00m
        };

        var pedido = new Pedido("PED-MULTI");
        pedido.AdicionarItem("Produto A", 50.00m, 2);
        pedido.AdicionarItem("Produto B", 30.00m, 1);
        pedido.AdicionarItem("Produto C", 100.00m, 3);

        var statusList = new List<string> { "aprovado" };

        _repositoryMock.Setup(x => x.ObterPorNumeroAsync("PED-MULTI")).ReturnsAsync(pedido);
        _validatorMock.Setup(x => x.Validar(request, pedido)).Returns(statusList);

        // Act
        var resultado = await _service.ProcessarStatusAsync(request);

        // Assert
        resultado.Status.Should().Contain("aprovado");
        _validatorMock.Verify(x => x.Validar(
            It.Is<StatusRequest>(r => r.ItensAprovados == 6 && r.ValorAprovado == 430.00m),
            It.IsAny<Pedido>()
        ), Times.Once);
    }

    #endregion
}