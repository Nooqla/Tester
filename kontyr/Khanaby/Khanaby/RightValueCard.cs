using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khanaby
{
    public static  class RightValueCard
    {
       public  enum Number   { One, Two, Three, Four, Five };
       public enum Color     { Red = 1, White = 2, Yellow = 3, Blue = 4, Green = 5};
       public enum ShotColor { R = 23, W = 43, Y = 51, B = 61, G = 81};
       public enum Command   { Play =1 , Drop =2 , Color=3, Rank=4 };
    }
}
