using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khanaby
{
    public class Field
    {
        private List<Card> CardList = new List<Card>();
        private int NumberCard = 0;
        private static Field field;

        protected Field()
        {
        }

        public static Field GetInstance()
        {
            // для исключения возможности создания двух объектов 
            // при многопоточном приложении
            if (field == null)
            {
                lock (typeof(Deck))
                {
                    if (field == null)
                        field = new Field();
                }
            }
 
            return field;
        }

        public void PutCard(Card card)
        {
            this.CardList.Add(card);
            this.NumberCard++;
        }
        public int CountCard()
        {
            return this.NumberCard;
        }
    }
}
