﻿using System;
using WpfRecon.Models;


namespace WpfRecon.ViewModels
{
    class MainPageVM
    {
        public event EventHandler ScanComplete;

        public static string NmapScanResults;
        public ScanResult ScanResult { get; private set; }

        //Dispays the output of the ping to the mainpage textblock called output.txt
        public string DisplayOutput(string IPaddress)
        {
            //return the results of the ping scan to the mainpge view
            LiveHost liveHost = new LiveHost();
            ScanResult = liveHost.PingSweep(IPaddress);

            return ScanResult.ToString();
        }
        //if the ping is succsessful then run an nmap scan and display the resutls in the results page 
        public string LoadNmapScanInBackground(Action completionCallback )
        {
            if (State.SuccessfulPing)
            {
                var DisplayNmap = new NMapScan();
                NmapScanResults = DisplayNmap.RunScan(State.IPAddress).Result;
                //completionCallback();
                ScanComplete.Invoke(this, null);
                return ScanResult.ToString();
            }

            else
            {
                return "Ping unsuccessful";

            }
        }

        
    }
}

