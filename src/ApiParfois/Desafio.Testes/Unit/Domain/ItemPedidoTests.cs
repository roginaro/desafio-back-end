using Desafio.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Desafio.Testes.Unit.Domain;

public class ItemPedidoTests
{
    #region Construtor - Casos de Sucesso

    [Fact]
    public void Construtor_DeveCriarItemPedido_QuandoParametrosForemValidos()
    {
        // Arrange
        var descricao = "Produto Teste";
        var precoUnitario = 50.00m;
        var quantidade = 2;

        // Act
        var item = new ItemPedido(descricao, precoUnitario, quantidade);

        // Assert
        item.Should().NotBeNull();
        item.Descricao.Should().Be(descricao);
        item.PrecoUnitario.Should().Be(precoUnitario);
        item.Quantidade.Should().Be(quantidade);
    }

    [Theory]
    [InlineData("Camiseta", 35.00, 1)]
    [InlineData("Calça Jeans", 115.50, 3)]
    [InlineData("Tênis", 250.99, 1)]
    [InlineData("Produto com nome muito longo para testar limite de caracteres", 10.00, 5)]
    public void Construtor_DeveCriarItemPedido_ComDiferentesValores(
        string descricao, 
        decimal precoUnitario, 
        int quantidade)
    {
        // Act
        var item = new ItemPedido(descricao, precoUnitario, quantidade);

        // Assert
        item.Descricao.Should().Be(descricao);
        item.PrecoUnitario.Should().Be(precoUnitario);
        item.Quantidade.Should().Be(quantidade);
    }

    #endregion

    #region Construtor - Validações de Descrição

    [Fact]
    public void Construtor_DeveLancarArgumentException_QuandoDescricaoForVazia()
    {
        // Arrange
        var descricao = "";
        var precoUnitario = 50.00m;
        var quantidade = 2;

        // Act
        Action act = () => new ItemPedido(descricao, precoUnitario, quantidade);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Descrição*");
    }

    [Fact]
    public void Construtor_DeveLancarArgumentException_QuandoDescricaoForNull()
    {
        // Arrange
        string descricao = null!;
        var precoUnitario = 50.00m;
        var quantidade = 2;

        // Act
        Action act = () => new ItemPedido(descricao, precoUnitario, quantidade);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Descrição*");
    }

    [Fact]
    public void Construtor_DeveLancarArgumentException_QuandoDescricaoForApenasEspacos()
    {
        // Arrange
        var descricao = "   ";
        var precoUnitario = 50.00m;
        var quantidade = 2;

        // Act
        Action act = () => new ItemPedido(descricao, precoUnitario, quantidade);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Descrição*");
    }

    #endregion

    #region Construtor - Validações de Preço Unitário

    [Fact]
    public void Construtor_DeveLancarArgumentException_QuandoPrecoUnitarioForZero()
    {
        // Arrange
        var descricao = "Produto Teste";
        var precoUnitario = 0m;
        var quantidade = 2;

        // Act
        Action act = () => new ItemPedido(descricao, precoUnitario, quantidade);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Preço unitário*maior que zero*");
    }

    [Fact]
    public void Construtor_DeveLancarArgumentException_QuandoPrecoUnitarioForNegativo()
    {
        // Arrange
        var descricao = "Produto Teste";
        var precoUnitario = -10.00m;
        var quantidade = 2;

        // Act
        Action act = () => new ItemPedido(descricao, precoUnitario, quantidade);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Preço unitário*maior que zero*");
    }

    [Theory]
    [InlineData(0.01)]
    [InlineData(1.00)]
    [InlineData(99.99)]
    [InlineData(1000.00)]
    public void Construtor_DeveCriarItemPedido_QuandoPrecoUnitarioForPositivo(decimal precoUnitario)
    {
        // Arrange
        var descricao = "Produto Teste";
        var quantidade = 2;

        // Act
        var item = new ItemPedido(descricao, precoUnitario, quantidade);

        // Assert
        item.PrecoUnitario.Should().Be(precoUnitario);
    }

    #endregion

    #region Construtor - Validações de Quantidade

    [Fact]
    public void Construtor_DeveLancarArgumentException_QuandoQuantidadeForZero()
    {
        // Arrange
        var descricao = "Produto Teste";
        var precoUnitario = 50.00m;
        var quantidade = 0;

        // Act
        Action act = () => new ItemPedido(descricao, precoUnitario, quantidade);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Quantidade*maior que zero*");
    }

    [Fact]
    public void Construtor_DeveLancarArgumentException_QuandoQuantidadeForNegativa()
    {
        // Arrange
        var descricao = "Produto Teste";
        var precoUnitario = 50.00m;
        var quantidade = -5;

        // Act
        Action act = () => new ItemPedido(descricao, precoUnitario, quantidade);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Quantidade*maior que zero*");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(100)]
    public void Construtor_DeveCriarItemPedido_QuandoQuantidadeForPositiva(int quantidade)
    {
        // Arrange
        var descricao = "Produto Teste";
        var precoUnitario = 50.00m;

        // Act
        var item = new ItemPedido(descricao, precoUnitario, quantidade);

        // Assert
        item.Quantidade.Should().Be(quantidade);
    }

    #endregion

    #region Método ObterValorTotal

    [Fact]
    public void ObterValorTotal_DeveCalcularCorretamente_QuandoChamado()
    {
        // Arrange
        var item = new ItemPedido("Produto Teste", 50.00m, 2);

        // Act
        var valorTotal = item.ObterValorTotal();

        // Assert
        valorTotal.Should().Be(100.00m);
    }

    [Theory]
    [InlineData(10.00, 1, 10.00)]
    [InlineData(25.50, 2, 51.00)]
    [InlineData(35.00, 3, 105.00)]
    [InlineData(99.99, 5, 499.95)]
    [InlineData(0.01, 100, 1.00)]
    public void ObterValorTotal_DeveCalcularCorretamente_ComDiferentesValores(
        decimal precoUnitario, 
        int quantidade, 
        decimal valorEsperado)
    {
        // Arrange
        var item = new ItemPedido("Produto Teste", precoUnitario, quantidade);

        // Act
        var valorTotal = item.ObterValorTotal();

        // Assert
        valorTotal.Should().Be(valorEsperado);
    }

    #endregion

    #region Propriedades

    [Fact]
    public void Propriedades_DevemSerSomenteParaLeitura()
    {
        // Arrange
        var item = new ItemPedido("Produto Teste", 50.00m, 2);

        // Assert
        item.Descricao.Should().Be("Produto Teste");
        item.PrecoUnitario.Should().Be(50.00m);
        item.Quantidade.Should().Be(2);

        // Verificar que não há setters públicos
        var descricaoProperty = typeof(ItemPedido).GetProperty(nameof(ItemPedido.Descricao));
        var precoProperty = typeof(ItemPedido).GetProperty(nameof(ItemPedido.PrecoUnitario));
        var quantidadeProperty = typeof(ItemPedido).GetProperty(nameof(ItemPedido.Quantidade));

        descricaoProperty!.SetMethod.Should().NotBeNull();
        descricaoProperty.SetMethod!.IsPrivate.Should().BeTrue();
        
        precoProperty!.SetMethod.Should().NotBeNull();
        precoProperty.SetMethod!.IsPrivate.Should().BeTrue();
        
        quantidadeProperty!.SetMethod.Should().NotBeNull();
        quantidadeProperty.SetMethod!.IsPrivate.Should().BeTrue();
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void Construtor_DeveCriarItemPedido_ComValorMuitoPequeno()
    {
        // Arrange
        var descricao = "Produto Barato";
        var precoUnitario = 0.01m;
        var quantidade = 1;

        // Act
        var item = new ItemPedido(descricao, precoUnitario, quantidade);

        // Assert
        item.PrecoUnitario.Should().Be(0.01m);
        item.ObterValorTotal().Should().Be(0.01m);
    }

    [Fact]
    public void Construtor_DeveCriarItemPedido_ComValorMuitoGrande()
    {
        // Arrange
        var descricao = "Produto Caro";
        var precoUnitario = 999999.99m;
        var quantidade = 1;

        // Act
        var item = new ItemPedido(descricao, precoUnitario, quantidade);

        // Assert
        item.PrecoUnitario.Should().Be(999999.99m);
        item.ObterValorTotal().Should().Be(999999.99m);
    }

    [Fact]
    public void Construtor_DeveCriarItemPedido_ComQuantidadeMuitoGrande()
    {
        // Arrange
        var descricao = "Produto em Massa";
        var precoUnitario = 1.00m;
        var quantidade = 10000;

        // Act
        var item = new ItemPedido(descricao, precoUnitario, quantidade);

        // Assert
        item.Quantidade.Should().Be(10000);
        item.ObterValorTotal().Should().Be(10000.00m);
    }

    [Fact]
    public void ObterValorTotal_DeveManterPrecisaoDecimal()
    {
        // Arrange
        var item = new ItemPedido("Produto Teste", 33.33m, 3);

        // Act
        var valorTotal = item.ObterValorTotal();

        // Assert
        valorTotal.Should().Be(99.99m);
    }

    #endregion
}