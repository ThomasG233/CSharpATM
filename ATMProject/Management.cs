// Team 3: Alba Henderson, Thomas Laland-Brown, Thomas Gourlay 
using ATMProject;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ATMProject
{
    public partial class Management : Form
    {

        public static Management instance;
        // Log for bank computer.
        public RichTextBox rTxtAccessLog = new RichTextBox();

        public Account[] ac = new Account[3];
        // Labels displaying account number.
        public Label[] lblAccount = new Label[3];
        public bool raceCondition = false;

        // Form constructor.
        public Management()
        {
            InitializeComponent();
            // Creates accounts stored by bank.
            ac[0] = new Account(300, 1111, 111111);
            ac[1] = new Account(750, 2222, 222222);
            ac[2] = new Account(3000, 3333, 333333);
            instance = this;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            drawManagementScreen();
            // Adds current status of Race Condition.
            rTxtAccessLog.Text = "Race Condition Disabled!\n";
        }
        // Draw the UI.
        public void drawManagementScreen()
        {
            // For each account in the Bank
            for(int i = 0; i < 3; i++)
            {
                // Display their account number and balance.
                lblAccount[i] = new Label();
                lblAccount[i].Text = "Account " + Convert.ToString(ac[i].getAccountNum()) + " currently has £"  + Convert.ToString(ac[i].getBalance() + ".");

                lblAccount[i].SetBounds(this.Width/2 - 100, 10 + (20 * (i + 1)), 300, 20);
                Controls.Add(lblAccount[i]);
            }
            // Log for all activity in accounts.
            rTxtAccessLog.SetBounds(10, 100, this.Width - 35, this.Height - 150);
            rTxtAccessLog.ReadOnly = true;
            Controls.Add(rTxtAccessLog);
        }
        // Strip-menu option to create an ATM form.
        private void stripBtnCreateATM_Click(object sender, EventArgs e)
        {
            // Create an ATM, and run it on a thread seperate from Management.
            ATM atm = new ATM(ac);
            // Accounts passed into this ATM.
            Thread newThread = new Thread(() => createATM(atm));
            newThread.Start();
        }
        // Declares a new ATM.
        private void createATM(ATM atm)
        {
            // Display a new ATM.
            atm.Size = new Size(900, 600);
            atm.ShowDialog();
        }

        private void stripExit_Click(object sender, EventArgs e)
        {
            // Closes ALL windows, rather than just the Bank PC.
            Environment.Exit(0);
        }

        // Option to turn on/off the Race Condition.
        private void toggleConditionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Race condition turned on if it's currently off.
            if(raceCondition == false)
            {
                rTxtAccessLog.Text += "Race Condition Enabled!\n";
                raceCondition = true;
            }
            // Race condition turned off if it's currently on.
            else
            {
                rTxtAccessLog.Text += "Race Condition Disabled!\n";
                raceCondition = false;
            }

        }
    }
    // ACCOUNT CLASS.
    public class Account
    {
        //the attributes for the account
        private int balance;
        private int pin;
        private int accountNum;
        // Allows access to account balances one at a time.
        public Mutex requests;

        // a constructor that takes initial values for each of the attributes (balance, pin, accountNumber)
        public Account(int balance, int pin, int accountNum)
        {
            this.balance = balance;
            this.pin = pin;
            this.accountNum = accountNum;
            this.requests = new Mutex();
        }

        //getter and setter functions for balance
        public int getBalance()
        {
            return balance;
        }
        public void setBalance(int newBalance)
        {
            this.balance = newBalance;
        }

        /*
         *   This funciton allows us to decrement the balance of an account
         *   it perfomes a simple check to ensure the balance is greater tha
         *   the amount being debeted
         *   
         *   reurns:
         *   true if the transactions if possible
         *   false if there are insufficent funds in the account
         */
        public Boolean decrementBalance(int amount)
        {
            if (this.balance > amount)
            {
                balance -= amount;
                return true;
            }
            else
            {
                return false;
            }
        }

        /*
         * This funciton check the account pin against the argument passed to it
         *
         * returns:
         * true if they match
         * false if they do not
         */
        public Boolean checkPin(int pinEntered)
        {
            if (pinEntered == pin)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public int getAccountNum()
        {
            return accountNum;
        }
    }
}
