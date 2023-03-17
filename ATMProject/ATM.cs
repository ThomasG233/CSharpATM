using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public static ATM instance;
        bool accNumberInserted = false;
        bool invalidDataPreviouslyEntered = false;
        bool onAccountScreen = false;
        public static string currentActionLog = "";

        // Holds reference to accounts from Management.
        Account[] ac = null;

        TextBox txtCardInputs = new TextBox();
        Label lblInstruction = new Label();
        String accountNumber = "";
        String pinNumber = "";

        Button[] btnSideMenu = new Button[3];
            
        // Basic constructor (required).
        public ATM()
        {
            InitializeComponent();
            drawATM();
            displayInitialATMScreen();
            // Thread th = Thread.CurrentThread;

            // ThreadExceptionDialog thread1 = new Thread(increment);
            // thread1.Start();
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
            for (int y = 0; y < 3; y++)
            {
                btnSideMenu[y] = new Button();
                btnSideMenu[y].SetBounds(20, 125 + (125 * j), 50, 50);
                btnSideMenu[y].BackColor = Color.White;
                btnSideMenu[y].Font = new Font("Arial", 20, FontStyle.Bold);
                btnSideMenu[y].Text = "->";
                j++;
                Controls.Add(btnSideMenu[y]);
            }

            btnSideMenu[0].Click += new EventHandler(BtnSideTop_Click);
            btnSideMenu[1].Click += new EventHandler(BtnSideMid_Click);
            btnSideMenu[2].Click += new EventHandler(BtnSideBottom_Click);

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

        public void BtnSideTop_Click(object sender, EventArgs e)
        {
            // Withdraw some cash
            if(onAccountScreen)
            {
                showWithdrawalScreen();
            }
        }

        public void BtnSideMid_Click(object sender, EventArgs e)
        {
            // View account balance.
            if(onAccountScreen)
            {
                clearATMScreen();
            }
        }
        public void BtnSideBottom_Click(object sender, EventArgs e)
        {
            // Remove card
            if(onAccountScreen)
            {

            }
        }

        public void showWithdrawalScreen()
        {

        }

        public void showBalanceScreen()
        {

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
                txtCardInputs.Text += Convert.ToString(((Button)sender).Text);
            }
            // If a PIN is currently being inserted.
            else if ((txtCardInputs.Text).Length < 4 && accNumberInserted)
            {
                txtCardInputs.Text += Convert.ToString(((Button)sender).Text);
            }
        }
        // Event handler for the cancel button.
        public void BtnCancel_Click(object sender, EventArgs e)
        {
            // Remove a number from the input box.
            if ((txtCardInputs.Text).Length > 0)
            {
                txtCardInputs.Text = (txtCardInputs.Text).Remove((txtCardInputs.Text).Length - 1);
            }
            // Cancel, closes ATM system.
            else if ((txtCardInputs.Text).Length == 0)
            {
                Close();
            }
        }
        public static void logUpdate(string newLog)
        {
            Management.instance.rTxtAccessLog.AppendText(newLog);
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
                    pinNumber = txtCardInputs.Text;
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
                                currentActionLog = "User signed in with account number: " + accountNumber + " and pin number:" + pinNumber + "\n";
                                logUpdate(currentActionLog);
                                currentActionLog = "";
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
            lblInstruction.Text = "Welcome, " + accountNumber + ".";
            lblInstruction.ForeColor = Color.Red;
            lblInstruction.Font = new Font("Arial", 30, FontStyle.Bold);
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
                lblOptions[i].SetBounds(100, 135 + (125 * i), 500, 30); ;
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
