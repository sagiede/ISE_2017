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
using System.Timers;
using System.IO;
using log4net.Appender;
using Pilots;
using System.Data.SqlClient;

using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Collections.ObjectModel;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using iTextSharp.text.pdf;
using iTextSharp.text;

namespace gui
{
    // test sagi new branch
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary> 
    public class MyViewModel
    {
        public ObservableDataSource<Point> Data { get; set; }

        public MyViewModel()
        {
            Data = new ObservableDataSource<Point>();
        }
    }

    public partial class MainWindow : Window
    {
        private static Boolean isVisible = true; // cahnge when press auto pilot
        private static Timer timerPliot = new Timer(1053); // timer of auto pilot
        private static Timer timerSemiPliot = new Timer(1053); // timer of auto pilot
        private static Timer theTimeNow = new Timer(1000); // timer of show time
        private static int timerNumber;
        public MyViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();
            theTimeNow.Enabled = true; // show the time
            theTimeNow.Elapsed += HandleTimerElapsedTime;
            timerPliot.Enabled = false;  // timer of pilot is off now                    
            timerPliot.Elapsed += HandleTimerElapsed;
            timerSemiPliot.Enabled = false;  // timer of pilot is off now                    
            timerSemiPliot.Elapsed += HandleTimerElapsedSemiPilot;
            //dataGrid.ItemsSource = LogicLayer.History.getLastDayCommodityHistoryOrderdByPrice(4);
            InitializeComponent();

            viewModel = new MyViewModel();
            DataContext = viewModel;
        }
        //the action of semi pilot
        private void HandleTimerElapsedSemiPilot(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                if (!(Pilots.SemiPilot.semiPilotTimer.Enabled))
                {
                    returnAllForSemiBuy();
                    returnAllForSemiSell();
                    semiStartBuy.Visibility = System.Windows.Visibility.Visible;
                    semiStopBuy.Visibility = System.Windows.Visibility.Hidden;
                    semiStartSell.Visibility = System.Windows.Visibility.Visible;
                    semiStopSell.Visibility = System.Windows.Visibility.Hidden;
                    timerSemiPliot.Stop();
                }
                output.Text = Pilots.SemiPilot.eventsData;
            });

        }
        //action of auto Pilot Button Click
        private void autoPilotButton_Click(object sender, RoutedEventArgs e)
        {
            //change
            //sound
            output.Text = "";
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(); //media for pilot
            player.SoundLocation = "Money.wav";
            player.Load();

            if (isVisible) // first click
            {
                timerNumber = 0;
                ((Button)sender).Background = Brushes.Red;
                ((Button)sender).Content = "Stop";
                isVisible = false;
                timerPliot.Enabled = true;
                tabControl.Visibility = System.Windows.Visibility.Hidden;
                moneypic.Visibility = System.Windows.Visibility.Visible;
                player.Play();

            }
            else // seconed click
            {
                timerPliot.Enabled = false;
                moneypic.Visibility = System.Windows.Visibility.Hidden;
                player.Stop();
                isVisible = true;
                tabControl.Visibility = System.Windows.Visibility.Visible;
                var brush = new ImageBrush();
                brush.ImageSource = new BitmapImage(new Uri("pline.jpg", UriKind.Relative));
                ((Button)sender).Background = brush;
                ((Button)sender).Content = "Auto-pilot";
            }
            try
            {
                Pilots.AutoPilot.runPilot();
            }
            catch (Exception e2)
            {
                output.Text = e2.Message;
                var brush = new ImageBrush();
                brush.ImageSource = new BitmapImage(new Uri("pline.jpg", UriKind.Relative));
                ((Button)sender).Background = brush;
                ((Button)sender).Content = "Auto-pilot";
                tabControl.Visibility = System.Windows.Visibility.Visible;
                moneypic.Visibility = System.Windows.Visibility.Hidden;
            }
        }
        //timer of clock event
        public void HandleTimerElapsedTime(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {

                time.Text = DateTime.Now.ToString("HH:mm:ss");
            });

        }

        //timer of auto pilot event
        public void HandleTimerElapsed(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                if(timerNumber == 1)
                    output.Text = Pilots.NewAutoPilot.actions;
                else
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

            output.Text = "";
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
                output.Text = "the buy request for commodity: " + commodity + " in price: " + price + " amount: " + amount + " id: " + response.ToString() + " sent";
            }
            catch (Exception e2)
            {
                output.Text = e2.Message;
            }
        }
        //the action of sell button
        private void SellButton_Click(object sender, RoutedEventArgs e)
        {
            output.Text = "";
            {
                int commodity = -1;
                int price = -1;
                int amount = -1;
                try
                {

                    commodity = int.Parse(commodityT.Text);
                    price = int.Parse(priceT.Text);
                    amount = int.Parse(amountT.Text);
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
                    output.Text = "the sell request for commodity: " + commodity + " in price: " + price + " amount: " + amount + " id: " + response.ToString() + " sent";
                }
                catch (Exception e3)
                {
                    output.Text = e3.Message;
                }
            }
        }
        // cancel button
        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            output.Text = "";
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
                    if (response == true)
                        output.Text = "cancel for commodity " + commodity + " is done";
                }
                catch (Exception e1)
                {
                    output.Text = e1.Message;
                }
            }
        }
        // user query  radio button
        private void userQRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            output.Text = "";
            clearAllQueris();
            exportB.Visibility = System.Windows.Visibility.Visible;
            userQButton.Visibility = System.Windows.Visibility.Visible;


        }
        //clear from window all the irelevant elements in the queris
        private void clearAllQueris()
        {
            exportB.Visibility = System.Windows.Visibility.Hidden;
            marketQText.Visibility = System.Windows.Visibility.Hidden;
            buySellQText.Visibility = System.Windows.Visibility.Hidden;
            labelId1.Visibility = System.Windows.Visibility.Hidden;
            labeId2.Visibility = System.Windows.Visibility.Hidden;
            MarketQButton.Visibility = System.Windows.Visibility.Hidden;
            userQButton.Visibility = System.Windows.Visibility.Hidden;
            BuySellQButton.Visibility = System.Windows.Visibility.Hidden;
        }
        //market query  radio button
        private void MarketQButton_Click(object sender, RoutedEventArgs e)
        {
            output.Text = "";
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
        //user query  radio button
        private void marketQRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            clearAllQueris();
            marketQText.Visibility = System.Windows.Visibility.Visible;
            MarketQButton.Visibility = System.Windows.Visibility.Visible;
            labeId2.Visibility = System.Windows.Visibility.Visible;
        }
        // buy sell query button
        private void buySellQRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            clearAllQueris();
            buySellQText.Visibility = System.Windows.Visibility.Visible;
            BuySellQButton.Visibility = System.Windows.Visibility.Visible;
            labelId1.Visibility = System.Windows.Visibility.Visible;
        }
        // user query button
        private void userQButton_Click(object sender, RoutedEventArgs e)
        {
            output.Text = "";
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
        //buy sell query button
        private void BuySellQButton_Click(object sender, RoutedEventArgs e)
        {
            output.Text = "";
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

        // semi-pilot buy start
        private void semiPilotSubmmitBuy(object sender, RoutedEventArgs e)
        {
            output.Text = "";
            semiStartBuy.Visibility = System.Windows.Visibility.Hidden;
            semiStopBuy.Visibility = System.Windows.Visibility.Visible;
            {
                int commodity = -1;
                int price = -1;
                int amount = -1;
                try
                {

                    commodity = int.Parse(commoditySPTextBuy.Text);
                    price = int.Parse(priceSPTextBuy.Text);
                    amount = int.Parse(amountSPTextBuy.Text);

                }
                catch (Exception)
                {
                    semiStartBuy.Visibility = System.Windows.Visibility.Visible;
                    semiStopBuy.Visibility = System.Windows.Visibility.Hidden;
                    output.Text = "invalid input";
                    return;
                }
                try
                {
                    clearAllForSemiBuy();
                    Pilots.SemiPilot.runSemiPilot(commodity, price, amount, true);
                    timerSemiPliot.Start();
                }
                catch (Exception e3)
                {
                    timerSemiPliot.Stop();
                    returnAllForSemiBuy();
                    semiStartBuy.Visibility = System.Windows.Visibility.Visible;
                    semiStopBuy.Visibility = System.Windows.Visibility.Hidden;
                    output.Text = e3.Message;
                    return;
                }
            }
        }
        // click stop semi pilot
        private void semiStopBuy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                timerSemiPliot.Stop();
                Pilots.SemiPilot.stopSemiPilot();
                returnAllForSemiBuy();
                semiStartBuy.Visibility = System.Windows.Visibility.Visible;
                semiStopBuy.Visibility = System.Windows.Visibility.Hidden;

            }
            catch (Exception e2)
            {
                returnAllForSemiBuy();
                timerSemiPliot.Stop();
                output.Text = e2.Message;
                return;
            }
        }
        //clear all windows when semi pilot start 
        public void clearAllForSemiBuy()
        {
            queries.Visibility = System.Windows.Visibility.Hidden;
            cancel.Visibility = System.Windows.Visibility.Hidden;
            buy.Visibility = System.Windows.Visibility.Hidden;
            history.Visibility = System.Windows.Visibility.Hidden;
            SemiAutoSell.Visibility = System.Windows.Visibility.Hidden;
            sell.Visibility = System.Windows.Visibility.Hidden;
        }
        // return items vissible
        public void returnAllForSemiBuy()
        {
            queries.Visibility = System.Windows.Visibility.Visible;
            cancel.Visibility = System.Windows.Visibility.Visible;
            buy.Visibility = System.Windows.Visibility.Visible;
            history.Visibility = System.Windows.Visibility.Visible;
            SemiAutoSell.Visibility = System.Windows.Visibility.Visible;
            sell.Visibility = System.Windows.Visibility.Visible;
        }
        // return items to hidden
        public void clearAllForSemiSell()
        {
            queries.Visibility = System.Windows.Visibility.Hidden;
            cancel.Visibility = System.Windows.Visibility.Hidden;
            buy.Visibility = System.Windows.Visibility.Hidden;
            history.Visibility = System.Windows.Visibility.Hidden;
            SemiAutoBuy.Visibility = System.Windows.Visibility.Hidden;
            sell.Visibility = System.Windows.Visibility.Hidden;
        }
        // return items vissible
        public void returnAllForSemiSell()
        {
            queries.Visibility = System.Windows.Visibility.Visible;
            cancel.Visibility = System.Windows.Visibility.Visible;
            buy.Visibility = System.Windows.Visibility.Visible;
            history.Visibility = System.Windows.Visibility.Visible;
            SemiAutoBuy.Visibility = System.Windows.Visibility.Visible;
            sell.Visibility = System.Windows.Visibility.Visible;
        }
        //buy history radio button
        private void buyHistoryRadio_Checked(object sender, RoutedEventArgs e)
        {
            output.Text = "";
            LogicLayer.MarketClientConnection mc1 = new LogicLayer.MarketClientConnection();
            dataGrid.ItemsSource = mc1.getBuyHistory();
        }
        //sell history radio button
        private void sellHistoryRadio_Checked(object sender, RoutedEventArgs e)
        {
            output.Text = "";
            LogicLayer.MarketClientConnection mc1 = new LogicLayer.MarketClientConnection();
            dataGrid.ItemsSource = mc1.getSellHistory();
        }
        //cancel history radio button
        private void cancelHistoryRadio_Checked(object sender, RoutedEventArgs e)
        {
            dataGrid.ItemsSource = "";
            output.Text = File.ReadAllText("cancelLog.log");
        }

        //semi pilot sell start
        private void semiStartSell_Click(object sender, RoutedEventArgs e)
        {
            output.Text = "";
            semiStartSell.Visibility = System.Windows.Visibility.Hidden;
            semiStopSell.Visibility = System.Windows.Visibility.Visible;
            {
                int commodity = -1;
                int price = -1;
                int amount = -1;
                try
                {

                    commodity = int.Parse(commoditySPTextSell.Text);
                    price = int.Parse(priceSPTextSell.Text);
                    amount = int.Parse(amountSPTextSell.Text);

                }
                catch (Exception)
                {
                    semiStartSell.Visibility = System.Windows.Visibility.Visible;
                    semiStopSell.Visibility = System.Windows.Visibility.Hidden;
                    output.Text = "invalid input";
                    return;
                }
                try
                {
                    timerSemiPliot.Start();
                    clearAllForSemiSell();
                    Pilots.SemiPilot.runSemiPilot(commodity, price, amount, false);
                }
                catch (Exception e3)
                {
                    timerSemiPliot.Stop();
                    returnAllForSemiSell();
                    semiStartBuy.Visibility = System.Windows.Visibility.Visible;
                    semiStopBuy.Visibility = System.Windows.Visibility.Hidden;
                    output.Text = e3.Message;
                    return;
                }
            }
        }
        // semi pilot stop sell
        private void semiStopSell_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                timerSemiPliot.Stop();
                Pilots.SemiPilot.stopSemiPilot();
                returnAllForSemiSell();
                semiStartSell.Visibility = System.Windows.Visibility.Visible;
                semiStopSell.Visibility = System.Windows.Visibility.Hidden;

            }
            catch (Exception e2)
            {
                returnAllForSemiSell();
                timerSemiPliot.Stop();
                output.Text = e2.Message;
                return;
            }
        }

        private void historyByDateB_Click(object sender, RoutedEventArgs e)
        {
            output.Text = "";
            DateTime start = DateTime.Parse(dateStart.Text);
            DateTime end = DateTime.Parse(dateEnd.Text);
            LogicLayer.MarketClientConnection mc1 = new LogicLayer.MarketClientConnection();
            historyDateDataGrid.ItemsSource = mc1.getBuyHistoryByDate(start, end);
        }

        private void SellhistoryByDateB_Click(object sender, RoutedEventArgs e)
        {
            output.Text = "";
            try
            {
                DateTime start = DateTime.Parse(dateStartS.Text);
                DateTime end = DateTime.Parse(dateEndS.Text);

                LogicLayer.MarketClientConnection mc1 = new LogicLayer.MarketClientConnection();
                SellhistoryDateDataGrid.ItemsSource = mc1.getSellHistoryByDate(start, end);

            }
            catch (Exception e2)
            {
                output.Text = "please select date";
            }
        }

        private void chartClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                InitializeComponent();
                output.Text = "";
                viewModel = new MyViewModel();
                DataContext = viewModel;
                int commodityNum = int.Parse(commodityForGraph.Text);
                int index = comboBox.SelectedIndex;
                output.Text += index + " " + commodityForGraph.Text;
                IQueryable<float> a = LogicLayer.History.getCommodityHistoryOrderedByDate(commodityNum, index);
                int pointsAmount = a.Count();
                int modulu = 1;
                if (pointsAmount > 50 & pointsAmount < 100)
                    modulu = 2;
                if (pointsAmount > 100 & pointsAmount < 300)
                    modulu = 5;
                if (pointsAmount > 300 & pointsAmount < 800)
                    modulu = 10;
                if (pointsAmount > 800 & pointsAmount < 3000)
                    modulu = 20;
                if (pointsAmount > 3000)
                    modulu = 50;
                int i = 0;
                foreach (float b in a)
                    if (i++ % modulu == 0)
                        viewModel.Data.Collection.Add(new Point(i, b));
            }
            catch (Exception e4)
            {
                output.Text = e4.Message;
            }


        }

        private void button_Click(object sender, RoutedEventArgs e)
        {

            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "Document"; // Default file name
            dlg.DefaultExt = ".pdf"; // Default file extension
            dlg.Filter = "PDF documents (.pdf)|*.pdf"; // Filter files by extension 

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results 
            if (result == true)
            {
                // Open document
                string res = dlg.FileName;

                var doc1 = new Document();
                PdfWriter.GetInstance(doc1, new FileStream(res, FileMode.Create));
                try
                {
                    LogicLayer.MarketClientConnection mc = new LogicLayer.MarketClientConnection();
                    MarketItems.MarketUserData response = (MarketUserData)mc.SendQueryUserRequest();
                    doc1.Open();
                    var f = FontFactory.GetFont("David", 11, Font.BOLD);
                    doc1.Add(new iTextSharp.text.Paragraph(response.ToString(), f));
                    doc1.Close();
                }
                catch (Exception e1)
                {
                    output.Text = e1.Message;
                }


            }
        }

        private void newAutoPilot_clicked(object sender, RoutedEventArgs e)
        {
            output.Text = "";
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(); //media for pilot
            player.SoundLocation = "Money.wav";
            player.Load();

            if (isVisible) // first click
            {
                timerNumber = 1;
                ((Button)sender).Background = Brushes.Red;
                ((Button)sender).Content = "Stop";
                isVisible = false;
                timerPliot.Enabled = true;
                tabControl.Visibility = System.Windows.Visibility.Hidden;
                moneypic.Visibility = System.Windows.Visibility.Visible;
                player.Play();

            }
            else // seconed click
            {
                timerPliot.Enabled = false;
                moneypic.Visibility = System.Windows.Visibility.Hidden;
                player.Stop();
                isVisible = true;
                tabControl.Visibility = System.Windows.Visibility.Visible;
                var brush = new ImageBrush();
                brush.ImageSource = new BitmapImage(new Uri("pline.jpg", UriKind.Relative));
                ((Button)sender).Background = brush;
                ((Button)sender).Content = "Risk-pilot";
            }
            try
            {
                Pilots.NewAutoPilot.runPilot();
            }
            catch (Exception e2)
            {
                output.Text = e2.Message;
                var brush = new ImageBrush();
                brush.ImageSource = new BitmapImage(new Uri("pline.jpg", UriKind.Relative));
                ((Button)sender).Background = brush;
                ((Button)sender).Content = "Auto-pilot";
                tabControl.Visibility = System.Windows.Visibility.Visible;
                moneypic.Visibility = System.Windows.Visibility.Hidden;
            }
        }
    }
}