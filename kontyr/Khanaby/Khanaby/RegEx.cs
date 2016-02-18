using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Khanaby
{
    public static  class RegEx
    {
        public static int HashSum(string a)
        {
            int hashsum = 0;
            for (int i = 0; i < a.Length; i++)
            {
                hashsum += a[i].GetHashCode() % 51 + a[a.Length - i - 1].GetHashCode() % 51;
            }
            return hashsum + Convert.ToInt32(Math.Pow(Convert.ToDouble(a.Length.GetHashCode()), 3D));
        }
        public static bool ReSpeak(string s, string[] ss)
        {
               
               List<Regex> reg = new List<Regex>();
               bool solution = false;
               for (int i = 0; i < ss.Length; i++)
               {
                    reg.Add(new Regex(ss[i], RegexOptions.IgnoreCase));            
               }

               foreach (Regex r in reg)
               {
                   if (r.IsMatch(s))
                   {
                       solution = true;
                   }
                   else
                   {
                       solution = false;
                   }
               }
               return solution;
        }
        public static int GetCommand(string s)
        {
            int c = 0;


            if (s.ToLower().Contains("play"))
                {
                    c = 1;
                }
                else if(s.ToLower().Contains("drop"))
                {
                    c =2;
                }
            else if (s.ToLower().Contains("color"))
                {
                    c = 3;
                }
            else if (s.ToLower().Contains("rank"))
                {
                    c = 4;
                }
                else
                {
                    c = -1;
                }
            
            return c;
        }
    
        
    }
}
