using System;
using System.Threading;

namespace TestingConsole
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("This is testing console output ...");

            var startCode = "Start";
            var stopCode = "Stop";

            var isMeasuringOn = false;
            var counter = 0;
            var canceled = false;
            var randomNumber = new Random();

            while (!canceled)
            {
                Console.WriteLine($"This is message #{counter++}");
                Thread.Sleep(300);

                // each 10th run trigger measuring
                if (counter % 10 == 0)
                {
                    isMeasuringOn = true;
                    Console.WriteLine(startCode);
                }
                if (isMeasuringOn)
                {
                    Thread.Sleep(100 * randomNumber.Next(10, 50));
                    Console.WriteLine(stopCode);

                    isMeasuringOn = false;
                }
                
                // each 5th run ask for cancel
                if(!isMeasuringOn && counter % 5 == 0)
                    try
                    {
                        Console.WriteLine("To cancel this loop, press 'C'.");
                        string cancel = Reader.ReadLine(3000);
                        if (!String.IsNullOrEmpty(cancel) && String.Compare(cancel.Trim(), "c", true) == 0)
                        {
                            canceled = true;
                        }
                    }
                    catch (TimeoutException e)
                    {
                        Console.WriteLine(e.Message);
                    }
            }
        }
    }
}
