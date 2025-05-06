using System.Diagnostics;
using System.Timers;

namespace AutoLock;
using Timer = System.Timers.Timer;

public partial class Form1 : Form
{
    private Timer alertTimer;
    private Timer waitTimer;
    public Form1()
    {
        InitializeComponent();
        this.Hide(); 
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
        ShowYesNoAlert();
    }

    private void ShowYesNoAlert()
    {
        this.Hide();
        var timeoutTimer = new Timer();
        timeoutTimer.Interval = 30000; // 30 seconds
        timeoutTimer.Elapsed += TimeoutTimer_Tick;
        timeoutTimer.Start();
        var result = MessageBox.Show(
            "Are you still there?",
            "Confirmation",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (result == DialogResult.Yes)
        {
            timeoutTimer.Stop();
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
    }

    private void StartWaitTimer()
    {
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
        Process.Start("rundll32.exe", "user32.dll,LockWorkStation");
    }
}
