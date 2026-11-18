using System;
using System.Drawing;
using System.IO;
using System.Linq;
using ПИ32_Гусев_NUM4.NeuroNet;
using System.Windows.Forms;

namespace ПИ32_Гусев_NUM4
{
    public partial class FormMain : Form
    {
        private double[] inputPixels; //Массив входных данных
        private Network network; //обЪявление нейросети

        public FormMain()
        {
            InitializeComponent();

            inputPixels = new double[15];
            network = new Network();

            buttonLearn.Click += button_SaveTrainSample_Click;
            buttonTest.Click += button_SaveTestSample_Click;
        }

        private void Change_button_status(object sender, EventArgs e)
        {
            if (((Button)sender).BackColor == Color.White)
            {
                ((Button)sender).BackColor = Color.Black;
                inputPixels[((Button)sender).TabIndex] = 1d;
            }
            else
            {
                ((Button)sender).BackColor = Color.White;
                inputPixels[((Button)sender).TabIndex] = 0d;
            }
        }

        private void button_SaveTrainSample_Click(object sender, EventArgs e)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "train.txt";
            string tmpStr = numericUpDowoutput.Value.ToString();

            for (int i = 0; i < inputPixels.Length; i++)
            {
                tmpStr += " " + inputPixels[i].ToString();
            }
            tmpStr += "\n";

            File.AppendAllText(path, tmpStr);
        }

        private void button_SaveTestSample_Click(object sender, EventArgs e)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "test.txt";
            string tmpStr = numericUpDowoutput.Value.ToString();

            for (int i = 0; i < inputPixels.Length; i++)
            {
                tmpStr += " " + inputPixels[i].ToString();
            }
            tmpStr += "\n";

            File.AppendAllText(path, tmpStr);
        }

        private void buttonrecognize_Click(object sender, EventArgs e)
        {
            network.ForwardPass(inputPixels);
            label_output.Text = network.Fact.ToList().IndexOf(network.Fact.Max()).ToString();
            label_PROBALITY.Text = (100 * network.Fact.Max()).ToString("0.00") + " %";
        }

        private void FormMain_Load(object sender, EventArgs e)
        {

        }
    }
}
