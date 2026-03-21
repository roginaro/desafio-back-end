using Desafio.Application.Models;
using Desafio.Domain.Entities;
using Desafio.Domain.Enums;

namespace Desafio.Application.Validators;

public class StatusValidator : IStatusValidator
{
    // Aqui pode ser usado um pattern de estratégia com interfaces,
    // para validar cada regra de forma mais modular (facil de estender, modificar e testar),
    // mas para simplicidade, mantive tudo na mesma classe.

    public List<string> Validar(StatusRequest request, Pedido? pedido)
    {
        var statusList = new List<string>();

        // Regra 1: Pedido não localizado
        if (pedido == null)
        {
            statusList.Add(StatusPedido.CODIGO_PEDIDO_INVALIDO.ToString());
            return statusList;
        }

        // Regra 2: Status REPROVADO
        if (EhStatusReprovado(request.Status))
        {
            statusList.Add(StatusPedido.REPROVADO.ToString());
            return statusList;
        }

        // A partir daqui, só processa se status for APROVADO
        if (!EhStatusAprovado(request.Status))
        {
            return statusList;
        }

        // Validar aprovação completa ou parcial
        ValidarAprovacao(request, pedido, statusList);

        return statusList;
    }

    private static bool EhStatusReprovado(string status)
    {
        return status.Equals(StatusPedido.REPROVADO.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    private static bool EhStatusAprovado(string status)
    {
        return status.Equals(StatusPedido.APROVADO.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    private void ValidarAprovacao(StatusRequest request, Pedido pedido, List<string> statusList)
    {
        var valorTotalPedido = pedido.ObterValorTotal();
        var qtdTotalItensPedido = pedido.ObterQuantidadeTotalItens();

        // Regra 3: APROVADO completo (valor e quantidade exatos)
        if (EhAprovacaoCompleta(request, valorTotalPedido, qtdTotalItensPedido))
        {
            statusList.Add(StatusPedido.APROVADO.ToString());
            return;
        }

        // Regras 4, 5, 6, 7: Validações de divergências
        ValidarDivergenciaValor(request.ValorAprovado, valorTotalPedido, statusList);
        ValidarDivergenciaQuantidade(request.ItensAprovados, qtdTotalItensPedido, statusList);
    }

    private static bool EhAprovacaoCompleta(StatusRequest request, decimal valorTotal, int qtdTotal)
    {
        return request.ValorAprovado == valorTotal && request.ItensAprovados == qtdTotal;
    }

    private void ValidarDivergenciaValor(decimal valorAprovado, decimal valorTotal, List<string> statusList)
    {
        if (valorAprovado < valorTotal)
        {
            statusList.Add(StatusPedido.APROVADO_VALOR_A_MENOR.ToString());
        }
        else if (valorAprovado > valorTotal)
        {
            statusList.Add(StatusPedido.APROVADO_VALOR_A_MAIOR.ToString());
        }
    }

    private void ValidarDivergenciaQuantidade(int qtdAprovada, int qtdTotal, List<string> statusList)
    {
        if (qtdAprovada < qtdTotal)
        {
            statusList.Add(StatusPedido.APROVADO_QTD_A_MENOR.ToString());
        }
        else if (qtdAprovada > qtdTotal)
        {
            statusList.Add(StatusPedido.APROVADO_QTD_A_MAIOR.ToString());
        }
    }
}