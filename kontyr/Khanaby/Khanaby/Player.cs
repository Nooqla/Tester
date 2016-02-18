using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Khanaby
{
    public sealed class Player
    {
        private DeckPlayer myDeck = new DeckPlayer();
        private int CountCardForHand = 0;
        private bool Risk = true;
        private int[,] CRisk = new int[5, 5];
        public Player()
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    this.CRisk[i, j] = 0;
                }
            }

        }
        public int GetCRisk(int i, int j)
        {
            return this.CRisk[i, j];
        }
        public void SetCrisk(int i, int j, int value)
        {
            this.CRisk[i, j] = value;
        }
        public void Push(string[] path)
        {
            var newPath = path.Where(x => x != null).ToArray();
            if (newPath.ToArray().Length >= 1)
            {
                myDeck.Push(newPath.ToArray());
            }
            this.CountCardForHand = myDeck.GetNumberCard();
        }
        public void Push(Card card)
        {
            this.myDeck.Push(card);
            this.CountCardForHand++;
        }
        public Card Play(int i)
        {
            try
            {
                this.CountCardForHand--;
                return myDeck.GetCard(i);
            }
            catch(ArgumentOutOfRangeException e)
            {
                throw new CardException("Нет такой карты в руке");
            }
        }
        public Card Drop(int i)
        {
            try
            {
                this.CountCardForHand--;
                return this.myDeck.GetCard(this.myDeck.GetNumberCard());
            }
            catch (ArgumentOutOfRangeException e)
            {
                throw new CardException("Нет такой карты в руке");
            }
            
        }
        public void TellColor(string s)
        {
            string[] tell = null;
            while (true)
            {
                String[] words = s.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var newPath = words.Select(item => item.ToUpperInvariant()).ToArray();
                s = String.Join(" ", newPath);
                string[] ss = new string[3] { "COLOR", "RED | BLUE | GREEN | WHITE | YELLOW", "[0-4]" };
                if (RegEx.ReSpeak(s, ss))
                    break;
               Console.WriteLine("Введите корректную строку");              
            }
        }
        public void TellRank(string s)
        {
            string[] tell = null;
            while (true)
            {
                String[] words = s.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var newPath = words.Select(item => item.ToUpperInvariant()).ToArray();
                s = String.Join(" ", newPath);
                string[] ss = new string[2] { "TELL", "[0-4]"};
                if (RegEx.ReSpeak(s, ss))
                            break;
                Console.WriteLine("Введите корректную строку");
            }
        }
        public bool Filed
        {
            get { return this.Risk;  }
            set { this.Risk = value; }
        }
        public int GetCountForHand()
        {
            return this.CountCardForHand;
        }
        public override string ToString()
        {
            string s = "";
            for (int i = 1; i <= this.CountCardForHand; i++)
            {
                Card card = myDeck.GetCardToTell(i);
                s += card.ToString() +" ";
            }
            return s;
        }
    }
}
