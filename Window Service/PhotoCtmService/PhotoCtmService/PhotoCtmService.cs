using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;


namespace PhotoCtmService
{
    public partial class PhotoCtmService : ServiceBase
    {
        Thread m_thread = null;
       
        public PhotoCtmService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {

            //#if DEBUG
            //while (!Debugger.IsAttached)      // Waiting until debugger is attached
            //{
            //    RequestAdditionalTime(1000);  // Prevents the service from timeout
            //    Thread.Sleep(1000);           // Gives you time to attach the debugger   
            //}
            //RequestAdditionalTime(20000);     // for Debugging the OnStart method,
            //// increase as needed to prevent timeouts
            //#endif
            
            
              m_thread = new Thread(new ThreadStart(ThreadProc));
            // start the thread
            m_thread.Start();
           
        }

        protected override void OnStop()
        {
       
        }

        public void ThreadProc()
        {
            // we're going to wait 2 minutes between calls to Download Images, so 
            // set the interval to 120000 milliseconds 

            int interval = 100000; // 2 minutes    
            // this variable tracks how many milliseconds have gone by since 
            // the last call to download image. Set it to zero to indicate we're 
            // starting fresh
            int elapsed = 0;
            // because we don't want to use 100% of the CPU, we will be 
            // sleeping for 1 second between checks to see if it's time to 
            // call donwload image
            int waitTime = 1000; // 1 second
            try
            {
                // do this loop forever (or until the service is stopped)
                while (true)
                {
                    // if enough time has passed
                    if (interval >= elapsed)
                    {
                        // reset how much time has passed to zero
                        elapsed = 0;
                        // call DownloadImages()
                        PhotoCTMDownloader objDownloader=new PhotoCTMDownloader();
                        objDownloader.DownloadImages();
                    }
                    // Sleep for 1 second
                    Thread.Sleep(waitTime);
                    // indicate that 1 additional second has passed
                    elapsed += waitTime;
                }
            }
            catch (ThreadAbortException)
            {
                // we want to eat the excetion because we don't care if the 
                // thread has aborted since we probably did it on purpose by 
                // stopping the service.
            }
        }
    }
}
