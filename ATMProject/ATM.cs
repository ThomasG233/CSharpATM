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
        public ATM()
        {
            InitializeComponent();
            drawATM();
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
                    btnKeyPad[x, y].SetBounds(650 + (75 * x), 100 + (75 * y), 75, 75);
                    btnKeyPad[x, y].Font = new Font("Arial", 24, FontStyle.Bold);
                    btnKeyPad[x, y].Text = Convert.ToString(i);
                    btnKeyPad[x,y].BackColor = Color.White;
                    Controls.Add(btnKeyPad[x, y]);
                }
            }
            btnKeyPad[0, 3].Font = new Font("Arial", 12, FontStyle.Bold);
            btnKeyPad[0, 3].BackColor = Color.Yellow;
            btnKeyPad[0, 3].Text = "Cancel";
            btnKeyPad[1, 3].Text = "0";
            btnKeyPad[2, 3].Font = new Font("Arial", 12, FontStyle.Bold);
            btnKeyPad[2, 3].BackColor = Color.Green;
            btnKeyPad[2, 3].Text = "Enter";

            Button[,] btnMenu = new Button[1, 3];
            int j = 0;
            for (int y = 0; y < 3; y++)
            {
                btnMenu[0, y] = new Button();
                btnMenu[0, y].SetBounds(8, 100 + (125 * j), 100, 50);
                btnMenu[0,y].BackColor = Color.White;
                j++;
                Controls.Add(btnMenu[0, y]);
            }

            Panel pnlScreen = new Panel();
            pnlScreen.BackColor = Color.Black;
            pnlScreen.SetBounds(115, 50, 525, 400);
            Controls.Add(pnlScreen);



        }
        private void ATM_Load(object sender, EventArgs e)
        {

        }
    }
}
