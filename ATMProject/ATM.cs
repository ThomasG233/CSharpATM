// Team 3: Alba Henderson, Thomas Laland-Brown, Thomas Gourlay 
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ATMProject
{
    public partial class ATM : Form
    {
        // Required to pass through into Management.
        private delegate void SafeCallDelegate(string text);

        public static ATM instance;

        // Track which screen a user is currently on.
        bool accNumberInserted = false;
        bool invalidDataPreviouslyEntered = false;
        bool onAccountScreen = false;
        bool onWithdrawalScreen = false;
        bool onDepositScreen = false;

        // Account Index of the account being logged into.
        int accountIndex;

        // All sound effects.
        SoundPlayer keypad = new SoundPlayer(@"../../keypad.wav");
        SoundPlayer success = new SoundPlayer(@"../../successChime.wav");
        SoundPlayer error = new SoundPlayer(@"../../error.wav");
        SoundPlayer confirm = new SoundPlayer(@"../../confirm.wav");
        SoundPlayer whirr = new SoundPlayer(@"../../whirr.wav");
        SoundPlayer exit = new SoundPlayer(@"../../exit.wav");

        int validAttempts = 0;
        // Holds reference to accounts from Management.
        Account[] ac = null;

        // Recurring elements.
        TextBox txtCardInputs = new TextBox();
        Label lblInstruction = new Label();

        // Account input.
        string accountNumber = "";
        string pinNumber = "";

        Button[] btnSideMenu = new Button[5];
            

        // Basic constructor (required).
        public ATM()
        {
            InitializeComponent();
            drawATM();
            instance = this;
            displayInitialATMScreen();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
        }

        // Constructor to pass in accounts.
        public ATM(Account[] accounts)
        {
            // Passes in reference to account from Management.
            ac = accounts;
            InitializeComponent();
            instance = this;
            drawATM();
            displayInitialATMScreen();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
        }

        // Draw the ATM UI.
        public void drawATM()
        {
            this.BackColor = Color.LightGray;
            // Keypad to the right side.
            Button[,] btnKeyPad = new Button[3,4];
            int i = 0;
            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 3; x++)
                {
                    // Draw all buttons onto the screen.
                    btnKeyPad[x, y] = new Button();
                    i++;
                    btnKeyPad[x, y].SetBounds(650 + (75 * x), 75 + (75 * y), 75, 75);
                    btnKeyPad[x, y].Font = new Font("Arial", 24, FontStyle.Bold);
                    btnKeyPad[x, y].Text = Convert.ToString(i);
                    btnKeyPad[x,y].BackColor = Color.White;
                    // Only provide typical event handler for numbered keypads.
                    if(y != 3)
                    {
                        btnKeyPad[x, y].Click += new EventHandler(BtnKeyPad_Click);
                    }
                    Controls.Add(btnKeyPad[x, y]);
                }
            }
            // Unique buttons with other event handlers
            // Cancel Button..
            btnKeyPad[0, 3].Font = new Font("Arial", 12, FontStyle.Bold);
            btnKeyPad[0, 3].BackColor = Color.Red;
            btnKeyPad[0, 3].Text = "Cancel";
            btnKeyPad[0, 3].Click += new EventHandler(BtnCancel_Click);
            
            btnKeyPad[1, 3].Text = "0";
            btnKeyPad[1, 3].Click += new EventHandler(BtnKeyPad_Click);

            // Enter button.
            btnKeyPad[2, 3].Font = new Font("Arial", 12, FontStyle.Bold);
            btnKeyPad[2, 3].BackColor = Color.Green;
            btnKeyPad[2, 3].Text = "Enter";
            btnKeyPad[2, 3].Click += new EventHandler(BtnEnter_Click);

            // Buttons for left side of screen.
            for (int y = 0; y < 5; y++)
            {
                btnSideMenu[y] = new Button();
                btnSideMenu[y].SetBounds(20, 125 + (65 * y), 50, 50);
                btnSideMenu[y].BackColor = Color.White;
                btnSideMenu[y].Font = new Font("Arial", 20, FontStyle.Bold);
                btnSideMenu[y].Text = "->";
                Controls.Add(btnSideMenu[y]);
            }

            // Add all event handlers.
            btnSideMenu[0].Click += new EventHandler(BtnSideFirst_Click);
            btnSideMenu[1].Click += new EventHandler(BtnSideSecond_Click);
            btnSideMenu[2].Click += new EventHandler(BtnSideThird_Click);
            btnSideMenu[3].Click += new EventHandler(BtnSideFourth_Click);
            btnSideMenu[4].Click += new EventHandler(BtnSideFifth_Click);

            // Back screen.
            Panel pnlScreen = new Panel();
            pnlScreen.BackColor = Color.Black;
            pnlScreen.SetBounds(80, 50, 555, 400);
            pnlScreen.SendToBack();
            Controls.Add(pnlScreen);
            
            // Small card reader slot.
            Panel pnlCardReader = new Panel();
            pnlCardReader.BackColor = Color.Black;
            pnlCardReader.SetBounds(655, 410, 215, 5);
            Controls.Add(pnlCardReader);
        }
        // Remove an amount from the account.
        public bool deductFromAccount(int amountToDeduct)
        {
            int accountBalance = 0;
            // Race condition results in no Mutex being used.
            if (Management.instance.raceCondition == false)
            {
                // Thread pauses until the account is not being accessed.
                ac[accountIndex].requests.WaitOne();
            }
            // When available for access, creates a local copy of account balance.
            accountBalance = ac[accountIndex].getBalance();
            // Pause created for race condition, given 5 seconds to simulate.
            Thread.Sleep(5000);  

            // Must be a valid amount to remove.
            if(accountBalance >= amountToDeduct)
            {
                accountBalance -= amountToDeduct;
                // Update bank computer with new amount, adding to log.
                ac[accountIndex].setBalance(accountBalance);
                logUpdateSafe("User with account number " + ac[accountIndex].getAccountNum() + " has taken out £" + amountToDeduct + ", leaving £" + accountBalance + "\n");
                updateAccountLabelSafe("Account " + ac[accountIndex].getAccountNum() + " currently has £" + ac[accountIndex].getBalance());
                
                if(Management.instance.raceCondition == false)
                {
                    // Free up the Mutex for other threads to access the account.
                    ac[accountIndex].requests.ReleaseMutex();
                }
                return true;
            }
            else
            {
                if(Management.instance.raceCondition == false)
                {
                    // Release the mutex, even if unsuccessful.
                    ac[accountIndex].requests.ReleaseMutex();
                }
                return false;
            }
        }

        // Remove an amount from the account.
        public void addToAccount(int amountToAdd)
        {
            Panel pnlMoney = new Panel();
            pnlMoney.BackColor = Color.LimeGreen;
            pnlMoney.SetBounds(655, 415, 215, 5);
            Controls.Add(pnlMoney);
            int accountBalance = 0;
            // Race condition results in no Mutex being used.
            if (Management.instance.raceCondition == false)
            {
                // Thread pauses until the account is not being accessed.
                ac[accountIndex].requests.WaitOne();
            }
            // When available for access, creates a local copy of account balance.
            accountBalance = ac[accountIndex].getBalance();
            // Pause created for race condition, given 5 seconds to simulate.
            Thread.Sleep(5000);

            accountBalance += amountToAdd;
            // Update bank computer with new amount, adding to log.
            ac[accountIndex].setBalance(accountBalance);
            logUpdateSafe("User with account number " + ac[accountIndex].getAccountNum() + " has added £" + amountToAdd + ", now at £" + accountBalance + "\n");
            updateAccountLabelSafe("Account " + ac[accountIndex].getAccountNum() + " currently has £" + ac[accountIndex].getBalance());

            if (Management.instance.raceCondition == false)
            {
                // Free up the Mutex for other threads to access the account.
                ac[accountIndex].requests.ReleaseMutex();
            }
            Controls.Remove(pnlMoney);
        }

        // Display the UI for removing money.
        public void showDeductionScreen(int amountToDeduct)
        {
            onDepositScreen = false;
            clearATMScreen();
            Controls.Add(lblInstruction);
            // Informs the user that money is being removed.
            lblInstruction.Text = "Withdrawing £" + amountToDeduct.ToString() + "...";
            lblInstruction.Font = new Font("Arial", 20, FontStyle.Bold);
            lblInstruction.SetBounds(220, 225, 300, 50);
            lblInstruction.ForeColor = Color.Red;
            lblInstruction.TextAlign = ContentAlignment.MiddleCenter;        
            lblInstruction.BringToFront();
            lblInstruction.Refresh();
            // Check if a deduction was successful.
            bool validDeduction = deductFromAccount(amountToDeduct);
            onWithdrawalScreen = false;
            // If money couldn't be removed.
            if (!validDeduction)
            {
                // Display a prompt to the user about an insufficient amount.
                lblInstruction.Text = "Insufficient Funds.";
                lblInstruction.Refresh();
                // Update the management log.
                logUpdateSafe("User with account number " + ac[accountIndex].getAccountNum() + " tried to take out £" + amountToDeduct + ", but doesn't have enough in their account.\n");
                error.Play();
                // Simulate a pause so that the user can read the message.
                Thread.Sleep(3000);
            }
            else
            {
                // Dispense card.
                whirr.Play();
            }
            clearATMScreen();
            displayAccountOptionsScreen();
        }

        // Display the UI for removing money.
        public void showAdditionScreen(int amountToAdd)
        {
            onDepositScreen = false;
            clearATMScreen();
            Controls.Add(lblInstruction);
            // Informs the user that money is being removed.
            lblInstruction.Text = "Depositing £" + amountToAdd.ToString() + "...";
            lblInstruction.Font = new Font("Arial", 20, FontStyle.Bold);
            lblInstruction.SetBounds(220, 225, 300, 50);
            lblInstruction.ForeColor = Color.Red;
            lblInstruction.TextAlign = ContentAlignment.MiddleCenter;
            lblInstruction.BringToFront();
            lblInstruction.Refresh();
            // Add the money into the account.
            addToAccount(amountToAdd);
            
            clearATMScreen();
            whirr.Play();
            displayAccountOptionsScreen();
        }


        public void BtnSideFirst_Click(object sender, EventArgs e)
        {
            // Selects option to withdraw some cash
            if(onAccountScreen)
            {
                clearATMScreen();
                showWithdrawalScreen();
            }
            // Withdraw £10 from an account.
            else if (onWithdrawalScreen)
            {
                showDeductionScreen(10);
            }
            else if(onDepositScreen)
            {
                showAdditionScreen(10);
            }
        }
        public void BtnSideSecond_Click(object sender, EventArgs e)
        {
            if (onAccountScreen)
            {
                clearATMScreen();
                showDepositScreen();
            }
            // Withdraw £20 from an account.
            else if (onWithdrawalScreen)
            {
                showDeductionScreen(20);
            }
            // Add £20 to the account.
            else if (onDepositScreen)
            {
                showAdditionScreen(20);
            }
        }

        public void BtnSideThird_Click(object sender, EventArgs e)
        {
            // View an account's balance.
            if(onAccountScreen)
            {
                clearATMScreen();
                showBalanceScreen();
                displayAccountOptionsScreen();
            }
            // Withdraw £40 from an account.
            else if (onWithdrawalScreen)
            {
                deductFromAccount(40);
            }
            // Add £40 to the account.
            else if (onDepositScreen)
            {
                showAdditionScreen(40);
            }
        }

        public void BtnSideFourth_Click(object sender, EventArgs e)
        {
            // Withdraw £100 from an account.
            if (onWithdrawalScreen)
            {
                showDeductionScreen(100);
            }
            else if (onDepositScreen)
            {
                showAdditionScreen(100);
            }
        }
        public void BtnSideFifth_Click(object sender, EventArgs e)
        {
            // Remove card from the machine.
            if(onAccountScreen)
            {
                clearATMScreen();
                showReturnCardScreen();
                // Takes 3 seconds to remove card.
                Thread.Sleep(3000);
                Close();
            }
            // Withdraw £500 from an account.
            else if (onWithdrawalScreen)
            {
                showDeductionScreen(500);
            }
            else if (onDepositScreen)
            {
                showAdditionScreen(500);
            }
        }
        // Display UI for a card removed.
        public void showReturnCardScreen()
        {
            // Remove card SFX.
            exit.Play();
            onAccountScreen = false;
            // Display text informing a user's card is dispensed.
            lblInstruction.Text = "Returning card. Goodbye!";
            lblInstruction.Font = new Font("Arial", 20, FontStyle.Bold);
            lblInstruction.SetBounds(160, 225, 400, 50);
            lblInstruction.ForeColor = Color.Red;
            Controls.Add(lblInstruction);
            lblInstruction.BringToFront();
            lblInstruction.Refresh();
            // Card exiting machine.
            Panel pnlCard = new Panel();
            pnlCard.BackColor = Color.Red;
            pnlCard.SetBounds(655, 415, 215, 5);
            Controls.Add(pnlCard);
        }
        // Display UI for the withdrawal screen.
        public void showWithdrawalScreen()
        {
            // No longer on account screen.
            onAccountScreen = false;
            onWithdrawalScreen = true;
            lblInstruction.Text = "How much should be withdrawn?";
            lblInstruction.ForeColor = Color.Red;
            lblInstruction.Font = new Font("Arial", 20, FontStyle.Bold);
            lblInstruction.SetBounds(110, 60, 500, 50);
            Controls.Add(lblInstruction);
            lblInstruction.BringToFront();

            // Amounts which can be withdrawn.
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
            // Displays amounts.
            amounts[0].Text = "£10";
            amounts[1].Text = "£20";
            amounts[2].Text = "£40";
            amounts[3].Text = "£100";
            amounts[4].Text = "£500";
        }

        // Display UI for the deposit screen.
        public void showDepositScreen()
        {
            // No longer on account screen.
            onAccountScreen = false;
            onDepositScreen = true;
            lblInstruction.Text = "How much are you inserting?";
            lblInstruction.ForeColor = Color.Red;
            lblInstruction.Font = new Font("Arial", 20, FontStyle.Bold);
            lblInstruction.SetBounds(110, 60, 500, 50);
            Controls.Add(lblInstruction);
            lblInstruction.BringToFront();

            // Amounts which can be added into the account.
            Label[] amounts = new Label[5];
            for (int i = 0; i < 5; i++)
            {
                amounts[i] = new Label();
                amounts[i].SetBounds(100, 130 + (65 * i), 200, 50);
                amounts[i].BackColor = Color.Black;
                amounts[i].ForeColor = Color.Salmon;
                amounts[i].Font = new Font("Arial", 25, FontStyle.Bold);
                Controls.Add(amounts[i]);
                amounts[i].BringToFront();
            }
            // Displays amounts.
            amounts[0].Text = "£10";
            amounts[1].Text = "£20";
            amounts[2].Text = "£40";
            amounts[3].Text = "£100";
            amounts[4].Text = "£500";

        }

        // Displays UI for the balance screen.
        public void showBalanceScreen()
        {
            onAccountScreen = false;
            // Displays balance to user.
            lblInstruction.Font = new Font("Arial", 20, FontStyle.Bold);
            lblInstruction.Text = "Account Balance = £" + ac[accountIndex].getBalance();
            lblInstruction.SetBounds(160, 225, 400, 50);
            lblInstruction.ForeColor = Color.Red;
            lblInstruction.TextAlign = ContentAlignment.MiddleCenter;
            Controls.Add(lblInstruction);
            lblInstruction.BringToFront();
            lblInstruction.Refresh();
            // Gives 3 seconds to read the amount.
            Thread.Sleep(3000);
        }
        // Displays the Welcome screen for a user.
        public void showWelcomeScreen()
        {
            // Plays chime to indicate successful login.
            success.Play();
            // Displays welcome message.
            lblInstruction.Font = new Font("Arial", 30, FontStyle.Bold);
            lblInstruction.Text = "Welcome, " + accountNumber + ".";
            lblInstruction.SetBounds(135, 225, 450, 50);
            lblInstruction.ForeColor = Color.Red;
            lblInstruction.TextAlign = ContentAlignment.MiddleCenter;
            Controls.Add(lblInstruction);
            lblInstruction.BringToFront();
            lblInstruction.Refresh();
            // Gives time to read the message.
            Thread.Sleep(2000);
            clearATMScreen();
            displayAccountOptionsScreen();
        }

        // Event handler for when a number on the keypad is pressed.
        public void BtnKeyPad_Click(object sender, EventArgs e)
        {
            if(!onAccountScreen && !onDepositScreen && !onWithdrawalScreen)
            {
                // If an account number is currently being inserted.
                if ((txtCardInputs.Text).Length < 6 && !accNumberInserted)
                {
                    txtCardInputs.Text += ((Button)sender).Text;
                    keypad.Play();
                }
                // If a PIN is currently being inserted.
                else if ((txtCardInputs.Text).Length < 4 && accNumberInserted)
                {
                    pinNumber += ((Button)sender).Text;
                    txtCardInputs.Text += "*";
                    keypad.Play();
                }

            }
            
        }
        // Event handler for the cancel button.
        public void BtnCancel_Click(object sender, EventArgs e)
        {
            if (!onAccountScreen && !onWithdrawalScreen && !onDepositScreen)
            {     
                // Must have something entered to do a removal.
                if ((txtCardInputs.Text).Length > 0)
                {
                    txtCardInputs.Text = (txtCardInputs.Text).Remove((txtCardInputs.Text).Length - 1);
                    // Remove a number from the input box.
                    if (accNumberInserted)
                    {
                        // Remove last entered pin char.
                        pinNumber = pinNumber.Remove(pinNumber.Length - 1);
                    }
                }
            }
            else if(onAccountScreen)
            {
                clearATMScreen();
                showReturnCardScreen();
                // Takes 3 seconds to remove card.
                Thread.Sleep(3000);
                Close();
            }
            else if(onDepositScreen)
            {
                onDepositScreen = false;
                clearATMScreen();
                displayAccountOptionsScreen();
                confirm.Play();
            }
            else if(onWithdrawalScreen)
            {
                onWithdrawalScreen = false;
                clearATMScreen();
                displayAccountOptionsScreen();
                confirm.Play();
            }
           
        }

        // Microsoft documentation referenced, from https://learn.microsoft.com/en-gb/dotnet/desktop/winforms/controls/how-to-make-thread-safe-calls-to-windows-forms-controls?view=netframeworkdesktop-4.8
        public void logUpdateSafe(string newLog)
        {
            // Mandatory check before text can be added.
            if (Management.instance.rTxtAccessLog.InvokeRequired)
            {
                // Creates ability to delegate, in case invoke is needed first.
                var deleg = new SafeCallDelegate(logUpdateSafe);
                Management.instance.rTxtAccessLog.Invoke(deleg, new object[] { 
                    newLog 
                });
            }
            else
            {
                // Write the message into a log message.
                Management.instance.rTxtAccessLog.Text += newLog;
            }
        }
        // Microsoft documentation referenced, from https://learn.microsoft.com/en-gb/dotnet/desktop/winforms/controls/how-to-make-thread-safe-calls-to-windows-forms-controls?view=netframeworkdesktop-4.8
        public void updateAccountLabelSafe(string newLbl)
        {
            // Mandatory check before text can be added.
            if (Management.instance.lblAccount[accountIndex].InvokeRequired)
            {
                // Creates ability to delegate, in case invoke is needed first.
                var deleg = new SafeCallDelegate(updateAccountLabelSafe);
                Management.instance.lblAccount[accountIndex].Invoke(deleg, new object[] 
                { 
                    newLbl 
                });
            }
            else
            {
                // Write the message into a log message.
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
                    // Account number has been inserted, next value entered will be a pin.
                    accNumberInserted = true;
                }
            }
            // Pin Number is being inserted.
            else if(accNumberInserted && !onAccountScreen && !onWithdrawalScreen && !onDepositScreen)
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
                                // Update the log with a sign-in message.
                                logUpdateSafe("User signed in with account number " + accountNumber + " and pin number " + pinNumber + "\n");
                                txtCardInputs.Clear();
                                txtCardInputs.Hide();
                                clearATMScreen();
                                // Display "Welcome, Account" screen.
                                showWelcomeScreen();
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
                    validAttempts++;
                    // Prompt the user to re-enter.
                    clearATMScreen();
                    txtCardInputs.Clear();
                    displayInitialATMScreen();

                }
            }
            // Play a tone when pressed.
            confirm.Play();
        }
        // Screen for Account Options.
        public void displayAccountOptionsScreen()
        {
            onDepositScreen = false;
            onWithdrawalScreen = false;
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
            Label[] lblOptions = new Label[4];
            for (int i = 0; i < 4; i++)
            {
                // Set all label properties.
                lblOptions[i] = new Label();
                lblOptions[i].ForeColor = Color.Tomato;
                lblOptions[i].BackColor = Color.Black;
                if (i != 3)
                {
                    lblOptions[i].SetBounds(100, 135 + (130 * i), 500, 30);
                }
                lblOptions[i].Font = new Font("Arial", 20, FontStyle.Bold);
                Controls.Add(lblOptions[i]);
                lblOptions[i].BringToFront(); 
            }
            // Labels for each option.
            lblOptions[0].Text = "Withdraw some cash.";
            lblOptions[1].Text = "Check your total balance.";
            lblOptions[2].Text = "Remove card.";

            lblOptions[3].Text = "Deposit some cash.";
            lblOptions[3].SetBounds(100, 200, 500, 30);
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
            // Top Message.
            Label lblTop = new Label();
            lblTop.Text = "WELCOME TO BANK OF UOD";
            lblTop.ForeColor = Color.Red;
            lblTop.BackColor = Color.Black;
            lblTop.SetBounds(110, 75, 500, 50);
            lblTop.TextAlign = ContentAlignment.MiddleCenter;
            lblTop.Font = new Font("Arial", 25, FontStyle.Bold);
            Controls.Add(lblTop);
            lblTop.BringToFront();

            // Logo in the centre of the screen.
            PictureBox picUoD = new PictureBox();
            picUoD.ImageLocation = "../../uni_crest.png";
            picUoD.Size = new Size(200, 200);
            picUoD.Location = new Point(284, 135);
            picUoD.BackColor = Color.Black;
            Controls.Add(picUoD);
            picUoD.BringToFront();

            // User can attempt to enter their pin 3 times.
            if (validAttempts != 3)
            {
                lblInstruction.Text = "Enter your Account Number:";
                lblInstruction.ForeColor = Color.Tomato;
                lblInstruction.BackColor = Color.Black;
                lblInstruction.SetBounds(110, 325, 500, 50);
                lblInstruction.TextAlign = ContentAlignment.MiddleCenter;
                lblInstruction.Font = new Font("Arial", 15, FontStyle.Bold);
                Controls.Add(lblInstruction);
                lblInstruction.BringToFront();
            }
            // Prompt displayed if previous attempt was incorrect.
            if(invalidDataPreviouslyEntered && validAttempts != 3)
            {
                // Display error to user, prompted to re-enter.
                Label lblError = new Label();
                lblError.Text = "Invalid Account Number or PIN.";
                lblError.ForeColor = Color.Red;
                lblError.BackColor = Color.Black;
                lblError.Font = new Font("Arial", 15, FontStyle.Bold);
                lblError.TextAlign = ContentAlignment.MiddleCenter;
                lblError.SetBounds(110, 315, 500, 25);
                Controls.Add(lblError);
                error.Play();
                lblError.BringToFront();
                // Unhide the input box if it's not visible.
                if(txtCardInputs.Visible == false)
                {
                    txtCardInputs.Visible = true;
                }    
            }
            // User has ran out of valid attempts.
            else if(validAttempts == 3)
            {
                // Informs the user that they cannot re-enter again.
                Label lblError = new Label();
                lblError.Text = "Too many attempts, please try again later.";
                lblError.ForeColor = Color.Red;
                lblError.BackColor = Color.Black;
                lblError.Font = new Font("Arial", 15, FontStyle.Bold);
                lblError.TextAlign = ContentAlignment.MiddleCenter;
                lblError.SetBounds(110, 335, 500, 25);
                Controls.Add(lblError);

                // Dispenses card.
                Panel pnlCard = new Panel();
                pnlCard.BackColor = Color.Red;
                pnlCard.SetBounds(655, 415, 215, 5);
                Controls.Add(pnlCard);
                lblTop.Refresh();
                lblError.BringToFront();
                lblError.Refresh();

                error.Play();;
                txtCardInputs.Visible = false;
                // Gives time to user to read prompt before closing.
                Thread.Sleep(3000);
                Close();
            }

            // Input box for account number and pin.
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
