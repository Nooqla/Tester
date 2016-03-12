using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Game
{
    class Glushkov_Gleb
    {
        static void Main(string[] args)
        {
            //true - есть рискованные ходы
            //false - нету
            
            
            Game game = new Game(true);
            string s;
            do
            {
                s = Console.ReadLine();
                if (s != null)
                    game.SetAction(s);
                else
                    break;
            } while (true);
        }
        class Graph
        {
            private int[,] graph;
            public Graph(int i, int j)
            {
                this.graph = new int[i, j];

            }
            private int Size()
            {
                return this.graph.GetLength(0);
            }
            public bool ComparePos(int i, int j)
            {
                if (Compare(this.graph[i, j]))
                    return true;
                return false;
            }
            public void SetValue(int i, int j, int value)
            {
                this.graph[i, j] = value;
            }
            public int GetValue(int i, int j)
            {
                return this.graph[i, j];
            }
            private bool Compare(int x)
            {
                if (x == 1)
                    return true;
                return false;
            }
            //i - строка , j - столбцы , r - глубина
            public bool FindWay(int i, int j, int r)
            {
                if (r == 0)
                    return true;
                else if (r == j && ComparePos(i, r - 1))
                    return true;
                else if (r == j && !ComparePos(i, r - 1))
                    return false;
                else
                    return FindWay(i, ++j, r);
            }
            public void Clear()
            {
                for (int i = 0; i < this.graph.GetLength(0); i++)
                {
                    for (int j = 0; j < this.graph.GetLength(1); j++)
                    {
                        this.graph[i, j] = 0;
                    }
                }
            }
            public int FindWay(int i, int j)
            {
                if (ComparePos(i, j))
                    return 1;
                return 0;
            }
        }
        static class InfoForRegex
        {
            public const int MaxCardForHand = 5;
            private static string _ParseCard = "[YRGBW]+[1-5]";
            private static string _ParseStart = "START NEW GAME WITH DECK";
            private static string _ParsePlay = "PLAY CARD [0-4]";
            private static string _ParseDrop = "DROP CARD [0-4]";
            private static string _ParseTC = "TELL COLOR ((YELLOW)|(RED)|(BLUE)|(WHITE)|(GREEN)) FOR CARDS ([0-4]+)";
            private static string _ParseTR = "TELL RANK [1-5] FOR CARDS ([0-4]+)";
            private static string[] InfoParse = new string[6] { _ParseCard, _ParseStart, _ParsePlay, _ParseDrop, _ParseTC, _ParseTR };
            public enum Color { Red = 3, White = 4, Yellow = 5, Blue = 1, Green = 2 };
            public enum ShotColor { R = 3, W = 4, Y = 5, B = 1, G = 2 };
            public enum Command { Play, Drop, Color, Rank };
            public static string GetInfoParse(int i)
            {
                return InfoParse[i];
            }
            public static int GetShotColor(string s)
            {
                switch (s)
                {
                    case ("B"):
                        return 0;
                    case ("G"):
                        return 1;
                    case ("R"):
                        return 2;
                    case ("W"):
                        return 3;
                    case ("Y"):
                        return 4;
                    default:
                        return -1;
                }
            }
            public static string GetIntByColor(int s)
            {
                switch (s)
                {
                    case (0):
                        return "B";
                    case (1):
                        return "G";
                    case (2):
                        return "R";
                    case (3):
                        return "W";
                    case (4):
                        return "Y";
                    default:
                        return null;
                }
            }
        }
        static class ParseString
        {
            public static string[] GetUpperStrToArray(string s)
            {
                string[] words = s.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                return words.Select(item => item.ToUpperInvariant()).Where(item => item != null).ToArray();
            }
            //Проверка регулярными выражениями на корректность вводимых данных
            public static bool ControlCard<T>(IEnumerable<T> path, string s)
            {
                List<Regex> _Reg = new List<Regex>();
                bool _Solution = false;
                s = s.ToUpper();
                if (path is String)
                {
                    _Reg.Add(new Regex(path.ToString(), RegexOptions.Compiled));
                }
                else if (path is Array)
                {
                    var newPath = path.Select(item => item.ToString().ToUpperInvariant()).ToArray();
                    foreach (var p in path)
                    {
                        _Reg.Add(new Regex(p.ToString(), RegexOptions.Compiled));
                    }
                }
                foreach (Regex r in _Reg)
                {
                    if (r.IsMatch(s))
                    {
                        _Solution = true;
                    }
                    else
                    {
                        _Solution = false;
                        break;
                    }
                }
                return _Solution;
            }
            //Получить все числа из строки
            public static char[] GetDigit(string s)
            {           
                return  s.Where(x => Char.IsDigit(x)).ToArray();
            }
            //Скомпоновать данные 1 элемент - команда(новая игра,сыграть карту,сбросить карту,подсказать цвет,подсказать ранг), затем данные для определеного действия игрока
            public static Tuple<int,List<string>> GeneralParse(string s)
            {
                s = s.ToUpper();
                int k = -1;
                List<string> _data = new List<string>();
                for (int i = 1; i < 6; i++)
                {
                    if (ControlCard(InfoForRegex.GetInfoParse(i), s))
                    {
                        k = i - 1;
                        break;
                    }
                }
                if (k == 0)
                {
                    Regex reg = new Regex("START NEW GAME WITH DECK ");
                    var cards = ParseString.GetUpperStrToArray(reg.Replace(s, " "));
                    foreach (string card in cards)
                    {
                        if (ControlCard(InfoForRegex.GetInfoParse(0), card))
                        {
                            _data.Add(card);
                        }
                    }                    
                }
                else if (k != -1)
                {
                    char[] xx = HelperParse(s);
                    foreach (char x in xx)
                        _data.Add(x.ToString());
                }
                if (k == -1)
                    throw new CardException("Не известная команда");
                return Tuple.Create<int, List<string>>(k, _data);
            }
            //Скомпоновать вспомогательные данные для корректного выполнения хода
            private static char[] HelperParse(string s)
            {
                var newPath = GetUpperStrToArray(s);
                bool IsDigit = newPath[2].ToString().Length == newPath[2].ToString().Where(c => char.IsDigit(c)).Count();
                if (!IsDigit)
                {
                    foreach (var value in Enum.GetValues(typeof(InfoForRegex.Color)))
                    {
                        if (newPath[2].ToString().Contains(value.ToString().ToUpper()))
                        {
                            newPath[2] = (Convert.ToInt32((int)value)).ToString();
                            break;
                        }
                    }
                }
                return GetDigit(String.Join(" ", newPath));
            }           
        }
        interface IPlayer
        {
            Action<string> Command { get; set; } //Подписка на событие для смены хода
            void Push(Card card);               // Добавить карту в руку 
            void Push(string path);
            void Clear();                       //Очистить руку
            Card PlayerAction(string s, InfoForRegex.Command c, string _s);//Выбор действия 
        }
        //действия которые могут выполняться игроками
        abstract class ParentPlayer
        {
            protected abstract Card Play(int i, string _s);
            protected abstract Card Drop(int i);
            protected abstract void TellColor(string[] s);
            protected abstract void TellRank(string[] s);
            public abstract int GetCountCard();
        }
        class Player : ParentPlayer, IPlayer
        {
            #region filed
            //Карты в руке
            private PlayerDeck myDeck = new PlayerDeck();
            //То ,что сказал игрок для другого
            private int[] _IsSay;
            //Корректность карты
            private bool _CorrectCard = false;
            //Граф для сохранения информации
            private Graph _graphColorChange = new Graph(InfoForRegex.MaxCardForHand, InfoForRegex.MaxCardForHand);
            private Graph _graphRankChange = new Graph(InfoForRegex.MaxCardForHand, InfoForRegex.MaxCardForHand);
            private string _name;
            private readonly bool IsLevel;
            private readonly int CountCard;
            #endregion
            public Player(string s, bool level)
            {
                this._name = s;
                this.IsLevel = level;
                this.CountCard = InfoForRegex.MaxCardForHand;
            }
            public void SetColor(int i, int j, int value)
            {               
                SetChange(i, j, value, _graphColorChange);
            }
            public void SetRank(int i, int j, int value)
            {              
                SetChange(i, j, value, _graphRankChange);
            }
            private void SetChange(int i, int j, int value, Graph _graph)
            {
                bool p = false;
                for (int k = 0; k < CountCard; k++)
                {
                    _graph.SetValue(k, j, 0);
                }
                _graph.SetValue(i, j, 1);              
                for (int k = 0; k < CountCard; k++)
                {
                    for (int h = 0; h < CountCard; h++)
                    {
                        for (int z = 0; z < CountCard; z++)
                        {
                            if (_graph.GetValue(z, h) == 1)
                            {
                                p = true;
                                break;
                            }
                        }
                        if (p) { }
                        else if (h != j)
                            if (_graph.GetValue(k, h) != 3)
                                _graph.SetValue(k, h, 2);
                        p = false;
                    }

                }
                bool pp = false;
                for (int z = 0; z < CountCard; z++)
                {
                    for (int h = 0; h < CountCard; h++)
                    {
                        if (_graph.GetValue(h, z) == 1)
                        {
                            pp = true;
                            break;
                        }
                    }
                    if (!pp)
                        _graph.SetValue(i, z, 3);
                    pp = false;
                }
            }           
            private void CheckChange(int i, Graph gr, bool cond, int value, string _s)
            {
                int possibility = 0;
                int pos = 0;
                for (int k = 0; k < CountCard; k++)
                {
                    if (gr.GetValue(k, i) == 2)
                    {
                        pos++;
                        if (cond)
                        {
                            if (_s.Contains(InfoForRegex.GetIntByColor(value) + (k)))
                                possibility++;
                        }
                        else
                        {
                            if (_s.Contains(InfoForRegex.GetIntByColor(k) + (value)))
                                possibility++;
                        }
                    }
                }

                if (possibility == pos)
                    _CorrectCard = true;
                else
                    _CorrectCard = false;
            }           
            #region не игровые функции
            public void Push(Card card)
            {
                myDeck.Push(card);
            }
            public void Push(string path)
            {
                myDeck.Push(path);
            }
            public bool CorrectCard
            {
                get { return _CorrectCard; }
            }
            public void Clear()
            {
                myDeck.Clear();
                _graphColorChange.Clear();
                _graphRankChange.Clear();
            }
            protected virtual void SetNewInfo(int i)
            {
                for (int j = 0; j < CountCard; j++)
                {
                    for (int k = i; k < CountCard - 1; k++)
                    {
                        _graphColorChange.SetValue(j, k, _graphColorChange.GetValue(j, k + 1));
                        _graphRankChange.SetValue(j, k, _graphRankChange.GetValue(j, k + 1));
                    }
                }
                for (int j = 0; j < CountCard; j++)
                {
                    _graphColorChange.SetValue(j, CountCard - 1, 0);
                    _graphRankChange.SetValue(j, CountCard - 1, 0);
                }
            }
            public override int GetCountCard()
            { return myDeck.CountCard(); }
            protected virtual int[] ConvertIsSay(string[] s, int pos)
            {
                int k = 0;
                int[] _mas = new int[s.Length + 1];
                int i = Convert.ToInt32(s[0]);
                _mas[k] = pos;
                _mas[++k] = i - 1;
                for (int j = 1; j < s.Length; j++)
                    _mas[j+1] = Convert.ToInt32(s[j]);
                return _mas;
            }
            public override string ToString()
            {
                string s = "";
                for (int i = 0; i < myDeck.CountCard(); i++)
                {
                    Card card = myDeck.GetCardToTell(i);
                    s += card.ToString() + " ";
                }
                return s;
            }
            public int[] IsSay
            {
                get { return _IsSay; }
                private set { _IsSay = value; }
            }
            #endregion
            protected override Card Play(int i, string _s)
            {
                //Если есть информация о ранге и цвете данной карты считаем ,что ход корректный
                Card fc = (Card)myDeck.GetCard(i).Clone();
                if (IsLevel)
                {
                    if (_graphRankChange.ComparePos(fc.Number - 1, i) && _graphColorChange.ComparePos(fc.Color, i))
                        _CorrectCard = true;
                    else if (!_graphRankChange.ComparePos(fc.Number - 1, i) && _graphColorChange.ComparePos(fc.Color, i))
                    {
                        this.CheckChange(i, _graphRankChange, true, fc.Color, _s);
                    }
                    else if (_graphRankChange.ComparePos(fc.Number - 1, i) && !_graphColorChange.ComparePos(fc.Color, i))
                    {
                        this.CheckChange(i, _graphColorChange, false, fc.Number-1, _s);
                    }
                    else
                    {
                        _CorrectCard = false;
                    }
                    SetNewInfo(i);
                }
                else
                {
                    _CorrectCard = true;
                }
                return fc;
            }
            protected override Card Drop(int i)
            {
                try
                {
                    Card fc = (Card)myDeck.GetCard(i).Clone();
                    if (IsLevel)
                        SetNewInfo(i);
                    return fc;
                }
                catch (ArgumentOutOfRangeException e)
                {
                    throw new CardException("Нет такой карты в руке");
                }
            }
            protected override void TellColor(string[] s)
            {
                _IsSay = ConvertIsSay(s, 0);
            }
            protected override void TellRank(string[] s)
            {
                _IsSay = ConvertIsSay(s, 1);
            }           
            //Выбор действия по команде
            public Card PlayerAction(string s, InfoForRegex.Command c, string _s)
            {
                if (Command != null)
                {
                    switch (c)
                    {
                        case InfoForRegex.Command.Play:
                            {
                                Command(s);
                                return this.Play(Convert.ToInt32(s), _s);
                            }
                        case InfoForRegex.Command.Drop:
                            {
                                Command(s);
                                return this.Drop(Convert.ToInt32(s));
                            }
                        case InfoForRegex.Command.Color:
                            {
                                Command(s);
                                this.TellColor(ParseString.GetUpperStrToArray(s));
                                return null;
                            }
                        case InfoForRegex.Command.Rank:
                            {
                                Command(s);
                                this.TellRank(ParseString.GetUpperStrToArray(s));
                                return null;
                            }
                        default:
                            throw new CardException("Неизвестный ход");
                    }
                }
                else
                {
                    throw new CardException("Неизвестная команда");
                }
            }
            public Action<string> Command { get; set; }
        }
        sealed class Card : ICloneable
        {
            private int _Number = -1;
            private int _Color = -1;
            private string _ShotColor = String.Empty;
            public Card(string color, int number)
            {
                this._Number = number;
                this._ShotColor = color;
                this._Color = Convert.ToInt32(InfoForRegex.GetShotColor(color));
            }
            public int Number
            {
                get { return this._Number; }
            }
            public int Color
            {
                get { return this._Color; }
            }
            public string ShotColor
            {
                get { return this._ShotColor; }
            }
            public object Clone()
            {
                return this.MemberwiseClone();
            }
            public override string ToString()
            {
                return this._ShotColor + this._Number;
            }
        }
        sealed class CardException : ApplicationException
        {
            public CardException() { }

            public CardException(string message) : base(message) { }

            public CardException(string message, Exception inner) : base(message, inner) { }

            protected CardException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        }
        abstract class ParentDeck
        {
            protected virtual void PushParent(List<Card> l, Card card)
            {
                l.Add(card);
            }
            protected virtual void PushParent(List<Card> l, string path)
            {
                var newPath = ParseString.GetUpperStrToArray(path);
                foreach (string card in newPath)
                {
                    l.Add(new Card(card[0].ToString(), Convert.ToInt32(card[1].ToString())));
                }
            }
            protected virtual Card GetCardParent(List<Card> l)
            {
                try
                {
                    Card card = (Card)l[0].Clone();
                    l.RemoveAt(0);
                    return card;
                }
                catch (ArgumentOutOfRangeException e)
                {
                    throw e;
                }
            }
            protected virtual Card GetCardParent(List<Card> l, int index)
            {
                try
                {
                    Card card = (Card)l[index].Clone();
                    l.RemoveAt(index);
                    return card;
                }
                catch (ArgumentOutOfRangeException e)
                {
                    throw e;
                }
            }
            public abstract int CountCard();
            public abstract void Clear();
            public abstract void Push(Card card);
            public abstract void Push(string path);
            public abstract Card GetCard();
            public abstract Card GetCard(int index);
        }
        //Класс поля игры
        sealed class FieldDeck : ParentDeck 
        {
            //Текущие карты на столе
            private List<Card> CardList = new List<Card>();
            private Graph graph;
            private static readonly Lazy<FieldDeck> lazy =
            new Lazy<FieldDeck>(() => new FieldDeck());
            public static FieldDeck Instance { get { return lazy.Value; } }
            private FieldDeck()
            {
                graph = new Graph(InfoForRegex.MaxCardForHand, InfoForRegex.MaxCardForHand);
            }
            public override int CountCard()
            {
                return CardList.Count;
            }
            public override void Clear()
            {
                CardList.Clear();
                graph.Clear();
            }
            public override void Push(Card card)
            {
                //Если есть карта такого же цвета и ранга большего на 1 ,чем текущий ,то добавляем
                //Иначе вызываем ошибку,которая обработается в классе Game
                int color = card.Color;
                if (graph.FindWay(color, 0, card.Number - 1) && !graph.ComparePos(color, card.Number - 1))
                {
                    graph.SetValue(color, card.Number - 1, 1);
                    base.PushParent(CardList, card);
                }
                else
                {
                    throw new CardException("Некорректная карта");
                }
            }
            public override void Push(string path)
            {
                //Если есть карта такого же цвета и ранга большего на 1 ,чем текущий ,то добавляем
                //Иначе вызываем ошибку,которая обработается в классе Game
                var newPath = ParseString.GetUpperStrToArray(path);
                foreach (string card in newPath)
                {
                    Card cards = new Card((card[0]).ToString(), Convert.ToInt32((card[1]).ToString()));
                    Push(cards);
                }
            }

            public override Card GetCard()
            {
                throw new CardException("Нельзя брать карты из этой колоды");
            }

            public override Card GetCard(int index)
            {
                throw new CardException("Нельзя брать карты из этой колоды");
            }
            public override string ToString()
            {
                string s = "Table: ";
                string n = String.Empty;
                int sumn = 0;
                int[] sumns = new int[InfoForRegex.MaxCardForHand];
                for (int i = 0; i < InfoForRegex.MaxCardForHand; i++)
                {
                    sumn = 0;
                    n = String.Empty;
                    for (int j = 0; j < InfoForRegex.MaxCardForHand; j++)
                    {
                        if (graph.FindWay(i, j) == 1)
                        {
                            sumn++;
                        }

                    }
                    sumns[i] = sumn;
                }
                for (int i = 0; i < sumns.Length; i++)
                {
                    n = string.Format(InfoForRegex.GetIntByColor(i) + sumns[i].ToString() + " ");
                    s += n;
                }

                return s;
            }


        }
        //Класс главной колоды
        class GeneralDeck : ParentDeck
        {

            private List<Card> CardList = new List<Card>();
            private static readonly Lazy<GeneralDeck> lazy =
            new Lazy<GeneralDeck>(() => new GeneralDeck());
            public static GeneralDeck Instance { get { return lazy.Value; } }
            private GeneralDeck()
            {
            }
            public override int CountCard()
            {
                return CardList.Count;
            }
            public override void Clear()
            {
                CardList.Clear();
            }

            public override void Push(Card card)
            {
                base.PushParent(CardList, card);
            }

            public override void Push(string path)
            {
                base.PushParent(CardList, path);
            }

            public override Card GetCard()
            {
                return base.GetCardParent(CardList);
            }

            public override Card GetCard(int index)
            {
                throw new CardException("Бери карты по порядку");
            }
        }
        //Класс колоды игрока
        sealed class PlayerDeck : ParentDeck
        {
            private List<Card> CardList = new List<Card>();
            public override int CountCard()
            {
                return CardList.Count;
            }
            public override void Clear()
            {
                CardList.Clear();
            }
            public override void Push(Card card)
            {
                if (CardList.Count < InfoForRegex.MaxCardForHand)
                {
                    CardList.Add(card);
                }
                else
                {
                    throw new CardException("Количество карт для игрока превысило допустимое значение");
                }
            }
            public override void Push(string path)
            {
                var newPath = ParseString.GetUpperStrToArray(path);
                foreach (string card in newPath)
                {
                    if (CardList.Count < InfoForRegex.MaxCardForHand)
                    {
                        Card cards = new Card((card[0]).ToString(), Convert.ToInt32((card[1]).ToString()));
                        CardList.Add(cards);
                    }
                    else
                    {
                        throw new CardException("Количество карт для игрока превысило допустимое значение");
                    }
                }
            }
            public override Card GetCard()
            {
                return base.GetCardParent(CardList);
            }
            public override Card GetCard(int index)
            {
                return base.GetCardParent(CardList, index);
            }
            public Card GetCardToTell(int index)
            {
                try
                {
                    Card card = (Card)CardList[index].Clone();
                    return card;
                }
                catch (ArgumentOutOfRangeException e)
                {
                    throw e;
                }
            }
        }
        //Класс колоды-сброса
        sealed class StackDeck : ParentDeck
        {
            private static readonly Lazy<StackDeck> lazy =
            new Lazy<StackDeck>(() => new StackDeck());
            public static StackDeck Instance { get { return lazy.Value; } }
            private StackDeck()
            {
            }
            private List<Card> CardList = new List<Card>();
            public override int CountCard()
            {
                return CardList.Count;
            }
            public override void Clear()
            {
                CardList.Clear();
            }
            public override void Push(Card card)
            {
                base.PushParent(CardList, card);
            }
            public override void Push(string path)
            {
                base.PushParent(CardList, path);
            }
            public override Card GetCard()
            {
                throw new CardException("Нельзя брать карты из этой колоды");
            }
            public override Card GetCard(int index)
            {
                throw new CardException("Нельзя брать карты из этой колоды");
            }
        }
        sealed class Game
        {
            #region filed
            private GeneralDeck gd = GeneralDeck.Instance;
            private FieldDeck fd = FieldDeck.Instance;
            private StackDeck sd = StackDeck.Instance;
            private Player _one = null;
            private Player _two = null;
            private bool Conflict = false;
            private bool TurnPlayer = true;
            private int CorrectCard = 0;
            private int Turn = 0;
            private int[] PlayerIsSay = new int[7];
            private bool Isay = false;
            private bool IsGame = false;
            private bool IsLevel;
            #endregion
            public Game(bool level)
            {
                _one = new Player("one", level);
                _two = new Player("two", level);
                //Подписка на смену игрока и подсчета ходов
                _one.Command = newProgress;
                _two.Command = newProgress;
                IsLevel = level;
            }
            //Метод , в котором происходит обработка и выполнение ходов игроков
            public void SetAction(string s)
            {
                try
                {
                    //Номер команды + Список аргументов для выполнения команд
                    var MyTurple = ParseString.GeneralParse(s);
                    int c = MyTurple.Item1;
                    string com = string.Join(" ", MyTurple.Item2.Select(x => x.ToString()).ToArray());
                    //Выполнение текущего хода
                    if (TurnPlayer)
                        SetCommand(c, com, _one);
                    else
                        SetCommand(c, com, _two);
                    //Проверить сказанное игроком
                    if (Isay)
                    {
                        Isay = false;
                        if (TurnPlayer)
                            SupportPlayer(_one);
                        else
                            SupportPlayer(_two);
                    }

                }
                catch (CardException ex)
                {
                    Conflict = true;
                }
                //Выход из игры
                if (Conflict || fd.CountCard() == 25 || gd.CountCard() == 0)
                {
                    EndString(s);
                    ClearGame();
                    IsGame = false;
                }
            }
            //Вывод данных о состояние игры
            private void EndString(string s)
            {
                Console.WriteLine("Turn: {0}, cards: {1}, with risk: {2}", Turn, fd.CountCard(), fd.CountCard() - CorrectCard);
            }
            private void newProgress(string s)
            {
                if (!Conflict)
                {
                    Turn++;
                    TurnPlayer = !TurnPlayer;
                }
            }
            //Проверка сказаного игроком. 
            private void SupportPlayer(Player pl)
            {

                int pos = PlayerIsSay[0];
                int count = PlayerIsSay.Length - 2;
                var newPath = ParseString.GetUpperStrToArray(pl.ToString());
                int max = newPath.ToList().Count;
                int line = PlayerIsSay[1];
                if (pos == 0)
                {
                    for (int i = 0; i < max; i++)
                    {
                        if (InfoForRegex.GetShotColor(newPath[i][0].ToString()) == line)
                            count--;
                    }
                    for (int i = 2; i < PlayerIsSay.Length; i++)
                    {
                        pl.SetColor(line, PlayerIsSay[i], 1);
                    }
                }
                else
                {
                    for (int i = 0; i < max; i++)
                    {
                        if (Convert.ToInt32(newPath[i][1].ToString()) - 1 == line)
                            count--;
                    }
                    for (int i = 2; i < PlayerIsSay.Length; i++)
                    {
                        pl.SetRank(line, PlayerIsSay[i], 1);
                    }
                }
                if (count != 0)
                    throw new CardException("Вас пытаються надуть");
                PlayerIsSay = null;
            }
            //Подготовка к новой игре. Так как создал лишь 1 объект класса Game
            private void ClearGame()
            {
                gd.Clear();
                fd.Clear();
                sd.Clear();
                _one.Clear();
                _two.Clear();
                Turn = 0;
                Conflict = false;
                TurnPlayer = true;
                CorrectCard = 0;
                Isay = false;
            }
            private void SetCommand(int index, string s, Player pl)
            {
                switch (index)
                {
                    case 0:
                        {
                            ClearGame();
                            IsGame = true;
                            var newpath = ParseString.GetUpperStrToArray(s);
                            for (int i = 0; i < newpath.Length; i++)
                            {
                                try
                                {
                                    if (_one.GetCountCard() < 5)
                                        _one.Push(newpath[i]);
                                    else
                                        _two.Push(newpath[i]);
                                    newpath[i] = null;
                                }
                                catch (CardException ex)
                                {
                                    break;
                                }
                            }
                            newpath = newpath.Where(x => x != null).ToArray();
                            gd.Push(string.Join(" ", newpath));
                            if (gd.CountCard() == 0)
                                throw new CardException("Пустая колода");
                            break;
                        }
                    case 1:
                        {
                            if (IsGame)
                            {
                                Card fc = null;
                                try
                                {
                                    fc = (Card)pl.PlayerAction(s, InfoForRegex.Command.Play, fd.ToString()).Clone();
                                    fd.Push(fc);
                                    pl.Push(gd.GetCard());
                                    if (pl.CorrectCard)
                                        CorrectCard++;
                                }
                                catch (CardException e)
                                {
                                    Conflict = true;
                                    sd.Push(fc);
                                }
                            }
                            break;
                        }
                    case 2:
                        {
                            if (IsGame)
                            {
                                sd.Push(pl.PlayerAction(s, InfoForRegex.Command.Drop, null));
                                pl.Push(gd.GetCard());
                            }
                            break;
                        }
                    case 3:
                        {
                            if (IsGame)
                            {
                                pl.PlayerAction(s, InfoForRegex.Command.Color, null);
                                PlayerIsSay = pl.IsSay;
                                Isay = true;
                            }
                            break;
                        }
                    case 4:
                        {
                            if (IsGame)
                            {
                                pl.PlayerAction(s, InfoForRegex.Command.Rank, null);
                                PlayerIsSay = pl.IsSay;
                                Isay = true;
                            }
                            break;
                        }
                    default:
                        throw new CardException("Нет такой команды");
                }
            }
        }
    }
}
