using DAL.EFCore.Entities.Device;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.EFCore.DbContext.EntitiConfiguration
{
    public class EfDeviceOptionConfig : IEntityTypeConfiguration<EfDeviceOption>
    {
        public void Configure(EntityTypeBuilder<EfDeviceOption> builder)
        {
            //связать приватно поле _exchangeKeys с типом ExchangeKeysCollection в БД (типа string)
            builder.Property<string>("ExchangeKeysCollection")
                   .HasField("_exchangeKeys");
        }
    }
}