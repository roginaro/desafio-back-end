using Desafio.Application.Models;
using Desafio.Domain.Entities;

namespace Desafio.Application.Validators;

public class StatusValidator : IStatusValidator
{
    // Aqui pode ser usado um pattern de estratégia para validar cada regra
    // de forma mais modular (facil de estender, modificar e testar),
    // mas para simplicidade, mantive tudo na mesma classe.

    private const string STATUS_CODIGO_PEDIDO_INVALIDO = "CODIGO_PEDIDO_INVALIDO";
    private const string STATUS_REPROVADO = "REPROVADO";
    private const string STATUS_APROVADO = "APROVADO";
    //private const string STATUS_INEXISTENTE = "INEXISTENTE";
    private const string STATUS_APROVADO_VALOR_A_MENOR = "APROVADO_VALOR_A_MENOR";
    private const string STATUS_APROVADO_VALOR_A_MAIOR = "APROVADO_VALOR_A_MAIOR";
    private const string STATUS_APROVADO_QTD_A_MENOR = "APROVADO_QTD_A_MENOR";
    private const string STATUS_APROVADO_QTD_A_MAIOR = "APROVADO_QTD_A_MAIOR";

    public List<string> Validar(StatusRequest request, Pedido? pedido)
    {
        var statusList = new List<string>();

        // Regra 1: Pedido não localizado
        if (pedido == null)
        {
            statusList.Add(STATUS_CODIGO_PEDIDO_INVALIDO);
            return statusList;
        }

        // Regra 2: Status REPROVADO
        if (EhStatusReprovado(request.Status))
        {
            statusList.Add(STATUS_REPROVADO);
            return statusList;
        }

        // A partir daqui, só processa se status for APROVADO
        if (!EhStatusAprovado(request.Status))
        {
            //statusList.Add(STATUS_INEXISTENTE);
            return statusList;
        }

        // Validar aprovação completa ou parcial
        ValidarAprovacao(request, pedido, statusList);

        return statusList;
    }

    // Método privado para verificar se é REPROVADO
    private static bool EhStatusReprovado(string status)
    {
        return status.Equals(STATUS_REPROVADO, StringComparison.OrdinalIgnoreCase);
    }

    // Método privado para verificar se é APROVADO
    private static bool EhStatusAprovado(string status)
    {
        return status.Equals(STATUS_APROVADO, StringComparison.OrdinalIgnoreCase);
    }

    // Método privado para validar aprovação
    private void ValidarAprovacao(StatusRequest request, Pedido pedido, List<string> statusList)
    {
        var valorTotalPedido = pedido.ObterValorTotal();
        var qtdTotalItensPedido = pedido.ObterQuantidadeTotalItens();

        // Regra 3: APROVADO completo (valor e quantidade exatos)
        if (EhAprovacaoCompleta(request, valorTotalPedido, qtdTotalItensPedido))
        {
            statusList.Add(STATUS_APROVADO);
            return;
        }

        // Regras 4, 5, 6, 7: Validações de divergências
        ValidarDivergenciaValor(request.ValorAprovado, valorTotalPedido, statusList);
        ValidarDivergenciaQuantidade(request.ItensAprovados, qtdTotalItensPedido, statusList);
    }

    // Método privado para verificar aprovação completa
    private static bool EhAprovacaoCompleta(StatusRequest request, decimal valorTotal, int qtdTotal)
    {
        return request.ValorAprovado == valorTotal && request.ItensAprovados == qtdTotal;
    }

    // Método privado para validar divergência de valor
    private void ValidarDivergenciaValor(decimal valorAprovado, decimal valorTotal, List<string> statusList)
    {
        if (valorAprovado < valorTotal)
        {
            statusList.Add(STATUS_APROVADO_VALOR_A_MENOR);
        }
        else if (valorAprovado > valorTotal)
        {
            statusList.Add(STATUS_APROVADO_VALOR_A_MAIOR);
        }
    }

    // Método privado para validar divergência de quantidade
    private void ValidarDivergenciaQuantidade(int qtdAprovada, int qtdTotal, List<string> statusList)
    {
        if (qtdAprovada < qtdTotal)
        {
            statusList.Add(STATUS_APROVADO_QTD_A_MENOR);
        }
        else if (qtdAprovada > qtdTotal)
        {
            statusList.Add(STATUS_APROVADO_QTD_A_MAIOR);
        }
    }
}