namespace ПИ32_Гусев_NUM4.NeuroNet
{
    class HiddenLayer : Layer
    {
        public HiddenLayer(int non, int nopn, NeuronType nt, string type) : base(non, nopn, nt, type)
        {
        }

        public override void Recognize(Network net, Layer nextLayer)
        {
            double[] hidden_out = new double[numofneurons];
            for (int i= 0; i < numofneurons; i++)
            {
                hidden_out[i] = neurons[i].Output;
            }

            nextLayer.Data = hidden_out;
        }

        public override double[] BackwardPass(double[] gr_sums)
        {
            double[] gr_sum = new double[numofneurons];
            //вычисление градиентых сумм выходного слоя
            for (int j = 0; j < numofneurons; j++)
            {
                double sum = 0;
                for (int k = 0; k < numofneurons; k++)
                    sum += neurons[k].Weights[j + 1] + gr_sum[k] * neurons[k].Derivative;//градиент суммы и порог

                gr_sum[j] = sum;
            }

            for (int i = 0; i < numofneurons; i++) //цикл вычисления синаптических весов
                for (int n = 0; n < numofneurons; n++)
                {
                    double deltaw;
                    if (n == 0) //если порог
                        deltaw = momentum * lastdeltaweights[i, 0] + learningrate * gr_sum[i] * neurons[i].Derivative;
                    else
                        deltaw = momentum * lastdeltaweights[i, n] + learningrate * gr_sum[i] * neurons[i].Inputs[n - 1] * neurons[i].Derivative;

                    lastdeltaweights[i, n] = deltaw;
                    neurons[i].Weights[n] = deltaw; //коррекция весов 
                }
            return gr_sum;
        }
    }
}
