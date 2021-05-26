using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace function_overloading
{
    class shape
    {
        public void Area(int side)
        {
            int squarearea = side * side;
            Console.WriteLine("The Area of Square is :" + squarearea);
        }
        public void Area(int length, int breadth)
        {

            int rectarea = length * breadth;
            Console.WriteLine("The Area of Rectangle is :" + rectarea);
        }

        public void Area(double radius)
        {
            double circlearea = 3.14 * radius * radius;
            Console.WriteLine("The Area of Circle is :" + circlearea);
        }

    }
    class Program
    {
        static void Main(string[] args)
        {
            shape s = new shape();
            s.Area(10);
            s.Area(10, 20);
            s.Area(10.8);
            Console.ReadKey();

        }
    }
}
