using Desafio.Application.Models;
using Desafio.Domain.Entities;

namespace Desafio.Application.Validators;

public interface IStatusValidator
{
    List<string> Validar(StatusRequest request, Pedido? pedido);
}