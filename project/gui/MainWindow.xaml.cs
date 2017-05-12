using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LogicLayer;
using MarketItems;
using Pilots;
using System.Timers;
using System.IO;

namespace gui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Boolean isVisible = true; // cahnge when press auto pilot
        private static readonly log4net.ILog pilotLogger = log4net.LogManager.GetLogger("pilotLogger");
        private static readonly log4net.ILog cancelLogger = log4net.LogManager.GetLogger("cancelLogger");
        private static readonly log4net.ILog sellingLogger = log4net.LogManager.GetLogger("sellingLogger");
        private static readonly log4net.ILog buyingLogger = log4net.LogManager.GetLogger("buyingLogger");
        private static readonly log4net.ILog mainLog = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static Timer timer = new Timer(3000); // timer of auto pilot
        private static Timer theTimeNow = new Timer(10); // timer of show time
        private static int buySellChangedSemiPilot = -1; //sell=4 buy=2
        public MainWindow()
        {
            InitializeComponent();
            theTimeNow.Enabled = true; // show the time
            theTimeNow.Elapsed += HandleTimerElapsedTime; 
            timer.Enabled = false;  // timer of pilot is off now                    
            timer.Elapsed += HandleTimerElapsed;
            
        }

        private void autoPilotButton_Click(object sender, RoutedEventArgs e)
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(); //media for pilot
            player.SoundLocation = "C:\\Users\\etay2\\Desktop\\ISE17_project\\project\\gui\\Money.wav";
            player.Load();

            if (isVisible) // first click
            {
                isVisible = false;

                timer.Enabled = true;
                tabControl.Visibility = System.Windows.Visibility.Hidden;
                moneypic.Visibility= System.Windows.Visibility.Visible;
                player.Play();

            }
            else // seconed click
            {
                timer.Enabled = false;
                moneypic.Visibility = System.Windows.Visibility.Hidden;
                player.Stop();
                isVisible = true;
                tabControl.Visibility = System.Windows.Visibility.Visible;
            }
            try
            {
  
                Pilots.AutoPilot.runPilot();

            }
            catch (Exception e2)
            {
                output.Text = e2.Message;
          
            }
        }
        //timer of clock event
        public void HandleTimerElapsedTime(object sender, EventArgs e)
        { 
            Dispatcher.Invoke(() => {

                time.Text = DateTime.Now.ToString("HH:mm:ss");
           });

        }
        
        //timer of auto pilot event
        public void HandleTimerElapsed(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() => {
                
                output.Text = Pilots.AutoPilot.actions;
            });
        }
        // close all active threads when close the program
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        { 
            Environment.Exit(0);
        }

        private void buyButton_Click(object sender, RoutedEventArgs e)
        {
            int commodity = -1;
            int price = -1;
            int amount = -1;
            try
            {

                commodity = int.Parse(commodityBuy.Text);
                price = int.Parse(PriceBuy.Text);
                amount = int.Parse(amountBuy.Text);
            }
            catch (Exception)
            {
                output.Text = "invalid input";
                return;
            }
            try
            {
                LogicLayer.MarketClientConnection mc = new LogicLayer.MarketClientConnection();
                int response = mc.SendBuyRequest(price, commodity, amount);
                output.Text = response.ToString();
            }
            catch (Exception e2)
            {
                output.Text = e2.Message;
            }
        }
        private void SellButton_Click(object sender, RoutedEventArgs e)
        {
            {
                int commodity = -1;
                int price = -1;
                int amount = -1;
                try
                {

                    commodity = int.Parse(commodityBuy.Text);
                    price = int.Parse(PriceBuy.Text);
                    amount = int.Parse(amountBuy.Text);
                }
                catch (Exception)
                {
                    output.Text = "invalid input";
                    return;
                }
                try
                {
                    LogicLayer.MarketClientConnection mc = new LogicLayer.MarketClientConnection();
                    int response = mc.SendSellRequest(price, commodity, amount);
                    output.Text = response.ToString();
                }
                catch (Exception e3)
                {
                    output.Text = e3.Message;
                }
            }
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            {
                int commodity = -1;
                try
                {
                    commodity = int.Parse(cancelCommodity.Text);
                }
                catch (Exception)
                {
                    output.Text = "invalid input";
                    return;
                }
                try
                {
                    LogicLayer.MarketClientConnection mc = new LogicLayer.MarketClientConnection();
                    Boolean response = mc.SendCancelBuySellRequest(commodity);
                    output.Text = response.ToString();
                }
                catch (Exception e1)
                {
                    output.Text = e1.Message;
                }
            }
        }

        private void userQRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            clearAllQueris();
            userQButton.Visibility = System.Windows.Visibility.Visible;


        }
        //clear from window all the irelevant elements in the queris
        private void clearAllQueris()
        {
            marketQText.Visibility = System.Windows.Visibility.Hidden;
            buySellQText.Visibility = System.Windows.Visibility.Hidden;
            labelId1.Visibility = System.Windows.Visibility.Hidden;
            labeId2.Visibility = System.Windows.Visibility.Hidden;
            MarketQButton.Visibility = System.Windows.Visibility.Hidden;
            userQButton.Visibility = System.Windows.Visibility.Hidden;
            BuySellQButton.Visibility = System.Windows.Visibility.Hidden;
            cancelAllCommit.Visibility = System.Windows.Visibility.Hidden;
        }

        private void MarketQButton_Click(object sender, RoutedEventArgs e)
        {
            
                int commodity = -1;
                try
                {
                    commodity = int.Parse(marketQText.Text);
                }
                catch (Exception)
                {
                    output.Text = "invalid input";
                    return;
                }
                try
                {
                    LogicLayer.MarketClientConnection mc = new LogicLayer.MarketClientConnection();
                    MarketItems.MarketCommodityOffer response = (MarketCommodityOffer)mc.SendQueryMarketRequest(commodity);
                    output.Text = response.ToString();
                }
                catch (Exception e1)
                {
                    output.Text = e1.Message;
                }
            }

        private void marketQRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            clearAllQueris();
            marketQText.Visibility = System.Windows.Visibility.Visible;
            MarketQButton.Visibility = System.Windows.Visibility.Visible;
            labeId2.Visibility = System.Windows.Visibility.Visible;
        }

        private void buySellQRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            clearAllQueris();
            buySellQText.Visibility = System.Windows.Visibility.Visible;
            BuySellQButton.Visibility = System.Windows.Visibility.Visible;
            labelId1.Visibility = System.Windows.Visibility.Visible;
        }

        private void userQButton_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                LogicLayer.MarketClientConnection mc = new LogicLayer.MarketClientConnection();
                MarketItems.MarketUserData response = (MarketUserData)mc.SendQueryUserRequest();
                output.Text = response.ToString();
            }
            catch (Exception e1)
            {
                output.Text = e1.Message;
            }
        }

        private void BuySellQButton_Click(object sender, RoutedEventArgs e)
        {
            int id = -1;
            try
            {
                id = int.Parse(buySellQText.Text);
            }
            catch (Exception)
            {
                output.Text = "invalid input";
                return;
            }
            try
            {
                LogicLayer.MarketClientConnection mc = new LogicLayer.MarketClientConnection();
                MarketItems.MarketItemQuery response = (MarketItemQuery)mc.SendQueryBuySellRequest(id);
                output.Text = response.ToString();
            }
            catch (Exception e1)
            {
                output.Text = e1.Message;
            }
        }

        private void cancelAllCommit_Click(object sender, RoutedEventArgs e)
        {
            
            try
            {
                LogicLayer.MarketClientConnection mc = new LogicLayer.MarketClientConnection();
                Boolean response = mc.cancelAllRequests();
                output.Text = response.ToString();
            }
            catch (Exception e1)
            {
                output.Text = e1.Message;
            }
        }

        private void cancelAllButton_Checked(object sender, RoutedEventArgs e)
        {
            clearAllQueris();
            cancelAllCommit.Visibility = System.Windows.Visibility.Visible;

        }
        // semi-pilot
        private void semiPilotSubmmit(object sender, RoutedEventArgs e)
        {
            semiStart.Visibility = System.Windows.Visibility.Hidden;
            semiStop.Visibility = System.Windows.Visibility.Visible;
            {
                int commodity = -1;
                int price = -1;
                int amount = -1;
                try
                {

                    commodity = int.Parse(commoditySPText.Text);
                    price = int.Parse(priceSPText.Text);
                    amount = int.Parse(amountSPText.Text);
                    
                }
                catch (Exception)
                {
                    semiStart.Visibility = System.Windows.Visibility.Visible;
                    semiStop.Visibility = System.Windows.Visibility.Hidden;
                    output.Text = "invalid input";
                    return;
                }
                try
                {
                    if (buySellChangedSemiPilot == 2)
                    {
                        Pilots.SemiPilot p1 = new Pilots.SemiPilot(commodity, price, amount, true);
                        p1.runAlgo();
                    }
                    else if (buySellChangedSemiPilot == 4)
                    {
                        Pilots.SemiPilot p1 = new Pilots.SemiPilot(commodity, price, amount, false);
                        p1.runAlgo();
                    }

                    }
                catch (Exception e3)
                {
                    semiStart.Visibility = System.Windows.Visibility.Visible;
                    semiStop.Visibility = System.Windows.Visibility.Hidden;
                    output.Text = e3.Message;
                    return;
                }
            }
        }

        private void buyHistoryRadio_Checked(object sender, RoutedEventArgs e)
        {
            
            output.Text = File.ReadAllText("C:\\Users\\etay2\\Desktop\\ISE17_project\\project\\project\\bin\\Debug\\mainLog.log");
        }
        private void sellHistoryRadio_Checked(object sender, RoutedEventArgs e)
        {

            output.Text = File.ReadAllText("C:\\Users\\etay2\\Desktop\\ISE17_project\\project\\project\\bin\\Debug\\mainLog.log");
        }
        private void cancelHistoryRadio_Checked(object sender, RoutedEventArgs e)
        {

            output.Text = File.ReadAllText("C:\\Users\\etay2\\Desktop\\ISE17_project\\project\\project\\bin\\Debug\\mainLog.log");
        }
        // click stop semi pilot
        private void semiStop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                
                semiStart.Visibility = System.Windows.Visibility.Visible;
                semiStop.Visibility = System.Windows.Visibility.Hidden;
               
            }
            catch(Exception e2)
            {
                output.Text = e2.Message;
                return;
            }
        }

        private void sellRequestRadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}

