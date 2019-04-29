using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Exam_70_483_Study
{
    class Objective1_1
    {
        [ThreadStatic]
        public static int _field;

        public static void ThreadMethod()
        {
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("ThreadProc: {0}", i);
                Thread.Sleep(0);
            }
        }

        public static void ThreadMethod(Object o)
        {
            for (int i = 0; i < (int)o; i++)
            {
                Console.WriteLine("ThreadProc2: {0}", i);
                Thread.Sleep(0);
            }
        }

        //Thread with parameter
        public static void Test01()
        {
            Thread t = new Thread(new ThreadStart(ThreadMethod));
            t.IsBackground = false;
            t.Start();

            Thread t2 = new Thread(new ParameterizedThreadStart(ThreadMethod));
            t2.Start(5);

            for (int i = 0; i < 4; i++)
            {
                Console.WriteLine("Main Thread: Do some work.");
                Thread.Sleep(0);
            }

            t.Join();
            t2.Join();

            Console.ReadKey();


        }

        //Thread with shared variable
        public static void Test02()
        {
            bool stopped = false;

            Thread t3 = new Thread(new ThreadStart(() =>
            {
                while (!stopped)
                {
                    Console.WriteLine("Running...");
                    Thread.Sleep(1000);
                }
            }));

            t3.Start();
            Console.WriteLine("Press 'E' to exit");

            while (!stopped)
            {
                ConsoleKeyInfo input = Console.ReadKey();

                if (input.KeyChar == 'E')
                {
                    stopped = true;
                }
            }
            t3.Join();
        }

        //Thread with ThreadStatic variable
        public static void Test03()
        {
            new Thread(() =>
            {
                for (int x = 0; x < 10; x++)
                {
                    _field++;
                    Console.WriteLine("Thread A: {0}", _field);
                    Thread.Sleep(50);
                }
            }
            ).Start();

            new Thread(() =>
            {
                for (int x = 0; x < 10; x++)
                {
                    _field++;
                    Console.WriteLine("Thread B: {0}", _field);
                    Thread.Sleep(50);
                }
            }
            ).Start();
            Console.ReadKey();
        }

        //ThreadPool
        public static void Test04()
        {
            ThreadPool.QueueUserWorkItem((s) =>
            {
                Console.WriteLine("Working on a thread from threadpool");
            });
        }

        //Task
        public static void Test05()
        {
            Task t = Task.Run(() => {
                for (int x = 0; x < 100; x++)
                {
                    Console.Write('*');
                }
            });

            t.Wait();
        }

        //Task with return
        public static void Test06()
        {
            Task<int> t = Task.Run(() => {
                return 42;
            });
            Console.WriteLine(t.Result);
        }

        //Continuation Task
        public static void Test07()
        {
            Task<int> t = Task.Run(() => {
                return 42;
            });

            t.ContinueWith((i) => {
                Console.WriteLine("Canceled");
            }, TaskContinuationOptions.OnlyOnCanceled);

            t.ContinueWith((i) => {
                Console.WriteLine("Faulted");
            }, TaskContinuationOptions.OnlyOnFaulted);

            var completedTask = t.ContinueWith((i) => {
                Console.WriteLine("Completed:{0}", i.Result);
            }, TaskContinuationOptions.OnlyOnRanToCompletion);

            completedTask.Wait();
        }

        //Child Tasks
        public static void Test08()
        {
            Task<Int32[]> parent = Task.Run(() => {
                var results = new Int32[3];
                var tasks = new List<Task>();
                tasks.Add(new Task(() => results[0] = 0, TaskCreationOptions.AttachedToParent));
                tasks.Add(new Task(() => results[1] = 1, TaskCreationOptions.AttachedToParent));
                tasks.Add(new Task(() => results[2] = 2, TaskCreationOptions.AttachedToParent));
                foreach(Task t in tasks) { t.Start(); }
                Task.WaitAll(tasks.ToArray());
                return results;
            });

            var finalTask = parent.ContinueWith<int>(parentTask => {
                int result = 0;
                foreach (int i in parentTask.Result)
                {
                    result += (int)Math.Pow(i + 2, 2);//(i + 3) * (i + 3);
                }
                return result;
            });

            Console.WriteLine("final result = {0}", finalTask.Result);
        }

        //TaskFactory with passing a state object and WaitAll method
        public static void Test09()
        {
            Task<Int32[]> parent = Task.Run(() => {
                var results = new Int32[3];

                TaskFactory tf = new TaskFactory(TaskCreationOptions.AttachedToParent, TaskContinuationOptions.ExecuteSynchronously);

                var tasks = new List<Task>();
                for (int i = 0; i < 3; i++)
                {
                    tasks.Add(
                    tf.StartNew((o) =>
                    {
                        int v = (int)o;
                        Console.WriteLine("Child Thread {0} is running.", v);
                        results[v] = v;
                        Thread.Sleep(v * 10 * (v + 5));
                    }, i));
                }

                Task.WaitAll(tasks.ToArray());

                return results;
            });

            var finalTask = parent.ContinueWith<int>(parentTask => {
                int result = 0;
                foreach (int i in parentTask.Result)
                {
                    result += (i + 3) * (i + 3);
                }
                return result;
            });

            Console.WriteLine("final result = {0}", finalTask.Result);
        }

        public static Task SleepAsyncA(int millisecondsTimeout)
        {
            return Task.Run(() => { Console.WriteLine("SleepA task is running."); Thread.Sleep(millisecondsTimeout); Console.WriteLine("SleepA task is finished."); });
        }

        public static Task<bool> SleepAsyncB(int millisecondsTimeout)
        {
            TaskCompletionSource<bool> tcs = null;
            var t = new Timer(delegate { Console.WriteLine("Timer callback is running."); tcs.TrySetResult(true); Console.WriteLine("Timer callback is finished."); }, null, -1, -1);
            tcs = new TaskCompletionSource<bool>(t);
            t.Change(millisecondsTimeout, -1);
            return tcs.Task;
        }

        public static void Test10()
        {
            Task t = SleepAsyncA(3000);
            Console.WriteLine("SleepA Task status: {0}", t.Status.ToString());
            //t.Wait();
            Console.WriteLine("SleepA Task status: {0}", t.Status.ToString());
        }

        public static void Test11()
        {
            Task<bool> t = SleepAsyncB(5000);
            Console.WriteLine("SleepB Task status: {0}", t.Status.ToString());
            //t.Wait();
            //Console.WriteLine("SleepB Task result: {0}", t.Result);
        }


        //Using BlockingCollection<T>
        public static void Test12()
        {
            BlockingCollection<string> col = new BlockingCollection<string>();
            Task read = Task.Run(() => {
                //while (true) { Console.WriteLine("Read task: {0}, collection size = {1}", col.Take(), col.Count); }
                foreach (string v in col.GetConsumingEnumerable()) { Console.WriteLine("Read task: {0}, collection size = {1}", v, col.Count); }
            });

            Task write = Task.Run(() => {
                while (true)
                {
                    string s = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(s)) { break; }
                    col.Add(s);
                    Console.WriteLine("Add task: {0}, collection size = {1}", s, col.Count);
                }
            });

            write.Wait();
        }

        //Enumerating a ConcurrentBag
        public static void Test13()
        {
            ConcurrentBag<int> bag = new ConcurrentBag<int>();
            Task.Run(() => {
                bag.Add(42);
                bag.Add(33);
                Thread.Sleep(1000);
                bag.Add(21);
            });
            Task.Run(() => {
                foreach (int i in bag) { Console.WriteLine("ConcurrentBag read: {0}", i); }
            }).Wait();
        }

        //Using a ConcurrentStack & a ConcurrentQueue
        public static void Test14()
        {
            ConcurrentStack<int> stack = new ConcurrentStack<int>();
            stack.Push(42);

            int result;
            if (stack.TryPop(out result)) { Console.WriteLine("Popped: {0}", result); }

            stack.PushRange(new int[] { 1, 2, 3 });
            int[] values = new int[2];
            stack.TryPopRange(values);
            foreach (int i in values) { Console.WriteLine("Popped: {0}", i); }
            stack.PushRange(values);


            values = new int[10];
            int count = stack.TryPopRange(values);
            int[] popValues = new int[count];
            Array.Copy(values, 0, popValues, 0, length: count);
            Console.WriteLine("PopRange count: {0}", count);
            foreach (int i in popValues) { Console.WriteLine("Popped: {0}", i); }


            ConcurrentQueue<int> queue = new ConcurrentQueue<int>();
            queue.Enqueue(42);
            if (queue.TryDequeue(out result)) { Console.WriteLine("Dequeued: {0}", result); }

        }
    }
}
