using DAL.EFCore.Mappers;

namespace DAL.EFCore.Repository
{
    public class EfBaseRepository
    {
        static EfBaseRepository()
        {
            AutoMapperConfig.Register();
        }
    }
}