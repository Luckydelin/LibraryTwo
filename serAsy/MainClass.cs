using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class MainClass
{
    public static void Main(string[] args)
    {
        Console.WriteLine("hello");
        ser serv=new ser();
        serv.Start("192.168.3.159", 51234);
        while(true)
        {
            string str =Console.ReadLine();
            switch(str)
            {
                case "quit":return;
            }
        }
    }
}
