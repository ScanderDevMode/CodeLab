using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testApp
{
    internal class testApp
    {

        public static void Main() {

            int i = 6;


            string s = i.ToString("##");

            Console.WriteLine(s);

            string str = String.Format("{0:g}", DateTime.Now.TimeOfDay);

            Console.WriteLine(str);
        }


    }
}
