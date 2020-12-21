
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace oop_16
{
    class Program
    {

        static int[] Eratosthenes(int N)
        {
            int counter = 0;
            bool[] positions = new bool[N];

            for(int i = 2; i < N; i++)
            {
                positions[i] = true;
            }
            positions[0] = positions[1] = false;


            for (int i = 2; i < N; i++)
            {
                if (positions[i])
                {
                    for (int j = 2; i * j < N; j++)
                    {
                        positions[i * j] = false;
                    }
                }
            }

            foreach(bool pos in positions)
            {
                if (pos)
                    counter++;
            }

            int[] numbers = new int[counter];
            for (int i = 0, j = 0; i < N; i++)
            {
                if (positions[i])
                {
                    numbers[j] = i;
                    j++;
                }
            }

            return numbers; 
        }
        static int[] Eratosthenes(int N, CancellationToken token)
        {
            int counter = 0;
            bool[] positions = new bool[N];

            for (int i = 2; i < N; i++)
            {
                positions[i] = true;
            }
            positions[0] = positions[1] = false;


            for (int i = 2; i < N; i++)
            {
                if (token.IsCancellationRequested)
                {
                    Console.WriteLine("Задача прервавана токеном (1)");
                    return null;
                }

                if (positions[i])
                {
                    for (int j = 2; i * j < N; j++)
                    {
                        positions[i * j] = false;
                    }
                }
            }

            foreach (bool pos in positions)
            {
                if (token.IsCancellationRequested)
                {
                    Console.WriteLine("Задача прервавана токеном (2)");
                    return null;
                }
                if (pos)
                    counter++;
            }

            int[] numbers = new int[counter];
            for (int i = 0, j = 0; i < N; i++)
            {
                if (token.IsCancellationRequested)
                {
                    Console.WriteLine("Задача прервавана токеном (3)");
                    return null;
                }

                if (positions[i])
                {
                    numbers[j] = i;
                    j++;
                }
            }
            return numbers;
        }

        static void Main(string[] args)
        {
            Task<int[]> task1 = new Task<int[]>(() => Eratosthenes(1000000000));
            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();
            task1.Start();
            if (!task1.IsCompleted)
            {
                Console.WriteLine($"Выполняется таск1 | {task1.Status}");
            }
            stopwatch.Stop();
            TimeSpan timeSpan = stopwatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds / 10);
            Console.WriteLine("Отсортировали за " + elapsedTime + " - " + task1.Status);
            Console.WriteLine();

            //

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = cancellationTokenSource.Token;
            Task<int[]> task2 = new Task<int[]>(() => Eratosthenes(100000000, token));

            stopwatch.Restart();
            task2.Start();
            Console.WriteLine("Введите Y для отмены операции или другой символ для ее продолжения:");
            
            if (Console.ReadLine() == "Y")
            {
                cancellationTokenSource.Cancel();
            }

            stopwatch.Stop();
            timeSpan = stopwatch.Elapsed;
            elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds / 10);
            Console.WriteLine("Отсортировали за " + elapsedTime + " - " + task1.Status);
            Console.WriteLine();
   
            // 



            Console.ReadKey();
        }
    }
}
