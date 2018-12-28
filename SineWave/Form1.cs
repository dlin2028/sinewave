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
        Graphics gfx;

        public Form1()
        {
            InitializeComponent();

        }

        NeuralNet net;
        double[][] inputs;
        double[][] outputs;

        private void Form1_Load(object sender, EventArgs e)
        {
            gfx = CreateGraphics();

            int seed = Guid.NewGuid().GetHashCode();
            Random rand = new Random(seed);

            List<Layer> layers = new List<Layer>();
            layers.Add(new Layer(Activations.Tanh, 1, 1));
            layers.Add(new Layer(Activations.Tanh, 1, 5));
            layers.Add(new Layer(Activations.Tanh, 5, 5));
            layers.Add(new Layer(Activations.Tanh, 5, 1));

            net = new NeuralNet(layers.ToArray());
            net.Randomize(rand);

            inputs = new double[ClientSize.Width][];
            outputs = new double[ClientSize.Width][];
            for (int i = 0; i < ClientSize.Width; i++)
            {
                inputs[i] = new double[] { (double)i / ClientSize.Width };
                outputs[i] = new double[] { (Math.Sin(i/(15*Math.PI)))/2 };
            }

            for (int i = 0; i < ClientSize.Width; i++)
            {
                if(inputs[i][0] <= 0 || inputs[i][0] >= 1)
                {
                    ;
                }
                if (outputs[i][0] <= 0 || outputs[i][0] >= 1)
                {
                    ;
                }
            }

            for (int i = 0; i < 100; i++)
            {
                net.Backprop(inputs, outputs, 0.9f);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label1.Text = net.MAE(inputs, outputs).ToString();

            int result = (int)(100 * (net.Compute(inputs[0])[0] - 0.5f));
            int nextResult = (int)(100 * (net.Compute(inputs[1])[0] - 0.5f));

            int desiredResult = (int)(100 * (outputs[0][0] - 0.5f));
            int nextDesiredResult = (int)(100 * (outputs[1][0] - 0.5f));

            for (int i = 0; i < ClientSize.Width; i++)
            {

                gfx.DrawLine(new Pen(Color.Black, 1), (float)inputs[i][0] * ClientSize.Width, ClientSize.Height / 2 + desiredResult, i+1, ClientSize.Height / 2 + nextDesiredResult);
                desiredResult = nextDesiredResult;
                nextDesiredResult = (int)(100d * outputs[i][0]);
                
                //backprop
                gfx.DrawLine(new Pen(Color.Red, 5), i, ClientSize.Height / 2 + result, i + 1, ClientSize.Height / 2 + nextResult);
                result = nextResult;
                nextResult = (int)(100f * net.Compute(inputs[i])[0]);
            }

            net.Backprop(inputs, outputs, 0.9f);
            gfx.Clear(Color.White);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
