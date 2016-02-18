using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khanaby
{
    class Program
    {
        static void Main(string[] args)
        {
            
            
           
            try
            {
               // Console.WriteLine("Start game with deck R1 R2 G3 B2 G2 Y1 Y2 R2 B3 R2 R5 Y1 Y2 W5 B5");
                Game game = Game.GetInstance(); 

            }
            catch (CardException e)
            {
                Console.WriteLine(e.Message);
            }

            
            Console.ReadLine();
            
        }

    }
}
