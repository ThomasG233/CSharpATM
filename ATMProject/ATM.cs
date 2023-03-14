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
            Button[,] btnKeyPad = new Button[3,3];
            int i = 0;
            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 3; x++)
                {
                    btnKeyPad[x,y] = new Button();
                    i++;
                    btnKeyPad[x,y].SetBounds(100 + (50 * x), 100 + (50 * y), 50, 50);
                    btnKeyPad[x,y].Text = Convert.ToString(i);
                    Controls.Add(btnKeyPad[x, y]);
                }

            }
        }
        private void ATM_Load(object sender, EventArgs e)
        {

        }
    }
}
