using DAL.EFCore.Entities.Device;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.EFCore.DbContext.EntitiConfiguration
{
    public class EfDeviceOptionConfig : IEntityTypeConfiguration<EfDeviceOption>
    {
        public void Configure(EntityTypeBuilder<EfDeviceOption> builder)
        {
            //связать приватно поле _exchangeKeysMetaData с типом ExchangeKeysCollection в БД (типа string).  Для сериализации объекта в JSON.
            builder.Property<string>("ExchangeKeysCollection")
                   .HasField("_exchangeKeysMetaData");
        }
    }
}