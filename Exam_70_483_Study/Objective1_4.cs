using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exam_70_483_Study
{
    class Objective1_4
    {
        //LISTING 1-75 Using a delegate
        public delegate int Calculate(int x, int y);

        public static int Add(int x, int y) { return x + y; }
        public static int Multiply(int x, int y) { return x * y; }

        public static void Test01()
        {
            Calculate calc = Add;
            Console.WriteLine(calc(3, 4));

            calc = Multiply;
            Console.WriteLine(calc(3, 4));
        }


        //LISTING 1-76 A multicast delegate
        public static void MethodOne() { Console.WriteLine("Method One"); }
        public static void MethodTwo() { Console.WriteLine("Method Two"); }
        public delegate void Del();
        public static void Test02()
        {
            Del d = MethodOne;
            d += MethodTwo;
            d += MethodTwo;

            d();
        }
    }
}
