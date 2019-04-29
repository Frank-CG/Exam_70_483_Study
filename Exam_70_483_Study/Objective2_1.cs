using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exam_70_483_Study
{
    class Objective2_1
    {
        //Using the FlagAttribute for an enum
        [Flags]
        enum Days { None = 0x0, Sunday = 0x01, Monday = 0x02, Tuesday = 0x04, Wednesday = 0x08, Thursday = 0x10, Friday = 0x20, Saturday = 0x40 }

        public static void Test01()
        {
            Days readingDays = Days.Monday | Days.Saturday;
            Console.WriteLine("Reading days = {0} (0x{1:X})", readingDays,(int)readingDays);
        }

        //Creating a custom struct
        public struct Point
        {
            public int x, y;
            public Point(int p1, int p2) { x = p1; y = p2; }

            public override string ToString()
            {
                return "Point coordinate: (" + x + ", " + y + ")";
            }
        }

        public static void Test02()
        {
            Point pt = new Point(0, 1);
            Console.WriteLine(pt);
        }
    }
}
