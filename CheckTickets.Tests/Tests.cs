using Xunit;

namespace CheckTickets.Tests
{
  public class Tests
  {
    [Fact]
    public void TestTicketCheck()
    {
      var ticketsAvailable = TicketService.TicketsAvailable();
    }
  }
}
