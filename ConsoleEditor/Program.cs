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
            Editor.LoadFile(File.ReadLines("Sample.js"));
            Editor.Run();
            // editor.RunCustomConsole();
        }
    }
}