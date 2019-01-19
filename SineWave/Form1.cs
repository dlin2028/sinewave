using NeuralNetwork;
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

namespace SineWave
{
    public partial class Form1 : Form
    {
        private static readonly object lockObject = new object();

        Bitmap map;
        Graphics gfx;

        public Form1()
        {
            InitializeComponent();
        }

        NeuralNet net;
        double[][] inputs;
        double[][] outputs;

        
        int size = 100;
        double range = Math.PI * 3;
        double scale;

        private void Form1_Load(object sender, EventArgs e)
        {
            map = new Bitmap(ClientSize.Width, ClientSize.Height);
            gfx = Graphics.FromImage(map);

            scale = ClientSize.Width / range;

            int seed = Guid.NewGuid().GetHashCode();
            Random rand = new Random(seed);

            // 1, 100, 1
            net = new NeuralNet(1, (100, Activations.Tanh), (100, Activations.Tanh), (1, Activations.Tanh));
            net.Randomize(rand);

            (inputs, outputs) = GenerateSine(size, range);


            backgroundWorker1.RunWorkerAsync();
        }

        private (double[][] inputs, double[][] outputs) GenerateSine(int size, double range)
        {
            double[][] inputs = new double[size][];
            double[][] outputs = new double[size][];

            double x = 0;
            for (int i = 0; i < size; i++)
            {
                inputs[i] = new double[] { x };
                outputs[i] = new double[] { Math.Sin(x) };
                x += range / size;
            }

            return (inputs, outputs);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            NeuralNet currentNet;
            lock (lockObject)
            {
                currentNet = net.Clone();
            }
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
                lock (lockObject)
                {
                    net.Backprop(inputs, outputs, 0.0125, 0.2, 1);
                }
            }
        }
    }
}
