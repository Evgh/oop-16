
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
            Task<int[]> task1 = new Task<int[]>(() => Eratosthenes(100000000));
            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();
            task1.Start();
            if (!task1.IsCompleted)
            {
                Console.WriteLine($"Выполняется таск1 | {task1.Status}");
            }
            task1.Wait();
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
            task2.Wait();
            stopwatch.Stop();
            timeSpan = stopwatch.Elapsed;
            elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds / 10);
            Console.WriteLine("Отсортировали за " + elapsedTime + " - " + task1.Status);
            Console.WriteLine();

            // 

            Task<int> task3 = new Task<int>(() => { var rr = new Random(); return rr.Next(3, 15); });
            task3.Start();

            Task<int[][]> task4 = new Task<int[][]>(() => { if (task2.Result == null)
                                                                return null;        

                                                            int[][] arr = new int[task3.Result][];
                                                            for (int i = 0; i < arr.Length; i++) 
                                                            {
                                                                arr[i] = new int[task3.Result];
                                                            }    
                                                            for (int i = 0; i < arr.Length; i++) 
                                                            {
                                                                for (int j = 0; j < arr[i].Length; j++) 
                                                                {
                                                                    arr[i][j] = task1.Result[i]+task2.Result[j];
                                                                }               
                                                                
                                                            }    
                                                            return arr;
                                                          });
            task4.Start();
            task4.Wait();

            if (task4.Result != null)
            {
                foreach (int[] a in task4.Result)
                {
                    foreach (int b in a)
                    {
                        Console.Write($"{b} ");
                    }
                    Console.WriteLine();
                }

            }

            //

            Task fifth = new Task(() => Console.WriteLine("O HI Mark"));
            Task sixth = new Task(() => Console.WriteLine("Anyway"));
            Task seventh = sixth.ContinueWith((t) => { Console.WriteLine("How is your"); });
            Task eighth = new Task(() => Console.WriteLine("Session life"));

            sixth.Start();
            eighth.Start();
            eighth.GetAwaiter().GetResult();
            fifth.Start();


            Console.ReadKey();
        }
    }
}
