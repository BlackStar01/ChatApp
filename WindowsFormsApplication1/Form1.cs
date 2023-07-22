using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Communication;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        
        private TcpClient comm;

        public Form1(string h, int p)
        {
            InitializeComponent();

            // connect to server
            comm = new TcpClient(h, p);
        }

       
        private void sendOperation(object sender, EventArgs e)
        {
            string expr = textBox1.Text;

            string[] tabOp = expr.Split(' ');
            double op1 = double.Parse(tabOp[0]);
            double op2 = double.Parse(tabOp[2]);
            char op = char.Parse(tabOp[1]);

            Net.sendMsg(comm.GetStream(), new Expr(op1, op2, op));
            textBox2.Text = ((Result)Net.rcvMsg(comm.GetStream())).ToString();
        }
    }
}
