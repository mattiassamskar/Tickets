using System;
using System.Configuration;
using System.Globalization;
using System.Net;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace CheckTickets
{
  public static class CheckTickets
  {
    [FunctionName("CheckTickets")]
    public static void Run([TimerTrigger("0 0 7 * * *")]TimerInfo myTimer, TraceWriter log)
    {
      try
      {
        log.Info($"C# Timer trigger function executed at: {DateTime.Now}");
        var smsText = TicketService.TicketsAvailable() ? "Train tickets available!" : "No tickets as of " + GetSwedishDateTimeNow();
        SmsService.SendSms(smsText);
      }
      catch (Exception e)
      {
        SmsService.SendSms(e.Message);
        throw;
      }
    }

    private static string GetSwedishDateTimeNow()
    {
      return TimeZoneInfo.ConvertTimeFromUtc(DateTime.Now.ToUniversalTime(),
          TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time"))
        .ToString("g", new CultureInfo("sv-SE"));
    }
  }

  public static class TicketService
  {
    public static bool TicketsAvailable()
    {
      using (var webClient = new WebClient())
      {
        var content = webClient.UploadString("https://mtrexpress.se/api/mtr/trains/calendar",
          "{\"travel_period\":{\"date_from\":\"2018-02-15\",\"date_to\":\"2020-01-16\"}}");
        
        if (!content.Contains("{\"status\":\"success\",\"data\":{\"calendar\":{\"2018\""))
          throw new Exception("Cannot find ticket data.");

        return content.Contains("2018-06-05");
      }
    }
  }

  public class SmsService
  {
    public static void SendSms(string message)
    {
      var accountSid = ConfigurationManager.AppSettings["AccountSid"];
      var authToken = ConfigurationManager.AppSettings["AuthToken"];
      var toPhoneNumber1 = ConfigurationManager.AppSettings["ToPhoneNumber1"];
      var fromPhoneNumber = ConfigurationManager.AppSettings["FromPhoneNumber"];

      TwilioClient.Init(accountSid, authToken);

      MessageResource.Create(new PhoneNumber(toPhoneNumber1), from: new PhoneNumber(fromPhoneNumber), body: message);
    }
  }
}
