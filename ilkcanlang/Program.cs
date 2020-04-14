using System;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Linq;
namespace ilkcanlang
{
    class MainClass
    {
        static List<string> varnames = new List<string>();
        static List<string> vars = new List<string>();
        static List<List<string>> lexiled = new List<List<string>>();
        public static void Main(string[] args)
        {
            lexiled.Add(new List<string>());
            lexiled.Add(new List<string>());

            Lexile(File.ReadAllText(args[0]));

            Interpret();

        }



        static void varop(string varname, string value)
        {
            string add = "";
            add = value;

            if (varnames.Contains(varname))
            {
                int varindex = varnames.IndexOf(varname);
                vars[varindex] = value;
              }
            else
            {
                varnames.Add(varname);
                vars.Add(add);
            }

        }

        static string getvar(string var)
        {
            try
            {
                int varindex = varnames.IndexOf(var);
                return vars[varindex];
            }
            catch
            {
                return String.Empty;
            }
        }

        static void Interpret()
        {
            int k = 0;
            List<List<List<string>>> part = new List<List<List<string>>>();
            part.Add(new List<List<string>>());
            part[0].Add(new List<string>());
            part[0].Add(new List<string>());


            for (int i = 0; i < lexiled[0].Count; i++)
            {

                if (lexiled[0][i] != "EOL")
                {
                    part[k][0].Add(lexiled[0][i]);
                    part[k][1].Add(lexiled[1][i]);
                }
                else
                {
                    k++;
                    part.Add(new List<List<string>>());
                    part[k].Add(new List<string>());
                    part[k].Add(new List<string>());

                }
            }

            //we have a list of 2 lists, splitted by EOL's.

            foreach (List<List<string>> line in part)
            {
                string cache = String.Empty;
                for (int a = 0; a < line[0].Count; a++)
                {

                    string def = line[0][a];
                    string cmd = line[1][a];


                    switch (def)
                    {

                        case "NUM":
                            cache = cache + cmd;
                            break;
                        case "OP":
                            if(cmd == ">")
                            {
                                try
                                {
                                    if (line[0][a + 1] == "VAR")
                                    {
                                        varop(line[1][a + 1], cache);
                                    }
                                }
                                catch
                                {
                                    Console.Write(cache);
                                }
                            }else if(cmd == "<")
                            {
                                if(line[0][a - 1] == "VAR")
                                {
                                    varop(line[1][a - 1],Console.ReadLine());
                                }
                            }
                            break;
                        case "VAR":
                            cache = cache + getvar(cmd);
                            break;
                        case "STR":
                            cache = cache + cmd.Substring(1, cmd.Length - 2).Replace("\\n", "\n");
                            break;
                    }

                    //Console.WriteLine("{0}\t{1}", def, cmd);
                }
                //Console.WriteLine("---");
            }




        }


        static void Lexile(string code)
        {
            List<List<string>> codeparsed = new List<List<string>>();
            foreach (var items in Regex.Split(code, "\n"))
            {
                codeparsed.Add(Regex.Split(items, "(\\/.*\\/)? (\\/.*\\/)?").Where(x => !string.IsNullOrEmpty(x)).ToList());
            }


            foreach (var items in codeparsed)
            {
                foreach (var item in items)
                {
                    if (Regex.IsMatch(item, "^\\d*$")) lexadd("NUM", item);
                    else if (Regex.IsMatch(item, "^[<>,;:]$")) lexadd("OP", item);
                    else if (Regex.IsMatch(item, "^@.*$")) lexadd("VAR", item);
                    else if (Regex.IsMatch(item, "^\\/.*\\/$")) lexadd("STR", item);
                    else if (Regex.IsMatch(item, "^\\.$")) lexadd("EOL", item);
                    else if (Regex.IsMatch(item, "^[\\[\\]\\(\\)]$")) lexadd("PAR", item);
                    else if (Regex.IsMatch(item, "^#.*$")) lexadd("CMT", item);
                    else if (Regex.IsMatch(item, "^[+\\-*\\/%&|\\^!]$")) lexadd("MATH", item);
                    else lexadd("CMD", item);

                }
                lexadd("EOL", "\n");
            }



        }

        static void lexadd(string a, string b)
        {
            lexiled[0].Add(a);
            lexiled[1].Add(b);
        }

    }

}
