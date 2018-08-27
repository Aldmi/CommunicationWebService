using DAL.EFCore.Entities.Exchange;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.EFCore.DbContext.EntitiConfiguration
{
    public class EfExchangeOptionConfig : IEntityTypeConfiguration<EfExchangeOption>
    {
        public void Configure(EntityTypeBuilder<EfExchangeOption> builder)
        {           
            builder.Property<string>("KeyTransportMetaData")
                .HasField("_keyTransportMetaData")
                .IsRequired();

            //EfProvider.
            //Связь 1к1.
            builder.HasOne(e => e.Provider)
                   .WithOne(e => e.EfExchangeOption)
                   .HasForeignKey<EfProvider>(e => e.EfExchangeOptionId);
        }
    }
}