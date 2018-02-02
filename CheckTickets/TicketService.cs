using System.Net;

namespace CheckTickets
{
  public class TicketService
  {
    public bool Check()
    {
      var dateString = "Nu kan du boka resor fram till 1 juni";

      const string endpoint = "https://mtrexpress.se/gw/site/content/sv-SE";

      using (var webClient = new WebClient())
      {
        var content = webClient.DownloadString(endpoint);
       
      }


      return true;
    }
  }
}
