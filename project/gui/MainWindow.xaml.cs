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

namespace gui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly log4net.ILog cancelLogger = log4net.LogManager.GetLogger("cancelLogger");
        private static readonly log4net.ILog sellingLogger = log4net.LogManager.GetLogger("sellingLogger");
        private static readonly log4net.ILog buyingLogger = log4net.LogManager.GetLogger("buyingLogger");
        private static readonly log4net.ILog mainLog = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public MainWindow()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
           // string str = buyButton.Visibility();
         //   tabControl.Visibility = System.Windows.Visibility.!(tabControl);
            try
            {
                LogicLayer.AutoPilot.runPilot();
            }
            catch(Exception e2)
            {
                output.Text = e2.Message;
            }
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }


        private void amountBuy_TextChanged(object sender, TextChangedEventArgs e)
        {

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

        private void button1_Click(object sender, RoutedEventArgs e)
        {

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

        private void radioButton_Checked(object sender, RoutedEventArgs e)
        {
            clearAll();
            userQButton.Visibility = System.Windows.Visibility.Visible;


        }
        private void clearAll()
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

        private void marketQText_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void textBox1_Copy_TextChanged(object sender, TextChangedEventArgs e)
        {

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
            clearAll();
            marketQText.Visibility = System.Windows.Visibility.Visible;
            MarketQButton.Visibility = System.Windows.Visibility.Visible;
            labeId2.Visibility = System.Windows.Visibility.Visible;


        }

        private void buySellQRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            clearAll();
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
            clearAll();
            cancelAllCommit.Visibility = System.Windows.Visibility.Visible;

        }
    }
}

