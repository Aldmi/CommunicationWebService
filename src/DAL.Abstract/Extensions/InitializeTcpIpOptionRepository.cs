using System.Threading.Tasks;
using DAL.Abstract.Concrete;

namespace DAL.Abstract.Extensions
{
    public static class InitializeTcpIpOptionRepository
    {
        public static async Task InitializeAsync(this ITcpIpOptionRepository rep)
        {
            //Если есть хотя бы 1 элемент то НЕ иннициализировать
            if (await rep.CountAsync(option=> true) > 0) 
            {
                return;
            }

            //var tcpIpList = new List<TcpIpOption>
            //{
            //    new TcpIpOption
            //    {
            //       Id=1,
            //       Name = "TcpIp table 1",
            //       AutoStart = true,
            //    }
            //};

            //await rep.AddRangeAsync(tcpIpList);
        }
    }
}