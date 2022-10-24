using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;

namespace FlexitHisMVC
{
    public class StandardMessages
    {
        public static void CallSerilog(Exception ex)
        {
            try
            {
                //var config = new ConfigurationBuilder()
                //.AddJsonFile("appsettings.json")
                //.Build();

                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                //Serilog.Core.Logger logger = new Serilog.LoggerConfiguration()
                //.ReadFrom.Configuration(config)
                //.WriteTo.MySQL(connectionString: "server=MYSQL5030.site4now.net;database=db_a50752_bq;uid=a50752_bq;pwd='baxqazan12345';charset=utf8;Pooling=true;", "SerilogLogs")
                //.CreateLogger();
                //logger.Write(level: Serilog.Events.LogEventLevel.Error, exception: ex, messageTemplate: ex.Message + $"-------- {AzerbaijanStandardTime.ToString("dd.MM.yyyy HH:mm")}");
                //ex = new Exception("z");
                //LogEventProperty logEventProperty = new LogEventProperty.;
                //LogEvent logEvent = new LogEvent(AzerbaijanStandardTimeOffSet, LogEventLevel.Error, ex, ex.Message, LogEventProperty.); ;
                Log.Error(ex, ex.Message);
            }
            catch (Exception)
            {


            }
            finally
            {
                //Log.CloseAndFlush();
            }
        }

        public static DateTime AzerbaijanStandardTime
        {
            get
            {
                DateTime serverTime = DateTime.Now; // gives you current Time in server timeZone
                DateTime utcTime = serverTime.ToUniversalTime(); // convert it to Utc using timezone setting of server computer

                TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Azerbaijan Standard Time");
                DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi); // convert from utc to local

                return localTime;
            }
        }

        //public static DateTimeOffset AzerbaijanStandardTimeOffSet
        //{
        //    get
        //    {
        //        DateTimeOffset serverTime = DateTimeOffset.Now; // gives you current Time in server timeZone
        //        DateTimeOffset utcTime = serverTime.ToUniversalTime(); // convert it to Utc using timezone setting of server computer

        //        TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Azerbaijan Standard Time");
        //        DateTimeOffset localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi); // convert from utc to local

        //        return localTime;
        //    }
        //}
    }
}