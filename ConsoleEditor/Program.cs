using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Terminal.Gui;

namespace ConsoleEditor
{
    class Program
    {
        static void Main(string[] args)
        {
            // RunGuiCS();
            var editor = new Editor();
            editor.LoadFile(File.ReadLines("Sample.html"));
            editor.RunCustomConsole();
        }
        
        

        private static void RunGuiCS()
        {
            
            Application.Init();
            var top = Application.Top;
            var win = new Window("Editor")
            {
                X = 0,
                Y = 1,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };
            top.Add(win);
            win.Add(new TextField());
            win.Add(new TextView()
            {
                X = 0,
                Y = 0,
                Height = Dim.Fill(),
                Width = Dim.Fill(),
            });
         
            Application.Run();  
        }
    }

    public class Editor
    {
        public Editor()
        {
            Console.Clear();
        }
        public List<string> Text { get; set; } = new List<string>();
        public void RunCustomConsole()
        {
            var (leftPos, topPos) = Console.GetCursorPosition();
            while (true)
            {
                var key= Console.ReadKey(true);
                

                Console.SetCursorPosition(leftPos, topPos);
                if (key.Key == ConsoleKey.Enter)
                {
                    Console.SetCursorPosition(0, Console.GetCursorPosition().Top+1);
                    leftPos = 0;
                    topPos++;
                }
                else if (key.Key == ConsoleKey.Escape)
                {
                    Console.Write("[esc]");
                    leftPos+=5;
                }
                else
                {
                    Console.Write(key.KeyChar);
                    leftPos++;
                }
                
                
                WriteStatusBar(key);
                
            }
        }

        private static void WriteStatusBar(ConsoleKeyInfo consoleKeyInfo)
        {
            var currentPos = Console.GetCursorPosition();
            var status = $"Key: {consoleKeyInfo.Key.ToString().PadRight(10)} Pos: {currentPos} ";
            Console.SetCursorPosition(0, Console.WindowHeight-1);
            
            Console.Write(status.PadRight(Console.WindowWidth));
            Console.SetCursorPosition(currentPos.Left, currentPos.Top);
        }

        public void LoadFile(IEnumerable<string> readLines)
        {
            this.Text = readLines.ToList();

            for (var i = 0; i < this.Text.Count && i < Console.WindowHeight - 1; i++)
            {
                 Console.SetCursorPosition(0, i);
                 Console.WriteLine(this.Text[i]);
            }
        }
    }
}