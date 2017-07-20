using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GseWeb.Models;
using MySql.Data.MySqlClient;
using System.Data.Common;
using System.Configuration;

namespace GseWeb.DAL
{
    public static class Hours
    {
        public static Hour GetHour(string UserId, DateTime date)
        {
            using (var conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["GestionaleDB"].ToString()))
            using (var cmd = new MySqlCommand(Properties.Resources.GetHour, conn))
            {
                conn.Open();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandTimeout = 5;
                cmd.Parameters.AddWithValue("@UserId", UserId);
                cmd.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));

                //System.Data.DataTable _dataTable = new System.Data.DataTable();
                //var _da = new MySqlDataAdapter(cmd);
                //_da.Fill(_dataTable);

                var results = cmd.ExecuteReader();
                return results.Cast<DbDataRecord>().Select(x =>
                new Hour
                {
                    Date = (DateTime)x["Date"],
                    Start = (TimeSpan)x["Start"],
                    End = (TimeSpan)x["End"],
                    Break = (TimeSpan)x["Break"],
                    Travel = (TimeSpan)x["Travel"],
                    WorkTypeId = Convert.ToInt32( x["WorkTypeId"]),
                    Note = (string)x["Note"],
                    OffSite = Convert.ToBoolean(x["OffSite"]),
                    Results = new HourResult
                    {
                        WorkTime = (TimeSpan)x["WorkTime"],
                        DiffTime = (TimeSpan)x["DiffTime"],
                    }
                }
                ).FirstOrDefault();
            }
        } 

        public static void CreateHour(string UserId, Hour hour)
        {
            using (var conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["GestionaleDB"].ToString()))
            using (var cmd = new MySqlCommand(Properties.Resources.CreateHour, conn))
            {
                conn.Open();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandTimeout = 5;
                cmd.Parameters.AddWithValue("@UserId", UserId);
                cmd.Parameters.AddWithValue("@Date", hour.Date.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@Start", hour.Start);
                cmd.Parameters.AddWithValue("@End", hour.End);
                cmd.Parameters.AddWithValue("@Break", hour.Break);
                cmd.Parameters.AddWithValue("@Travel", hour.Travel);
                cmd.Parameters.AddWithValue("@WorkTypeId", hour.WorkTypeId);
                cmd.Parameters.AddWithValue("@Note", hour.Note ?? "");
                cmd.Parameters.AddWithValue("@OffSite", hour.OffSite);

                var results = cmd.ExecuteNonQuery();
            }
        }

        public static void EditHour(string UserId, Hour hour)
        {
            using (var conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["GestionaleDB"].ToString()))
            using (var cmd = new MySqlCommand(Properties.Resources.EditHour, conn))
            {
                conn.Open();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandTimeout = 5;
                cmd.Parameters.AddWithValue("@UserId", UserId);
                cmd.Parameters.AddWithValue("@Date", hour.Date.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@Start", hour.Start);
                cmd.Parameters.AddWithValue("@End", hour.End);
                cmd.Parameters.AddWithValue("@Break", hour.Break);
                cmd.Parameters.AddWithValue("@Travel", hour.Travel);
                cmd.Parameters.AddWithValue("@WorkTypeId", hour.WorkTypeId);
                cmd.Parameters.AddWithValue("@Note", hour.Note ?? "");
                cmd.Parameters.AddWithValue("@OffSite", hour.OffSite);

                var results = cmd.ExecuteNonQuery();
            }
        }

        public static void DeleteHour(string UserId, DateTime date)
        {
            using (var conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["GestionaleDB"].ToString()))
            using (var cmd = new MySqlCommand(Properties.Resources.DeleteHour, conn))
            {
                conn.Open();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandTimeout = 5;
                cmd.Parameters.AddWithValue("@UserId", UserId);
                cmd.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
                var results = cmd.ExecuteNonQuery();
            }
        }

        public static WorkType[] GetWorkTypes()
        {
            using (var conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["GestionaleDB"].ToString()))
            using (var cmd = new MySqlCommand(Properties.Resources.GetWorkTypes, conn))
            {
                conn.Open();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandTimeout = 5;
                //cmd.Parameters.AddWithValue("@VALUE", value);

                //DataTable _dataTable = new DataTable();
                //var _da = new MySqlDataAdapter(cmd);
                //_da.Fill(_dataTable);

                var results = cmd.ExecuteReader();
                return results.Cast<DbDataRecord>().Select(x =>
                new WorkType
                {
                    Id = (int)x["Id"],
                    Description = (string)x["Description"],
                }
                ).ToArray();
            }
        }

        public static Hour[] GetReportMonth(string UserId, int year, int month)
        {
            using (var conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["GestionaleDB"].ToString()))
            using (var cmd = new MySqlCommand(Properties.Resources.ReportMonthHour, conn))
            {
                conn.Open();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandTimeout = 5;
                cmd.Parameters.AddWithValue("@UserId", UserId);
                cmd.Parameters.AddWithValue("@Year", year);
                cmd.Parameters.AddWithValue("@Month", month);

                //System.Data.DataTable _dataTable = new System.Data.DataTable();
                //var _da = new MySqlDataAdapter(cmd);
                //_da.Fill(_dataTable);

                var results = cmd.ExecuteReader();
                return results.Cast<DbDataRecord>().Select(x =>
                new Hour
                {
                    Date = (DateTime)x["Date"],
                    Start = (TimeSpan)x["Start"],
                    End = (TimeSpan)x["End"],
                    Break = (TimeSpan)x["Break"],
                    Travel = (TimeSpan)x["Travel"],
                    WorkTypeId = Convert.ToInt32(x["WorkTypeId"]),
                    Note = (string)x["Note"],
                    OffSite = Convert.ToBoolean(x["OffSite"]),
                    Results = new HourResult
                    {
                        WorkTime = (TimeSpan)x["WorkTime"],
                        DiffTime = (TimeSpan)x["DiffTime"],
                    }
                }
                ).ToArray();
            }
        }

        public static MonthTotal[] GetReportMonthTotal(string UserId, int year, int month)
        {
            using (var conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["GestionaleDB"].ToString()))
            using (var cmd = new MySqlCommand(Properties.Resources.ReportMonthTotal, conn))
            {
                conn.Open();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandTimeout = 5;
                cmd.Parameters.AddWithValue("@UserId", UserId);
                cmd.Parameters.AddWithValue("@Year", year);
                cmd.Parameters.AddWithValue("@Month", month);
                //cmd.Parameters.AddWithValue("@Tot", new TimeSpan());
                //cmd.Parameters.AddWithValue("@Ord", new TimeSpan());

                System.Data.DataTable _dataTable = new System.Data.DataTable();
                var _da = new MySqlDataAdapter(cmd);
                _da.Fill(_dataTable);

                var results = cmd.ExecuteReader();
                return results.Cast<DbDataRecord>().Select(x =>
                new MonthTotal
                {
                    WorkTypeId = Convert.ToInt32(x["WorkTypeId"]),
                    Description = (string)x["Description"],
                    Value = (TimeSpan)x["Value"],
                }
                ).ToArray();
            }
        }

        public static Festivity[] GetFestivity(int year, int month)
        {
            using (var conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["GestionaleDB"].ToString()))
            using (var cmd = new MySqlCommand(Properties.Resources.GetFestivity, conn))
            {
                conn.Open();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandTimeout = 5;
                cmd.Parameters.AddWithValue("@Year", year);
                cmd.Parameters.AddWithValue("@Month", month);

                //System.Data.DataTable _dataTable = new System.Data.DataTable();
                //var _da = new MySqlDataAdapter(cmd);
                //_da.Fill(_dataTable);

                var results = cmd.ExecuteReader();
                return results.Cast<DbDataRecord>().Select(x =>
                new Festivity
                {
                    Date = (DateTime)x["Date"],
                    Description = (string)x["Description"],
                }
                ).ToArray();
            }
        }

    }
}