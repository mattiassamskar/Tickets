using System;
using System.Net;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

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

        if (TicketService.TicketsAvailable())
          SmsService.SendSms("Train tickets available!");
      }
      catch (Exception e)
      {
        SmsService.SendSms(e.Message);
        throw;
      }
    }
  }

  public static class TicketService
  {
    public static bool TicketsAvailable()
    {
      using (var webClient = new WebClient())
      {
        var content = webClient.DownloadString("https://mtrexpress.se/gw/site/content/sv-SE");

        if(!content.Contains("Nu kan du boka resor fram till"))
          throw new Exception("Cannot find ticket data.");

        return !content.Contains("Nu kan du boka resor fram till 1 juni");
      }
    }
  }

  public class SmsService
  {
    public static void SendSms(string trainTicketsAvailable)
    {
      
    }
  }
}
