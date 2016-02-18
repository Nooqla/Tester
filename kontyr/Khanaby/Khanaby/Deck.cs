using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Khanaby
{
    public class Deck : IDeck
    {
      
        private List<Card> CardList = new List<Card>();
        private int NumberCard = 0;
        private static  Deck deck;

        protected Deck()
        {
        }

        public  static Deck GetInstance()
        {
            // для исключения возможности создания двух объектов 
            // при многопоточном приложении
            if (deck == null)
            {
                lock (typeof(Deck))
                {
                    if (deck == null)
                        deck = new Deck();
                }
            }
 
            return deck;
        }
        public virtual void Push(string[] path)
        {
            var newPath = path.Select(item => item.ToUpperInvariant()).ToArray();
            newPath = newPath.Where(item => item != null).ToArray();
            Regex reg = new Regex("[YRGBW]+[1-5]", RegexOptions.IgnoreCase);
            foreach (string card in newPath)
            {
                if (reg.IsMatch(card))
                {
                    this.CardList.Add(new Card((card[0]).ToString(), Convert.ToInt32((card[1]).ToString())));
                    this.NumberCard++;
                }
            }
            this.CardList.Reverse();
        }

        public void ClearDeck()
        {
            this.CardList.Clear();
        }
        public int GetNumberCard()
        {
            return this.CardList.Count;
        }
        public Card GetCard(int i)
        {
            try
            {
                Card card = (Card)this.CardList[i - 1].Clone();
                this.DeleteCard(i);
                return card;
            }
            catch (ArgumentOutOfRangeException e)
            {
                throw new ArgumentOutOfRangeException();
            }
        }
        protected  virtual void DeleteCard(int i)
        {
            try
            {

                this.CardList.Remove(this.CardList[i - 1]);
                this.NumberCard--;
            }
            catch (ArgumentOutOfRangeException e)
            {
                throw new ArgumentOutOfRangeException();
            }
        }
    }
}
