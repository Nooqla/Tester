using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khanaby
{
    public sealed class StackCardList
    {
        private List<Card> stackCard = new List<Card>();
        private static StackCardList stack;

        private StackCardList()
        {
        }

        public static StackCardList GetInstance()
        {
            // для исключения возможности создания двух объектов 
            // при многопоточном приложении
            if (stack == null)
            {
                lock (typeof(StackCardList))
                {
                    if (stack == null)
                        stack = new StackCardList();
                }
            }
 
            return stack;
        }
        /*public  Card getCardForStack()
        {
            try
            {
                return this.stackCard[this.stackCard.Count - 1];
            }
            catch(ArgumentOutOfRangeException e)
            {
                throw new ArgumentOutOfRangeException();
            }
        }*/
        public void SetCardStack(Card card)
        {
            this.stackCard.Add(card);
        }
    }
}
