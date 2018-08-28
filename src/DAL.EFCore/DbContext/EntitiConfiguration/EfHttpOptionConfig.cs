using DAL.EFCore.Entities.Device;
using DAL.EFCore.Entities.Transport;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.EFCore.DbContext.EntitiConfiguration
{
    public class EfHttpOptionConfig : IEntityTypeConfiguration<EfHttpOption>
    {
        public void Configure(EntityTypeBuilder<EfHttpOption> builder)
        {
            //связать приватно поле _headersMetaData с типом HeadersCollection в БД (типа string). Для сериализации объекта в JSON.
            builder.Property<string>("HeadersCollection")
                   .HasField("_headersMetaData");
        }
    }
}