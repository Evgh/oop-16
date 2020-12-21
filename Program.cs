
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
        static void Main(string[] args)
        {
            Task<int[]> task1 = new Task<int[]>(() => { return new int[1]; });
            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();
            task1.Start();
            while (!task1.IsCompleted)
            {
                Console.WriteLine($"Выполняется таск | {task1.Status}");
            }
            stopwatch.Stop();
            TimeSpan timeSpan = stopwatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds / 10);
            Console.WriteLine("Отсортировали за " + elapsedTime + " - " + task1.Status);
            Console.WriteLine();

            //

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = cancellationTokenSource.Token;

            Console.ReadKey();
        }
    }
}
