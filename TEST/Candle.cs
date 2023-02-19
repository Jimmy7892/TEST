

using Bot;
using Skender.Stock.Indicators;



public class Candle : IQuote  ///////////////////// TEST
{
    

    public Candle(DateTime date, decimal open, decimal high, decimal low, decimal close, decimal volume)
    {
        Date = date;
        Open = open;
        High = high;
        Close = close;
        Low = low;
        Volume = volume;

    }

    public DateTime Date { get; set; }
    public decimal Open { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    public decimal Close { get; set; }
    public decimal Volume { get; set; }

}




