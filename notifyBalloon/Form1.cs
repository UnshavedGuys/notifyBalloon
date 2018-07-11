using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace notifyBalloon
{
    public partial class Form1 : Form
    {
        NotifyIcon notifyIcon;
        string iconPath = "Resources\\ucb.ico";
        //string iconPath = Properties.Resources.ucb;
        string HeaderText;
        string MessageText;
        private EventLog eventLog1;
        
        public Form1()
        {
            InitializeComponent();
            string eventSourceName = "ServiceDesk App";
            string logName = "SDLog";
            eventLog1 = new EventLog();
            if (!EventLog.SourceExists(eventSourceName))
            {
                EventLog.CreateEventSource(eventSourceName, logName);
            }
            eventLog1.Source = eventSourceName;
            eventLog1.Log = logName;
            ShowTrayIcon();
        }

        private void ShowTrayIcon()
        {
            Container bpcomponents = new Container();
            ContextMenu contextmenu1 = new ContextMenu();
            MenuItem runMenu = new MenuItem
            {
                Index = 1,
                Text = "Повторить уведомление"
            };
            runMenu.Click += new EventHandler(RunMenu_Click);
            contextmenu1.MenuItems.AddRange(
                new MenuItem[] {
                    runMenu
                }
                );
            notifyIcon = new NotifyIcon(bpcomponents)
            {
                Icon = new Icon(iconPath),
                ContextMenu = contextmenu1,
                Visible = true,
                Text = "ServiceDesk App"
            };
            eventLog1.WriteEntry("ServiceDesk App started", EventLogEntryType.Information, 1);
            // Set up a timer to trigger every time.  
            System.Timers.Timer timer = new System.Timers.Timer
            {
                Interval = 10000 // 10 seconds  
            };
            timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer);
            timer.Start();
        }

        public void GetEvent()
        {
            HeaderText = "ServiceDesk Message";
            EventLogEntryCollection myLogEntryCollection = eventLog1.Entries;
            int myCount = myLogEntryCollection.Count;
            for(int i = myCount-1;i>0;i--)
            {
                EventLogEntry myLogEntry = myLogEntryCollection[i];
                if (myLogEntry.InstanceId.Equals(13) && myLogEntry.TimeWritten > DateTime.Now.AddHours(-24))
                {
                    //Console.WriteLine(myLogEntry.Source + " was the source of last event of id " + myLogEntry.InstanceId);
                    Console.WriteLine(myLogEntry.Message);
                    MessageText = "Нам очень жаль, но ваш компьютер требует перезагрузки." + myLogEntry.TimeWritten;
                    notifyIcon.ShowBalloonTip(10000, HeaderText, MessageText, ToolTipIcon.None);
                    return;
                }
            }
        }

        void RunMenu_Click(object sender, EventArgs e)
        {
            eventLog1.WriteEntry("context menu click", EventLogEntryType.Information, 10);
            GetEvent();

        }

        public void OnTimer(object sender, System.Timers.ElapsedEventArgs args)
        {
            eventLog1.WriteEntry("Monitoring the System", EventLogEntryType.Information, 2);
            GetEvent();
        }
    }
}
