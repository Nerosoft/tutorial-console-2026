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
        string state;
        Dictionary<string, List<string>> myculc = new Dictionary<string, List<string>>();
        List<string> AddSearchculc = new List<string>() { Guid.NewGuid().ToString("N").Substring(0, 3), "??" };
        string option { get; set; }
        public MyClass(string state, MyClass select = null)
        {
            this.state = state;
            if (state == "select")
            {
                JsonNode json = JsonNode.Parse(File.ReadAllText("data.json"));
                //Dictionary<string, Dictionary<string, string>> dic = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(json["admin"]["culc"]);
                //myculc = dic.Select(x => new List<string> (x.Value.Select(y => y.Value)).Prepend(x.Key).ToList()).ToList();
                myculc = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(json["admin"]["culc"]);
            }
            //for edit item
            else if (select != null)
            {
                option = "operation";
                //myculc.Add(makeId(), select.getCulc()[state]);
            }
            else if (state == "search" || state == "add")
                option = "number";
        }
        string makeId()
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
            var id = AddSearchculc.First();
            AddSearchculc.Clear();
            AddSearchculc.Add(id);
            AddSearchculc.Add("??");
            //myculc = new List<List<string>> {
            //        new List<string> { new MyClass("select").getCulc().Find(x => x[0].Equals(state)) != null? state : (state == "search"?"??":Guid.NewGuid().ToString("N").Substring(0,3)) ,"??"}
            //    };
        }
        public void setCulc(List<string> culc)
        {
            option = "operation";
            //myculc[0] = culc;
        }
        public void setState(string state)
        {
            this.state = state;
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
            //JsonNode json = JsonNode.Parse(File.ReadAllText("data.json"));
            //json["admin"]["language"] = "arabic";
            //File.WriteAllText("data.json", json.ToJsonString(new JsonSerializerOptions { WriteIndented = true,Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping}));
            //Dictionary<string, Dictionary<string, string>> dic = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(json["admin"]["culc"]);
            //List<List<string>> list = dic.Select(x =>new List<string>(x.Value.Select(y=>y.Value))).ToList();
            while (true)
            {
                if(state == "select")
                {
                    CulcClass = InitMyClass();
                    Console.Write("Select your item ? ");
                    state = Console.ReadLine();
                }
                else if (CulcClass != null && CulcClass.getOption() == "confirem")
                {
                    Console.Write("Do you aggree Y/N? ");
                    string confirem = Console.ReadLine();
                    if (state == "add" && confirem == "y")
                    {
                        JsonNode json = JsonNode.Parse(File.ReadAllText("data.json"));
                        json["admin"]["culc"][CulcClass.getAddsearch().First()] =
                            JsonSerializer.SerializeToNode(
                                CulcClass.getAddsearch().Skip(1),
                                new JsonSerializerOptions
                                {
                                    WriteIndented = true,
                                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                                });
                        File.WriteAllText("data.json", json.ToJsonString(
                            new JsonSerializerOptions
                            {
                                WriteIndented = true,
                                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                            }));

                        Console.WriteLine("Successfully add item");
                        CulcClass.resetCulc();
                    }
                    else if (state == "search" && confirem == "y")
                    {
                        Console.WriteLine("Successfully search item");
                        //go to edit
                        EditCulc = SearchCulc;
                        CulcClass.resetOption();
                        //save key like select key
                        state = CulcClass.getCulc().First().Key;
                        CulcClass.setState(state);
                    }
                    else
                    {
                        Console.WriteLine("Cancel confirem");
                        CulcClass.setOption("operation");
                    }

                }
                else if(state == "add" || state == "search" || CulcClass.getCulc().Where(x=>x.Key == state)!= null)
                {
                    CulcClass = InitMyClass();
                    Console.Write(CulcClass.getOption() == "operation" ? "Enter your operator ? " : "Enter your number ? ");
                    string number = Console.ReadLine();
                    if (number == "delete")
                    {
                        //showTable(new List<List<string>> { new MyClass("select").getCulc().Find(x => x[0].Equals(EditCulc.getCulc()[0][0])) }, "delete table");
                        Console.Write("do you want to delete this item Y/N ? ");
                        number = Console.ReadLine();
                        if (number.Equals("y"))
                        {
                            Console.WriteLine("Successfully delete item");
                            state = "select";
                            EditCulc = null;
                        }
                        Console.WriteLine("Delete item is cancel");
                    }
                    //make search by total use max min function
                    //else if (state.Equals("search") && number.Contains(">") || state.Equals("search") && number.Contains("<"))
                    //    showTable(number.Contains("<")? new MyClass("select").getCulc().FindAll(x => int.Parse(x.Last()) < int.Parse(number.Substring(1))) : new MyClass("select").getCulc().FindAll(x => int.Parse(x.Last()) > int.Parse(number.Substring(1))) , number.Contains("<")? "min search table" : "max search table");
                    //else if (state.Equals("search") && number.Contains("&"))
                    //    CulcClass.setCulc(new MyClass("select").getCulc().Find(x => x[0].Equals(number.Substring(1))));

                    else if (CulcClass.getOption().Equals("operation") && number.Equals("="))
                        CulcClass.setOption("confirem");
                    else if (state == "add" && number == "help")
                        Console.WriteLine("welcome in add item screen");
                    else if (state == "search" && number == "help")
                        Console.WriteLine("welcome in search item screen");
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
        static void showTable<T>(T culc2, string title = "Table all result")
        {
            string colume = "column", total = "Total", empty = "empty", Id = "ID";
            List<List<string>> culc = new List<List<string>> {
                new List<string> { Id, total },
            };
            if (typeof(T) == typeof(List<string>))
                culc.Add(new List<string>((List<string>)(object)culc2));
            else 
                culc.AddRange(((Dictionary<string, List<string>>)(object)culc2).Select(item => item.Value.Prepend(item.Key).ToList()));
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
                    AddCulc = AddCulc ?? new MyClass(state);
                    showTable(AddCulc.getAddsearch(), "welcome in add table");
                    return AddCulc;
                case "select":
                    SelectCucl = new MyClass(state);
                    showTable(SelectCucl.getCulc());
                    return SelectCucl;
                case "search":
                    showTable(SelectCucl.getCulc());
                    SearchCulc = SearchCulc ?? new MyClass(state);

                    //if (SearchCulc.getCulc()[0].Count > 2)
                    //{
                    //    List<List<string>> result = select.getCulc().Where(x => x.Skip(1).Take(SearchCulc.getCulc().First().Count - 2).SequenceEqual(SearchCulc.getCulc().First().GetRange(1, SearchCulc.getCulc().First().Count - 2))).ToList();
                    //    showTable(result, "search table");
                    //}
                    showTable(SearchCulc.getCulc(), "welcome in search table");
                    return SearchCulc;
                default:
                    EditCulc = EditCulc ?? new MyClass(state, SelectCucl);
                    showTable(EditCulc.getCulc(), "Edit table");
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