using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        JsonNode json;
        string option;
        string state;
        Dictionary<string, List<string>> myculc = new Dictionary<string, List<string>>();
        List<string> AddSearchculc;
        public MyClass(string state, string helpMessage, string tableTitlt, MyClass select = null)
        {
            this.state = state;
            HelpMessage = helpMessage;
            TableTitlt = tableTitlt;
            json = JsonNode.Parse(File.ReadAllText("data.json"));
            if (state == "select")
                myculc = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(json["admin"]["culc"]);
            
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
        public void saveCulcDataBase(MyClass obj)
        {
            json["admin"]["culc"].AsObject()[obj.getAddsearch().First()] =  JsonSerializer.SerializeToNode(obj.getAddsearch().Skip(1));
            File.WriteAllText("data.json", json.ToJsonString(
                new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                }));
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
        public void addValueList(string number)
        {
            AddSearchculc.Insert(AddSearchculc.Count-1, number);
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
        static void Main(string[] args)
        {
            while (true)
            {
                if(state == "select")
                {
                    CulcClass = InitMyClass();
                    Console.Write("Select your item ? ");
                    state = Console.ReadLine();
                    if (state.Contains("#"))
                    {
                        showTable(SelectCucl.getCulc()[state.Substring(1)], "delete table");
                        Console.Write("do you want to delete this item Y/N ? ");
                        string op = Console.ReadLine();
                        if (op == "y")
                        {
                            JsonNode json = JsonNode.Parse(File.ReadAllText("data.json"));
                            json["admin"]["culc"].AsObject().Remove(state.Substring(1));
                            File.WriteAllText("data.json", json.ToJsonString(
                                new JsonSerializerOptions
                                {
                                    WriteIndented = true,
                                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                                }));
                            Console.WriteLine("Successfully delete item");
                        }
                        state = "select";
                        Console.WriteLine("Delete item is cancel");
                    }
                }
                else if (CulcClass != null && CulcClass.getOption() == "confirem")
                {
                    Console.Write("Do you aggree Y/N? ");
                    string confirem = Console.ReadLine();
                    if (state == "search" && confirem == "y")
                    {
                        Console.WriteLine("Successfully search item");
                        //go to edit
                        EditCulc = SearchCulc;
                        CulcClass.resetOption();
                        //save key like select key
                        state = CulcClass.getAddsearch().First();
                        CulcClass.setState(state);
                    }
                    else if (state == "add" && confirem == "y")
                    {
                        SelectCucl.saveCulcDataBase(CulcClass);
                        Console.WriteLine("Successfully add item");
                        CulcClass.resetCulc();
                    }
                   
                    else if(confirem == "y")
                    {
                        SelectCucl.saveCulcDataBase(CulcClass);
                        Console.WriteLine("Successfully edit item");
                        CulcClass.resetOption();
                    }
                    else
                    {
                        Console.WriteLine("Cancel confirem");
                        CulcClass.setOption("operation");
                    }

                }
                else if(state == "add" || state == "search" || SelectCucl.getCulc()[state] != null)//.TryGetValue(state, out List<string>value))
                {
                    CulcClass = InitMyClass();
                    Console.Write(CulcClass.getOption() == "operation" ? "Enter your operator ? " : "Enter your number ? ");
                    string number = Console.ReadLine();
                    //make search by total use max min function
                    if (state == "search" && number.Contains("<") || state == "search" && number.Contains(">"))
                        showTable(SelectCucl.getCulc().Values.Where(x => number.Contains("<") && int.Parse(x.Last()) < int.Parse(number.Substring(1)) || number.Contains(">") && int.Parse(x.Last()) > int.Parse(number.Substring(1))).ToList(), number.Contains("<") ? "min search table" : "max search table");
                    else if (state == "search" && number.Contains("&"))
                        CulcClass.setCulc(SelectCucl.getCulc()[number.Substring(1)].Prepend(number.Substring(1)).ToList());
                    else if (state != "add" && state != "search" && CulcClass.getOption() == "operation" && number == "=" &&
                         EditCulc.getAddsearch().Skip(1).SequenceEqual(SelectCucl.getCulc()[EditCulc.getState()]))
                        Console.WriteLine("no change value");
                    else if (CulcClass.getOption().Equals("operation") && number.Equals("="))
                        CulcClass.setOption("confirem");
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
                        CulcClass.addValueList(number);
                        if (state != "search")
                            CulcClass.getAddsearch()[CulcClass.getAddsearch().Count - 1] = CulcClass.getAddsearch().Count == 3 || CulcClass.getOption() == "number" ? "??" : new DataTable().Compute(CulcClass.getAddsearch().GetRange(1, CulcClass.getAddsearch().Count - 2).Aggregate((a, b) => a + b), null).ToString();
                    }
                }
            }
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
                    SelectCucl = SelectCucl??new MyClass(state, "welcome in select item screen", "Table all result");
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