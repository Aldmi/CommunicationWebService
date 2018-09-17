﻿using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Abstract.Concrete;
using DAL.Abstract.Entities.Options.Transport;

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

            var tcpIpList = new List<TcpIpOption>
            {
                new TcpIpOption
                { 
                   Id=1,
                   Name = "RemoteTcpIpTable 1",
                   AutoStart = true,
                },
                new TcpIpOption
                { 
                    Id=2,
                    Name = "RemoteTcpIpTable 2",
                    AutoStart = true,
                }
            };

           await rep.AddRangeAsync(tcpIpList);
        }
    }
}