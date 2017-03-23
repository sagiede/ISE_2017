using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicLayer;
using MarketClient;

namespace project
{
    class Program
    {
        static void Main(string[] args)
        {
            PipeConnection pc = new PipeConnection();
            while (true)
            {
                Console.WriteLine("Welcome to Algo-Trading application, what do you wish to do?");
                Console.WriteLine("1-Buy\n2-Sell\n3-Cancel\n4-Queries");
                int command = int.Parse(Console.ReadLine());

                if(command == 4)
                    Console.WriteLine("Which query would yo like to send?");
                    Console.WriteLine("\n1 -Buy/Sell status\n2-User status\n3-Market Status\n");
            }
        }
    }
}
