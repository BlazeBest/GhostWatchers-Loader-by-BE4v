using System;
using System.Linq;
using System.Threading;
using Donteco;
using GWLoader;
using GWLoader.Attributes;

namespace GiveExpOrMoney
{
    [ModuleInfo("GiveExpOrMoney", "0.1v", "BlazeBest")]
    public class Class1 : GWLoader.GWLoader
    {
        public static void Main()
        {
            new Thread(() => { while (true) { consoleThread(); } }).Start();
        }

        public static void consoleThread()
        {
            string[] args = new string[0];
            string consoleRead = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(consoleRead)) return;
            try
            {
                args = ConsoleResource.SplitCommandLine(consoleRead).ToArray();
            }
            catch { return; }

            switch (args[0])
            {
                case "clear":
                    {
                        Console.Clear();
                        break;
                    }
                case "give_money":
                    {
                        try
                        {
                            int count = int.Parse(args[1]);
                            GiveMoney(count);
                        }
                        catch { Console.WriteLine("usage: give_money <count>"); }
                        break;
                    }
                case "remove_money":
                    {
                        try
                        {
                            int count = int.Parse(args[1]);
                            RemoveMoney(count);
                        }
                        catch { Console.WriteLine("usage: remove_money <count>"); }
                        break;
                    }
                case "set_money":
                    {
                        try
                        {
                            int count = int.Parse(args[1]);
                            SetMoney(count);
                        }
                        catch { Console.WriteLine("usage: set_money <count>"); }
                        break;
                    }
                case "get_money":
                    {
                        try
                        {
                            Console.WriteLine("Your money is: " + GetMoney());
                        }
                        catch { Console.WriteLine("usage: get_money"); }
                        break;
                    }
                case "add_exp":
                    {
                        try
                        {
                            int count = int.Parse(args[1]);
                            AddExp(count);
                        }
                        catch { Console.WriteLine("usage: add_exp <count>"); }
                        break;
                    }
                case "get_exp":
                    {
                        try
                        {
                            Console.WriteLine("Your exp is: " + GetExp());
                        }
                        catch { Console.WriteLine("usage: get_exp"); }
                        break;
                    }
                default: return;
            }
        }

        public static int GetMoney()
        {
            return DataStorage.Get("dollars", 0);
        }

        private static void GiveMoney(int count)
        {
            if (count < 1) return;
            int num = DataStorage.Get("dollars", 0);
            num += count;
            DataStorage.Set("dollars", num, true);
        }

        private static void SetMoney(int count)
        {
            if (count < 0) return;
            DataStorage.Set("dollars", count, true);
        }

        private static void RemoveMoney(int count)
        {
            int num = DataStorage.Get("dollars", 0);
            if (num - count < 0) return;
            num -= count;
            DataStorage.Set("dollars", num, true);
        }

        public static void AddExp(int value)
        {
            DataStorage.Update("exp", (int e) => e + value, true);
        }

        public static int GetExp()
        {
            return DataStorage.Get("exp", 0);
        }

    }
}
