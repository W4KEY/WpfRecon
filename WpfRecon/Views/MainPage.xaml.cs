﻿using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using WpfRecon.Models;
using WpfRecon.ViewModels;


namespace WpfRecon
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public const bool V = false;

        //Main page view model 
        MainPageVM MPVM = new MainPageVM();

        //All parts are checked scan defimed in the nmap scan 
        public static bool AllPortCheck = false;
        //A whole network scan defined in the nmap scan
        public static bool WholeNetworkCheck = false;
        //local scan defined in the nM
        public static bool LocalNetworkCheck = false;
        

        public MainPage()
        {
            InitializeComponent();

        }

        
        //This is in here as a place holder for the IPAddress textbox. 
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
             

        }

        //This process runs the live host and the nmap scan if the ping was a success.
        private void ScanprocessReturn()
        {
            //Error checking to make sure the IP Address is in the correct format

            if( (LocalNetwork.IsChecked == false) && (CheckValidation.IsValidateIP(IpAddress.Text) != "True"))
            {
                    Output.Text = "This IP Address is not in the correct format!  Correct example 127.0.0.1";
            }
            else
            {

                //this variable is called from the nmap scan to run a scan on all 65535 ports 
                AllPortCheck = AllPorts.IsChecked.Value;

                //this variable is called from the nmap scan to run a nmap scan on all devices on a class C network
                WholeNetworkCheck = WholeNetwork.IsChecked.Value;
                //this is set to true to show generic progress and not a percentage style
                pbStatus.IsIndeterminate = true;
                //start an async progress bar output
                BackgroundWorker worker = new BackgroundWorker();
                worker.WorkerReportsProgress = true;
                worker.DoWork += worker_DoWork;

                //output the results of the View model and scan and display them in the output text block
                Output.Text = (MPVM.DisplayOutput(IpAddress.Text));


                //if the live host scan was a success then make the progress bar visable 

                if (State.SuccessfulPing)
                { //(ASCII ART FOR A BIT OF FUN) the formatting is for the center of the screen as it is a fixed size

                    Output.Text += "\n";
                    Output.Text += "\n                                 ##########################";
                    Output.Text += "\n                                 ### FULL SCAN IN PROGRESS ###";
                    Output.Text += "\n                                 ##########################";
                    Output.Text += "\n";

                    // show status bar when scan is running 
                    pbStatus.Visibility = Visibility.Visible;
                }

                MPVM.ScanComplete += ScanCompleteHandler;

                worker.RunWorkerAsync();
            }
            }
        

        //This is a keyscan on using the enter key to run the scan rather than having to click the button
        private void RichTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                ScanprocessReturn();

                e.Handled = true;
            }
        }
        //This is the click button to run the scan 
        private void Scan_Click(object sender, RoutedEventArgs e)
        {
           ScanprocessReturn();
        }

        //Automate the nmap scan to run in the background whilst the progress bar is working 
        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            MPVM.LoadNmapScanInBackground(() => { });
        }

        //Once the scan has succsessfully completed hide the progress bar and inform the user to check for the full results in the results page         
        private void ScanCompleteHandler(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke(() => {
                pbStatus.Visibility = Visibility.Hidden;
                //In case redirect is lagging an error come up informing the user to click results
                Output.Text += "\nNMap Scan Completed, Check Results Pages for details.";
                NavigationService.Navigate(new Uri("Views/Results.xaml", UriKind.Relative));
            });

        }

        //Navigation pane section 
        private void Home_Click(object sender, RoutedEventArgs e)
        {
            // View The Home page  
            NavigationService.Navigate(new Uri("Views/MainPage.xaml", UriKind.Relative));
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {

            // View The About page 
            NavigationService.Navigate(new Uri("Views/About.xaml", UriKind.Relative));
        }

        private void Results_Click(object sender, RoutedEventArgs e)
        {

            // View The Results page 
            NavigationService.Navigate(new Uri("Views/Results.xaml", UriKind.Relative));
        }

        //this is the check box on the xaml that gets called from the variable AP from the nmap scan
        public void AllPorts_Checked(object sender, RoutedEventArgs e)
        {

        }

        //this is the chack box that gets called from the WN variable from the Nmap Scan 
        private void WholeNetwork_Checked(object sender, RoutedEventArgs e)
        {

        }


        //this is the checkbox for conducting a local scan that will blank out the IP Address textbox
        private void LocalNetwork_Checked_1(object sender, RoutedEventArgs e)
        {

            if (LocalNetwork.IsChecked == true)
            {
                //Clears any input the user entered before the local scan was started. 
                IpAddress.Text = String.Empty;
                //Hides the IP Address field so the user cant enter an IP Adress (Human error)
                IpAddress.Visibility = System.Windows.Visibility.Hidden;
                LocalNetworkCheck = true;
            }
            else
            {
                //Make the field visable for use.
                IpAddress.Visibility = System.Windows.Visibility.Visible;
            }
        }
    }
}

