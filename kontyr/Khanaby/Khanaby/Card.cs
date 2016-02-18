using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Khanaby
{
    public class Card : ICloneable
    {
        private int _Number = 0;
        private int _Color = 0;
        private string _ShotColor ="";
        
        public Card( string color,int number)
        {
            int MaxLength = Enum.GetValues(typeof(RightValueCard.Number)).Length;
            int z = RegEx.HashSum(color);
            if (MaxLength >= number && number >= 1)
            {
                this._Number = number;
            }
            else
            {
                throw new CardException("Введите корректное число от 1 до 5");
            }
            int k = 0;
            foreach(int i  in Enum.GetValues(typeof(RightValueCard.ShotColor)))
            {
                k++;
                if (z == i)
                {
                    this._ShotColor = color;
                    this._Color = k;
                    break;
                }
                
            }
            if(this._Color == 0)
            {

                    throw new CardException("Введите корректный цвет");               
            }
            
        }
        public int Number
        {
            get { return this._Number; }
        }
        public string GetShortColor()
        {
            return this._ShotColor;
        }
        public int Color
        {
            get { return this._Color; }
        }
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        public string ToString()
        {
            return this._ShotColor + this._Number;
        }
    }
}
