using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Khanaby
{
    
    public sealed class DeckPlayer : Deck, IDeck 
    {
        const int MaxCardForHand = 5;
        List<Card> CardList = new List<Card>();
        int NumberCard = 0;
        public  override void Push(string[] path)
        {          
            foreach (string card in path)
            {
                    this.CardList.Add(new Card((card[0]).ToString(), Convert.ToInt32((card[1]).ToString())));
                    this.NumberCard++;  
            }
        }
        public  void Push(Card card)
        {
                this.CardList.Add(card);
                this.NumberCard++;   
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
        public Card GetCardToTell(int i)
        {
            try
            {
                Card card = (Card)this.CardList[i - 1].Clone();
                return card;
            }
            catch (ArgumentOutOfRangeException e)
            {
                throw new ArgumentOutOfRangeException();
            }
        }
        protected override void DeleteCard(int i)
        {
            try
            {
                this.NumberCard--;
                this.CardList.Remove(this.CardList[i - 1]);
            }
            catch (ArgumentOutOfRangeException e)
            {
                throw new ArgumentOutOfRangeException();
            }
        }

    }
}
