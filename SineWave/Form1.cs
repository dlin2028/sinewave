using NeuralNetwork;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SineWave
{
    public partial class Form1 : Form
    {
        Bitmap map;
        Graphics gfx;

        public Form1()
        {
            InitializeComponent();
        }

        NeuralNet net;
        double[][] inputs;
        double[][] outputs;

        double scale = 1;

        private void Form1_Load(object sender, EventArgs e)
        {

            map = new Bitmap(ClientSize.Width, ClientSize.Height);
            gfx = Graphics.FromImage(map);

            scale = ClientSize.Width / (Math.PI * 2);

            int seed = Guid.NewGuid().GetHashCode();
            Random rand = new Random(seed);

            // 1, 100, 1
            net = new NeuralNet(1, (100, Activations.Tanh));
            net.Randomize(rand);

            inputs = new double[ClientSize.Width][];
            outputs = new double[ClientSize.Width][];
            for (int i = 0; i < ClientSize.Width; i++)
            {
                double x = (double)i / ClientSize.Width * 2 * Math.PI;
                double y = Math.Sin(x);

                inputs[i] = new double[] { x };
                outputs[i] = new double[] { y };
            }

            backgroundWorker1.RunWorkerAsync();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            var currentNet = net.Clone();
            label1.Text = currentNet.MAE(inputs, outputs).ToString();

            
            gfx.Clear(Color.White);
            for (int i = 0; i < inputs.Length; i++)
            {
                //data
                gfx.FillRectangle(Brushes.Black, (int)(inputs[i][0] * scale), (int)(outputs[i][0] * scale + ClientSize.Height/2), 2, 2);

                //network
                gfx.FillRectangle(Brushes.Red, (int)(inputs[i][0] * scale), (int)(currentNet.Compute(inputs[i])[0] * scale + ClientSize.Height / 2), 2, 2);
            }
            bitBox.Image = map;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while(true)
            {
                net.Backprop(inputs, outputs, 0.0005f);
            }
        }
    }
}
