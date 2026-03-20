using Desafio.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Desafio.Testes.Unit.Domain;

public class PedidoTests
{
    #region Construtor - Casos de Sucesso

    [Fact]
    public void Construtor_DeveCriarPedido_QuandoNumeroForValido()
    {
        // Arrange
        var numeroPedido = "PED-001";

        // Act
        var pedido = new Pedido(numeroPedido);

        // Assert
        pedido.Should().NotBeNull();
        pedido.NumeroPedido.Should().Be(numeroPedido);
        pedido.Itens.Should().BeEmpty();
        pedido.DataCriacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData("PED-001")]
    [InlineData("PEDIDO-123")]
    [InlineData("ABC-XYZ-999")]
    [InlineData("1")]
    public void Construtor_DeveCriarPedido_ComDiferentesNumeros(string numeroPedido)
    {
        // Act
        var pedido = new Pedido(numeroPedido);

        // Assert
        pedido.NumeroPedido.Should().Be(numeroPedido);
        pedido.Itens.Should().BeEmpty();
    }

    #endregion

    #region Construtor - Validações

    [Fact]
    public void Construtor_DeveLancarArgumentException_QuandoNumeroForVazio()
    {
        // Arrange
        var numeroPedido = "";

        // Act
        Action act = () => new Pedido(numeroPedido);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Número do pedido*");
    }

    [Fact]
    public void Construtor_DeveLancarArgumentException_QuandoNumeroForNull()
    {
        // Arrange
        string numeroPedido = null!;

        // Act
        Action act = () => new Pedido(numeroPedido);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Número do pedido*");
    }

    [Fact]
    public void Construtor_DeveLancarArgumentException_QuandoNumeroForApenasEspacos()
    {
        // Arrange
        var numeroPedido = "   ";

        // Act
        Action act = () => new Pedido(numeroPedido);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Número do pedido*");
    }

    #endregion

    #region AdicionarItem - Casos de Sucesso

    [Fact]
    public void AdicionarItem_DeveAdicionarItem_QuandoParametrosForemValidos()
    {
        // Arrange
        var pedido = new Pedido("PED-001");

        // Act
        pedido.AdicionarItem("Produto A", 50.00m, 2);

        // Assert
        pedido.Itens.Should().HaveCount(1);
        pedido.Itens.First().Descricao.Should().Be("Produto A");
        pedido.Itens.First().PrecoUnitario.Should().Be(50.00m);
        pedido.Itens.First().Quantidade.Should().Be(2);
    }

    [Fact]
    public void AdicionarItem_DeveAdicionarMultiplosItens()
    {
        // Arrange
        var pedido = new Pedido("PED-001");

        // Act
        pedido.AdicionarItem("Produto A", 50.00m, 2);
        pedido.AdicionarItem("Produto B", 30.00m, 1);
        pedido.AdicionarItem("Produto C", 100.00m, 3);

        // Assert
        pedido.Itens.Should().HaveCount(3);
        pedido.Itens.Should().Contain(i => i.Descricao == "Produto A");
        pedido.Itens.Should().Contain(i => i.Descricao == "Produto B");
        pedido.Itens.Should().Contain(i => i.Descricao == "Produto C");
    }

    #endregion

    #region AdicionarItem - Validações

    [Fact]
    public void AdicionarItem_DeveLancarArgumentException_QuandoDescricaoForVazia()
    {
        // Arrange
        var pedido = new Pedido("PED-001");

        // Act
        Action act = () => pedido.AdicionarItem("", 50.00m, 2);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Descrição*");
    }

    [Fact]
    public void AdicionarItem_DeveLancarArgumentException_QuandoPrecoForZero()
    {
        // Arrange
        var pedido = new Pedido("PED-001");

        // Act
        Action act = () => pedido.AdicionarItem("Produto A", 0m, 2);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Preço unitário*");
    }

    [Fact]
    public void AdicionarItem_DeveLancarArgumentException_QuandoPrecoForNegativo()
    {
        // Arrange
        var pedido = new Pedido("PED-001");

        // Act
        Action act = () => pedido.AdicionarItem("Produto A", -10m, 2);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Preço unitário*");
    }

    [Fact]
    public void AdicionarItem_DeveLancarArgumentException_QuandoQuantidadeForZero()
    {
        // Arrange
        var pedido = new Pedido("PED-001");

        // Act
        Action act = () => pedido.AdicionarItem("Produto A", 50.00m, 0);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Quantidade*");
    }

    [Fact]
    public void AdicionarItem_DeveLancarArgumentException_QuandoQuantidadeForNegativa()
    {
        // Arrange
        var pedido = new Pedido("PED-001");

        // Act
        Action act = () => pedido.AdicionarItem("Produto A", 50.00m, -1);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Quantidade*");
    }

    #endregion

    #region ObterValorTotal

    [Fact]
    public void ObterValorTotal_DeveRetornarZero_QuandoNaoHouverItens()
    {
        // Arrange
        var pedido = new Pedido("PED-001");

        // Act
        var valorTotal = pedido.ObterValorTotal();

        // Assert
        valorTotal.Should().Be(0m);
    }

    [Fact]
    public void ObterValorTotal_DeveCalcularCorretamente_ComUmItem()
    {
        // Arrange
        var pedido = new Pedido("PED-001");
        pedido.AdicionarItem("Produto A", 50.00m, 2);

        // Act
        var valorTotal = pedido.ObterValorTotal();

        // Assert
        valorTotal.Should().Be(100.00m);
    }

    [Fact]
    public void ObterValorTotal_DeveCalcularCorretamente_ComMultiplosItens()
    {
        // Arrange
        var pedido = new Pedido("PED-001");
        pedido.AdicionarItem("Produto A", 50.00m, 2);   // 100
        pedido.AdicionarItem("Produto B", 30.00m, 1);   // 30
        pedido.AdicionarItem("Produto C", 100.00m, 3);  // 300

        // Act
        var valorTotal = pedido.ObterValorTotal();

        // Assert
        valorTotal.Should().Be(430.00m);
    }

    [Fact]
    public void ObterValorTotal_DeveRecalcular_AposAdicionarItem()
    {
        // Arrange
        var pedido = new Pedido("PED-001");
        pedido.AdicionarItem("Produto A", 50.00m, 2);

        var valorAntes = pedido.ObterValorTotal();

        // Act
        pedido.AdicionarItem("Produto B", 30.00m, 1);
        var valorDepois = pedido.ObterValorTotal();

        // Assert
        valorAntes.Should().Be(100.00m);
        valorDepois.Should().Be(130.00m);
    }

    [Fact]
    public void ObterValorTotal_DeveManterPrecisaoDecimal()
    {
        // Arrange
        var pedido = new Pedido("PED-001");
        pedido.AdicionarItem("Produto A", 33.33m, 3);

        // Act
        var valorTotal = pedido.ObterValorTotal();

        // Assert
        valorTotal.Should().Be(99.99m);
    }

    #endregion

    #region ObterQuantidadeTotalItens

    [Fact]
    public void ObterQuantidadeTotalItens_DeveRetornarZero_QuandoNaoHouverItens()
    {
        // Arrange
        var pedido = new Pedido("PED-001");

        // Act
        var quantidade = pedido.ObterQuantidadeTotalItens();

        // Assert
        quantidade.Should().Be(0);
    }

    [Fact]
    public void ObterQuantidadeTotalItens_DeveCalcularCorretamente_ComUmItem()
    {
        // Arrange
        var pedido = new Pedido("PED-001");
        pedido.AdicionarItem("Produto A", 50.00m, 5);

        // Act
        var quantidade = pedido.ObterQuantidadeTotalItens();

        // Assert
        quantidade.Should().Be(5);
    }

    [Fact]
    public void ObterQuantidadeTotalItens_DeveCalcularCorretamente_ComMultiplosItens()
    {
        // Arrange
        var pedido = new Pedido("PED-001");
        pedido.AdicionarItem("Produto A", 50.00m, 2);
        pedido.AdicionarItem("Produto B", 30.00m, 3);
        pedido.AdicionarItem("Produto C", 100.00m, 1);

        // Act
        var quantidade = pedido.ObterQuantidadeTotalItens();

        // Assert
        quantidade.Should().Be(6);  // 2 + 3 + 1
    }

    [Fact]
    public void ObterQuantidadeTotalItens_DeveRecalcular_AposAdicionarItem()
    {
        // Arrange
        var pedido = new Pedido("PED-001");
        pedido.AdicionarItem("Produto A", 50.00m, 2);

        var qtdAntes = pedido.ObterQuantidadeTotalItens();

        // Act
        pedido.AdicionarItem("Produto B", 30.00m, 3);
        var qtdDepois = pedido.ObterQuantidadeTotalItens();

        // Assert
        qtdAntes.Should().Be(2);
        qtdDepois.Should().Be(5);
    }

    #endregion

    #region AtualizarNumeroPedido

    [Fact]
    public void AtualizarNumeroPedido_DeveAtualizarNumero_QuandoNumeroForValido()
    {
        // Arrange
        var pedido = new Pedido("PED-001");
        var novoNumero = "PED-002";

        // Act
        pedido.AtualizarNumeroPedido(novoNumero);

        // Assert
        pedido.NumeroPedido.Should().Be(novoNumero);
    }

    [Fact]
    public void AtualizarNumeroPedido_DeveLancarArgumentException_QuandoNumeroForVazio()
    {
        // Arrange
        var pedido = new Pedido("PED-001");

        // Act
        Action act = () => pedido.AtualizarNumeroPedido("");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Número do pedido*");
    }

    [Fact]
    public void AtualizarNumeroPedido_DeveLancarArgumentException_QuandoNumeroForNull()
    {
        // Arrange
        var pedido = new Pedido("PED-001");

        // Act
        Action act = () => pedido.AtualizarNumeroPedido(null!);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Número do pedido*");
    }

    #endregion

    #region SubstituirItens

    [Fact]
    public void SubstituirItens_DeveSubstituirTodosItens_QuandoListaForValida()
    {
        // Arrange
        var pedido = new Pedido("PED-001");
        pedido.AdicionarItem("Produto A", 50.00m, 2);
        pedido.AdicionarItem("Produto B", 30.00m, 1);

        var novosItens = new[]
        {
            ("Produto X", 100.00m, 1),
            ("Produto Y", 200.00m, 2)
        };

        // Act
        pedido.SubstituirItens(novosItens);

        // Assert
        pedido.Itens.Should().HaveCount(2);
        pedido.Itens.Should().Contain(i => i.Descricao == "Produto X");
        pedido.Itens.Should().Contain(i => i.Descricao == "Produto Y");
        pedido.Itens.Should().NotContain(i => i.Descricao == "Produto A");
        pedido.Itens.Should().NotContain(i => i.Descricao == "Produto B");
    }

    [Fact]
    public void SubstituirItens_DeveLimparItens_QuandoListaForVazia()
    {
        // Arrange
        var pedido = new Pedido("PED-001");
        pedido.AdicionarItem("Produto A", 50.00m, 2);

        var novosItens = Array.Empty<(string, decimal, int)>();

        // Act
        pedido.SubstituirItens(novosItens);

        // Assert
        pedido.Itens.Should().BeEmpty();
    }

    [Fact]
    public void SubstituirItens_DeveLancarArgumentNullException_QuandoListaForNull()
    {
        // Arrange
        var pedido = new Pedido("PED-001");

        // Act
        Action act = () => pedido.SubstituirItens(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void SubstituirItens_DeveValidarCadaItem_QuandoSubstituir()
    {
        // Arrange
        var pedido = new Pedido("PED-001");
        pedido.AdicionarItem("Produto A", 50.00m, 2);

        var novosItens = new[]
        {
            ("Produto X", 0m, 1) // Preço inválido
        };

        // Act
        Action act = () => pedido.SubstituirItens(novosItens);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Preço unitário*");
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void Pedido_DeveSuportarGrandeQuantidadeDeItens()
    {
        // Arrange
        var pedido = new Pedido("PED-001");

        // Act
        for (int i = 1; i <= 100; i++)
        {
            pedido.AdicionarItem($"Produto {i}", 10.00m, 1);
        }

        // Assert
        pedido.Itens.Should().HaveCount(100);
        pedido.ObterValorTotal().Should().Be(1000.00m);
        pedido.ObterQuantidadeTotalItens().Should().Be(100);
    }

    [Fact]
    public void Itens_DeveSerReadOnly()
    {
        // Arrange
        var pedido = new Pedido("PED-001");
        pedido.AdicionarItem("Produto A", 50.00m, 2);

        // Act
        var itens = pedido.Itens;

        // Assert
        itens.Should().BeAssignableTo<IReadOnlyCollection<ItemPedido>>();
        itens.Should().HaveCount(1);
    }

    #endregion
}