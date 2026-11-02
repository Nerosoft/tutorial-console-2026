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
    internal class MyClass
    {
        string HelpMessage;
        string TableTitlt;
        string option;
        string state;
        List<List<string>> myculc = new List<List<string>>();
        List<string> AddSearchculc;
        public MyClass(string state, string helpMessage, string tableTitlt, List<string> select = null, List<List<string>> culc = null)//change Dictionary ito list
        {
            this.state = state;
            HelpMessage = helpMessage;
            TableTitlt = tableTitlt;
            //for edit item
            if (select != null)
            {
                option = "operation";
                AddSearchculc = select;
            }
            else if (state == "search" || state == "add")
            {
                if (state == "search")
                    myculc = culc;
                option = "number";
                AddSearchculc = new List<string>() { state == "search" ? "??" : makeId(), "??" };
            }
            
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

        public string makeId()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 3);
        }
        public List<List<string>> getCulc()
        {
            return myculc;
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
            option = "operation";
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
            SearchCulc = new MyClass("search", "welcome in search item screen", "Table search all result", null, json["admin"]["culc"].AsObject().Select(x => x.Value.Deserialize<List<string>>().Prepend(x.Key).ToList()).ToList());
            CulcClass = SearchCulc;
            showTable(CulcClass.getCulc(), CulcClass.getTableTitlt());
            while (true)
            {
                if (CulcClass != null && CulcClass.getOption() == "confirem")
                {
                    Console.Write("Do you aggree Y/N? ");
                    string confirem = Console.ReadLine();
                    //make delete item
                    if (CulcClass.getState() == "search" && confirem == "y" && number.Contains("#"))
                    {
                        json["admin"]["culc"].AsObject().Remove(number.Substring(1));
                        saveFile("Successfully delete item");
                    }
                    else if (CulcClass.getState() == "search" && confirem == "y")
                    {
                        Console.WriteLine("Successfully search item");
                        //go to edit
                        EditCulc = new MyClass("edit", "Welcome in edit screen", "Welcome in edit table", SearchCulc.getAddsearch().ToList());
                        CulcClass = EditCulc;
                        showTable(CulcClass.getAddsearch(), CulcClass.getTableTitlt());
                        continue;// cancel change option

                    }
                    else if (confirem == "y")
                    {
                        json["admin"]["culc"].AsObject()[CulcClass.getAddsearch().First()] = JsonSerializer.SerializeToNode(CulcClass.getAddsearch().Skip(1));
                        saveFile(CulcClass.getState() == "add" ? "Successfully add item" : "Successfully edit item");
                        if (CulcClass.getState() == "add")
                            CulcClass.resetCulc();
                    }
                    else
                        Console.WriteLine("Cancel confirem");
                    CulcClass.resetOption();
                }
                else
                {
                    Console.Write(CulcClass.getOption() == "operation" ? "Enter your operator ? " : "Enter your number ? ");
                    number = Console.ReadLine();
                    if (number == "add")
                    {
                        CulcClass = AddCulc ?? new MyClass(number, "welcome in add item screen", "welcome in add table");
                        showTable(CulcClass.getAddsearch(), CulcClass.getTableTitlt());
                    }
                    else if (number == "search")
                    {
                        CulcClass = SearchCulc ?? new MyClass(number, "welcome in search item screen", "Table all result and search");
                        showTable(CulcClass.getAddsearch(), CulcClass.getTableTitlt());
                    }
                    else if (number == "edit" && SearchCulc.getCulc().Find(x => x.First() == number) != null)
                    {
                        CulcClass = EditCulc;
                        showTable(CulcClass.getAddsearch(), CulcClass.getTableTitlt());
                    }
                    else if (CulcClass.getState() == "edit" && CulcClass.getOption() == "operation" && number == "=" &&
                        SearchCulc.getCulc().Find(x => x.SequenceEqual(CulcClass.getAddsearch())) != null)
                        Console.WriteLine("no change value");
                    else if (CulcClass.getState() == "search" && number.Contains("#") || CulcClass.getOption() == "operation" && number == "=")
                    {
                        CulcClass.setOption("confirem");
                        //if (CulcClass.getState() == "search" && number.Contains("#") || CulcClass.getState() == "add")//for all add and search and edit
                            showTable(
                                CulcClass.getState() == "add" || CulcClass.getState() == "edit" || 
                                CulcClass.getState() == "search" && !number.Contains("#") ? CulcClass.getAddsearch()
                                : SearchCulc.getCulc().Find(x => x.First() == number.Substring(1)),
                                CulcClass.getState() == "add" || CulcClass.getState() == "edit" ||
                                CulcClass.getState() == "search" ? CulcClass.getTableTitlt() : "delete table");
                      


                    }

                    //make search by total use max min function
                    else if (CulcClass.getState() == "search" && number.Contains("<") || CulcClass.getState() == "search" && number.Contains(">"))
                        showTable(SearchCulc.getCulc().FindAll(x => number.Contains("<") && int.Parse(x.Last()) < int.Parse(number.Substring(1)) || number.Contains(">") && int.Parse(x.Last()) > int.Parse(number.Substring(1))).ToList(), number.Contains("<") ? "min search table" : "max search table");
                    else if (CulcClass.getState() == "search" && number.Contains("&"))
                    {
                        CulcClass.setCulc(SearchCulc.getCulc().Find(x => x.First() == number.Substring(1)));
                        showTable(CulcClass.getAddsearch(), CulcClass.getTableTitlt());
                    }

                    //show table culc
                    else if (number == "lee")
                        showTable(SearchCulc.getCulc(), SearchCulc.getTableTitlt());


                    else if (number == "help")
                        Console.WriteLine(CulcClass.getMessageHelp());
                    else if (number == "clear")
                    {
                        Console.WriteLine("Successfully cleare table");
                        CulcClass.resetCulc();
                        CulcClass.resetOption();
                    }
                    else
                    {
                        CulcClass.resetOption();
                        CulcClass.getAddsearch().Insert(CulcClass.getAddsearch().Count - 1, number);
                        CulcClass.getAddsearch()[CulcClass.getAddsearch().Count - 1] = CulcClass.getState() == "search" ||
                            CulcClass.getAddsearch().Count == 3 || CulcClass.getOption() == "number" ? "??" : 
                            
                            new DataTable().Compute(CulcClass.getAddsearch().GetRange(1, CulcClass.getAddsearch().Count - 2).Aggregate((a, b) => a + b), null).ToString();
                        
                        if (CulcClass.getState() == "search" && SearchCulc.getCulc().FindAll(x => x.GetRange(1, CulcClass.getAddsearch().Count - 2).SequenceEqual(CulcClass.getAddsearch().GetRange(1, CulcClass.getAddsearch().Count - 2))).Any())
                            showTable(SearchCulc.getCulc().FindAll(x => x.GetRange(1, CulcClass.getAddsearch().Count - 2).SequenceEqual(CulcClass.getAddsearch().GetRange(1, CulcClass.getAddsearch().Count - 2))), "search table");
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
            SearchCulc = new MyClass("search", "welcome in select item screen", "Table all result", null, json["admin"]["culc"].AsObject().Select(x => x.Value.Deserialize<List<string>>().Prepend(x.Key).ToList()).ToList());

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