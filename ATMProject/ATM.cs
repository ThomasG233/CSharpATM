using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ATMProject
{
    public partial class ATM : Form
    {
        private delegate void SafeCallDelegate(string text);

        public static ATM instance;
        bool accNumberInserted = false;
        bool invalidDataPreviouslyEntered = false;
        bool onAccountScreen = false;
        bool onWithdrawalScreen = false;
        public static string currentActionLog = "";
        int accountIndex;
        // Holds reference to accounts from Management.
        Account[] ac = null;

        TextBox txtCardInputs = new TextBox();
        Label lblInstruction = new Label();
        string accountNumber = "";
        string pinNumber = "";

        Button[] btnSideMenu = new Button[5];
            
        // Basic constructor (required).
        public ATM()
        {
            InitializeComponent();
            drawATM();
            displayInitialATMScreen();
        }
        // Constructor to pass in accounts.
        public ATM(Account[] accounts)
        {
            ac = accounts;
            InitializeComponent();
            instance = this;
            drawATM();
            displayInitialATMScreen();
        }

        public void increment()
        {
            for (int i = 0; i < 10; i++)
            {
                // Form2.text = "i";
                Thread.Sleep(1000);
            }
        }
        // Draw the ATM UI.
        public void drawATM()
        {
            this.BackColor = Color.LightGray;
            Button[,] btnKeyPad = new Button[3,4];
            int i = 0;
            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 3; x++)
                {
                    btnKeyPad[x, y] = new Button();
                    i++;
                    btnKeyPad[x, y].SetBounds(650 + (75 * x), 75 + (75 * y), 75, 75);
                    btnKeyPad[x, y].Font = new Font("Arial", 24, FontStyle.Bold);
                    btnKeyPad[x, y].Text = Convert.ToString(i);
                    btnKeyPad[x,y].BackColor = Color.White;
                    if(y != 3)
                    {
                        btnKeyPad[x, y].Click += new EventHandler(BtnKeyPad_Click);
                    }
                    Controls.Add(btnKeyPad[x, y]);
                }
            }
            btnKeyPad[0, 3].Font = new Font("Arial", 12, FontStyle.Bold);
            btnKeyPad[0, 3].BackColor = Color.Red;
            btnKeyPad[0, 3].Text = "Cancel";
            btnKeyPad[0, 3].Click += new EventHandler(BtnCancel_Click);

            btnKeyPad[1, 3].Text = "0";
            btnKeyPad[1, 3].Click += new EventHandler(BtnKeyPad_Click);

            btnKeyPad[2, 3].Font = new Font("Arial", 12, FontStyle.Bold);
            btnKeyPad[2, 3].BackColor = Color.Green;
            btnKeyPad[2, 3].Text = "Enter";
            btnKeyPad[2, 3].Click += new EventHandler(BtnEnter_Click);

            int j = 0;
            for (int y = 0; y < 5; y++)
            {
                btnSideMenu[y] = new Button();
                btnSideMenu[y].SetBounds(20, 125 + (65 * j), 50, 50);
                btnSideMenu[y].BackColor = Color.White;
                btnSideMenu[y].Font = new Font("Arial", 20, FontStyle.Bold);
                btnSideMenu[y].Text = "->";
                j++;
                Controls.Add(btnSideMenu[y]);
            }

            btnSideMenu[0].Click += new EventHandler(BtnSideFirst_Click);
            btnSideMenu[1].Click += new EventHandler(BtnSideSecond_Click);
            btnSideMenu[2].Click += new EventHandler(BtnSideThird_Click);
            btnSideMenu[3].Click += new EventHandler(BtnSideFourth_Click);
            btnSideMenu[4].Click += new EventHandler(BtnSideFifth_Click);

            Panel pnlScreen = new Panel();
            pnlScreen.BackColor = Color.Black;
            pnlScreen.SetBounds(80, 50, 555, 400);
            pnlScreen.SendToBack();
            Controls.Add(pnlScreen);
            

            Panel pnlCardReader = new Panel();
            pnlCardReader.BackColor = Color.Black;
            pnlCardReader.SetBounds(655, 410, 215, 5);
            Controls.Add(pnlCardReader);
        }

        public bool deductFromAccount(int amountToDeduct)
        {
            int accountBalance = 0;
            if (Management.instance.raceCondition == false)
            {
                Management.requests.WaitOne();
            }

            accountBalance = ac[accountIndex].getBalance();
            Thread.Sleep(3000);
                      

            if (accountBalance >= amountToDeduct)
            {
                accountBalance -= amountToDeduct;
                ac[accountIndex].setBalance(accountBalance);
                logUpdateSafe("User with account number " + ac[accountIndex].getAccountNum() + " has taken out £" + amountToDeduct + ", leaving £" + accountBalance + "\n");
                updateAccountLabelSafe("Account " + ac[accountIndex].getAccountNum() + " currently has £" + ac[accountIndex].getBalance());
                if (Management.instance.raceCondition == false)
                {
                    Management.requests.ReleaseMutex();
                }
                return true;
            }
            else
            {
                if (Management.instance.raceCondition == false)
                {
                    Management.requests.ReleaseMutex();
                }
                return false;
            }
        }

        public void showDeductionScreen(int amountToDeduct)
        {
            clearATMScreen();
            Controls.Add(lblInstruction);
            lblInstruction.Text = "Withdrawing £" + amountToDeduct.ToString() + "...";
            lblInstruction.Font = new Font("Arial", 20, FontStyle.Bold);
            lblInstruction.SetBounds(220, 225, 300, 50);
            lblInstruction.ForeColor = Color.Red;
            lblInstruction.TextAlign = ContentAlignment.MiddleCenter;        
            lblInstruction.BringToFront();
            lblInstruction.Refresh();
            bool validDeduction = deductFromAccount(amountToDeduct);
            onWithdrawalScreen = false;
            if (!validDeduction)
            {
                lblInstruction.Text = "Insufficient Funds.";
                lblInstruction.Refresh();
                logUpdateSafe("User with account number " + ac[accountIndex].getAccountNum() + " tried to take out £" + amountToDeduct + ", but doesn't have enough in their account.\n");
                Thread.Sleep(3000);
            }
            clearATMScreen();
            displayAccountOptionsScreen();
        }

        public void BtnSideFirst_Click(object sender, EventArgs e)
        {
            // Withdraw some cash
            if(onAccountScreen)
            {
                clearATMScreen();
                showWithdrawalScreen();
            }
            // Withdraw £10
            else if (onWithdrawalScreen)
            {
                showDeductionScreen(10);
            }
        }
        public void BtnSideSecond_Click(object sender, EventArgs e)
        {
            // Withdraw £20
            if(onWithdrawalScreen)
            {
                showDeductionScreen(20);
            }
        }

        public void BtnSideThird_Click(object sender, EventArgs e)
        {
            // View account balance.
            if(onAccountScreen)
            {
                clearATMScreen();
                showBalanceScreen();
                displayAccountOptionsScreen();
            }
            // Withdraw £40
            else if(onWithdrawalScreen)
            {
                deductFromAccount(40);
            }
        }

        public void BtnSideFourth_Click(object sender, EventArgs e)
        {
            // Withdraw £100
            if (onWithdrawalScreen)
            {
                showDeductionScreen(100);
            }
        }
        public void BtnSideFifth_Click(object sender, EventArgs e)
        {
            // Remove card
            if(onAccountScreen)
            {
                clearATMScreen();
                showReturnCardScreen();
                Thread.Sleep(3000);
                Close();
            }
            // Withdraw £200
            else if(onWithdrawalScreen)
            {
                showDeductionScreen(500);
            }
        }
        public void showReturnCardScreen()
        {
            onAccountScreen = false;
            lblInstruction.Text = "Returning card. Goodbye!";
            lblInstruction.Font = new Font("Arial", 20, FontStyle.Bold);
            lblInstruction.SetBounds(160, 225, 400, 50);
            lblInstruction.ForeColor = Color.Red;
            Controls.Add(lblInstruction);
            lblInstruction.BringToFront();
            lblInstruction.Refresh();

            Panel pnlCard = new Panel();
            pnlCard.BackColor = Color.Red;
            pnlCard.SetBounds(655, 415, 215, 5);
            Controls.Add(pnlCard);
        }
        /*
        public Account withdrawMethod(int withdrawlAmount)
        {
            if (ac[accountIndex].getBalance() < withdrawlAmount)
            {
                //timer reference https://stackoverflow.com/questions/14015086/how-do-i-get-a-text-block-to-display-for-5-seconds
                lblInstruction.Text = "Error, account balance too low!";
                DispatcherTimer timer = new DispatcherTimer();
                timer.Tick += {
                    lblInstruction.Text = "How much should be withdrawn?";
                    timer.Stop();
                };
                timer.Interval = TimeSpan.FromSeconds(4);
                timer.Start();
            }
            else
            {
                int currentBalance = ac[accountIndex].getBalance();
                ac[accountIndex].setBalance(currentBalance - withdrawlAmount);
                return Account;
            }
        }
        */

        public void showWithdrawalScreen()
        {
            onAccountScreen = false;
            onWithdrawalScreen = true;
            lblInstruction.Text = "How much should be withdrawn?";
            lblInstruction.ForeColor = Color.Red;
            lblInstruction.Font = new Font("Arial", 20, FontStyle.Bold);
            lblInstruction.SetBounds(110, 60, 500, 50);
            Controls.Add(lblInstruction);
            lblInstruction.BringToFront();

            Label[] amounts = new Label[5];
            for(int i = 0; i < 5; i++)
            {
                amounts[i] = new Label();
                amounts[i].SetBounds(100, 130 + (65 * i), 200, 50);
                amounts[i].BackColor = Color.Black;
                amounts[i].ForeColor = Color.Salmon;
                amounts[i].Font = new Font("Arial", 25, FontStyle.Bold);
                Controls.Add(amounts[i]);
                amounts[i].BringToFront();
            }
            amounts[0].Text = "£10";
            amounts[1].Text = "£20";
            amounts[2].Text = "£40";
            amounts[3].Text = "£100";
            amounts[4].Text = "£500";
        }

        public void showBalanceScreen()
        {
            lblInstruction.Font = new Font("Arial", 20, FontStyle.Bold);
            lblInstruction.Text = "Account Balance = £" + ac[accountIndex].getBalance();
            lblInstruction.SetBounds(170, 225, 400, 50);
            lblInstruction.ForeColor = Color.Red;
            lblInstruction.TextAlign = ContentAlignment.MiddleCenter;
            Controls.Add(lblInstruction);
            lblInstruction.BringToFront();
            lblInstruction.Refresh();
            Thread.Sleep(3000);
            
        }

        private void ATM_Click1(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ATM_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        // Event handler for when a number on the keypad is pressed.
        public void BtnKeyPad_Click(object sender, EventArgs e)
        {
            // If an account number is currently being inserted.
            if ((txtCardInputs.Text).Length < 6 && !accNumberInserted)
            {
                txtCardInputs.Text += ((Button)sender).Text;
            }
            // If a PIN is currently being inserted.
            else if ((txtCardInputs.Text).Length < 4 && accNumberInserted)
            {
                pinNumber += ((Button)sender).Text;
                txtCardInputs.Text += "*";
            }
        }
        // Event handler for the cancel button.
        public void BtnCancel_Click(object sender, EventArgs e)
        {
            if ((txtCardInputs.Text).Length > 0)
            {
                txtCardInputs.Text = (txtCardInputs.Text).Remove((txtCardInputs.Text).Length - 1);
                // Remove a number from the input box.
                if (accNumberInserted)
                {
                    pinNumber = pinNumber.Remove(pinNumber.Length - 1);
                }
                else
                {

                }
            }
            
        }

        // referenced from https://learn.microsoft.com/en-gb/dotnet/desktop/winforms/controls/how-to-make-thread-safe-calls-to-windows-forms-controls?view=netframeworkdesktop-4.8
        public void logUpdateSafe(string newLog)
        {
            if(Management.instance.rTxtAccessLog.InvokeRequired)
            {
                var d = new SafeCallDelegate(logUpdateSafe);
                Management.instance.rTxtAccessLog.Invoke(d, new object[] { newLog });
            }
            else
            {
                Management.instance.rTxtAccessLog.Text += newLog;
            }
        }

        public void updateAccountLabelSafe(string newLbl)
        {
            if (Management.instance.lblAccount[accountIndex].InvokeRequired)
            {
                var d = new SafeCallDelegate(updateAccountLabelSafe);
                Management.instance.lblAccount[accountIndex].Invoke(d, new object[] { newLbl });
            }
            else
            {
                Management.instance.lblAccount[accountIndex].Text = newLbl;
            }
        }
        // Event handler for the enter button.
        public void BtnEnter_Click(object sender, EventArgs e)
        {
            // If the account number is being asked for
            if (!accNumberInserted)
            {
                // Account number must be 6 numbers.
                if ((txtCardInputs.Text).Length == 6)
                {
                    // Save the account number, prompt the user for their pin.
                    lblInstruction.Text = "Enter your PIN Number:";
                    accountNumber = txtCardInputs.Text;
                    txtCardInputs.Text = "";
                    accNumberInserted = true;
                }
            }
            // Pin Number is being inserted.
            else if(accNumberInserted)
            {
                bool dataValidated = false;
                // Pin number must be 4 numbers.
                if ((txtCardInputs.Text).Length == 4)
                {
                    // Save the pin number.
                    txtCardInputs.Text = "";
                    // Check each account on file.
                    for (int i = 0; i < 3; i++)
                    {
                        // If the account number exists.
                        if (ac[i].getAccountNum() == Convert.ToInt32(accountNumber))
                        {
                            // If the pin number is valid.
                            if (ac[i].checkPin(Convert.ToInt32(pinNumber)))
                            {
                                // Account is valid, display account screen
                                dataValidated = true;
                                accountIndex = i;
                                logUpdateSafe("User signed in with account number " + accountNumber + " and pin number " + pinNumber + "\n");
                                txtCardInputs.Clear();
                                txtCardInputs.Hide();
                                clearATMScreen();
                                displayAccountOptionsScreen();
                            }
                            else
                            {
                                // Pin invalid, stop checking.
                                break;
                            }
                        }    
                    }
                }
                // If the account number/pin is incorrect.
                if(!dataValidated)
                {
                    // Reset and inform the user of incorrect details.
                    invalidDataPreviouslyEntered = true;
                    accNumberInserted = false;
                    accountNumber = "";
                    pinNumber = "";
                    // Prompt the user to re-enter.
                    clearATMScreen();
                    txtCardInputs.Clear();
                    displayInitialATMScreen();

                }
            }
        }
        // Screen for Account Options.
        public void displayAccountOptionsScreen()
        {
            // Enables the side buttons.
            onAccountScreen = true;
            // Instruction Label.
            lblInstruction.Text = "Please choose from the following menu items:";
            lblInstruction.ForeColor = Color.Red;
            lblInstruction.Font = new Font("Arial", 15, FontStyle.Bold);
            lblInstruction.SetBounds(110, 60, 500, 50);
            Controls.Add(lblInstruction);
            lblInstruction.BringToFront();

            // Menu options.
            Label[] lblOptions = new Label[3];
            for (int i = 0; i < 3; i++)
            {
                lblOptions[i] = new Label();
                lblOptions[i].ForeColor = Color.Tomato;
                lblOptions[i].BackColor = Color.Black;
                lblOptions[i].SetBounds(100, 135 + (130 * i), 500, 30); ;
                lblOptions[i].Font = new Font("Arial", 20, FontStyle.Bold);
                Controls.Add(lblOptions[i]);
                lblOptions[i].BringToFront(); 
            }
            lblOptions[0].Text = "Withdraw some cash.";
            lblOptions[1].Text = "Check your total balance.";
            lblOptions[2].Text = "Remove card.";
        }
        // Clear the ATM screen.
        public void clearATMScreen()
        {
            // Remove all labels.
            foreach (var labels in Controls.OfType<Label>().ToList())
            {
                Controls.Remove(labels);
            }
            // Remove all images.
            foreach (var images in Controls.OfType<PictureBox>().ToList())
            {
                Controls.Remove(images);
            }
        }

        // Display the first ATM screen.
        public void displayInitialATMScreen()
        {
            Label lblTop = new Label();
            lblTop.Text = "WELCOME TO BANK OF UOD";
            lblTop.ForeColor = Color.Red;
            lblTop.BackColor = Color.Black;
            lblTop.SetBounds(110, 75, 500, 50);
            lblTop.TextAlign = ContentAlignment.MiddleCenter;
            lblTop.Font = new Font("Arial", 25, FontStyle.Bold);
            Controls.Add(lblTop);
            
            lblTop.BringToFront();

            PictureBox picUoD = new PictureBox();
            picUoD.ImageLocation = "../../uni_crest.png";
            picUoD.Size = new Size(200, 200);
            picUoD.Location = new Point(284, 135);
            picUoD.BackColor = Color.Black;
            Controls.Add(picUoD);
            picUoD.BringToFront();
            

            lblInstruction.Text = "Enter your Account Number:";
            lblInstruction.ForeColor = Color.Tomato;
            lblInstruction.BackColor = Color.Black;
            lblInstruction.SetBounds(110, 325, 500, 50);
            lblInstruction.TextAlign = ContentAlignment.MiddleCenter;
            lblInstruction.Font = new Font("Arial", 15, FontStyle.Bold);
            
            Controls.Add(lblInstruction);
            lblInstruction.BringToFront();

            // Prompt displayed if previous attempt was incorrect.
            if(invalidDataPreviouslyEntered)
            {
                Label lblError = new Label();
                lblError.Text = "Invalid Account Number or PIN.";
                lblError.ForeColor = Color.Red;
                lblError.BackColor = Color.Black;
                lblError.Font = new Font("Arial", 15, FontStyle.Bold);
                lblError.TextAlign = ContentAlignment.MiddleCenter;
                lblError.SetBounds(110, 315, 500, 25);
                Controls.Add(lblError);
                lblError.BringToFront();
                if(txtCardInputs.Visible == false)
                {
                    txtCardInputs.Visible = true;
                }    
            }

            txtCardInputs.SetBounds(302, 375, 125, 100);
            txtCardInputs.ReadOnly = true;
            txtCardInputs.BackColor = Color.Gray;
            txtCardInputs.ForeColor = Color.DarkBlue;
            txtCardInputs.TextAlign = HorizontalAlignment.Center;
            Controls.Add(txtCardInputs);
            txtCardInputs.Font = new Font("Arial", 25, FontStyle.Bold);
            txtCardInputs.BringToFront();
        }
        private void ATM_Load(object sender, EventArgs e)
        {

        }
    }
}
