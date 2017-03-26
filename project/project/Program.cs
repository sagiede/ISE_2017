﻿using System;
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

            LogicLayer.PipeConnection pc = new LogicLayer.PipeConnection(); // response depend the input
            while (true)
            {
                Console.WriteLine("---------------------------------------------------------------\n\nWelcome to Algo-Trading application,to go back to main manu you can press -1 at any point");
                Console.WriteLine("\nwhat do you wish to do?");
                Console.WriteLine("1- Buy\n2- Sell\n3- Cancel\n4- Queries");
                int command = checkvalid2(); // the first choose of the client
      
                if (command == -1) { }
                    //want to buy
                    else if (command == 1)
                        buyingProcces(pc);
                    // want to sell
                    else if (command == 2)
                        sellingProcces(pc);
                    // want to cancel
                    else if (command == 3)
                        cancelingProcces(pc);
                    // want a query
                    else if (command == 4)
                    {
                        Console.WriteLine("Which query would yo like to send?");
                        Console.WriteLine("\n1- Buy/Sell status\n2- User status\n3- Market Status\n");
                        command = checkvalid2();
                   
                        // want go back
                        if (command == -1) { }
                        //Buy/Sell status
                        else if (command == 1)
                        {
                            Console.WriteLine("please enter the transaction ID");
                              int id = checkvalid2();
                           
                              if (id != -1)
                                    Console.WriteLine(pc.SendQueryBuySellRequest(id));

                                //user status
                                else if (command == 2)
                                    Console.WriteLine(pc.SendQueryUserRequest());
                                //Market Status
                                else if (command == 3)
                                {
                                    Console.WriteLine("please enter the stock number you wish to ask about");
                                    int  commodity = checkvalid2();
                                   
                                        if (commodity != -1)
                                            Console.WriteLine(pc.SendQueryMarketRequest(commodity));
                                    }
                                }//else
                                else
                                    Console.WriteLine("you have entered invaild number, please follow the instructions");
                            }
                        
                            }//while
                        }//main
                    
        //if the client whant to buy commodity
        private static void buyingProcces(PipeConnection pc)
        { 
            Console.WriteLine("Which commodity would yo like to buy?");

            int commodity = checkvalid2();

            if (commodity == -1)
                return;
            Console.WriteLine("\nHow many?");
            int amount = checkvalid2();
            if (amount == -1)
                return;
            Console.WriteLine("\nEnter your price");
            int price = checkvalid2();
            if (price == -1)
                return;

            pc.SendBuyRequest(price , commodity , amount );
        }
        //if the client want to cancel commodity
        private static void cancelingProcces(PipeConnection pc)
        {
            Console.WriteLine("which transaction would you like to cancel?");
            int idNum = checkvalid2();
            if (idNum == -1)
                return;

            bool ans = (pc.SendCancelBuySellRequest(idNum));
            if (ans)
                Console.WriteLine("transcation canceled succecfully");
            else
                Console.WriteLine("no such transcation");
        }
        //if the client want to sell commodity
        private static void sellingProcces(PipeConnection pc)
        {
            Console.WriteLine("Which commodity would yo like to sell?");
            int commodity = checkvalid2();
            if (commodity == -1)
                return;
            Console.WriteLine("\nHow many?");
            int amount = checkvalid2();
            if (amount == -1)
                return;
            Console.WriteLine("\nEnter your price");
            int price = checkvalid2();
            if (price == -1)
                return;

            pc.SendBuyRequest(price, commodity, amount);
        }
        public static bool checkvalid(string s1)
        {

            if(s1 == "-1")
                return true;
            //   if (s1.Length > 1) return false;
            foreach (Char c in s1)
            {
                if (c < '0' | c > '9')
                    return false;
            }
            return true;
        }
        public static int checkvalid2()
        {
            try
            {
                int num = int.Parse(Console.ReadLine());
                return num;
            }
            catch(Exception e)
            {
                Console.WriteLine("please enter valid input");
                return -1;
            }
            
            
        }

    }//class

}

