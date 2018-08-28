using DAL.EFCore.DbContext.EntitiConfiguration;
using DAL.EFCore.Entities.Device;
using DAL.EFCore.Entities.Exchange;
using DAL.EFCore.Entities.Transport;
using Microsoft.EntityFrameworkCore;

namespace DAL.EFCore.DbContext
{
    public sealed class Context : Microsoft.EntityFrameworkCore.DbContext
    {
        private readonly string _connStr;  // строка подключенния


        #region Reps

        public DbSet<EfSerialOption> SerialPortOptions { get; set; }
        public DbSet<EfTcpIpOption> TcpIpOptions { get; set; }
        public DbSet<EfHttpOption> HttpOptions { get; set; }
        public DbSet<EfDeviceOption> DeviceOptions { get; set; }
        public DbSet<EfExchangeOption> ExchangeOptions { get; set; }

        #endregion



        #region ctor

        public Context(string connStr)
        {
            _connStr = connStr;
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;//Отключение Tracking для всего контекста
            Database.EnsureCreated(); //Если БД нет, то создать.
        }

        #endregion



        #region Config

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //Context сам получает строку подключения при миграции и работе.
            //(Рабоатет для миграции и работы!!!!!!!)
            // var config = JsonConfigLib.GetConfiguration();
            //var connectionString = config.GetConnectionString("MainDbConnection");
            //optionsBuilder.UseSqlServer(connectionString);
            //optionsBuilder.UseSqlServer(connectionString, ob => ob.MigrationsAssembly(typeof(Context).GetTypeInfo().Assembly.GetName().Name));\

            optionsBuilder.UseSqlServer(_connStr);
        }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           modelBuilder.ApplyConfiguration(new EfDeviceOptionConfig());
           modelBuilder.ApplyConfiguration(new EfExchangeOptionConfig());
           modelBuilder.ApplyConfiguration(new EfHttpOptionConfig());
           base.OnModelCreating(modelBuilder);
        }

        #endregion
    }
}