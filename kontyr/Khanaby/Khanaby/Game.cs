using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Khanaby
{
    public class Game
    {
        #region Filed
        private static Game game;
        private Deck Deck = Deck.GetInstance();
        private Field FieldCard = Field.GetInstance();
        private List<Player> PlayerList = new List<Player>();
        private StackCardList ScL = StackCardList.GetInstance();
        const int MaxCountForHand = 5;
        private bool Conflict = false;
        private bool GameState = true;
        private int CT = 0;
        private int Turn = 0;
        #endregion
        public static Game GetInstance()
        {

            if (game == null)
            {
                lock (typeof(Deck))
                {
                    if (game == null)
                        game = new Game(s);
                }
            }

            return game;
        }
        private Game()
        {
            #region GameStart
            PlayerList.Add(new Player());
            PlayerList.Add(new Player());
            
            while (true)
            {
                foreach (Player pl in PlayerList)
                {
                    int k = pl.GetCountForHand();
                    for (int i = 0; i <k ; i++)
                    {
                        pl.Drop(i + 1);
                    }
                }
                string s = Console.ReadLine();
                String[] words = s.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var newPath = words.Select(item => item.ToUpperInvariant()).ToArray();
                s = String.Join(" ", newPath);
                var rez = new List<string>();
                string[] ss = new string[2] { "START NEW GAME WITH DECK", "[YRGBW]+[1-5]" };
                string[] j = new string[1] { "[YRGBW]+[1-5]" };
                List<string> player = new List<string>();
                string[] array = new string[25];
                if (RegEx.ReSpeak(s, ss))
                {
                    for (int i = 0; i < words.Length; i++)
                    {
                        if (RegEx.ReSpeak(words[i], j))
                        {
                            player.Add(words[i]);

                        }
                    }
                    foreach (Player pl in PlayerList)
                    {
                        player.CopyTo(0, array, 0, MaxCountForHand);
                        pl.Push(array);
                        player.RemoveRange(0, MaxCountForHand);
                        array.ToList().Clear();

                    }
                    Deck.Push(player.ToArray());
                    if (Deck.GetNumberCard() == 0)
                    {
                        Console.WriteLine("Не достаточно карт");
                        continue;
                    }
                }
                else
                {
                    Console.WriteLine("Введите корректную строку");
                    continue;
                }
                break;

            }
            #endregion
            #region GameLogic
            while ((Deck.GetNumberCard() != 1 || FieldCard.CountCard() != 25) && (!Conflict))
            {
                for (int i = 0; i < PlayerList.Count; i++)
                {
                    this.Turn++;
                    if (i == 0)
                    {
                        Console.SetCursorPosition(0, 1);
                        Console.WriteLine("Next Player: " + PlayerList[i + 1].ToString());
                        if (CT != 0)
                        {
                            Console.SetCursorPosition(0, CT + 1);
                        }
                        else
                        {
                            Console.SetCursorPosition(0, this.Turn + 1);
                        }

                    }
                    else
                    {
                        Console.SetCursorPosition(0, 1);
                        Console.WriteLine("Next Player: " + PlayerList[i - 1].ToString());
                        if (CT != 0)
                        {
                            Console.SetCursorPosition(0, CT + 1);
                        }
                        else
                        {
                            Console.SetCursorPosition(0, this.Turn + 1);
                        }
                    }
                    string command = Console.ReadLine();
                    int c = RegEx.GetCommand(command);
                    if (this.CommandNow(c, PlayerList[i], command))
                    {
                        CT = Console.CursorTop;
                        command = Console.ReadLine();
                        c = RegEx.GetCommand(command);
                        this.CommandNow(c, PlayerList[i], command);
                    }
                    if (!GameState)
                    {
                        Conflict = true;
                        break;
                    }
                }


            }
            #endregion

        }


        private bool CommandNow(int c, Player pl, string s)
        {
            try
            {
                char[] xx = s.Where(x => Char.IsDigit(x)).ToArray();
                if (c == 1)
                {
                    Regex reg = new Regex("[0-4]");
                    if (reg.IsMatch(s))
                    {
                        for (int i = 0; i < xx.Length; i++)
                        {
                            if (Deck.GetNumberCard() == 1 && GameState)
                            {
                                GameState = false;
                                break;
                            }
                            else
                            {
                                FieldCard.PutCard(pl.Play(Convert.ToInt32(xx[i].ToString()) - i + 1));
                                pl.Push((Card)Deck.GetCard(1));
                            }

                        }

                    }
                }
                else if (c == 2)
                {
                    Regex reg = new Regex("[0-4]");
                    if (reg.IsMatch(s))
                    {
                        for (int i = 1; i <= xx.Length; i++)
                        {
                            ScL.SetCardStack(pl.Drop(Convert.ToInt32(xx[i].ToString()) - i + 1));
                        }
                    }
                }
                else if (c == 3)
                {
                    Regex reg = new Regex("[0-4]");
                    if (reg.IsMatch(s))
                    {


                        pl.TellColor(s);

                    }
                }
                else if (c == 4)
                {
                    Regex reg = new Regex("[0-4]");
                    if (reg.IsMatch(s))
                    {


                        pl.TellRank(s);

                    }

                }
                else
                {

                    throw new CardException("Введите корректную команду");
                }

            }
            catch (CardException ex)
            {
                Console.WriteLine(ex.Message);
                return true;
            }
            return false;

        }
    }
}
