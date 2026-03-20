using Desafio.Application.Models;
using Desafio.Application.Validators;
using Desafio.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Desafio.Testes.Unit.Application.Validators;

public class StatusValidatorTests
{
    private readonly IStatusValidator _validator;

    public StatusValidatorTests()
    {
        _validator = new StatusValidator();
    }

    #region Regra 1: Pedido Não Localizado

    [Fact]
    public void Validar_DeveRetornarCodigoPedidoInvalido_QuandoPedidoForNull()
    {
        // Arrange
        var request = new StatusRequest
        {
            Pedido = "PED-001",
            Status = "APROVADO",
            ValorAprovado = 100,
            ItensAprovados = 2
        };
        Pedido? pedido = null;

        // Act
        var resultado = _validator.Validar(request, pedido);

        // Assert
        resultado.Should().HaveCount(1);
        resultado.Should().Contain("CODIGO_PEDIDO_INVALIDO");
    }

    #endregion

    #region Regra 2: Status REPROVADO

    [Theory]
    [InlineData("REPROVADO")]
    [InlineData("reprovado")]
    [InlineData("Reprovado")]
    [InlineData("RePrOvAdO")]
    public void Validar_DeveRetornarReprovado_QuandoStatusForReprovado(string status)
    {
        // Arrange
        var request = new StatusRequest
        {
            Pedido = "PED-001",
            Status = status,
            ValorAprovado = 100,
            ItensAprovados = 2
        };
        var pedido = CriarPedidoComItens(100, 2);

        // Act
        var resultado = _validator.Validar(request, pedido);

        // Assert
        resultado.Should().HaveCount(1);
        resultado.Should().Contain("REPROVADO");
    }

    #endregion

    #region Regra 3: APROVADO Completo

    [Theory]
    [InlineData("APROVADO")]
    [InlineData("aprovado")]
    [InlineData("Aprovado")]
    [InlineData("ApRoVaDo")]
    public void Validar_DeveRetornarAprovado_QuandoValorEQuantidadeForemExatos(string status)
    {
        // Arrange
        var request = new StatusRequest
        {
            Pedido = "PED-001",
            Status = status,
            ValorAprovado = 100,
            ItensAprovados = 2
        };
        var pedido = CriarPedidoComItens(100, 2);

        // Act
        var resultado = _validator.Validar(request, pedido);

        // Assert
        resultado.Should().HaveCount(1);
        resultado.Should().Contain("APROVADO");
    }

    #endregion

    #region Regra 4: APROVADO_VALOR_A_MENOR

    [Fact]
    public void Validar_DeveRetornarAprovadoValorAMenor_QuandoValorAprovadoForMenor()
    {
        // Arrange
        var request = new StatusRequest
        {
            Pedido = "PED-001",
            Status = "APROVADO",
            ValorAprovado = 80,  // Menor que 100
            ItensAprovados = 2
        };
        var pedido = CriarPedidoComItens(100, 2);

        // Act
        var resultado = _validator.Validar(request, pedido);

        // Assert
        resultado.Should().Contain("APROVADO_VALOR_A_MENOR");
    }

    #endregion

    #region Regra 5: APROVADO_VALOR_A_MAIOR

    [Fact]
    public void Validar_DeveRetornarAprovadoValorAMaior_QuandoValorAprovadoForMaior()
    {
        // Arrange
        var request = new StatusRequest
        {
            Pedido = "PED-001",
            Status = "APROVADO",
            ValorAprovado = 120,  // Maior que 100
            ItensAprovados = 2
        };
        var pedido = CriarPedidoComItens(100, 2);

        // Act
        var resultado = _validator.Validar(request, pedido);

        // Assert
        resultado.Should().Contain("APROVADO_VALOR_A_MAIOR");
    }

    #endregion

    #region Regra 6: APROVADO_QTD_A_MENOR

    [Fact]
    public void Validar_DeveRetornarAprovadoQtdAMenor_QuandoQuantidadeAprovadaForMenor()
    {
        // Arrange
        var request = new StatusRequest
        {
            Pedido = "PED-001",
            Status = "APROVADO",
            ValorAprovado = 100,
            ItensAprovados = 1  // Menor que 2
        };
        var pedido = CriarPedidoComItens(100, 2);

        // Act
        var resultado = _validator.Validar(request, pedido);

        // Assert
        resultado.Should().Contain("APROVADO_QTD_A_MENOR");
    }

    #endregion

    #region Regra 7: APROVADO_QTD_A_MAIOR

    [Fact]
    public void Validar_DeveRetornarAprovadoQtdAMaior_QuandoQuantidadeAprovadaForMaior()
    {
        // Arrange
        var request = new StatusRequest
        {
            Pedido = "PED-001",
            Status = "APROVADO",
            ValorAprovado = 100,
            ItensAprovados = 3  // Maior que 2
        };
        var pedido = CriarPedidoComItens(100, 2);

        // Act
        var resultado = _validator.Validar(request, pedido);

        // Assert
        resultado.Should().Contain("APROVADO_QTD_A_MAIOR");
    }

    #endregion

    #region Múltiplos Status

    [Fact]
    public void Validar_DeveRetornarMultiplosStatus_QuandoHouverDivergenciaEmValorEQuantidade()
    {
        // Arrange
        var request = new StatusRequest
        {
            Pedido = "PED-001",
            Status = "APROVADO",
            ValorAprovado = 80,   // Menor que 100
            ItensAprovados = 3    // Maior que 2
        };
        var pedido = CriarPedidoComItens(100, 2);

        // Act
        var resultado = _validator.Validar(request, pedido);

        // Assert
        resultado.Should().HaveCount(2);
        resultado.Should().Contain("APROVADO_VALOR_A_MENOR");
        resultado.Should().Contain("APROVADO_QTD_A_MAIOR");
    }

    [Fact]
    public void Validar_DeveRetornarMultiplosStatus_QuandoValorEQuantidadeForemMaiores()
    {
        // Arrange
        var request = new StatusRequest
        {
            Pedido = "PED-001",
            Status = "APROVADO",
            ValorAprovado = 120,  // Maior que 100
            ItensAprovados = 3    // Maior que 2
        };
        var pedido = CriarPedidoComItens(100, 2);

        // Act
        var resultado = _validator.Validar(request, pedido);

        // Assert
        resultado.Should().HaveCount(2);
        resultado.Should().Contain("APROVADO_VALOR_A_MAIOR");
        resultado.Should().Contain("APROVADO_QTD_A_MAIOR");
    }

    [Fact]
    public void Validar_DeveRetornarMultiplosStatus_QuandoValorEQuantidadeForemMenores()
    {
        // Arrange
        var request = new StatusRequest
        {
            Pedido = "PED-001",
            Status = "APROVADO",
            ValorAprovado = 80,   // Menor que 100
            ItensAprovados = 1    // Menor que 2
        };
        var pedido = CriarPedidoComItens(100, 2);

        // Act
        var resultado = _validator.Validar(request, pedido);

        // Assert
        resultado.Should().HaveCount(2);
        resultado.Should().Contain("APROVADO_VALOR_A_MENOR");
        resultado.Should().Contain("APROVADO_QTD_A_MENOR");
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void Validar_DeveRetornarListaVazia_QuandoStatusNaoForAprovadoNemReprovado()
    {
        // Arrange
        var request = new StatusRequest
        {
            Pedido = "PED-001",
            Status = "PENDENTE",
            ValorAprovado = 100,
            ItensAprovados = 2
        };
        var pedido = CriarPedidoComItens(100, 2);

        // Act
        var resultado = _validator.Validar(request, pedido);

        // Assert
        resultado.Should().BeEmpty();
    }

    [Fact]
    public void Validar_DeveRetornarAprovado_QuandoValoresForemZero()
    {
        // Arrange
        var request = new StatusRequest
        {
            Pedido = "PED-001",
            Status = "APROVADO",
            ValorAprovado = 0,
            ItensAprovados = 0
        };
        var pedido = new Pedido("PED-001");

        // Act
        var resultado = _validator.Validar(request, pedido);

        // Assert
        resultado.Should().HaveCount(1);
        resultado.Should().Contain("APROVADO");
    }

    [Fact]
    public void Validar_DeveRetornarAprovado_QuandoPedidoTiverMultiplosItens()
    {
        // Arrange
        var request = new StatusRequest
        {
            Pedido = "PED-001",
            Status = "APROVADO",
            ValorAprovado = 350,  // 100 + 150 + 100
            ItensAprovados = 6    // 2 + 3 + 1
        };
        var pedido = new Pedido("PED-001");
        pedido.AdicionarItem("Produto A", 50, 2);   // 100
        pedido.AdicionarItem("Produto B", 50, 3);   // 150
        pedido.AdicionarItem("Produto C", 100, 1);  // 100

        // Act
        var resultado = _validator.Validar(request, pedido);

        // Assert
        resultado.Should().HaveCount(1);
        resultado.Should().Contain("APROVADO");
    }

    #endregion

    #region Métodos Auxiliares

    private Pedido CriarPedidoComItens(decimal valorTotal, int quantidadeTotal)
    {
        var pedido = new Pedido("PED-001");
        var precoUnitario = valorTotal / quantidadeTotal;
        pedido.AdicionarItem("Produto Teste", precoUnitario, quantidadeTotal);
        return pedido;
    }

    #endregion
}