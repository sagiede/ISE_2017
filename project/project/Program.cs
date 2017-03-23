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
                Console.WriteLine("---------------------------------------------------------------\n\nWelcome to Algo-Trading application,to go back to main manu you can press -1 at any point");
                Console.WriteLine("\nwhat do you wish to do?");
                Console.WriteLine("1- Buy\n2- Sell\n3- Cancel\n4- Queries");
                int command = int.Parse(Console.ReadLine());

                if (command == -1) { }
                else if (command == 1)
                    buyingProcces(pc);
                else if (command == 2)
                    sellingProcces(pc);
                else if (command == 3)
                    cancelingProcces(pc);
                else if (command == 4)
                {
                    Console.WriteLine("Which query would yo like to send?");
                    Console.WriteLine("\n1- Buy/Sell status\n2- User status\n3- Market Status\n");
                    command = int.Parse(Console.ReadLine());
                    if (command == -1) { }
                    else if (command == 1)
                    {
                        Console.WriteLine("please enter the transaction ID");
                        int id = int.Parse(Console.ReadLine());
                        if (id != -1)
                        Console.WriteLine(pc.SendQueryBuySellRequest(id));
                    }
                    else if (command == 2)
                        Console.WriteLine(pc.SendQueryUserRequest());
                    else if (command == 3)
                    {
                        Console.WriteLine("please enter the stock number you wish to ask about");
                        int commodity = int.Parse(Console.ReadLine());
                        if (commodity != -1)
                        Console.WriteLine(pc.SendQueryMarketRequest(commodity));
                    }
                }//else
                    else
                    Console.WriteLine("you have entered invaild number, please follow the instructions");
            }//while
        }//main

        private static void buyingProcces(PipeConnection pc)
        {
            Console.WriteLine("Which commodity would yo like to buy?");
            int commodity = int.Parse(Console.ReadLine());
            if (commodity == -1)
                return;
            Console.WriteLine("\nHow many?");
            int amount = int.Parse(Console.ReadLine());
            if (amount == -1)
                return;
            Console.WriteLine("\nEnter your price");
            int price = int.Parse(Console.ReadLine());
            if (price == -1)
                return;

            Console.WriteLine(pc.SendBuyRequest(price , commodity , amount ));
        }

        private static void cancelingProcces(PipeConnection pc)
        {
            Console.WriteLine("which transaction would you like to cancel?");
            int idNum = int.Parse(Console.ReadLine());
            if (idNum == -1)
                return;

            bool ans = (pc.SendCancelBuySellRequest(idNum));
            if (ans)
                Console.WriteLine("transcation canceled succecfully");
            else
                Console.WriteLine("no such transcation");
        }

        private static void sellingProcces(PipeConnection pc)
        {
            Console.WriteLine("Which commodity would yo like to sell?");
            int commodity = int.Parse(Console.ReadLine());
            if (commodity == -1)
                return;
            Console.WriteLine("\nHow many?");
            int amount = int.Parse(Console.ReadLine());
            if (amount == -1)
                return;
            Console.WriteLine("\nEnter your price");
            int price = int.Parse(Console.ReadLine());
            if (price == -1)
                return;

            Console.WriteLine(pc.SendSellRequest(price, commodity, amount));
        }

    }//class

}

