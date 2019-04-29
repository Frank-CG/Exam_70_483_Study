using System;
using System.Reflection;

namespace Exam_70_483_Study
{
    class Program
    {
        static void Main(string[] args)
        {
            String index = "02";
            String func = "Test" + index;
            String className = "Exam_70_483_Study." + "Objective2_1";
            Type t = Type.GetType(className);
            t.GetMethod(func, BindingFlags.Public|BindingFlags.Static).Invoke(null,null);
            Console.WriteLine("Test {0} is finished.", index);
            Console.ReadKey();
        }
    }
}
