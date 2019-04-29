using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Exam_70_483_Study
{
    class Objective1_2
    {
        //LISTING 1-35 Accessing shared data in a multithreaded application
        public static void Test01()
        {
            int n = 0;

            var up = Task.Run(() => {
                for(int i = 0; i < 1000000; i++) { n++; }
            });

            for(int i = 0; i < 1000000; i++) { n--; }

            up.Wait();
            Console.WriteLine("Final result: {0}", n);
        }


        //LISTING 1-36 Using the lock keyword
        public static void Test02()
        {
            int n = 0;
            object _lock = new object();

            var up = Task.Run(() => {
                for (int i = 0; i < 1000000; i++) {
                    lock(_lock)
                        n++;
                }
            });

            for (int i = 0; i < 1000000; i++) {
                lock(_lock)
                    n--;
            }

            up.Wait();
            Console.WriteLine("Final result: {0}", n);
        }

        //LISTING 1-39 A potential problem with multithreaded code
        private static int _flag = 0;
        private static int _value = 0;
        public static void Thread1() { _value = 5; _flag = 1; }
        public static void Thread2() { if (_flag == 1) Console.WriteLine(_value); }
        public static void Test03()
        {
            Thread t1 = new Thread(new ThreadStart(Thread1));
            Thread t2 = new Thread(new ThreadStart(Thread2));
            t1.Start();
            t2.Start();
        }


        //LISTING 1-40 Using Interlocked class
        public static void Test04()
        {
            int n = 0;

            var up = Task.Run(() => {
                for (int i = 0; i < 1000000; i++) { Interlocked.Increment(ref n); }
            });

            for (int i = 0; i < 1000000; i++) { Interlocked.Decrement(ref n); }

            up.Wait();
            Console.WriteLine("Final result: {0}", n);
        }

        //LISTING 1-42 Using a CancellationToken
        public static void Test05()
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = cancellationTokenSource.Token;

            Task task = Task.Run(() => {
                while (!token.IsCancellationRequested)
                {
                    Console.Write("*");
                    Thread.Sleep(1000);
                }
            }, token);

            Console.WriteLine("Press Enter to stop the task.");
            Console.ReadLine();
            cancellationTokenSource.Cancel();

            Console.WriteLine("Press Enter to stop the application.");
            Console.ReadLine();
        }
    }
}
