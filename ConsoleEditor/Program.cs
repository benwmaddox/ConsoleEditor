using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Terminal.Gui;

namespace ConsoleEditor
{
    class Program
    {
        static void Main(string[] args)
        {
            // RunGuiCS();
            var editor = new EditorV2();
            editor.LoadFile(File.ReadLines("Sample.html"));
            editor.Run();
            // editor.RunCustomConsole();

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
}