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
        bool accNumberInserted = false;
        bool onAccountScreen = false;

        TextBox txtCardInputs = new TextBox();
        Label lblInstruction = new Label();
        String accountNumber = "";
        String pinNumber = "";

        Button[] btnSideMenu = new Button[3];
            

        public ATM()
        {
            InitializeComponent();
            drawATM();
            displayInitialATMScreen();
            Thread th = Thread.CurrentThread;

            // ThreadExceptionDialog thread1 = new Thread(increment);
            // thread1.Start();
        }

        public void increment()
        {
            for (int i = 0; i < 10; i++)
            {
                // Form2.text = "i";
                Thread.Sleep(1000);
            }
        }

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

        public void BtnCancel_Click(object sender, EventArgs e)
        {
            if ((txtCardInputs.Text).Length > 0)
            {
                txtCardInputs.Text = (txtCardInputs.Text).Remove((txtCardInputs.Text).Length - 1);
            }
            else if ((txtCardInputs.Text).Length == 0)
            {
                Close();
            }
        }

        public void BtnEnter_Click(object sender, EventArgs e)
        {
            if (accNumberInserted == false)
            {
                if ((txtCardInputs.Text).Length == 6)
                {
                    lblInstruction.Text = "Enter your PIN Number:";
                    accountNumber = txtCardInputs.Text;
                    txtCardInputs.Text = "";
                    accNumberInserted = true;
                }
            }
            else
            {
                if ((txtCardInputs.Text).Length == 4)
                {
                    pinNumber = txtCardInputs.Text;
                    txtCardInputs.Text = "";
                    txtCardInputs.Hide();
                    
                    clearATMScreen();
                    displayAccountOptionsScreen();
                }
            }
        }
        public void displayAccountOptionsScreen()
        {
            lblInstruction.Text = "Welcome, " + accountNumber + ".";
            lblInstruction.ForeColor = Color.Red;
            lblInstruction.Font = new Font("Arial", 30, FontStyle.Bold);
            lblInstruction.SetBounds(110, 60, 500, 50);
            Controls.Add(lblInstruction);
            lblInstruction.BringToFront();


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

        public void clearATMScreen()
        {
            foreach (var labels in Controls.OfType<Label>().ToList())
            {
                Controls.Remove(labels);
            }
            foreach (var images in Controls.OfType<PictureBox>().ToList())
            {
                Controls.Remove(images);
            }
        }

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
