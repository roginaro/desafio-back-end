using Desafio.Application.Models;
using Desafio.Domain.Entities;

namespace Desafio.Application.Validators;

public class StatusValidator
{
    public List<string> Validar(StatusRequest request, Pedido? pedido)
    {
        var statusList = new List<string>();

        // Regra 1: Pedido não localizado
        if (pedido == null)
        {
            statusList.Add("CODIGO_PEDIDO_INVALIDO");
            return statusList;
        }

        // Regra 2: Status REPROVADO
        if (request.Status.ToUpper() == "REPROVADO")
        {
            statusList.Add("REPROVADO");
            return statusList;
        }

        // A partir daqui, só processa se status for APROVADO
        if (request.Status.ToUpper() != "APROVADO")
        {
            return statusList;
        }

        // Calcular valores do pedido
        var valorTotalPedido = pedido.ObterValorTotal();
        var qtdTotalItensPedido = pedido.ObterQuantidadeTotalItens();

        // Regra 3: APROVADO completo (valor e quantidade exatos)
        if (request.ValorAprovado == valorTotalPedido &&
            request.ItensAprovados == qtdTotalItensPedido)
        {
            statusList.Add("APROVADO");
            return statusList;
        }

        // Regras 4, 5, 6, 7: Validações de divergências
        // Importante: Podem retornar múltiplos status

        // Validação de VALOR
        if (request.ValorAprovado < valorTotalPedido)
        {
            statusList.Add("APROVADO_VALOR_A_MENOR");
        }
        else if (request.ValorAprovado > valorTotalPedido)
        {
            statusList.Add("APROVADO_VALOR_A_MAIOR");
        }

        // Validação de QUANTIDADE
        if (request.ItensAprovados < qtdTotalItensPedido)
        {
            statusList.Add("APROVADO_QTD_A_MENOR");
        }
        else if (request.ItensAprovados > qtdTotalItensPedido)
        {
            statusList.Add("APROVADO_QTD_A_MAIOR");
        }

        return statusList;
    }
}