using System.IO;
using Terminal.Gui;

namespace ConsoleEditor
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // RunGuiCS();
            // var editor = new Editor();
            Editor.LoadFile(File.ReadLines("Sample.html"));
            Editor.Run();
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
            win.Add(new TextView
            {
                X = 0,
                Y = 0,
                Height = Dim.Fill(),
                Width = Dim.Fill()
            });

            Application.Run();
        }
    }
}