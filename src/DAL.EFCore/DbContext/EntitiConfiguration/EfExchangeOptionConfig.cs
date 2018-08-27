using DAL.EFCore.Entities.Exchange;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.EFCore.DbContext.EntitiConfiguration
{
    public class EfExchangeOptionConfig : IEntityTypeConfiguration<EfExchangeOption>
    {
        public void Configure(EntityTypeBuilder<EfExchangeOption> builder)
        {
            //KeyTransport.
            //Связь 1к1 с объединением таблиц (KeyTransport входит в табл. EfExchangeOptions) 
            builder.HasOne(e => e.KeyTransport)
                   .WithOne(e => e.EfExchangeOption)
                   .HasForeignKey<EfKeyTransport>(e => e.EfExchangeOptionId);

            //EfProvider.
            //Связь 1к1.
            builder.HasOne(e => e.Provider)
                   .WithOne(e => e.EfExchangeOption)
                   .HasForeignKey<EfProvider>(e => e.EfExchangeOptionId);
        }
    }
}