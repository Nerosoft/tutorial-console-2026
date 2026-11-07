using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace tutorial_console
{
    internal class MyClass
    {
        string HelpMessage;
        string TableTitlt;
        string option;
        string state;
        string ConfiremMessage;
        string TitleResultCulc;
        string NavMessage;
        string FoundCulcMessage;
        string ClearMessage;
        string MinimizeMessage;
        string MaximizeMessage;
        string NumberMessage;
        string OperatorMessage;
        string SuccessfullyMessage;
        string CancelMessage;
        string ValueChangeMessage;
        string NullMessage;
        string DeleteMessage;
        string AgreeConfiremMessage;

        List<List<string>> myculc = new List<List<string>>();
        List<string> AddSearchculc;
        public MyClass(string state, JsonNode language, List<string> select = null, List<List<string>> culc = null)
        {
            this.state = state;
            HelpMessage = language["HelpMessage"].GetValue<string>();
            TableTitlt = language["TableTitlt"].GetValue<string>();
            ConfiremMessage = language["ConfiremMessage"].GetValue<string>();
            NavMessage = language["NavMessage"].GetValue<string>();
            ClearMessage = language["ClearMessage"].GetValue<string>();
            NumberMessage = language["NumberMessage"].GetValue<string>();
            OperatorMessage = language["OperatorMessage"].GetValue<string>();
            SuccessfullyMessage = language["SuccessfullyMessage"].GetValue<string>();
            CancelMessage = language["CancelMessage"].GetValue<string>();
            NullMessage = language["NullMessage"].GetValue<string>();
            AgreeConfiremMessage = language["AgreeConfiremMessage"].GetValue<string>();
            AddSearchculc = select?? new List<string>() { state == "search" ? "??" : makeId(), "??" };
            option = state == "edit"? "operation": "number";       
            if (state == "search")
            {
                myculc = culc;
                TitleResultCulc = language["TitleResultCulc"].GetValue<string>();
                FoundCulcMessage = language["FoundCulcMessage"].GetValue<string>();
                MinimizeMessage = language["MinimizeMessage"].GetValue<string>();
                MaximizeMessage = language["MaximizeMessage"].GetValue<string>();
                DeleteMessage = language["DeleteMessage"].GetValue<string>();
            }
            else if(state == "edit")
                ValueChangeMessage = language["ValueChangeMessage"].GetValue<string>();
        }
        public string getAgreeConfiremMessage()
        {
            return AgreeConfiremMessage;
        }

        public string getSuccessfullyMessage()
        {
            return SuccessfullyMessage;
        }
        public string getDeleteMessage()
        {
            return DeleteMessage;
        }
      
        public string getCancelMessage()
        {
            return CancelMessage;
        }
        public string getValueChangeMessage()
        {
            return ValueChangeMessage;
        }
        public string getNullMessage()
        {
            return NullMessage;
        }
        public string getNumberMessage()
        {
            return NumberMessage;
        }
        public string getOperatorMessage()
        {
            return OperatorMessage;
        }
        public string getMinimizeMessage()
        {
            return MinimizeMessage;
        }
        public string getMaximizeMessage()
        {
            return MaximizeMessage;
        }
        public string getTitleResultCulc()
        {
            return TitleResultCulc;
        }
        public string getTableTitlt()
        {
            return TableTitlt;
        }
        public void setTableTitlt(string tableTile)
        {
            TableTitlt = tableTile;
        }
        public string getMessageHelp()
        {
            return HelpMessage;
        }
        public void setMessageHelp(string message)
        {
            HelpMessage = message;
        }
        public string getConfiremMessage()
        {
            return ConfiremMessage;
        }
        public string getNavMessage()
        {
            return NavMessage;
        }
        public string getFoundCulcMessage()
        {
            return FoundCulcMessage;
        }
        public string getClearMessage()
        {
            resetCulc();
            return ClearMessage;
        }
        public string makeId()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 3);
        }
        public List<List<string>> getCulc()
        {
            return myculc;
        }
        public void setMyCulc(List<List<string>> culc)
        {
            myculc = culc;
        }
        public List<string> getAddsearch()
        {
            return AddSearchculc;
        }
        public string getOption()
        {
            return option;
        }
        public void setOption(string myOption)
        {
            option = myOption;
        }
        public void resetCulc()
        {
            option = "number";
            string id = state == "add"? this.makeId() : (state == "search"?"??": AddSearchculc.First());
            AddSearchculc.Clear(); 
            AddSearchculc.Add(id);
            AddSearchculc.Add("??");
        }
        public List<string> setCulc(List<string> culc)
        {
            AddSearchculc = culc;
            return AddSearchculc;
        }
        public void setState(string state)
        {
            this.state = state;
        }
        public string getState()
        {
            return this.state;
        }
    }
    internal class Program
    {
        static string number;
        static JsonNode json;
        static MyClass SearchCulc;
        static MyClass AddCulc;
        static MyClass EditCulc;
        static MyClass CulcClass; 
        static JsonObject language;
        static JsonObject color;

        static void Main(string[] args)
        {
            string DbName = "admin";
            //Console.WriteLine("Select your database? ");
            //DbName = Console.ReadLine();
            json = JsonNode.Parse(File.ReadAllText("data.json"));
            language = json[DbName][json[DbName]["language"].GetValue<string>()].AsObject();
            color = json[DbName][json[DbName]["style"].GetValue<string>()].AsObject();
            SearchCulc = new MyClass("search", language["Search"], null, json[DbName]["culc"].AsObject().Select(x => x.Value.Deserialize<List<string>>().Prepend(x.Key).ToList()).ToList());
            CulcClass = SearchCulc;
            showTable(CulcClass.getCulc(), CulcClass.getTitleResultCulc());
            while (true)
            {
                showTable(CulcClass.getOption() == "confirem" && CulcClass.getState() == "search" && number.Contains("&") ?  CulcClass.setCulc(CulcClass.getCulc().Find(x => x.First() == number.Substring(1)).ToList()):CulcClass.getAddsearch(), CulcClass.getOption() == "confirem"? CulcClass.getConfiremMessage() : CulcClass.getTableTitlt());
                PrintColor(CulcClass.getOption() == "confirem" ? CulcClass.getAgreeConfiremMessage() :(CulcClass.getOption() == "operation" ? CulcClass.getOperatorMessage() : CulcClass.getNumberMessage()), CulcClass.getOption() == "confirem" ?GetColor("Confirem") : (CulcClass.getOption() == "operation" ? GetColor("Value") : GetColor("Operator")));
                number = Console.ReadLine();
                if (number == "add" || number == "search" || number == "edit" && EditCulc != null)
                {
                    CulcClass = number == "search" ? SearchCulc : (number == "edit" ? EditCulc : (AddCulc ?? 
                        new MyClass(number, language["Add"])));
                    PrintColorLine(CulcClass.getNavMessage(), GetColor("Nav"));
                }
                //make search by total use max min function
                //show table culc
                else if (CulcClass.getState() == "search" && number.Contains("<") || CulcClass.getState() == "search" && number.Contains(">") || number == "lee")
                    showTable(number == "lee" ? SearchCulc.getCulc() : (CulcClass.getCulc().FindAll(x => number.Contains("<") && int.Parse(x.Last()) < int.Parse(number.Substring(1)) || number.Contains(">") && int.Parse(x.Last()) > int.Parse(number.Substring(1))).ToList()), number == "lee" ? SearchCulc.getTitleResultCulc() : (number.Contains("<") ? CulcClass.getMinimizeMessage() : CulcClass.getMaximizeMessage()));
                // make search item
                else if (CulcClass.getOption() == "confirem" && CulcClass.getState() == "search" && number == "e")
                {
                    EditCulc = new MyClass("edit",language["Edit"], CulcClass.getAddsearch().ToList());
                    PrintMessage();
                    CulcClass = EditCulc;
                }
                //make edit or add item
                else if (
                    CulcClass.getOption() == "confirem" && number == "d" && EditCulc != null && EditCulc.getAddsearch().First() == CulcClass.getAddsearch().First() && json[DbName]["culc"].AsObject().Remove(CulcClass.getAddsearch().First()) && (EditCulc = null) == null||
                    CulcClass.getOption() == "confirem" && number == "d" && json[DbName]["culc"].AsObject().Remove(CulcClass.getAddsearch().First()) ||
                    CulcClass.getOption() == "confirem" && number == "y" && (json[DbName]["culc"].AsObject()[CulcClass.getAddsearch().First()] = JsonSerializer.SerializeToNode(CulcClass.getAddsearch().Skip(1))) != null)
                {
                    File.WriteAllText("data.json", json.ToJsonString(new JsonSerializerOptions{WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping}));
                    json = JsonNode.Parse(File.ReadAllText("data.json"));
                    SearchCulc.setMyCulc(json[DbName]["culc"].AsObject().Select(x => x.Value.Deserialize<List<string>>().Prepend(x.Key).ToList()).ToList());
                    showTable(SearchCulc.getCulc(), SearchCulc.getTitleResultCulc());
                    PrintMessage();
                }
                else if(CulcClass.getOption() == "confirem" && number != "y")
                {
                    CulcClass.setOption("operation");
                    PrintColorLine(CulcClass.getCancelMessage(), GetColor("Message"));
                }

                //make help and edit and test change value and edit == null
                else if (number == "clear" || number == "help" || number == "edit" && EditCulc == null || CulcClass.getState() == "edit" && CulcClass.getOption() == "operation" && number == "=" && SearchCulc.getCulc().Exists(x => x.SequenceEqual(CulcClass.getAddsearch())))
                    PrintColorLine(number == "clear" ? CulcClass.getClearMessage() : (number == "help" ? CulcClass.getMessageHelp() : (number == "edit" ? CulcClass.getNullMessage() : CulcClass.getValueChangeMessage())), GetColor("Message"));
                else if (CulcClass.getState() == "search" && number.Contains("&") && CulcClass.getCulc().Exists(x => x.First() == number.Substring(1)) ||CulcClass.getState() != "search" && CulcClass.getOption() == "operation" && number == "=")
                    CulcClass.setOption("confirem");
                else
                {
                    CulcClass.setOption(CulcClass.getOption() == "number" ? "operation" : "number");
                    CulcClass.getAddsearch().Insert(CulcClass.getAddsearch().Count - 1, number);
                    try
                    {
                        CulcClass.getAddsearch()[CulcClass.getAddsearch().Count - 1] = CulcClass.getState() == "search" ||CulcClass.getAddsearch().Count == 3 ||CulcClass.getOption() == "number" ? "??" : new DataTable().Compute(CulcClass.getAddsearch().GetRange(1, CulcClass.getAddsearch().Count - 2).Aggregate((a, b) => a + b), null).ToString();
                    }
                    catch { }
                    if(CulcClass.getState() == "search" && CulcClass.getCulc().FindAll(x => CulcClass.getAddsearch().Count < x.Count && x.GetRange(1, CulcClass.getAddsearch().Count - 2).SequenceEqual(CulcClass.getAddsearch().GetRange(1, CulcClass.getAddsearch().Count - 2))).Any())
                        showTable(CulcClass.getCulc().FindAll(x => CulcClass.getAddsearch().Count < x.Count && x.GetRange(1, CulcClass.getAddsearch().Count - 2).SequenceEqual(CulcClass.getAddsearch().GetRange(1, CulcClass.getAddsearch().Count - 2))), CulcClass.getFoundCulcMessage());
                }
                InitMyClass();
            }
        }
        static void PrintColor(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(message);
            Console.ResetColor();
        }
        static void PrintColorLine(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }
        static void PrintMessage()
        {
            if (CulcClass.getState() == "edit")
            {
                CulcClass.setOption("operation");
                if (CulcClass.getAddsearch().First() == SearchCulc.getAddsearch().First())
                    SearchCulc.resetCulc();
            }
            else
                CulcClass.resetCulc();
            PrintColorLine(number == "d" ? CulcClass.getDeleteMessage() : CulcClass.getSuccessfullyMessage(), GetColor("Message"));
        }
        static void showTable<T>(T culc2, string title)
        {
            string colume = language["Table"]["Colume"].GetValue<string>(),
                total = language["Table"]["Total"].GetValue<string>(),
                empty = language["Table"]["Empty"].GetValue<string>(),
                Id = language["Table"]["Id"].GetValue<string>();
            List<List<string>> culc = new List<List<string>> {
                new List<string> { Id, total },
            };
            if (typeof(T) == typeof(List<string>))
                culc.Add(((List<string>)(object)culc2).ToList());
            else
                culc.AddRange(((List<List<string>>)(object)culc2).Select(x=>x.ToList()));
            int count = culc.Max(num => num.Count),
                word = Math.Max(Math.Max(colume.Length, empty.Length), culc.SelectMany(x => x).Max(x => x.Length)),
                lineSize = ((word * count) + count);
            displayLine(lineSize, GetColor("TitleTable"), GetColor("Table"), title);
            for (int v = 0; v < culc.Count; v++)
            {
                culc[v].InsertRange(culc[v].Count - 1, Enumerable.Repeat(v == 0 ? colume : empty, count - culc[v].Count));
                for (int i = 0; i < count; i++)
                {
                    if (culc[v][i].Length < word)
                    {
                        int abc = word - culc[v][i].Length;
                        for (int j = 0; j < abc; j++)
                            culc[v][i] += " ";
                    }
                    PrintColor(language["Table"]["Shep|"].GetValue<string>(), GetColor("Table"));
                    PrintColor(culc[v][i], GetColor("DataTable"));
                }
                displayLine(lineSize, GetColor("Table"), GetColor("Table"), language["Table"]["Shep|"].GetValue<string>());
            }
        }
        static void InitMyClass()
        {
            switch (CulcClass.getState())
            {
                case "add":
                    AddCulc = CulcClass;
                    break;
                case "search":
                    SearchCulc = CulcClass;
                    break;
                case "edit":
                    EditCulc = CulcClass;
                    break;
            }
        }
        static void displayLine(int count, ConsoleColor color1, ConsoleColor color2, string title) 
        {
            PrintColorLine(title, color1);
            for (int x = -1; x < count; x++)
                PrintColor(language["Table"]["Shep-"].GetValue<string>(), color2);
            Console.WriteLine();
        }
        static ConsoleColor GetColor(string mcolor)
        {
            mcolor = color[mcolor].GetValue<string>();
             switch (mcolor.ToLower())
            {
                case "blue":return ConsoleColor.Blue;
                case "red":return ConsoleColor.Red;
                case "green":return ConsoleColor.Green;
                case "yellow":return ConsoleColor.Yellow;
                case "cyan":return ConsoleColor.Cyan;
                case "magenta":return ConsoleColor.Magenta;
                case "white":return ConsoleColor.White;
                case "black":return ConsoleColor.Black;
                case "gray":return ConsoleColor.Gray;
                case "darkblue":return ConsoleColor.DarkBlue;
                case "darkred":return ConsoleColor.DarkRed;
                case "darkgreen":return ConsoleColor.DarkGreen;
                case "darkyellow":return ConsoleColor.DarkYellow;
                case "darkcyan":return ConsoleColor.DarkCyan;
                case "darkmagenta":return ConsoleColor.DarkMagenta;
                case "darkgray":return ConsoleColor.DarkGray;
                default:return ConsoleColor.White;
            };
        }
    }
}