using System.Diagnostics;
using System.Timers;

namespace AutoLock;
using Timer = System.Timers.Timer;

public partial class Form1 : Form
{
    private Timer alertTimer;
    private Timer waitTimer;
    private bool dialogOpen = false;
    private Timer timeoutTimer;
    
    public Form1()
    {
        InitializeComponent();
        InitializeAlertTimer();
    }
    
    private void InitializeAlertTimer()
    {
        alertTimer = new Timer(); 
        alertTimer.Interval = 30000; // 30 seconds
        alertTimer.Elapsed += AlertTimer_Tick;
        alertTimer.Start();
    }

    private void AlertTimer_Tick(object sender, EventArgs e)
    {
        if (!dialogOpen)
        {
            // Use Invoke to ensure UI operations happen on the UI thread
            this.Invoke((MethodInvoker)delegate
            {
                ShowYesNoAlert();
            });
        }
    }

    private void ShowYesNoAlert()
    {
        if (dialogOpen) return;
        
        dialogOpen = true;
        this.Hide();
        
        // Initialize timeout timer
        if (timeoutTimer != null)
        {
            timeoutTimer.Stop();
            timeoutTimer.Dispose();
        }
        
        timeoutTimer = new Timer();
        timeoutTimer.Interval = 30000; // 30 seconds
        timeoutTimer.Elapsed += TimeoutTimer_Tick;
        timeoutTimer.Start();
        
        var result = MessageBox.Show(
            "Are you still there?",
            "Confirmation",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        timeoutTimer.Stop();
        timeoutTimer.Dispose();
        
        if (result == DialogResult.Yes)
        {
            var wait = MessageBox.Show(
                "Do you want to wait 15 minutes?",
                "Confirmation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (wait == DialogResult.Yes)
            {
                alertTimer.Stop(); // Stop the 30s timer
                StartWaitTimer();
            }
        }
        else
        {
            Process.Start("rundll32.exe", "user32.dll,LockWorkStation");
        }
        
        dialogOpen = false;
    }

    private void StartWaitTimer()
    {
        if (waitTimer != null)
        {
            waitTimer.Stop();
            waitTimer.Dispose();
        }
        
        waitTimer = new Timer();
        waitTimer.Interval = 900000; // 15 minutes in milliseconds
        waitTimer.Elapsed += WaitTimer_Tick;
        waitTimer.Start();
    }

    private void WaitTimer_Tick(object sender, EventArgs e)
    {
        waitTimer.Stop();
        alertTimer.Start(); // Resume the 30s timer
    }

    private void TimeoutTimer_Tick(object sender, EventArgs e)
    {
        this.Invoke((MethodInvoker)delegate
        {
            dialogOpen = false;
            Process.Start("rundll32.exe", "user32.dll,LockWorkStation");
        });
    }
}
