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
            string game_progress = String.Empty;
            do
            {
                game_progress = Console.ReadLine();
                if (game_progress != null)
                    game.GameAction(game_progress);
                else
                    break;
            } while (true);
        }
        /// <summary>
        /// Двумерный массив для хранения информации о текущем наборе карт.
        /// </summary>
        class InfoStorage
        {
            private int[,] graph;
            /// <summary>
            /// Двумерный массив для хранения информации о картах в руке или на столе
            /// </summary>
            public InfoStorage()
            {
                this.graph = new int[SearchInformation.MaxCardForHand, SearchInformation.MaxCardForHand];
            }
            /// <summary>
            /// Сравнить  значение ячейки с единицей
            /// </summary>
            /// <param name="line"></param>
            /// <param name="column"></param>
            /// <returns></returns>
            public bool CompareWithOne(int line, int column)
            {
                if (CompareWithOne(this.graph[line, column]))
                    return true;
                return false;
            }
            /// <summary>
            /// Установить значение. В пределах [0,3]
            /// </summary>
            /// <param name="line">номер строки</param>
            /// <param name="column">номер столбца</param>
            /// <param name="value">значение</param>
            private void SetValueInCell(int line, int column, int value)
            {
                if (value >= 0 && value <= 3)
                {
                    this.graph[line, column] = value;
                }
                else
                {
                    ArgumentOutOfRangeException ex = new ArgumentOutOfRangeException();
                    throw ex;
                }
            }
            /// <summary>
            /// Получить информацию из ячейки
            /// </summary>
            /// <param name="line">номер строки</param>
            /// <param name="column">номер столбца</param>
            /// <returns></returns>
            private int GetValueOfCell(int line, int column)
            {
                if ((line >= 0 && line < SearchInformation.MaxCardForHand) && (column >= 0 && column < SearchInformation.MaxCardForHand))
                {
                    return this.graph[line, column];
                }
                else
                {
                    ArgumentOutOfRangeException ex = new ArgumentOutOfRangeException();
                    throw ex;
                }
            }
            private bool CompareWithOne(int digit)
            {
                if (digit == 1)
                    return true;
                return false;
            }
            /// <summary>
            /// Проверка правильной последовательности. Имееются ли элементы до значения depth
            /// </summary>
            /// <param name="line">Строка</param>
            /// <param name="column">Столбец</param>
            /// <param name="depth">Глубина = Рангу карты - 1</param>
            /// <returns></returns>
            public bool CheckCorrectPreviousPosition(int line, int depth)
            {
                if (depth == 0)
                    return true;
                else if (CompareWithOne(line, depth - 1))
                    return true;
                else
                    return false;
            }
            public void Clear()
            {
                Array.Clear(this.graph, 0, (SearchInformation.MaxCardForHand * SearchInformation.MaxCardForHand));
            }
            public int this[int line, int column]
            {
                get
                {
                    return GetValueOfCell(line, column);
                }
                set
                {
                    SetValueInCell(line, column, value);
                }
            }
        }
        /// <summary>
        /// Вспомогательный класс для хранения проверочных строк и представления входных данных для хранения о них информации 
        /// </summary>
        static class SearchInformation
        {
            public const int MaxCardForHand = 5;
            public static string _ParseCard = "[YRGBW]+[1-5]";
            public static string _ParseStart = "START NEW GAME WITH DECK";
            private static string _ParsePlay = "PLAY CARD [0-4]";
            private static string _ParseDrop = "DROP CARD [0-4]";
            private static string _ParseTC = "TELL COLOR ((YELLOW)|(RED)|(BLUE)|(WHITE)|(GREEN)) FOR CARDS ([0-4]+)";
            private static string _ParseTR = "TELL RANK [1-5] FOR CARDS ([0-4]+)";
            private static string[] InfoParse = new string[5] { _ParseStart, _ParsePlay, _ParseDrop, _ParseTC, _ParseTR };
            public enum Color { Blue = 1, Green = 2, Red = 3, White = 4, Yellow = 5 };
            public enum Command { Play, Drop, Color, Rank };
            public static int GetCountInfoParse
            {
                get { return InfoParse.Length; }
            }
            /// <summary>
            /// Вернуть строку для регулярного выражения
            /// (0 - начало игры, 1 - сыграть, 2 - сбросить, 3 - подсказать цвет, 4- подсказать ранг)
            /// </summary>
            /// <param name="position">позиция</param>
            /// <returns></returns>
            public static string GetInformationString(int position)
            {
                return InfoParse[position];
            }
            public static int GetShortColorToInt(string resource)
            {
                switch (resource)
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
            public static string GetIntByColor(int resource)
            {
                switch (resource)
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
        static class ParseInformationCard
        {
            /// <summary>
            /// Проверить является ли строка screening_line одним из действий данной игры
            /// </summary>
            /// <typeparam name="T">Array,string</typeparam>
            /// <param name="regex">строка для регулярного выражения</param>
            /// <param name="screening_line">строка проверки</param>
            /// <returns></returns>
            public static bool CheckCorrectAction<T>(IEnumerable<T> regex, string screening_line)
            {
                List<Regex> _RegexList = new List<Regex>();
                bool _Solution = true;
                screening_line = screening_line.ToUpper();
                if (regex is String)
                {
                    _RegexList.Add(new Regex(regex.ToString(), RegexOptions.Compiled));
                }
                else if (regex is Array)
                {
                    var new_regex = regex.Select(item => item.ToString().ToUpperInvariant()).ToArray();
                    foreach (var p in regex)
                    {
                        _RegexList.Add(new Regex(p.ToString(), RegexOptions.Compiled));
                    }
                }
                foreach (Regex r in _RegexList)
                {
                    if (!r.IsMatch(screening_line))
                    {
                        _Solution = false;
                        break;
                    }
                }
                return _Solution;
            }
            /// <summary>
            /// Скомпоновать данные.
            /// 1 элемент - номер команды(0 - новая игра, 1 - сыграть карту,2 - сбросить карту,3 - подсказать цвет,4 - подсказать ранг), 
            /// 2 элемент - дополнительная информация(список карт или список цветов,рангов в руке)
            /// </summary>
            /// <param name="initial_data">входная строка состояния игры</param>
            /// <returns></returns>
            public static Tuple<int, string> ConvertInputDataToCommand(string initial_data)
            {
                initial_data = initial_data.ToUpper();
                int number_command = -1;
                List<string> _data = new List<string>();
                for (int i = 0; i < SearchInformation.GetCountInfoParse; i++)
                {
                    if (CheckCorrectAction(SearchInformation.GetInformationString(i), initial_data))
                    {
                        number_command = i;
                        break;
                    }
                }
                if (number_command == 0)
                {
                    Regex reg = new Regex(SearchInformation._ParseStart);
                    var cards = reg.Replace(initial_data, " ").GetUpperStringToArray();
                    foreach (string card in cards)
                        if (CheckCorrectAction(SearchInformation._ParseCard, card))
                            _data.Add(card);
                }
                else if (number_command != -1)
                {
                    char[] data = ConvertInputDataToHelperString(initial_data);
                    foreach (char nd in data)
                        _data.Add(nd.ToString());
                }
                if (number_command == -1)
                    throw new CardException("Не известная команда");
                return Tuple.Create<int, string>(number_command, String.Join(" ", _data));
            }
            /// <summary>
            /// Скомпоновать вспомогательные данные для корректного выполнения хода(ранг или цвет) - Узнать какой цвет или ранг;
            /// </summary>
            /// <param name="resource"></param>
            /// <returns></returns>
            private static char[] ConvertInputDataToHelperString(string resource)
            {
                var newPath = resource.GetUpperStringToArray();
                bool IsDigit = newPath[2].ToString().Length == newPath[2].ToString().Where(c => char.IsDigit(c)).Count();
                if (!IsDigit)
                {
                    foreach (var value in Enum.GetValues(typeof(SearchInformation.Color)))
                    {
                        if (newPath[2].ToString().Contains(value.ToString().ToUpper()))
                        {
                            newPath[2] = (Convert.ToInt32((int)value)).ToString();
                            break;
                        }
                    }
                }
                return String.Join(" ", newPath).GetDigit();
            }
        }
        /// <summary>
        /// Действия игрока,которые он может совершать в большинстве карточных играх
        /// </summary>
        interface IPlayer
        {
            Action<string> Command { get; set; } //Подписка на событие для смены хода
            void Push(Card card);               // Добавить карту в руку 
            void Push(string path);
            void Clear();                       //Очистить руку
            /// <summary>
            /// Выполнить определенное действие(Сыграть,сбросить. Подсказать цвет,ранг)
            /// </summary>
            /// <param name="value">данные для комманды</param>
            /// <param name="command">комманда</param>
            /// <param name="state_table">информация о столе</param>
            /// <returns></returns>
            Card PlayerAction(string value, SearchInformation.Command command, string state_table);//Выбор действия 
        }
        /// <summary>
        /// Действия предусмотренные для игрока в этой игрой
        /// </summary>
        abstract class ParentPlayer
        {
            /// <summary>
            /// Вытащить карту из руки и определить является ли она рискованной или нет.
            /// </summary>
            /// <param name="number_in_hand">порядок карты в руке</param>
            /// <param name="state_table">информация о картах на столе</param>
            /// <returns></returns>
            protected abstract Card Play(int number_in_hand, string state_table);
            /// <summary>
            /// Вытащить карту для сброса в "Сброс"
            /// </summary>
            /// <param name="number_in_hand">порядок карты в руке</param>
            /// <returns></returns>
            protected abstract Card Drop(int number_in_hand);
            /// <summary>
            /// Сформировать данные для хранения цвета подсказанного игроком
            /// </summary>
            /// <param name="initial_data">входная строка предварительно разбитая на слова в массив</param>
            protected abstract void TellColor(string[] initial_data);
            /// <summary>
            /// Сформировать данные для хранения ранга подсказанного игроком
            /// </summary>
            /// <param name="initial_data">входная строка предварительно разбитая на слова в массив</param>
            protected abstract void TellRank(string[] initial_data);
            public abstract int GetCountCard
            {
                get;
            }
        }
        class Player : ParentPlayer, IPlayer
        {
            #region filed
            //Колода игрока
            private PlayerDeck myDeck;
            //Подсказка для другого игрока
            private int[] _IsSay;
            //Рискованная ли карта
            private bool _RiskyCard = true;
            //Массивы для сохранения информации
            private InfoStorage graphColorChange;
            private InfoStorage graphRankChange;
            private string Name;
            private readonly bool IsLevel;
            private readonly int MaxCountCard;
            public Action<string> Command { get; set; }
            #endregion
            public Player(string name, bool level)
            {
                myDeck = new PlayerDeck();
                graphColorChange = new InfoStorage();
                graphRankChange = new InfoStorage();
                Name = name;
                IsLevel = level;
                MaxCountCard = SearchInformation.MaxCardForHand;
            }
            /// <summary>
            /// Записать информацию о цвете карты
            /// </summary>
            /// <param name="line"></param>
            /// <param name="column"></param>
            /// <param name="value"></param>
            public virtual void SetColor(int line, int column, int value)
            {
                SetChange(line, column, value, graphColorChange);
            }
            /// <summary>
            /// Записать информацию о ранге карты
            /// </summary>
            /// <param name="line"></param>
            /// <param name="column"></param>
            /// <param name="value"></param>
            public virtual void SetRank(int line, int column, int value)
            {
                SetChange(line, column, value, graphRankChange);
            }
            /// <summary>
            /// Пересчитать состояние руки
            /// </summary>
            /// <param name="line">строка массива состояния</param>
            /// <param name="column">столбец массива состояния</param>
            /// <param name="value">точная информаци о карте</param>
            /// <param name="_graph">массив(цвет или ранг)</param>
            private void SetChange(int line, int column, int value, InfoStorage _graph)
            {
                #region helpinfo
                //значение 1 - игрок точно знает цвет или ранг карты взависимости от _graph
                //значение 2 - игрок имеет предположение, что данная карта имеет цвет или ранг n(строка)
                //значение 3 - игрок точно знает ,что данная карта не n(строка) цвета или ранга
                #endregion
                bool check_card = false;
                for (int k = 0; k < MaxCountCard; k++)
                    _graph[k, column] = 0;
                _graph[line, column] = 1;
                for (int _line = 0; _line < MaxCountCard; _line++)
                {
                    for (int _column = 0; _column < MaxCountCard; _column++)
                    {
                        for (int z = 0; z < MaxCountCard; z++)
                        {
                            if (_graph[z, _column] == 1)
                            {
                                check_card = true;
                                break;
                            }
                        }
                        if (_column != column && !check_card)
                            if (_graph[_line, _column] != 3)
                                _graph[_line, _column] = 2;
                        check_card = false;
                    }
                }
                //установим значение "3" в столбец где нет точного значения
                check_card = false;
                for (int _line = 0; _line < MaxCountCard; _line++)
                {
                    for (int _column = 0; _column < MaxCountCard; _column++)
                    {
                        if (_graph[_column, _line] == 1)
                        {
                            check_card = true;
                            break;
                        }
                    }
                    if (!check_card)
                        _graph[line, _line] = 3;
                    check_card = false;
                }
            }
            /// <summary>
            /// Подсчитать количество карт определенного цвета или ранга,которые можно положить на стол на текущий момент, и сравнить с состоянием стола
            /// </summary>
            /// <param name="number_card">номер карты в руке</param>
            /// <param name="_graph">состояние цветов или рангов карт</param>
            /// <param name="state">цвет или ранг</param>
            /// <param name="value">точное значение карты(цвет или ранг)</param>
            /// <param name="table_info">состояние стола</param>
            private void CheckTheCorrect(int number_card, InfoStorage _graph, bool state, int value, string table_info)
            {
                int possibility_table = 0;
                int possibility = 0;
                for (int k = 0; k < MaxCountCard; k++)
                {
                    if (_graph[k, number_card] == 2)
                    {
                        possibility++;
                        if (state)
                        {
                            if (table_info.Contains(SearchInformation.GetIntByColor(value) + (k)))
                                possibility_table++;
                        }
                        else
                        {
                            if (table_info.Contains(SearchInformation.GetIntByColor(k) + (value)))
                                possibility_table++;
                        }
                    }
                }

                if (possibility_table == possibility)
                    _RiskyCard = false;
                else
                    _RiskyCard = true;
            }
            /// <summary>
            /// Если нет точной информации о параметрах карты
            /// </summary>
            /// <param name="number_card">порядок карты в руке</param>
            /// <param name="table_info">информация о картах на столе</param>
            private void CheckTheCorrect(int number_card, string table_info)
            {
                var newData = table_info.StringToArray();
                int possibility_table = 0;
                List<Tuple<int, int>> Possibility = new List<Tuple<int, int>>();
                for (int i = 0; i < MaxCountCard; i++)
                {
                    if (graphColorChange[number_card, i] == 2)
                        for (int j = 0; j < MaxCountCard; j++)
                            if (graphRankChange[number_card, j] == 2)
                                Possibility.Add(Tuple.Create(i, j));

                }
                foreach (var cell in Possibility)
                {
                    foreach (var cell_table in newData)
                        if (cell_table.Contains(SearchInformation.GetIntByColor(cell.Item1) + (cell.Item2 - 1).ToString()))
                            possibility_table++;

                }
                if (possibility_table == Possibility.Count)
                    _RiskyCard = false;
                else
                    _RiskyCard = true;
            }
            /// <summary>
            /// Сыграть карту на стол
            /// </summary>
            /// <param name="number_card">Начиная с нуля до максимального значения</param>
            /// <param name="table_info">информация о столе, какие карты лежат в текущий момент</param>
            /// <returns></returns>
            protected override Card Play(int number_card, string table_info)
            {
                Card fc = (Card)myDeck[number_card].Clone();
                if (IsLevel)
                {
                    if (graphRankChange.CompareWithOne(fc.Number - 1, number_card) && graphColorChange.CompareWithOne(fc.Color, number_card))
                        _RiskyCard = false;
                    else if (!graphRankChange.CompareWithOne(fc.Number - 1, number_card) && graphColorChange.CompareWithOne(fc.Color, number_card))
                    {
                        this.CheckTheCorrect(number_card, graphRankChange, true, fc.Color, table_info);
                    }
                    else if (graphRankChange.CompareWithOne(fc.Number - 1, number_card) && !graphColorChange.CompareWithOne(fc.Color, number_card))
                    {
                        this.CheckTheCorrect(number_card, graphColorChange, false, fc.Number - 1, table_info);
                    }
                    else
                    {
                        this.CheckTheCorrect(number_card, table_info);
                    }
                    this.SetNewInfo(number_card);
                }
                else
                    _RiskyCard = false;
                return fc;
            }
            /// <summary>
            /// Сбросить карту в "сброс"
            /// </summary>
            /// <param name="number_card">Начиная с нуля до максимального значения</param>
            /// <returns></returns>
            protected override Card Drop(int number_card)
            {
                try
                {
                    Card fc = (Card)myDeck[number_card].Clone();
                    if (IsLevel)
                        this.SetNewInfo(number_card);
                    return fc;
                }
                catch (ArgumentOutOfRangeException e)
                {
                    throw new CardException("Нет такой карты в руке");
                }
            }
            /// <summary>
            /// Записать подсказку о цвете
            /// </summary>
            /// <param name="initial_data">Информация в виде массива для корректного заполнения объекта хранящего информацию о состояние руки</param>
            protected override void TellColor(string[] initial_data)
            {
                _IsSay = ConvertIsSay(initial_data, 0);
            }
            /// <summary>
            /// Записать подсказку о ранге
            /// </summary>
            /// <param name="initial_data">Информация в виде массива для корректного заполнения объекта хранящего информацию о состояние руки</param>
            protected override void TellRank(string[] initial_data)
            {
                _IsSay = ConvertIsSay(initial_data, 1);
            }
            //Выбор действия по команде
            public Card PlayerAction(string text_command, SearchInformation.Command c, string table_info)
            {
                if (Command != null)
                {
                    switch (c)
                    {
                        case SearchInformation.Command.Play:
                            {
                                Command(text_command);
                                return this.Play(Convert.ToInt32(text_command), table_info);
                            }
                        case SearchInformation.Command.Drop:
                            {
                                Command(text_command);
                                return this.Drop(Convert.ToInt32(text_command));
                            }
                        case SearchInformation.Command.Color:
                            {
                                Command(text_command);
                                this.TellColor(text_command.StringToArray());
                                return null;
                            }
                        case SearchInformation.Command.Rank:
                            {
                                Command(text_command);
                                this.TellRank(text_command.StringToArray());
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
            #region не игровые функции
            /// <summary>
            /// Положить карту в руку игрока
            /// </summary>
            /// <param name="card">Карта</param>
            public void Push(Card card)
            {
                myDeck.PushCard(card);
            }
            /// <summary>
            /// Положить карту в руку игрока
            /// </summary>
            /// <param name="data">Карта в строковом представление</param>
            public void Push(string data)
            {
                myDeck.PushCard(data);
            }
            /// <summary>
            /// Вернуть значение о рискованности карты
            /// </summary>
            public bool GetInfoRisky
            {
                get { return _RiskyCard; }
            }
            public void Clear()
            {
                myDeck.Clear();
                graphColorChange.Clear();
                graphRankChange.Clear();
            }
            /// <summary>
            /// Изменить информацию о состояние карт в руке
            /// </summary>
            /// <param name="i">номер карты в руке</param>
            protected virtual void SetNewInfo(int i)
            {
                for (int j = 0; j < MaxCountCard; j++)
                {
                    for (int k = i; k < MaxCountCard - 1; k++)
                    {
                        graphColorChange[j, k] = graphColorChange[j, k + 1];
                        graphRankChange[j, k] = graphRankChange[j, k + 1];
                    }
                }
                for (int j = 0; j < MaxCountCard; j++)
                {
                    graphColorChange[j, MaxCountCard - 1] = 0;
                    graphRankChange[j, MaxCountCard - 1] = 0;
                }
            }
            /// <summary>
            /// Количество карт в руке
            /// </summary>
            /// <returns></returns>
            public override int GetCountCard
            {
                get { return myDeck.CountCard; }
            }
            /// <summary>
            /// Сформировать сказаное игроком в числовой формат + добавить маркер о чем говорит игрок
            /// </summary>
            /// <param name="resource">строка подсказки</param>
            /// <param name="state">цвет или ранг (0 или 1)</param>
            /// <returns></returns>
            protected virtual int[] ConvertIsSay(string[] resource, int state)
            {
                int[] _mas = new int[resource.Length + 1];
                _mas[0] = state;
                _mas[1] = Convert.ToInt32(resource[0]) - 1;
                for (int j = 1; j < resource.Length; j++)
                    _mas[j + 1] = Convert.ToInt32(resource[j]);
                return _mas;
            }
            public override string ToString()
            {
                string s = string.Empty;
                for (int i = 0; i < myDeck.CountCard; i++)
                {
                    Card card = myDeck.ViewValueCard(i);
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
        }
        /// <summary>
        /// Класс карты
        /// </summary>
        class Card : ICloneable
        {
            private int _Number;
            private int _Color;
            private string _ShortColor;
            public Card(string color, int number)
            {
                this._Number = number;
                this._ShortColor = color;
                this._Color = Convert.ToInt32(SearchInformation.GetShortColorToInt(color));
            }
            public int Number
            {
                get { return this._Number; }
            }
            /// <summary>
            /// Вернуть числовое представление цвета(B-0,G-1,R-2,W-3,Y-4)
            /// </summary>
            public int Color
            {
                get { return this._Color; }
            }
            /// <summary>
            /// Вернуть короткое строковое значение цвета(B,G,R,W,Y)
            /// </summary>
            public string ShortColor
            {
                get { return this._ShortColor; }
            }
            public object Clone()
            {
                return this.MemberwiseClone();
            }
            public override string ToString()
            {
                return this._ShortColor + this._Number;
            }
        }
        class CardException : ApplicationException
        {
            public CardException() { }

            public CardException(string message) : base(message) { }

            public CardException(string message, Exception inner) : base(message, inner) { }

        }
        /// <summary>
        /// Главная колода
        /// </summary>
        class GeneralDeck   
        {
            #region singleton
            private static readonly Lazy<GeneralDeck> lazy =
            new Lazy<GeneralDeck>(() => new GeneralDeck());
            public static GeneralDeck Instance { get { return lazy.Value; } }
            #endregion
            private List<Card> CardList;
            protected GeneralDeck()
            {
                CardList = new List<Card>();
            }
            /// <summary>
            /// Добавить карту в колоду
            /// </summary>
            /// <param name="card">Карта</param>
            public virtual void PushCard(Card card)
            {
                CardList.Add(card);
            }
            /// <summary>
            /// Добавить карту в колоду
            /// </summary>
            /// <param name="data">Карта</param>
            public virtual void PushCard(string data)
            {
                var newData = data.StringToArray(); 
                foreach (string card in newData)
                {
                    PushCard(new Card(card[0].ToString(), Convert.ToInt32(card[1].ToString())));
                }
            }
            /// <summary>
            /// При взятие карты из колоды удалить её.
            /// </summary>
            /// <param name="index">позиция в руке</param>
            /// <returns></returns>
            private Card UpdateList(int index = 0)
            {
                try
                {
                    Card card = (Card)CardList[index].Clone();
                    CardList.RemoveAt(index);
                    return card;
                }
                catch (ArgumentOutOfRangeException e)
                {
                    throw new CardException("Ошибка.Получен отрицательный или превыщающий число карт индекс");
                }
            }
            /// <summary>
            /// Посмотреть значения ранга и цвета не извлекая карты из руки. Только для вывода состояния на экран.
            /// </summary>
            /// <param name="index">порядок карты в руке</param>
            /// <returns></returns>
            public virtual Card ViewValueCard(int index)
            {
                throw new CardException("Нельзя смотреть карты этой колоды");
            }
            /// <summary>
            /// Получить значение карты
            /// </summary>
            /// <param name="index">позиция в руке</param>
            /// <returns></returns>
            protected virtual Card GetValueCard(int index = 0)
            {
                Card card = (Card)CardList[index].Clone();
                return card;
            }
            public virtual Card this[int index]
            {
                get
                {
                    return UpdateList(index);
                }
            }
            public int CountCard
            {
                get { return CardList.Count; }
            }
            public virtual void Clear()
            {
                CardList.Clear();
            }
        }
        /// <summary>
        /// Колода игрока
        /// </summary>
        sealed class PlayerDeck : GeneralDeck
        {
            public PlayerDeck()
                : base()
            {}
            public override void PushCard(Card card)
            {
                if (CountCard < SearchInformation.MaxCardForHand)
                {
                    base.PushCard(card);
                }
                else
                {
                    throw new CardException("Количество карт для игрока превысило допустимое значение");
                }
            }
            public override void PushCard(string data)
            {
                Card cards = new Card((data[0]).ToString(), Convert.ToInt32((data[1]).ToString()));
                PushCard(cards);
            }
            /// <summary>
            /// Посмотреть значения ранга и цвета не извлекая карты из руки. Только для вывода состояния на экран.
            /// </summary>
            /// <param name="index">порядок карты в руке</param>
            /// <returns></returns>
            public override Card ViewValueCard(int index)
            {
                return base.GetValueCard(index);
            }
        }
        /// <summary>
        /// Поле игры
        /// </summary>
        sealed class FieldDeck : GeneralDeck
        {
            #region singleton
            private static readonly Lazy<FieldDeck> lazy =
            new Lazy<FieldDeck>(() => new FieldDeck());
            public static FieldDeck FieldInstance { get { return lazy.Value; } }
            #endregion
            private InfoStorage graph;
            private FieldDeck()
                : base()
            {
                graph = new InfoStorage();
            }
            public override void Clear()
            {
                base.Clear();
                graph.Clear();
            }
            public override void PushCard(Card card)
            {
                int color = card.Color;
                if (!graph.CompareWithOne(color, card.Number - 1) && graph.CheckCorrectPreviousPosition(color, card.Number - 1))
                {
                    graph[color, card.Number - 1] = 1;
                    base.PushCard(card);
                }
                else
                {
                    throw new CardException("Не корректная карта");
                }
            }
            public override void PushCard(string data)
            {
                var newData = data.GetUpperStringToArray();
                foreach (string cards in newData)
                {
                    Card card = new Card((cards[0]).ToString(), Convert.ToInt32((cards[1]).ToString()));
                    PushCard(card);
                }
            }
            public override string ToString()
            {
                string line_state = String.Empty;
                int max_rank = 0;
                int[] info_rank = new int[SearchInformation.MaxCardForHand];
                for (int i = 0; i < SearchInformation.MaxCardForHand; i++)
                {
                    for (int j = 0; j < SearchInformation.MaxCardForHand; j++)
                        if (graph.CompareWithOne(i, j))
                            max_rank++;

                    info_rank[i] = max_rank;
                    max_rank = 0;
                }
                for (int i = 0; i < info_rank.Length; i++)
                    line_state += string.Format(SearchInformation.GetIntByColor(i) + info_rank[i].ToString() + " ");
                return line_state;
            }
            public override Card this[int index]
            {
                get { throw new CardException("Нельзя брать карты из этой колоды"); }
            }
            public override Card ViewValueCard(int index)
            {
                return base.GetValueCard(index);
            }
        }
        /// <summary>
        /// Колода для сброса
        /// </summary>
        sealed class StackDeck : GeneralDeck
        {
            #region singleton
            private static readonly Lazy<StackDeck> lazy =
            new Lazy<StackDeck>(() => new StackDeck());
            public static StackDeck StackInstance { get { return lazy.Value; } }
            #endregion
            private StackDeck()
                : base()
            {
            }
            public override void PushCard(Card card)
            {
                base.PushCard(card);
            }
            public new void PushCard(string card)
            {
                    PushCard(new Card(card[0].ToString(), Convert.ToInt32(card[1].ToString())));
            }
            public override Card this[int index]
            {
                get { throw new CardException("Нельзя брать карты из этой колоды"); }
            }
        }
        /// <summary>
        /// Выполнить определенное действие(Сыграть,Сбросить,Подсказать цвет,Подсказать ранг)
        /// </summary>
        /// <param name="command">текстовое представление команды</param>
        /// <param name="player">текущий игрок</param>
        private delegate void GameOperation(string command, Player player);
        sealed class Game
        {
            #region filed
            private GeneralDeck generaldeck = GeneralDeck.Instance;
            GameOperation gameaction;
            private FieldDeck fieldeck = FieldDeck.FieldInstance;
            private StackDeck stackdeck = StackDeck.StackInstance;
            private Player _onePlayer;
            private Player _twoPlayer;
            private bool Conflict = false;// текущее состояние игры false - продолжаем игру, true - заканчиваем
            private bool TurnPlayer = true;// переменная для смены хода. true - первый игрок, false - 2
            private int CorrectCard = 0;//количество корректных карт
            private int Number_Of_Turns = 0;//количество ходов
            private int[] PlayerIsSay = new int[SearchInformation.MaxCardForHand + 2];//массив для хранения информации , подсказка для другого игрока
            private bool Isay = false;//Подсказал ли игрок на этом ходу
            private bool IsGame = false;//переменная для хранения состояния игры. Пока false нельзя сыграть,сбросить,подсказать карту.

            #endregion
            public Game(bool level)
            {
                _onePlayer = new Player("One", level);
                _twoPlayer = new Player("Two", level);
                //Подписка на смену игрока и подсчета ходов
                _onePlayer.Command = NewTurn;
                _twoPlayer.Command = NewTurn;
            }
            /// <summary>
            /// Обработка входящей строки с консоли
            /// </summary>
            /// <param name="resource">входной параметр с консоли</param>
            public void GameAction(string resource)
            {
                try
                {
                    //Номер команды + Список аргументов для выполнения команд
                    var MyTurple = ParseInformationCard.ConvertInputDataToCommand(resource);
                    int command_int = MyTurple.Item1;
                    //string command_text = string.Join(" ", MyTurple.Item2.Select(x => x.ToString()).ToArray());
                    //Выполнение текущего хода
                    if (TurnPlayer)
                        SetCommand(command_int, MyTurple.Item2, _onePlayer);
                    else
                        SetCommand(command_int, MyTurple.Item2, _twoPlayer);
                    //Проверить сказанное игроком
                    if (Isay)
                    {
                        Isay = false;
                        if (TurnPlayer)
                            CheckAndSetTellInfo(_onePlayer);
                        else
                            CheckAndSetTellInfo(_twoPlayer);
                    }

                }
                catch (CardException e)
                {
                    Conflict = true;
                }
                //Выход из игры.Некорректная карта, карт на столе 25, карты в главной колоде закончились
                if (Conflict || fieldeck.CountCard == 25 || generaldeck.CountCard == 0)
                {
                    OutputCurrentGameStatus(resource);
                    ClearGame();
                    IsGame = false;
                }
            }
            /// <summary>
            /// Вывод на консоль количества ходов, количества карт на столе, количество рискованных ходов
            /// </summary>
            /// <param name="resource">дополнительный параметр(состяние незавершенной игры)</param>
            private void OutputCurrentGameStatus(string resource)
            {
                Console.WriteLine("Turn: {0}, cards: {1}, with risk: {2}", Number_Of_Turns, fieldeck.CountCard, fieldeck.CountCard - CorrectCard);
            }
            /// <summary>
            /// Подсчет и смена хода
            /// </summary>
            /// <param name="s">Дебаг.Действие на текущем ходу</param>
            private void NewTurn(string s)
            {
                if (!Conflict)
                {
                    Number_Of_Turns++;
                    TurnPlayer = !TurnPlayer;
                }
            }
            /// <summary>
            /// Проверка сказанного игроком pl
            /// </summary>
            /// <param name="player">текущий игрок</param>
            private void CheckAndSetTellInfo(Player player)
            {
                int state = PlayerIsSay[0];
                int count = PlayerIsSay.Length - 2;
                var newPath = player.ToString().StringToArray();
                int max = newPath.ToList().Count;
                int line = PlayerIsSay[1];
                PlayerIsSay = PlayerIsSay.Skip(2).ToArray();
                if (state == 0)
                {
                    for (int i = 0; i < max; i++)
                    {
                        if (SearchInformation.GetShortColorToInt(newPath[i][0].ToString()) == line)
                            count--;
                    }
                    for (int i = 0; i < PlayerIsSay.Length; i++)
                    {
                        player.SetColor(line, PlayerIsSay[i], 1);
                    }
                }
                else
                {
                    for (int i = 0; i < max; i++)
                    {
                        if (Convert.ToInt32(newPath[i][1].ToString()) - 1 == line)
                            count--;
                    }
                    for (int i = 0; i < PlayerIsSay.Length; i++)
                    {
                        player.SetRank(line, PlayerIsSay[i], 1);
                    }
                }
                if (count != 0)
                    throw new CardException("Вас пытаються надуть");
                PlayerIsSay = null;
            }
            //Подготовка к новой игре. Так как создал лишь 1 экземпляр класса Game
            private void ClearGame()
            {
                generaldeck.Clear();
                fieldeck.Clear();
                stackdeck.Clear();
                _onePlayer.Clear();
                _twoPlayer.Clear();
                Number_Of_Turns = 0;
                Conflict = false;
                TurnPlayer = true;
                CorrectCard = 0;
                Isay = false;
            }
            private void StartNewGame(string command)
            {
                ClearGame();
                IsGame = true;
                var newData = command.GetUpperStringToArray();
                if (newData.Length > 10)
                {
                    for (int i = 0; i < SearchInformation.MaxCardForHand * 2; i++)
                    {
                        if (_onePlayer.GetCountCard < SearchInformation.MaxCardForHand)
                            _onePlayer.Push(newData[i]);
                        else
                            if (_twoPlayer.GetCountCard < SearchInformation.MaxCardForHand)
                                _twoPlayer.Push(newData[i]);
                    }
                    generaldeck.PushCard(string.Join(" ", newData.Skip(SearchInformation.MaxCardForHand * 2).ToArray()));
                }
                else
                   throw new CardException("Не корректное число карт для начала игры");
            }
            private void PlayCard(string command, Player player)
            {
                Card fc = null;
                try
                {
                    fc = (Card)player.PlayerAction(command, SearchInformation.Command.Play, fieldeck.ToString()).Clone();
                    fieldeck.PushCard(fc);
                    player.Push(generaldeck[0]);
                    if (!player.GetInfoRisky)
                        CorrectCard++;
                }
                catch (CardException e)
                {
                    Conflict = true;
                    stackdeck.PushCard(fc);
                }
            }
            private void DropCrad(string command, Player player)
            {
                stackdeck.PushCard(player.PlayerAction(command, SearchInformation.Command.Drop, null));
                player.Push(generaldeck[0]);
            }
            private void TellColor(string command, Player player)
            {
                player.PlayerAction(command, SearchInformation.Command.Color, null);
                PlayerIsSay = player.IsSay;
                Isay = true;
            }
            private void TellRank(string command, Player player)
            {
                player.PlayerAction(command, SearchInformation.Command.Rank, null);
                PlayerIsSay = player.IsSay;
                Isay = true;
            }
            /// <summary>
            /// Выполнение текущего действия
            /// </summary>
            /// <param name="index">(0-5)Новая игра,сыграть,сбросить,подсказать цвет,подсказать ранг</param>
            /// <param name="command">текущая комманда</param>
            /// <param name="pl">игрок,который выполняет ход</param>
            private void SetCommand(int index, string command, Player player)
            {
                switch (index)
                {
                    case 0:
                        {
                            StartNewGame(command);
                            break;
                        }
                    case 1:
                        {
                            if (IsGame)
                            {
                                gameaction = new GameOperation(PlayCard);
                                gameaction(command, player);
                            }
                            break;
                        }
                    case 2:
                        {
                            if (IsGame)
                            {
                                gameaction = new GameOperation(DropCrad);
                                gameaction(command, player);
                            }
                            break;
                        }
                    case 3:
                        {
                            if (IsGame)
                            {
                                gameaction = new GameOperation(TellColor);
                                gameaction(command, player);
                            }
                            break;
                        }
                    case 4:
                        {
                            if (IsGame)
                            {
                                gameaction = new GameOperation(TellRank);
                                gameaction(command, player);
                            }
                            break;
                        }
                    default:
                        throw new CardException("Нет такой команды");
                }
            }
        }
    }
    public static class StringExtensions
    {
        /// <summary>
        /// Перевести строку в верхний регистр и разбить по пробелам
        /// </summary>
        /// <param name="s">строка данных</param>
        /// <returns></returns>
        public static string[] GetUpperStringToArray(this string s)
        {
            string[] words = s.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return words.Select(item => item.ToUpperInvariant()).Where(item => item != null).ToArray();
        }
        public static string[] StringToArray(this string s)
        {
            return s.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        }
        /// <summary>
        /// Получить все числа из строки
        /// </summary>
        /// <param name="resource"></param>
        /// <returns></returns>
        public static char[] GetDigit(this string resource)
        {
            return resource.Where(x => Char.IsDigit(x)).ToArray();
        }
    }

}