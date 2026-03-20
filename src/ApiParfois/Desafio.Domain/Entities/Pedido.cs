using System;
using System.Collections.Generic;
using System.Text;

namespace Desafio.Domain.Entities
{

    public class Pedido:Entity
    {
        public string NumeroPedido { get; private set; }
        public DateTime DataCriacao { get; private set; }

        private readonly List<ItemPedido> _itens;
        public IReadOnlyCollection<ItemPedido> Itens => _itens.AsReadOnly();

        // Construtor para EF Core
        private Pedido()
        {
            _itens = new List<ItemPedido>();
            NumeroPedido = string.Empty;
        }

        public Pedido(string numeroPedido)
        {
            if (string.IsNullOrWhiteSpace(numeroPedido))
                throw new ArgumentException("Número do pedido não pode ser vazio", nameof(numeroPedido));

            NumeroPedido = numeroPedido;
            DataCriacao = DateTime.UtcNow;
            _itens = new List<ItemPedido>();
        }

        public void AdicionarItem(string descricao, decimal precoUnitario, int quantidade)
        {
            if (string.IsNullOrWhiteSpace(descricao))
                throw new ArgumentException("Descrição não pode ser vazia", nameof(descricao));

            if (precoUnitario <= 0)
                throw new ArgumentException("Preço unitário deve ser maior que zero", nameof(precoUnitario));

            if (quantidade <= 0)
                throw new ArgumentException("Quantidade deve ser maior que zero", nameof(quantidade));

            var item = new ItemPedido(descricao, precoUnitario, quantidade);
            _itens.Add(item);
        }

        public decimal ObterValorTotal()
        {
            return _itens.Sum(i => i.ObterValorTotal());
        }

        public int ObterQuantidadeTotalItens()
        {
            return _itens.Sum(i => i.Quantidade);
        }

        public void AtualizarNumeroPedido(string numeroPedido)
        {
            if (string.IsNullOrWhiteSpace(numeroPedido))
                throw new ArgumentException("Número do pedido não pode ser vazio", nameof(numeroPedido));

            NumeroPedido = numeroPedido;
        }

        public void SubstituirItens(IEnumerable<(string Descricao, decimal PrecoUnitario, int Quantidade)> itens)
        {
            if (itens is null) throw new ArgumentNullException(nameof(itens));

            _itens.Clear(); // EF vai entender como DELETE dos itens antigos (Cascade configurado)
            foreach (var (descricao, preco, qtd) in itens)
            {
                AdicionarItem(descricao, preco, qtd); // reaproveita validações do domínio
            }
        }

    }
}
