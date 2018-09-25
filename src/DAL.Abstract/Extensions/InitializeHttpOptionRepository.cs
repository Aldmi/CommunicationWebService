using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Abstract.Concrete;
using DAL.Abstract.Entities.Options.Transport;

namespace DAL.Abstract.Extensions
{
    public static class InitializeHttpOptionRepository
    {
        public static async Task InitializeAsync(this IHttpOptionRepository rep)
        {
            //Если есть хотя бы 1 элемент то НЕ иннициализировать
            if (await rep.CountAsync(option=> true) > 0) 
            {
                return;
            }

           // var httpList = new List<HttpOption>
           // {
           //     new HttpOption
           //     {
           //         Id= 1,
           //         Name= "Http table 1",
           //         Address= "http://Google.com",
           //         AutoStart= true,
           //         Headers = new Dictionary<string, string>
           //         {
           //             {"heaeder 1", "value 1"},
           //             {"heaeder 2", "value 2"}
           //         }
           //     }
           // };

           //await rep.AddRangeAsync(httpList);
        }
    }
}