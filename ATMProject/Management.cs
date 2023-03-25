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

        public RichTextBox rTxtAccessLog = new RichTextBox();

        public Account[] ac = new Account[3];

        public Label[] lblAccount = new Label[3];

        public bool raceCondition = false;


        /* public static Thread ThreadStart(Action method) // adapted from https://stackoverflow.com/questions/10313185/dynamically-create-thread
         {
             Thread t = new Thread(new ThreadStart(method));
             t.Start();
             return method;
         }*/

        public Management()
        {
            InitializeComponent();
            ac[0] = new Account(300, 1111, 111111);
            ac[1] = new Account(750, 2222, 222222);
            ac[2] = new Account(3000, 3333, 333333);
            instance = this;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            drawManagementScreen();
            rTxtAccessLog.Text = "Race Condition Disabled!\n";
        }

        public void drawManagementScreen()
        {
            for(int i = 0; i < 3; i++)
            {
                lblAccount[i] = new Label();
                lblAccount[i].Text = "Account " + Convert.ToString(ac[i].getAccountNum()) + " currently has £"  + Convert.ToString(ac[i].getBalance() + ".");

                lblAccount[i].SetBounds(this.Width/2 - 100, 10 + (20 * (i + 1)), 300, 20);
                Controls.Add(lblAccount[i]);
            }
            rTxtAccessLog.SetBounds(10, 100, this.Width - 35, this.Height - 150);
            rTxtAccessLog.ReadOnly = true;
            Controls.Add(rTxtAccessLog);
        }

        private void stripBtnCreateATM_Click(object sender, EventArgs e)
        {
            ATM atm = new ATM(ac);
            Thread newThread = new Thread(() => createATM(atm));
            // createATM(atm);
            newThread.Start();
        }

        private void createATM(ATM atm)
        {
            atm.Size = new Size(900, 600);
            atm.ShowDialog();
        }
        private void stripOptions_Click(object sender, EventArgs e)
        {

        }

        private void stripExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void toggleConditionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            if(raceCondition == false)
            {
                rTxtAccessLog.Text += "Race Condition Enabled!\n";
                raceCondition = true;
            }
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
