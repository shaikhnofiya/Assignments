using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Abstract
{
    abstract class mcn
    {
        public int add(int a, int b)
        {
            return (a + b);
        }
    }
    class mcn1 : mcn
    {
        public int mul(int a, int b)
        {
            return a * b;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            mcn1 ob = new mcn1();
            int result = ob.add(5, 10);
            Console.WriteLine("the result is {0}", result);
        }
    }
}
