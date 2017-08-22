namespace GseWeb.DAL
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using Models.Account;
    using Models.Hours;
    using Models.Orders;
    using Models.Vehicles;
    using Models.Holiday;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using Models;
    using System.Collections;
    using Models.Materials;

    public class GestionaleDB : DbContext
    {
        public GestionaleDB() : base("name=GestionaleDB")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<User>().Ignore(u => u.ConfirmPassword);
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Role> UserRoles { get; set; }
        public virtual DbSet<Profile> HoursProfiles { get; set; }
        public virtual DbSet<Hour> Hours { get; set; }
        public virtual DbSet<Festivity> Festivities { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<UserOrder> UsersOrders { get; set; }
        public virtual DbSet<WorkOrder> WorkOrders { get; set; }
        public virtual DbSet<UserVehicle> UsersVehicles { get; set; }
        public virtual DbSet<WorkVehicle> WorkVehicles { get; set; }
        public virtual DbSet<Holiday> Holidays { get; set; }
        public virtual DbSet<MonthFrozen> MonthsFrozen { get; set; }
        public virtual DbSet<Material> Materials { get; set; }


        // Intercetto il salvaggio dei dati per verificare se il mese e congelato
        public override int SaveChanges()
        {
            this.ChangeTracker.Entries().ToList()
                .ForEach(x =>
                {

                    switch (ObjectContext.GetObjectType(x.Entity.GetType()).Name)
                    {
                        case "Hour":
                        case "WorkOrder":
                        case "WorkVehicle":
                            dynamic entity = x.Entity;
                            Check_Frozen(entity.Date.Year, entity.Date.Month);
                            break;
                    }
                });
            return base.SaveChanges();
        }

        /// <summary>
        /// Nel caso che e congelato restituisce un errore
        /// </summary>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        public virtual void Check_Frozen(int Year, int Month)
        {
            if (MonthsFrozen.Any(x => x.Year == Year && x.Month == Month))
                throw new Exception("Il mese è congelato, contattare amministrazione");
        }

        public virtual Models.Hours.HoursOfDay[] UserWork(int Year, string UserId)
        {
            var data = ((IObjectContextAdapter)this)
                .ObjectContext.ExecuteStoreQuery<Models.Hours.HoursOfDay>("CALL sp_UserWork_Year({0}, {1});", Year, UserId).ToArray();
            foreach (var item in data)
            {
                item.WorkOrders = WorkOrders.Where(x => x.UserId == item.UserId && x.Date == item.Date).ToArray();
            }
            return data;
        }

        public virtual TimeSpan GetOrdinaryHours(string UserId, DayOfWeek DayOfWeek)
        {
            var user = Users.Find(UserId);
            var prof = user.HoursProfile;
            switch (DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return user.HoursProfile.Sun;
                case DayOfWeek.Monday:
                    return user.HoursProfile.Mon;
                case DayOfWeek.Tuesday:
                    return user.HoursProfile.Tue;
                case DayOfWeek.Wednesday:
                    return user.HoursProfile.Wed;
                case DayOfWeek.Thursday:
                    return user.HoursProfile.Thu;
                case DayOfWeek.Friday:
                    return user.HoursProfile.Fri;
                case DayOfWeek.Saturday:
                    return user.HoursProfile.Sat;
            }
            throw new Exception("Invalid Day of Week");
        }

        public virtual Hour[] getHours(int Year, int Month)
        {
            var data = ((IObjectContextAdapter)this)
                .ObjectContext.ExecuteStoreQuery<Hour>("CALL test({0}, {1});", Year, Month).ToArray();
            return data;
        }
    }
}