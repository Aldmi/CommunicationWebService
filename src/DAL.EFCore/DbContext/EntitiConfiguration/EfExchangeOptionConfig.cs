using DAL.EFCore.Entities.Exchange;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.EFCore.DbContext.EntitiConfiguration
{
    public class EfExchangeOptionConfig : IEntityTypeConfiguration<EfExchangeOption>
    {
        public void Configure(EntityTypeBuilder<EfExchangeOption> builder)
        {
            //связать приватно поле _keyTransportMetaData с типом KeyTransportMetaData в БД (типа string).  Для сериализации объекта в JSON.
            builder.Property<string>("KeyTransportMetaData")
                .HasField("_keyTransportMetaData")
                .IsRequired();

            //связать приватно поле _providerOptionMetaData с типом Provider в БД (типа string).  Для сериализации объекта в JSON.
            builder.Property<string>("ProviderOptionMetaData")
                .HasField("_providerOptionMetaData")
                .IsRequired();

            //EfProvider.
            //Связь 1к1.
            //builder.HasOne(e => e.Provider)
            //       .WithOne(e => e.EfExchangeOption)
            //       .HasForeignKey<EfProvider>(e => e.EfExchangeOptionId);
        }
    }
}