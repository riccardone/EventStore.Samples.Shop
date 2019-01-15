using System;
using EventStore.Shop.Sales.Adapter;

namespace EventStore.Shop.Sales.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            var endPoint = new EndPoint(new Repository());
            endPoint.Start();

            Console.WriteLine("Press enter to leave the program");
            Console.ReadLine();
        }
    }
}
