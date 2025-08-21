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
            double M = 500.0; //100.0 * N;
            double[,] E = new double[N, L];
            var A = new int[N];
            var Error = new double[N];
            double[,] D = new double[N, L];
            var money = new double[N];
            double[] productivity = { 1.0/1, 1.0 / 1.5, 1.0 / 2, 1.0 / 2.5, 1.0 / 3 }; // { 1.0/1, 1.0/2}; //
            double[] consumption = { 1.0 / 10, 1.0 / 10, 1.0 / 10, 1.0 / 10, 1.0 / 10 }; // { 1.0 / 3, 1.0 / 3 }; //
            List<double>[] trades = new List<double>[L];
            // Initialize each list
            for (int i = 0; i < trades.Length; i++)
            { trades[i] = new List<double>(); }
            Random rnd = new Random(0);

            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < L; j++)
                {
                    D[i, j] = 0.0;
                    E[i, j] = 0.0;
                }
                money[i] = M / N;
                A[i] = rnd.Next(L);
            }

            for (int step = 0; step < 200000; step++)
            {
                for (int i = 0; i < N; i++)
                {
                    // P1
                    int k = A[i];
                    // C1
                    E[i, k] += productivity[k];
                    for (int j = 0; j < L; j++)
                    {
                        D[i, j] += consumption[j];
                        int demand = Math.Min((int)D[i, j], (int)E[i, j]);
                        D[i, j] -= demand;
                        E[i, j] -= demand;
                    }
                }

                List<int> availableMarkets = new List<int>();
                for (int i = 0; i < L; i++)
                { availableMarkets.Add(i); }
                int attempts = 0;
                while (availableMarkets.Count > 0)
                {
                    attempts++;
                    if (attempts > N * L * 10) { break; }
                    int k = rnd.Next(availableMarkets.Count);
                    int j = availableMarkets[k];
                    List<int> buyers = new List<int>();
                    List<int> sellers = new List<int>();
                    for (int i = 0; i < N; i++)
                    {
                        if ((int)E[i, j] > (int)D[i, j])
                        { sellers.Add(i); }
                        if ((int)E[i, j] < (int)D[i, j])
                        { buyers.Add(i); }
                    }
                    if (sellers.Count == 0 || buyers.Count == 0)
                    { availableMarkets.Remove(j); }
                    else
                    {
                        int buy = buyers[rnd.Next(buyers.Count)];
                        int sell = sellers[rnd.Next(sellers.Count)];
                        double pbuy = rnd.NextDouble() * money[buy];
                        double psell = rnd.NextDouble() * money[sell];
                        double minimum = Math.Min(pbuy, psell);
                        double maximum = Math.Max(pbuy, psell);
                        double price = rnd.NextDouble() * (maximum - minimum) + minimum;
                        if (money[buy] >= price)
                        {
                            money[buy] -= price;
                            money[sell] += price;
                            E[buy, j]++;
                            E[sell, j]--;
                            trades[j].Add(price);
                        }
                    }
                }

                // S1
                if (step % 40 == 0)
                {
                    double e = 0;
                    for (int i = 0; i < N; i++)
                    {
                        for (int j = 0; j < L; j++)
                        {
                            e += D[i, j] * D[i, j];
                        }
                        e = Math.Sqrt(e);
                        if (e > Error[i])
                        {
                            int k = rnd.Next(L);
                            A[i] = k;
                        }
                        Error[i] = e;
                    }
                }
                if (step % 1000 == 0)
                {
                    for (int i = 0; i < L; i++)
                    {
                        double average = 0; // = trades[i].Sum() / (double)trades[i].Count
                        foreach (var value in trades[i])
                        { average += value; }
                        average /= trades[i].Count;

                        Console.WriteLine($"{step} | Average transaction price for market {i}: {average} | trades: {trades[i].Count}");
                        trades[i].Clear();
                    }
                    Console.WriteLine("");
                }
            }
        }
    }
}
