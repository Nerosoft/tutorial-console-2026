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
        Dictionary<string, List<string>> myculc = new Dictionary<string, List<string>>();
        List<string> AddSearchculc;
        public MyClass(string state, string helpMessage, string tableTitlt, MyClass select = null, Dictionary<string, List<string>> culc = null)//change Dictionary ito list
        {
            this.state = state;
            HelpMessage = helpMessage;
            TableTitlt = tableTitlt;
            if (state == "select")
                myculc = culc;
            
            //for edit item
            else if (select != null)
            {
                option = "operation";
                AddSearchculc = select.getCulc()[state].Prepend(state).ToList();
            }
            else if (state == "search" || state == "add")
            {
                option = "number";
                AddSearchculc = new List<string>() { state == "search" ? "??" : makeId(), "??" };
            }
            
        }
        public string getTableTitlt()
        {
            return TableTitlt;
        }
        public string getMessageHelp()
        {
            return HelpMessage;
        }
        public string makeId()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 3);
        }
        public Dictionary<string, List<string>> getCulc()
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
            option = option == "number" || option == "confirem" ? "operation":"number";
        }
        public void resetCulc()
        {
            option = "number";
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
        static string state = "select";
        static MyClass SearchCulc;
        static MyClass AddCulc;
        static MyClass EditCulc;
        static MyClass SelectCucl;
        static MyClass CulcClass;
        static JsonNode json;
        static void Main(string[] args)
        {
            json = JsonNode.Parse(File.ReadAllText("data.json"));
            while (true)
            {
                if (CulcClass != null && CulcClass.getOption() == "confirem")
                {
                    Console.Write("Do you aggree Y/N? ");
                    string confirem = Console.ReadLine();

                    if (state == "select" && confirem == "y")
                    {
                        json["admin"]["culc"].AsObject().Remove(state.Substring(1));
                        saveFile("Successfully delete item");
                        state = "select";
                    }
                    else if (state == "search" && confirem == "y")
                    {
                        Console.WriteLine("Successfully search item");
                        //go to edit
                        EditCulc = SearchCulc;
                        CulcClass.resetOption();
                        //save key like select key
                        state = CulcClass.getAddsearch().First();
                        CulcClass.setState(state);
                    }
                    else if (confirem == "y")
                    {
                        saveCulcDataBase(CulcClass, state == "add" ? "Successfully add item" : "Successfully edit item");
                        CulcClass.setOption(state == "add" ? "number" : "operation");
                    }
                    else if (state.Contains("#"))
                    {
                        state = "select";
                        Console.WriteLine("Cancel confirem");
                    }
                    else
                    {
                        CulcClass.setOption("operation");
                        Console.WriteLine("Cancel confirem");
                    }


                }
                else if(state == "select" || state == "add" || state == "search" || SelectCucl.getCulc()[state] != null)//.TryGetValue(state, out List<string>value))
                {
                    CulcClass = InitMyClass();
                    Console.Write(state == "select"? "Select your item ? " : (CulcClass.getOption() == "operation" ? "Enter your operator ? " : "Enter your number ? "));
                    string number = Console.ReadLine();
                    if(state == "select" && SelectCucl.getCulc()[number] != null)
                    {
                        state = number;
                    }
                    else if (state == "select" && number.Contains("#") || CulcClass.getOption().Equals("operation") && number.Equals("="))
                    {
                        CulcClass.setOption("confirem");
                        if(state == "select")//for all add and search and edit
                            showTable(CulcClass.getCulc()[number.Substring(1)], "delete table");
                    }
                    //make search by total use max min function
                    else if (state == "search" && number.Contains("<") || state == "search" && number.Contains(">"))
                        showTable(SelectCucl.getCulc().Values.Where(x => number.Contains("<") && int.Parse(x.Last()) < int.Parse(number.Substring(1)) || number.Contains(">") && int.Parse(x.Last()) > int.Parse(number.Substring(1))).ToList(), number.Contains("<") ? "min search table" : "max search table");
                    else if (state == "search" && number.Contains("&"))
                        CulcClass.setCulc(SelectCucl.getCulc()[number.Substring(1)].Prepend(number.Substring(1)).ToList());
                    else if (state != "add" && state != "search" && CulcClass.getOption() == "operation" && number == "=" &&
                         EditCulc.getAddsearch().Skip(1).SequenceEqual(SelectCucl.getCulc()[EditCulc.getState()]))
                        Console.WriteLine("no change value");
                    else if (number == "help")
                        Console.WriteLine(CulcClass.getMessageHelp());
                    else if (number == "clear")
                    {
                        Console.WriteLine("Successfully cleare table");
                        CulcClass.resetCulc();
                    }
                    else
                    {
                        CulcClass.resetOption();
                        CulcClass.getAddsearch().Insert(CulcClass.getAddsearch().Count - 1, number);
                        if (state == "search")
                            CulcClass.getAddsearch()[CulcClass.getAddsearch().Count - 1] = "??";
                        else
                            CulcClass.getAddsearch()[CulcClass.getAddsearch().Count - 1] = CulcClass.getAddsearch().Count == 3 || CulcClass.getOption() == "number" ? "??" : new DataTable().Compute(CulcClass.getAddsearch().GetRange(1, CulcClass.getAddsearch().Count - 2).Aggregate((a, b) => a + b), null).ToString();
                    }
                }
            }
        }
        static void saveCulcDataBase(MyClass obj, string message)
        {
            json["admin"]["culc"].AsObject()[obj.getAddsearch().First()] = JsonSerializer.SerializeToNode(obj.getAddsearch().Skip(1));
            saveFile(message);
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
        }
        static void showTable<T>(T culc2, string title)
        {
            string colume = "column", total = "Total", empty = "empty", Id = "ID";
            List<List<string>> culc = new List<List<string>> {
                new List<string> { Id, total },
            };
            if (typeof(T) == typeof(List<string>))
                culc.Add(new List<string>((List<string>)(object)culc2));
            else if (typeof(T) == typeof(Dictionary<string, List<string>>))
                culc.AddRange(((Dictionary<string, List<string>>)(object)culc2).Select(item => item.Value.Prepend(item.Key).ToList()));
            else
                culc.AddRange(((List<List<string>>)(object)culc2).ConvertAll(x=>new List<string>(x)));
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
        static MyClass InitMyClass()
        {
            switch (state)
            {
                case "add":
                    AddCulc = AddCulc ?? new MyClass(state, "welcome in add item screen", "welcome in add table");
                    showTable(AddCulc.getAddsearch(), AddCulc.getTableTitlt());
                    return AddCulc;
                case "select":
                    SelectCucl = SelectCucl??new MyClass(state, "welcome in select item screen", "Table all result", null, JsonSerializer.Deserialize<Dictionary<string, List<string>>>(json["admin"]["culc"]));
                    showTable(SelectCucl.getCulc(), SelectCucl.getTableTitlt());
                    return SelectCucl;
                case "search":
                    SearchCulc = SearchCulc ?? new MyClass(state, "welcome in search item screen", "Table all result and search");
                    showTable(SelectCucl.getCulc(), SearchCulc.getTableTitlt());

                    if (SearchCulc.getAddsearch().Count > 2)
                    {
                        List<List<string>> result = SelectCucl.getCulc().Values.Where(x => x.Take(SearchCulc.getAddsearch().Count - 2).
                        SequenceEqual(SearchCulc.getAddsearch().GetRange(1, SearchCulc.getAddsearch().Count - 2))).ToList();
                        showTable(result, "search table");
                    }
                    showTable(SearchCulc.getAddsearch(), SearchCulc.getTableTitlt());
                    return SearchCulc;
                default:
                    EditCulc = EditCulc ?? new MyClass(state, "welcome in edit item screen", "Table edit culc", SelectCucl);
                    showTable(EditCulc.getAddsearch(), EditCulc.getTableTitlt());
                    return EditCulc;
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