
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Concurrent;


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



        static BlockingCollection<int> storage;

        public class Producer
        {
            public Producer(int i)
            {
                Thread myThread = new Thread(produce);
                myThread.Name = $"Producer {i}";
                myThread.Start();
            }
            static void produce()
            {
                for (int i = int.Parse(Thread.CurrentThread.Name.Substring(9)); i < 10 + int.Parse(Thread.CurrentThread.Name.Substring(9)); i++)
                {
                    storage.Add(i);
                    Console.WriteLine("Завезён товар " + i);
                    Thread.Sleep(100);
                }
                if (Thread.CurrentThread.Name.Equals("Producer 5"))
                    storage.CompleteAdding();
            }
        }

        public class Consumer
        {
            public Consumer(int i)
            {
                Thread myThread = new Thread(consume);
                myThread.Name = $"Consumer {i}";
                myThread.Start();
            }
            static void consume()
            {
                int i;
                while (!storage.IsCompleted)
                {
                    if (storage.TryTake(out i))
                        Console.WriteLine("Куплен товар " + i);
                }
            }
        }
        async static void FactorialAsync()
        {
            await Task.Run(() => {

                for (int i = 0; i < 50000; i++)
                {
                    int result = 1;
                    for (int ii = 1; ii <= i; ii++)
                    {
                        result *= ii;
                    }
                }
            });
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

            //

            int[] arrStraight = new int[100000000];
            stopwatch.Restart();
            for(int i = 0; i < arrStraight.Length; i++)
            {
                arrStraight[i] = i/task3.Result + 42; 
            }
            stopwatch.Stop();
            timeSpan = stopwatch.Elapsed;
            string time1 = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds / 10);


            stopwatch.Restart();
            int[] arrParallel = new int[100000000];
            Parallel.For(0, task3.Result, (i) =>
                {
                    arrParallel[i] = i / task3.Result + 42;
                });

            stopwatch.Stop();
            timeSpan = stopwatch.Elapsed;
            string time2 = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds / 10);

            Console.WriteLine($"Последовательным циклом for посчитали за {time1}, параллельным за {time2}") ;

            // 

            stopwatch.Restart();
            foreach(int x in arrParallel)
            {
                int buff = x / task3.Result + 42;
            }

            stopwatch.Stop();
            timeSpan = stopwatch.Elapsed;
            time1 = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds / 10);


            stopwatch.Restart();
            Parallel.ForEach(arrParallel, (a) =>
            {
                int buff = a / task3.Result + 42; 
            });

            stopwatch.Stop();
            timeSpan = stopwatch.Elapsed;
            time2 = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds / 10);

            Console.WriteLine($"Последовательным циклом foreach посчитали за {time1}, параллельным за {time2}");

            Console.WriteLine();

            //

            Parallel.Invoke(
            () =>
                {
                    for (int i = 100000; i < 150000; i++)
                    {
                        int result = 1;
                        for (int ii = 1; ii <= i; ii++)
                        {
                            result *= ii;
                        }
                    }
                    Console.WriteLine("Первый все");
                }, 
            () =>
                {
                    for (int i = 0; i < 50000; i++)
                    {
                        int result = 1;
                        for (int ii = 1; ii <= i; ii++)
                        {
                            result *= ii;
                        }
                    }
                    Console.WriteLine("Второй все");
                }, 
            () =>
                {
                    for (int i = 50000; i < 100000; i++)
                    {
                        int result = 1;
                        for (int ii = 1; ii <= i; ii++)
                        {
                            result *= ii;
                        }
                    }
                    Console.WriteLine("Третий всё");
                });

            // 

            storage = new BlockingCollection<int>(5);
            for (int i = 1; i < 11; i++)
            {
                Consumer consumer = new Consumer(i);
            }
            for (int i = 1; i < 6; i++)
            {
                Producer producer = new Producer(i);
            }


            FactorialAsync();

            Console.ReadLine();
        }
    }
}
