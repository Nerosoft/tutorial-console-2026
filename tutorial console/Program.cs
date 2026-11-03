using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Policy;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace tutorial_console
{
    //make resit option and culc
    //make change  "Enter your operator ? " : "Enter your number ? "
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
        string ConfiremDelete;
        List <List<string>> myculc = new List<List<string>>();
        List<string> AddSearchculc;
        public MyClass(string state, string clearMessage, string navMessage, string confiremMessage, string helpMessage, string tableTitlt, List<string> select = null, List<List<string>> culc = null)
        {
            this.state = state;
            HelpMessage = helpMessage;
            TableTitlt = tableTitlt;
            ConfiremMessage = confiremMessage;
            NavMessage = navMessage;
            ClearMessage = clearMessage;
            //make edit item
            if (select != null)
            {
                option = "operation";
                AddSearchculc = select;
            }
            else if (state == "search" || state == "add")
            {
                if (state == "search")
                {
                    myculc = culc;
                    TitleResultCulc = "Table all result culc";
                    FoundCulcMessage = "Found:- culc table";
                    MinimizeMessage = "Minimize:- Found value table";
                    MaximizeMessage = "MaximizeM:- Found value table";
                    ConfiremDelete = "Confirem:- table delete";
                }
                option = "number";
                AddSearchculc = new List<string>() { state == "search" ? "??" : makeId(), "??" };
            }
            
        }
        public string getConfiremDelete()
        {
            return ConfiremDelete;
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
            resetOption();
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
        public void resetOption()
        {
            option = state == "search" || option == "number" || option == "confirem" ? "operation":"number";
        }
        public void resetCulc()
        {
            string id = state == "add"? this.makeId() : (state == "search"?"??": AddSearchculc.First());
            AddSearchculc.Clear(); 
            AddSearchculc.Add(id);
            AddSearchculc.Add("??");
        }
        public void setCulc(List<string> culc)
        {
            AddSearchculc = culc;
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
        static void Main(string[] args)
        {
            json = JsonNode.Parse(File.ReadAllText("data.json"));
            SearchCulc = new MyClass("search", "Clear:- Successfully cleare table", "Nav message:- Welcome in search screen", "Confirem:- table search", "Help message:- welcome in search screen", "Value:- search table", null, json["admin"]["culc"].AsObject().Select(x => x.Value.Deserialize<List<string>>().Prepend(x.Key).ToList()).ToList());
            CulcClass = SearchCulc;
            showTable(CulcClass.getCulc(), CulcClass.getTitleResultCulc());
            while (true)
            {
                if (CulcClass != null && CulcClass.getOption() == "confirem")
                {
                    CulcClass.resetOption();
                    Console.Write("Do you aggree Y/N? ");
                    string confirem = Console.ReadLine();
                    //make delete item
                    if (number.Contains("#") && confirem == "y")
                    {
                        json = JsonNode.Parse(File.ReadAllText("data.json"));
                        json["admin"]["culc"].AsObject().Remove(number.Substring(1));
                        saveFile("Successfully delete item");
                    }
                    // make search item
                    else if (CulcClass.getState() == "search" && confirem == "y")
                    {
                        Console.WriteLine("Successfully search item");
                        //go to edit
                        EditCulc = new MyClass("edit", "Clear:- Successfully cleare table", "Nav message:- Welcome in edit screen", "Confirem:- table edit", "Help message:- welcome in edit screen", "Value:- edit table", SearchCulc.getAddsearch().ToList());
                        CulcClass = EditCulc;
                        showTable(CulcClass.getAddsearch(), CulcClass.getTableTitlt());
                    }
                    //make edit or add item
                    else if (confirem == "y")
                    {
                        json = JsonNode.Parse(File.ReadAllText("data.json"));
                        json["admin"]["culc"].AsObject()[CulcClass.getAddsearch().First()] = JsonSerializer.SerializeToNode(CulcClass.getAddsearch().Skip(1));
                        saveFile(CulcClass.getState() == "add" ? "Successfully add item" : "Successfully edit item");
                        if (CulcClass.getState() == "add")
                            CulcClass.resetCulc();
                    }
                    else
                        Console.WriteLine("Cancel confirem");
                    //print table heeeeer
                }
                else
                {
                    Console.Write(CulcClass.getOption() == "operation" ? "Enter your operator ? " : "Enter your number ? ");
                    number = Console.ReadLine();
                    if (number == "add" || number == "search" || number == "edit" && EditCulc != null)
                    {
                        CulcClass = number == "search"? SearchCulc : (number == "edit"? EditCulc : (AddCulc ?? new MyClass(number, "Clear:- Successfully cleare table", "Nav message:- Welcome in add screen", "Confirem:- table add", "Help message :- welcome in add item screen", "Value:- add table")));
                        Console.WriteLine(CulcClass.getNavMessage());
                        showTable(CulcClass.getAddsearch(), CulcClass.getTableTitlt());
                    }
                    //make help and edit and test change value and edit == null
                    else if (number == "clear" || number == "help" || number == "edit" && EditCulc == null || CulcClass.getState() == "edit" && CulcClass.getOption() == "operation" && number == "=" && SearchCulc.getCulc().Find(x => x.SequenceEqual(CulcClass.getAddsearch())) != null)
                        Console.WriteLine(number == "clear"?CulcClass.getConfiremMessage():(number == "help"? CulcClass.getMessageHelp():(number == "edit"? "Edit null value please select value" : "no change value")));
                    
                    else if (CulcClass.getState() == "search" && number.Contains("&") || CulcClass.getState() == "search" && number.Contains("#") || CulcClass.getOption() == "operation" && number == "=")
                    {
                        CulcClass.setOption("confirem");
                        if(CulcClass.getState() == "search")
                            CulcClass.setCulc(CulcClass.getCulc().Find(x => x.First() == number.Substring(1)));
                        showTable(number.Contains("#") ? CulcClass.getCulc().Find(x => x.First() == number.Substring(1)):
                            CulcClass.getAddsearch(),
                            number.Contains("#") ? CulcClass.getConfiremDelete():CulcClass.getConfiremMessage());
                    }
                    //make search by total use max min function
                    //show table culc
                    else if (CulcClass.getState() == "search" && number.Contains("<") || CulcClass.getState() == "search" && number.Contains(">") || number == "lee")
                        showTable(number == "lee"? SearchCulc.getCulc():(CulcClass.getCulc().FindAll(x => number.Contains("<") && int.Parse(x.Last()) < int.Parse(number.Substring(1)) || number.Contains(">") && int.Parse(x.Last()) > int.Parse(number.Substring(1))).ToList()), number == "lee"?SearchCulc.getTitleResultCulc():(number.Contains("<") ? CulcClass.getMinimizeMessage() : CulcClass.getMaximizeMessage()));       
                    else
                    {
                        CulcClass.resetOption();
                        CulcClass.getAddsearch().Insert(CulcClass.getAddsearch().Count - 1, number);
                        CulcClass.getAddsearch()[CulcClass.getAddsearch().Count - 1] = CulcClass.getState() == "search" ||CulcClass.getAddsearch().Count == 3 || CulcClass.getOption() == "number" ? "??" :new DataTable().Compute(CulcClass.getAddsearch().GetRange(1, CulcClass.getAddsearch().Count - 2).Aggregate((a, b) => a + b), null).ToString();
                        if (CulcClass.getState() == "search" && CulcClass.getCulc().FindAll(x => x.GetRange(1, CulcClass.getAddsearch().Count - 2).SequenceEqual(CulcClass.getAddsearch().GetRange(1, CulcClass.getAddsearch().Count - 2))).Any())
                            showTable(CulcClass.getCulc().FindAll(x => x.GetRange(1, CulcClass.getAddsearch().Count - 2).SequenceEqual(CulcClass.getAddsearch().GetRange(1, CulcClass.getAddsearch().Count - 2))), CulcClass.getFoundCulcMessage());
                        showTable(CulcClass.getAddsearch(), CulcClass.getTableTitlt());
                    }
                    InitMyClass();


                }
            }
        }
        static void saveFile(string message)
        {
            File.WriteAllText("data.json", json.ToJsonString(
                new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                }));
            Console.WriteLine(message);
            SearchCulc.setMyCulc(json["admin"]["culc"].AsObject().Select(x => x.Value.Deserialize<List<string>>().Prepend(x.Key).ToList()).ToList());
        }
        static void showTable<T>(T culc2, string title)
        {
            string colume = "column", total = "Total", empty = "empty", Id = "ID";
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
            displayLine(lineSize, title);
            for (int v = 0; v < culc.Count; v++)
            {
                culc[v].InsertRange(culc[v].Count - 1, Enumerable.Repeat(culc[v].Count == 2 ? colume : empty, count - culc[v].Count));
                for (int i = 0; i < count; i++)
                {
                    if (culc[v][i].Length < word)
                    {
                        int abc = word - culc[v][i].Length;
                        for (int j = 0; j < abc; j++)
                            culc[v][i] += " ";
                    }
                    Console.Write($"|{culc[v][i]}");
                }
                displayLine(lineSize);
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
        static void displayLine(int count, string title = "|") 
        {
            Console.WriteLine(title);
            for (int x = -1; x < count; x++)
                Console.Write('-');
            Console.WriteLine();
        }
    }
}