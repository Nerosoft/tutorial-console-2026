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
        string NumberMessage;
        string OperatorMessage;
        string SuccessfullyMessage;
        string CancelMessage;
        string ValueChangeMessage;
        string NullMessage;
        string DeleteMessage;
        string ConfiremDeleteMessage;
        List <List<string>> myculc = new List<List<string>>();
        List<string> AddSearchculc;
        public MyClass(string state, string cancelMessage, string successfullyMessage, string numberMessage, string operatorMessage, string clearMessage, string navMessage, string confiremMessage, string helpMessage, string tableTitlt, List<string> select = null, List<List<string>> culc = null)
        {
            this.state = state;
            HelpMessage = helpMessage;
            TableTitlt = tableTitlt;
            ConfiremMessage = confiremMessage;
            NavMessage = navMessage;
            ClearMessage = clearMessage;
            NumberMessage = numberMessage;
            OperatorMessage = operatorMessage;
            SuccessfullyMessage = successfullyMessage;
            CancelMessage = cancelMessage;
            NullMessage = "Edit null value please select value";
            //make edit item
            if (select != null)
            {
                ValueChangeMessage = "No change value";
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
                    DeleteMessage = "Successfully delete item";
                    ConfiremDeleteMessage = "Cancel confirem delete";
                }
                option = "number";
                AddSearchculc = new List<string>() { state == "search" ? "??" : makeId(), "??" };
            }
            
        }
        public string getConfiremDeleteMessage()
        {
            return ConfiremDeleteMessage;
        }
        public string getDeleteMessage()
        {
            return DeleteMessage;
        }
        public string getSuccessfullyMessage()
        {
            return SuccessfullyMessage;
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
            option = "number";
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
            option = option == "number" || option == "confirem" ? "operation":"number";
        }
        public void resetCulc()
        {
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
        static void Main(string[] args)
        {
            json = JsonNode.Parse(File.ReadAllText("data.json"));
            SearchCulc = new MyClass("search", "Cancel confirem search", "Successfully search item", "set value or using #ID for delete and &ID for edit item? ", "set operator or using #ID for delete and &ID for edit item? ", "Clear:- Successfully cleare table", "Nav message:- Welcome in search screen", "Confirem:- table search", "Help message:- welcome in search screen", "Value:- search table", null, json["admin"]["culc"].AsObject().Select(x => x.Value.Deserialize<List<string>>().Prepend(x.Key).ToList()).ToList());
            CulcClass = SearchCulc;
            showTable(CulcClass.getCulc(), CulcClass.getTitleResultCulc());
            while (true)
            {
                if (CulcClass != null && CulcClass.getOption() == "confirem")
                {
                    showTable(number.Contains("#") || number.Contains("&") ?CulcClass.setCulc(CulcClass.getCulc().Find(x => x.First() == number.Substring(1)).ToList()):CulcClass.getAddsearch(), number.Contains("#") ? CulcClass.getConfiremDelete() : CulcClass.getConfiremMessage());
                    CulcClass.resetOption();
                    Console.Write("Do you aggree Y/N? ");
                    string confirem = Console.ReadLine();

                    //make delete item
                    if (number.Contains("#") && confirem == "y")
                    {
                        json = JsonNode.Parse(File.ReadAllText("data.json"));
                        json["admin"]["culc"].AsObject().Remove(number.Substring(1));
                        saveFile();
                        Console.WriteLine(CulcClass.getDeleteMessage());
                    }
                    // make search item
                    else if (CulcClass.getState() == "search" && confirem == "y")
                    {
                        Console.WriteLine(CulcClass.getSuccessfullyMessage());
                        //go to edit
                        EditCulc = new MyClass("edit", "Cancel confirem edit", "Successfully edit item", "Enter your number ? ", "Enter your operator ? ", "Clear:- Successfully cleare table", "Nav message:- Welcome in edit screen", "Confirem:- table edit", "Help message:- welcome in edit screen", "Value:- edit table", SearchCulc.getAddsearch().ToList());
                        CulcClass = EditCulc;
                        showTable(CulcClass.getAddsearch(), CulcClass.getTableTitlt());
                    }
                    //make edit or add item
                    else if (confirem == "y")
                    {
                        json = JsonNode.Parse(File.ReadAllText("data.json"));
                        json["admin"]["culc"].AsObject()[CulcClass.getAddsearch().First()] = JsonSerializer.SerializeToNode(CulcClass.getAddsearch().Skip(1));
                        saveFile();
                        if (CulcClass.getState() == "add")
                            CulcClass.resetCulc();
                        Console.WriteLine(CulcClass.getSuccessfullyMessage());
                    }
                    else
                    {
                        Console.WriteLine(number.Contains("#")?CulcClass.getConfiremDeleteMessage():CulcClass.getCancelMessage());
                        showTable(CulcClass.getAddsearch(), CulcClass.getTableTitlt());
                    }
                    
                    //print table heeeeer
                }
                else
                {
                    Console.Write(CulcClass.getOption() == "operation" ? CulcClass.getOperatorMessage() : CulcClass.getNumberMessage());
                    number = Console.ReadLine();
                    if (number == "add" || number == "search" || number == "edit" && EditCulc != null)
                    {
                        CulcClass = number == "search"? SearchCulc : (number == "edit"? EditCulc : (AddCulc ?? new MyClass(number, "Cancel confirem add", "Successfully add item", "Enter your number ? ", "Enter your operator ? ", "Clear:- Successfully cleare table", "Nav message:- Welcome in add screen", "Confirem:- table add", "Help message :- welcome in add item screen", "Value:- add table")));
                        Console.WriteLine(CulcClass.getNavMessage());
                        showTable(CulcClass.getAddsearch(), CulcClass.getTableTitlt());
                    }
                    //make help and edit and test change value and edit == null
                    else if (number == "clear" || number == "help" || number == "edit" && EditCulc == null || CulcClass.getState() == "edit" && CulcClass.getOption() == "operation" && number == "=" && SearchCulc.getCulc().Exists(x => x.SequenceEqual(CulcClass.getAddsearch())))
                        Console.WriteLine(number == "clear"?CulcClass.getClearMessage():(number == "help"? CulcClass.getMessageHelp():(number == "edit"? CulcClass.getNullMessage() : CulcClass.getValueChangeMessage())));
                    
                    else if (CulcClass.getState() == "search" && number.Contains("&") && CulcClass.getCulc().Exists(x => x.First() == number.Substring(1))||
                        CulcClass.getState() == "search" && number.Contains("#") && CulcClass.getCulc().Exists(x => x.First() == number.Substring(1)) || 
                        CulcClass.getOption() == "operation" && number == "=")
                        CulcClass.setOption("confirem");
 
                    //make search by total use max min function
                    //show table culc
                    else if (CulcClass.getState() == "search" && number.Contains("<") || CulcClass.getState() == "search" && number.Contains(">") || number == "lee")
                        showTable(number == "lee"? SearchCulc.getCulc():(CulcClass.getCulc().FindAll(x => number.Contains("<") && int.Parse(x.Last()) < int.Parse(number.Substring(1)) || number.Contains(">") && int.Parse(x.Last()) > int.Parse(number.Substring(1))).ToList()), number == "lee"?SearchCulc.getTitleResultCulc():(number.Contains("<") ? CulcClass.getMinimizeMessage() : CulcClass.getMaximizeMessage()));       
                    else
                    {
                        CulcClass.resetOption();
                        CulcClass.getAddsearch().Insert(CulcClass.getAddsearch().Count - 1, number);
                        try
                        {
                            CulcClass.getAddsearch()[CulcClass.getAddsearch().Count - 1] = CulcClass.getState() == "search" ||
                                CulcClass.getAddsearch().Count == 3 ||
                                CulcClass.getOption() == "number" ? "??" : new DataTable().Compute(CulcClass.getAddsearch().GetRange(1, CulcClass.getAddsearch().Count - 2).Aggregate((a, b) => a + b), null).ToString();
                        }
                        catch { }
                        if (CulcClass.getState() == "search" && CulcClass.getCulc().FindAll(x => CulcClass.getAddsearch().Count < x.Count && x.GetRange(1, CulcClass.getAddsearch().Count - 2).SequenceEqual(CulcClass.getAddsearch().GetRange(1, CulcClass.getAddsearch().Count - 2))).Any())
                            showTable(CulcClass.getCulc().FindAll(x => x.GetRange(1, CulcClass.getAddsearch().Count - 2).SequenceEqual(CulcClass.getAddsearch().GetRange(1, CulcClass.getAddsearch().Count - 2))), CulcClass.getFoundCulcMessage());
                        showTable(CulcClass.getAddsearch(), CulcClass.getTableTitlt());
                    }
                    InitMyClass();
                }
            }
        }
        static void saveFile()
        {
            File.WriteAllText("data.json", json.ToJsonString(
                new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                }));
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
                culc[v].InsertRange(culc[v].Count - 1, Enumerable.Repeat(v == 0 ? colume : empty, count - culc[v].Count));
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