
using Binance.Net;
using Binance.Net.Clients;
using Binance.Net.Enums;
using Binance.Net.Interfaces;
using Binance.Net.Objects;
using Bybit.Net.Clients;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Sockets;
using GenericParsing;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Net.WebSockets;
using System.Text;

namespace Bot
{
    public class TEST
    {
        public static void Main(string[] args)
        {
            BinanceSocketClient socketClient = new BinanceSocketClient();
            socketClient.ClientOptions.LogLevel = LogLevel.Trace;



            string path = @"C:\Users\jimmy\Desktop\US30\BTC.csv";

            var candleList = GetCandleFromFileBTC(path);
            var highLowLists = GetLowAndHighClassic(candleList);



            CHoCH_Detection(candleList, highLowLists);

 

        }
        public static void CHoCH_Detection(List<Candle> candleList, IEnumerable<List<Candle>> highLowLists )
        {
            var highList = highLowLists.First();
            var lowList = highLowLists.Last();

            for (int i = 10; i < candleList.Count - 10; i++)
            {         
                if (highList.Contains(candleList[i]))
                {
                    for (int a = i; a < candleList.Count; a++)
                    {
                        if(candleList[a].High > candleList[i].High)
                        {



                            Console.WriteLine("CHoCH" + " " + candleList[a].Date);
                            break;
                        }


                    }
                }


                highLowLists.First().Where(high => high.High <= candleList[i].High && high.Date <= candleList[i].Date);
                                                 
            }


        }
        static List<Candle> GetCandleFromFile(string path)
        {
               
            List<Candle> candleList = new();
  
            using (GenericParser parser = new())
            {

                parser.SetDataSource(path);

                char delimiter = ";"[0];

                parser.ColumnDelimiter = delimiter;
                parser.FirstRowHasHeader = false;



                string date, time, o, h, l, c, v;
                decimal open, high, low, close, volume;
                DateTime timeStamp;
                while (parser.Read())
                {
                   

                    date = parser[0];
                    time = parser[1];
                    o = parser[2];
                    h = parser[3];
                    l = parser[4];
                    c = parser[5];
                    v = parser[6];

                    string fullDate = date.Replace("/","-") + " " + time;                          //"2011-03-21 13:26";"yyyy-MM-dd HH:mm",
                    timeStamp = DateTime.ParseExact(fullDate, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);

                    open = decimal.Parse(o);
                    high = decimal.Parse(h);
                    low = decimal.Parse(l);
                    close = decimal.Parse(c);
                    volume = decimal.Parse(v);

                    Candle candle = new(timeStamp, open, high, low, close, volume);

                    candleList.Add(candle);

                    Console.WriteLine("Date: " + candle.Date + " Open: " + candle.Open + " High: " + candle.High + " Low: " + candle.Low + " Close: " + candle.Close + " Volume: " + candle.Volume);
                }
                Console.WriteLine("Candle count: " + candleList.Count);
                return candleList;

            }  
        }
        static List<Candle> GetCandleFromFileBTC(string path)
        {

            List<Candle> candleList = new();

            using (GenericParser parser = new())
            {

                parser.SetDataSource(path);

                char delimiter = ","[0];

                parser.ColumnDelimiter = delimiter;
                parser.FirstRowHasHeader = false;



                string o, h, l, c, v;

                decimal date, open, high, low, close, volume;
        
                DateTime timeStamp;
                while (parser.Read())
                {


                    date = decimal.Parse( parser[0]);             
                    o = parser[1];
                    h = parser[2];
                    l = parser[3];
                    c = parser[4];
                    v = parser[5];

                    timeStamp  = UnixTimeStampToDateTime((double)date);



                    open = decimal.Parse(o.Replace(".",","));
                    high = decimal.Parse(h.Replace(".", ","));
                    low = decimal.Parse(l.Replace(".", ","));
                    close = decimal.Parse(c.Replace(".", ","));
                    volume = decimal.Parse(v.Replace(".", ","));

                    Candle candle = new(timeStamp, open, high, low, close, volume);

                    candleList.Add(candle);

                    Console.WriteLine("Date: " + candle.Date + " Open: " + candle.Open + " High: " + candle.High + " Low: " + candle.Low + " Close: " + candle.Close + " Volume: " + candle.Volume);
                }
                Console.WriteLine("Candle count: " + candleList.Count);
                return candleList;

            }
        }
        public void GetLowAndHigh(List<Candle> candleList)
        {
            List<Candle> topCandleList = new();
            List<Candle> bottomCandleList = new();
            for (int i = 10; i < candleList.Count -10; i++)
            {

                var currentCandle = candleList[i];


                if ((currentCandle.High > candleList[i + 1].High) && (currentCandle.High > candleList[i + 2].High))
                {
                    if(currentCandle.Open < currentCandle.Close)
                    {
                        bottomCandleList.Add(currentCandle);
                        Console.WriteLine(currentCandle.Date + "       " + currentCandle.High);
                        i = i + 2;
                    }
                    else if(candleList[i - 1].Open < candleList[i - 1].Close)
                    {
                        bottomCandleList.Add(currentCandle);
                        Console.WriteLine(currentCandle.Date + "       " + currentCandle.High);
                        i = i + 2;
                    }
                  
                   
                }
            }  
        }
        public static IEnumerable<List<Candle>> GetLowAndHighClassic(List<Candle> candleList)
        {

            List<Candle> topCandleList = new();
            List<Candle> bottomCandleList = new();
            for (int i = 10; i < candleList.Count - 10; i++)
            {

                var currentCandle = candleList[i];

                // detect high
                if ((currentCandle.High > candleList[i + 1].High) && (currentCandle.High > candleList[i + 2].High) && (currentCandle.High > candleList[i - 1].High) && (currentCandle.High > candleList[i - 2].High))
                {             
                    topCandleList.Add(currentCandle);
                    Console.WriteLine(currentCandle.Date + "       " + currentCandle.High);
                    //i++;                 
                }

                // detect low
                if ((currentCandle.Low < candleList[i + 1].Low) && (currentCandle.Low < candleList[i + 2].Low) && (currentCandle.Low < candleList[i - 1].Low) && (currentCandle.Low < candleList[i - 2].Low))
                {
                    bottomCandleList.Add(currentCandle);
                    //Console.WriteLine(currentCandle.Date + "       " + currentCandle.Low);
                    //i++;
                }
            }


            return new List<List<Candle>> { topCandleList, bottomCandleList };
        }
        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddMilliseconds(unixTimeStamp);
            return dateTime;
        }
    }
}















