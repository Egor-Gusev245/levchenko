using System;
using System.IO;
using System.Windows.Forms;

namespace ПИ32_Гусев_NUM4.NeuroNet
{
    abstract class Layer
    {
        protected string name_Layer;
        string pathDirWeights;
        string pathFileWeights;
        protected int numofneurons;
        protected int numofprevneurons;
        protected const double learningrate = 0.05d; // скорость обучения
        protected const double momentum = 0.050d; // момент инерции
        protected double[,] lastdeltaweights; // веса предыдущей итерации слоя
        protected Neuron[] neurons; // массив нейронов текущего слоя

        public double[] Data
        {
            set
            {
                for (int i = 0; i < numofneurons; i++)
                {
                    neurons[i].Activator(value);
                }
            }

        }

        protected Layer(int non, int nopn, NeuronType nt, string nm_Layer)
        {
            numofneurons = non;
            numofprevneurons = nopn;
            neurons = new Neuron[non];
            name_Layer = nm_Layer;
            pathDirWeights = AppDomain.CurrentDomain.BaseDirectory + "memory\\";
            pathFileWeights = pathDirWeights + name_Layer + "_memory.csv";

            lastdeltaweights = new double[non, nopn + 1];
            double[,] Weights;

            if (File.Exists(pathFileWeights))
                Weights = WeightInitialize(MemoryMode.GET, pathFileWeights);
            else
            {
                Directory.CreateDirectory(pathDirWeights);
                Weights = WeightInitialize(MemoryMode.INIT, pathFileWeights);
            }

            for (int i = 0; i < non; i++)
            {
                double[] tmp_weights = new double[nopn + 1];
                for (int j = 0; j < nopn + 1; j++)
                {
                    tmp_weights[j] = Weights[i, j];
                }
                neurons[i] = new Neuron(tmp_weights, nt);
            }
        }

        public double[,] WeightInitialize(MemoryMode mm, string path)
        {
            char[] delim = new char[] { ';', ' ' }; // Разделитель слов - ; и пробел
            string tmpStr; //Временная строка для чтения
            string[] tmpStrWeights; //Временный массив строк
            double[,] weights = new double[numofneurons, numofprevneurons + 1];

            switch (mm)
            {
                case MemoryMode.GET:
                    tmpStrWeights = File.ReadAllLines(path); // считывание строк текстового массива весов
                    string[] memory_element;
                    for (int i = 0; i < numofneurons; i++)
                    {
                        memory_element = tmpStrWeights[i].Split(delim);
                        for (int j = 0; j < numofprevneurons; j++)
                        {
                            weights[i, j] = double.Parse(memory_element[j].Replace(',', '.'),
                                System.Globalization.CultureInfo.InvariantCulture);
                        }
                    }
                    break;
                case MemoryMode.SET:
                    tmpStrWeights = new string[numofneurons];

                    if (!File.Exists(path))
                    {
                        MessageBox.Show("ВНИМАНИЕ " + name_Layer + "_memory.csv синаптических весов не найден! После кнопки ОК " +
                            "создастся отсутствующий файл",
                             "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }

                    for (int i = 0; i < numofneurons; i++)
                    {
                        tmpStr = neurons[i].Weights[0].ToString();
                        for (int j = 0;j < numofprevneurons; j++)
                        {
                            tmpStr += delim[0] + neurons[i].Weights[j].ToString();
                        }
                        tmpStrWeights[i] = tmpStr;
                    }
                    break;
                case MemoryMode.INIT:
                    if (!File.Exists(path))
                    {
                        MessageBox.Show("ВНИМАНИЕ " + name_Layer + "_memory.csv синаптических весов не найден! После кнопки ОК " +
                            "создастся отсутствующий файл",
                             "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }

                    Random random = new Random();

                    for (int i = 0; i < numofneurons; i++)
                    {
                        for (int j = 0; j < numofprevneurons + 1; j++)
                        {
                            weights[i, j] = random.NextDouble() * 0.11 - 0.01;
                        }
                    }

                    // === СОХРАНЕНИЕ СГЕНЕРИРОВАННЫХ ВЕСОВ В ФАЙЛ ===
                    tmpStrWeights = new string[numofneurons];
                    for (int i = 0; i < numofneurons; i++)
                    {
                        tmpStr = weights[i, 0].ToString(
                            System.Globalization.CultureInfo.InvariantCulture);
                        for (int j = 1; j < numofprevneurons + 1; j++)
                        {
                            tmpStr += delim[0] + weights[i, j].ToString(
                                System.Globalization.CultureInfo.InvariantCulture);
                        }
                        tmpStrWeights[i] = tmpStr;
                    }
                    File.WriteAllLines(path, tmpStrWeights);
                    break;
            }
            return weights;
        }

        abstract public void Recognize(Network net, Layer nextLayer); //прямой проход

        abstract public double[] BackwardPass(double[] stuff); //обратный
    }
}
