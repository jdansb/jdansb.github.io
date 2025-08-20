using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp2
{
    internal class Program
    {
        static void Main()
        {
            int N = 200;
            int L = 5;
            double M = 500.0;//100.0 * N;
            double[,] E = new double[N,L];
            var A = new int[N];
            var Erro = new double[N];
            double[,] D = new double[N,L];
            var m = new double[N];
            double[] l =  { 1.0/1, 1.0 / 1.5, 1.0 / 2, 1.0 / 2.5, 1.0 / 3 }; // { 1.0/1, 1.0/2};//
            double[] c = { 1.0 / 10, 1.0 / 10, 1.0 / 10, 1.0 / 10, 1.0 / 10 }; // { 1.0 / 3, 1.0 / 3 };//
            List<double>[] trocas = new List<double>[L];
            // Inicializa cada lista
            for (int i = 0; i < trocas.Length; i++)
            {trocas[i] = new List<double>();}
            Random rnd = new Random(0);

            for (int i = 0; i < N; i++)
            { 
                for(int j = 0; j < L; j++)
                {
                    D[i, j] = 0.0;
                    E[i, j] = 0.0;
                }              
                m[i] = M / N;
                A[i] = rnd.Next(L);
            }

            for (int step = 0; step < 200000; step++)
            {
                
                for (int i = 0;  i < N;  i++)
                {
                    //P1
                    int k = A[i];
                    //C1
                    E[i, k] += l[k];
                    for (int j =0; j < L; j++)
                    {
                        D[i, j] += c[j];
                        int consumo = Math.Min((int)D[i, j], (int)E[i, j]);
                        D[i, j] -= consumo;
                        E[i, j] -= consumo;
                    }                   
                }

                List<int> Nres = new List<int>();
                for (int i = 0; i < L; i++)
                {Nres.Add(i);}
                int tentativa = 0;
                while (Nres.Count>0)
                {
                    tentativa++;
                    if (tentativa > N * L * 10) { break; }
                    int k = rnd.Next(Nres.Count);
                    int j = Nres[k];
                    List<int> compradores = new List<int>();
                    List<int> vendedores = new List<int>();
                    for (int i = 0; i < N; i++)
                    {
                        if ((int)E[i,j] > (int)D[i, j])
                        {vendedores.Add(i);}
                        if ((int)E[i, j] < (int)D[i, j])
                        { compradores.Add(i); }
                    }
                    if (vendedores.Count==0 || compradores.Count == 0)
                    {Nres.Remove(j); }
                    else
                    {   
                        int buy = compradores[rnd.Next(compradores.Count)];
                        int sell = vendedores[rnd.Next(vendedores.Count)];
                        double pbuy = rnd.NextDouble() * m[buy];
                        double psell = rnd.NextDouble() * m[sell];
                        double minimo = Math.Min(pbuy, psell);
                        double maximo = Math.Max(pbuy, psell);
                        double preco = rnd.NextDouble() * (maximo - minimo) + minimo;                       
                        if (m[buy] >= preco)
                        {   
                            m[buy] -= preco;
                            m[sell] += preco;
                            E[buy, j]++;
                            E[sell, j]--;
                            trocas[j].Add(preco);
                            
                        }
                    }

                }

                //S1
                if (step % 40 == 0)
                {
                    double e = 0;
                    for (int i = 0; i < N; i++)
                    { 
                        for (int j = 0; j < L; j++)
                        {
                            e += D[i, j]*D[i, j];
                        } 
                        e = Math.Sqrt(e);
                        if (e > Erro[i])
                        {
                            int k = rnd.Next(L);
                            A[i] = k;
                        }
                        Erro[i] = e;

                    }
                }
                if (step % 1000 == 0) 
                {
                    for (int i = 0; i < L; i++)
                    {
                        double media = 0; //= trocas[i].Sum() / (double)trocas[i].Count
                        foreach (var valor in trocas[i])
                        { media += valor;}
                        media /= trocas[i].Count;

                        Console.WriteLine($"{step} | Média de transações para o mercado {i}: {media} | trocas: {trocas[i].Count}");                        
                        trocas[i].Clear();
                    }
                    Console.WriteLine("");
                }
            }            
        }
    }
}
