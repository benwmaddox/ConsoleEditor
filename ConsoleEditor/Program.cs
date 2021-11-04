using System.IO;
using Terminal.Gui;

namespace ConsoleEditor
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // Editor.LoadFile(File.ReadLines("Sample.html"));
            // Editor.LoadFile(File.ReadLines("Sample.js"));
            // Editor.Run();
            //
            Application.Init();
            var top = Application.Top;
            var colorScheme = new ColorScheme()
            {
                Normal = Attribute.Make(Color.White, Color.DarkGray),
                HotFocus = Attribute.Make(Color.White, Color.Black),
                HotNormal = Attribute.Make(Color.White, Color.DarkGray),
                Focus = Attribute.Make(Color.White, Color.Black)
            };
            Application.Top.ColorScheme = colorScheme;
            var winLeft = new Window("Sample.html")
            {
                X = 0,
                Y = 0,
                Width = Dim.Percent(50),
                Height = Dim.Fill() - 1,
                ColorScheme = colorScheme
            };
            top.Add(winLeft);
            var textView = new TextView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill() - 3,
                ColorScheme = colorScheme,
                Text = File.ReadAllText("Sample.html")
            };

            var winRight = new Window("Sample.js")
            {
                X = Pos.Percent(50),
                Y = 0,
                Width = Dim.Percent(50),
                Height = Dim.Fill() - 1,
                ColorScheme = colorScheme
            };
            top.Add(winRight);
            
            var textViewRight = new TextView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill() - 3,
                ColorScheme = colorScheme,
                Text = File.ReadAllText("Sample.js")
            };
            winRight.Add(textViewRight);
            
            var commandBar = new TextField()
            {
                X = 0,
                Y = Pos.AnchorEnd()-1,
                Text = "",
                Width = Dim.Fill(),
                Height = 1,
                ColorScheme = colorScheme
                
            };
            
            top.Add(commandBar);
            winLeft.Add(textView);
            // var editor = new Editor(win);
            // editor.LoadFile(File.ReadLines("Sample.html"));
            
            // Application.MainLoop
            
            Application.Run();
            
            
        }
    }
}