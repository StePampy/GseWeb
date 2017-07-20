using GseWeb.Models.Hours;
using GseWeb.Models.Orders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GseWeb.Models.Hours
{
    public class HoursOfDay
    {
        [Key, Column(Order = 0)]
        public string UserId { get; set; }

        [Key, Column(Order = 1)]
        public DateTime Date { get; set; }
        public int Week { get; set; }
        public bool IsFestivity { get; set; }
        public TimeSpan Ordinary { get; set; }
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
        public TimeSpan Break { get; set; }
        public TimeSpan Travel { get; set; }
        public WorkType WorkType { get; set; }
        public string Note { get; set; }
        public bool OffSite { get; set; }
        public TimeSpan WorkTime { get; set; }
        public Holiday.Holiday_Type? Holiday { get; set; }
        public TimeSpan HolidayStart { get; set; }
        public TimeSpan HolidayEnd { get; set; }
        public TimeSpan HolidayTime { get; set; }

        [ForeignKey("Date")]
        public virtual ICollection<WorkOrder> WorkOrders { get; set; }

        /// <summary>
        /// Presenza Lavorativa
        /// </summary>
        public bool Presence
        {
            get
            {
                return (WorkTime + Travel) > new TimeSpan(0);
            }
        }
        /// <summary>
        /// Straordinario Previsto
        /// </summary>
        [NotMapped]
        public TimeSpan ExtraExpected { get; set; }

        [NotMapped]
        public TimeSpan OrdersTimeComplete
        {
            get
            {
                return TimeSpan.FromSeconds(WorkOrders.Sum(x => x.Hours.TotalSeconds));
            }
        }

        [NotMapped]
        public TimeSpan OrdersTimeApproved
        {
            get
            {
                return TimeSpan.FromSeconds(WorkOrders.Where(x => x.Approved).Sum(x => x.Hours.TotalSeconds));
            }
        }

        [NotMapped]
        public TimeSpan OrdersTimeWork
        {
            get
            {
                var cal = OrdersTimeApproved - Travel;
                return (cal < new TimeSpan(0)) ? new TimeSpan(0) : cal;
            }
        }

        [NotMapped]
        public TimeSpan OrdersTimeTravel
        {
            get
            {
                var cal = OrdersTimeApproved - OrdersTimeWork;
                return (cal < new TimeSpan(0)) ? new TimeSpan(0) : cal;
            }
        }

        [NotMapped]
        public TimeSpan OrdinaryRegular
        {
            get
            {
                return (Holiday != null && (Holiday == Models.Holiday.Holiday_Type.Permesso || Holiday == Models.Holiday.Holiday_Type.Ferie))
                    ? Ordinary - HolidayTime
                    : Ordinary;
            }
        }
        /// <summary>
        /// Tempo di lavoro regolare
        /// </summary>
        [NotMapped]
        public TimeSpan WorkTimeRegular
        {
            get
            {
                var workExp = ExtraExpected + OrdinaryRegular;
                return (OrdersTimeWork > workExp) ? workExp : OrdersTimeWork;
            }
        }

        /// <summary>
        /// Straordinario Regolare
        /// </summary>
        [NotMapped]
        public TimeSpan ExtraRegular
        {
            get
            {
                var cal = WorkTimeRegular - OrdinaryRegular;
                return (cal < new TimeSpan(0)) ? new TimeSpan(0) : cal;
            }
        }

        /// <summary>
        /// Tempo di lavoro ordinario
        /// </summary>
        [NotMapped]
        public TimeSpan WorkTimeOrdinary
        {
            get
            {
                return WorkTimeRegular - ExtraRegular;
            }
        }

        /// <summary>
        /// Monte Ore
        /// </summary>
        [NotMapped]
        public TimeSpan ExtraAmount
        {
            get
            {
                var rec = (WorkTypeRegular == WorkType.Recupero) ? LessTime : new TimeSpan(0);
                return  OrdersTimeWork - WorkTimeRegular - rec;
            }
        }

        /// <summary>
        /// Ore Dovute
        /// </summary>
        [NotMapped]
        public TimeSpan LessTime
        {
            get
            {
                var cal = OrdinaryRegular - OrdersTimeWork;
                return (cal < new TimeSpan(0)) ? new TimeSpan(0) : cal;
            }
        }

        [NotMapped]
        public WorkType WorkTypeRegular
        {
            get
            {
                if (Holiday != null && Holiday == Models.Holiday.Holiday_Type.Ferie && Ordinary != new TimeSpan(0))
                    return WorkType.Ferie;
                if (Holiday != null && Holiday == Models.Holiday.Holiday_Type.Permesso && Ordinary != new TimeSpan(0))
                    return WorkType.PermessoRetribuito;
                if ((WorkTime + Travel) > OrdersTimeComplete)
                    return WorkType.NoCommesse;
                if (OrdersTimeApproved != OrdersTimeComplete)
                    return WorkType.NoApproved;
                if (OrdersTimeTravel > OrdinaryRegular)
                    return WorkType.Viaggio;
                if (LessTime > new TimeSpan(0) && (int)WorkType > 5)
                    return this.WorkType;
                if (LessTime > new TimeSpan(0) && (int)WorkType <= 5 && Date <= DateTime.Now)
                    return WorkType.NonGiustificato;
                if (ExtraRegular > new TimeSpan(0))
                    return WorkType.Straordinario;
                if (!Presence)
                    return WorkType.Default;
                return WorkType.Lavoro;
            }
        }
    }
}