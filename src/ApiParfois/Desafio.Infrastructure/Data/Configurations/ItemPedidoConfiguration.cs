using Desafio.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Desafio.Infrastructure.Data.Configurations;

public class ItemPedidoConfiguration : IEntityTypeConfiguration<ItemPedido>
{
    public void Configure(EntityTypeBuilder<ItemPedido> builder)
    {
        builder.ToTable("ItensPedido");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id)
            .ValueGeneratedOnAdd();

        builder.Property(i => i.Descricao)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(i => i.PrecoUnitario)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(i => i.Quantidade)
            .IsRequired();

        builder.Property(i => i.PedidoId)
            .IsRequired();
    }
}