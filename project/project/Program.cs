using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicLayer;
using MarketClient;
using MarketItems;

namespace project
{
    class Program
    {

        private static log4net.ILog mainLog = log4net.LogManager.GetLogger("mainLog");

        static void Main(string[] args)
        {
            // Pilots.AutoPilot.runPilot();
            LogicLayer.MarketClientConnection mc = new LogicLayer.MarketClientConnection(); // response depend the input
            String a = mc.SendQueryUserRequestsRequest();
            //foreach (var tmp in a)
            //    Console.WriteLine(tmp);
            Console.ReadLine();
            //runTrading(mc);
        }//Main
        
        private static void runTrading(MarketClientConnection mc) { 
            while (true)
            {              
                Console.WriteLine("---------------------------------------------------------------\nWelcome to Algo-Trading application. to go back to main menu, you can press -1 at any point");
                Console.WriteLine("What do you wish to do?");
                Console.WriteLine("1- Buy\n2- Sell\n3- Cancel\n4- Queries\n5- delete all open requests");
                int command = checkInputValid(); // the first choose of the client

                if (command == -1) { }
                //want to buy
                else if (command == 1)
                    buyingProcces(mc);
                // want to sell
                else if (command == 2)
                    sellingProcces(mc);
                // want to cancel
                else if (command == 3)
                    cancelingProcces(mc);
                // want to cancel all the request
                else if(command==5)
                {
                    if(mc.cancelAllRequests()==true)
                    {
                        Console.WriteLine("all your asks are canceled");
                    }
                    
                }
                else if (command == 4)
                {
                    Console.WriteLine("Which query would yo like to send?");
                    Console.WriteLine("1- Buy/Sell status\n2- User status\n3- Market Status");
                    command = checkInputValid();
                    // want go back
                    if (command == -1) { }
                    //Buy/Sell status
                    else if (command == 1)
                    {
                        Console.WriteLine("please enter the transaction ID");
                        int id = checkInputValid();
                        if (id != -1)
                            Console.WriteLine(mc.SendQueryBuySellRequest(id));
                        //user status
                    }
                    else if (command == 2)
                        Console.WriteLine(mc.SendQueryUserRequest());
                    //Market Status
                    else if (command == 3)
                    {
                        Console.WriteLine("Please enter the stock number you wish to ask about");
                        int commodity = checkInputValid();
                        if (commodity != -1)
                            Console.WriteLine(mc.SendQueryMarketRequest(commodity));
                        else
                            Console.WriteLine("You have entered invaild number, please follow the instructions");
                    }
                }
                
            }//while
        }
        //if the client whant to buy commodity
        private static void buyingProcces(MarketClientConnection mc)
        { 
            Console.WriteLine("Which commodity would yo like to buy?");

            int commodity = checkInputValid();
            if (commodity == -1)
                return;
            Console.WriteLine("How many?");
            int amount = checkInputValid();
            if (amount == -1)
                return;
            Console.WriteLine("Enter your price");
            int price = checkInputValid();
            if (price == -1)
                return;
            int response = mc.SendBuyRequest(price, commodity, amount);
            if (response != -1)
                Console.WriteLine(response);
        }
        //if the client want to cancel commodity
        private static void cancelingProcces(MarketClientConnection mc)
        {
            Console.WriteLine("which transaction would you like to cancel?");
            int idNum = checkInputValid();
            if (idNum == -1)
                return;
            bool ans = (mc.SendCancelBuySellRequest(idNum));
            if (ans)
                Console.WriteLine("transcation canceled succecfully");
            
        }
        //if the client want to sell commodity
        private static void sellingProcces(MarketClientConnection mc)
        {
            Console.WriteLine("Which commodity would yo like to sell?");
            int commodity = checkInputValid();
            if (commodity == -1)
                return;
            Console.WriteLine("How many?");
            int amount = checkInputValid();
            if (amount == -1)
                return;
            Console.WriteLine("Enter your price");
            int price = checkInputValid();
            if (price == -1)
                return;
            int response = mc.SendSellRequest(price, commodity, amount);
            if (response != -1)
                Console.WriteLine(response);
        }
        //check valid of input, just integer accept
        public static int checkInputValid()
        {
            try
            {
                int num = int.Parse(Console.ReadLine());
                return num;
            }
            catch(Exception)
            {
                Console.WriteLine("please enter valid input");
                return -1;
            }   
            
        }
    }//end of Program
}

