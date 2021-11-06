using System;
using System.IO;
using System.Linq;
using Terminal.Gui;
using Attribute = Terminal.Gui.Attribute;

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
                Normal = Attribute.Make(Color.White, Color.Black),
                HotFocus = Attribute.Make(Color.White, Color.Black),
                HotNormal = Attribute.Make(Color.White, Color.Black),
                Focus = Attribute.Make(Color.White, Color.Black)
            };
            Application.Top.ColorScheme = colorScheme;
            var editableSection = new View()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill() - 1
            };
            top.Add(editableSection);
            
            var winLeft = new Window("Sample.html")
            {
                X = 0,
                Y = 0,
                Width = Dim.Percent(50),
                Height = Dim.Fill() - 1,
                ColorScheme = colorScheme
            };
            // top.Add(winLeft);
            editableSection.Add(winLeft);
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
            // top.Add(winRight);
            editableSection.Add(winRight);
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

            var commandView = new View()
            {
                X = 0,
                Y = Pos.AnchorEnd()-1,
                Width = Dim.Fill(),
                Height = 1,
                ColorScheme = colorScheme
            };
            var commandBar = new TextField()
            {
                X = 8,
                Y = 0,
                Text = "",
                Width = Dim.Fill(),
                Height = 1,
                ColorScheme = colorScheme
            };
            commandView.Add(new Label()
            {
                X = 0,
                Y = 0,
                Text = "Command> ",
                ColorScheme = colorScheme
            });
            commandView.Add(commandBar);
            top.Add(commandView);
            winLeft.Add(textView);
            // var editor = new Editor(win);
            // editor.LoadFile(File.ReadLines("Sample.html"));
            
            // Application.MainLoop
            
            // Application.Run();
            var timeoutToken = Application.MainLoop.AddTimeout(TimeSpan.FromSeconds(10), loop =>
            {

                editableSection.Add();
                var bufferWin = new Window("Buffer")
                {
                    X = Pos.Percent(50),
                    Y = 0,
                    Width = Dim.Fill(),
                    Height = Dim.Fill() - 1,
                    ColorScheme = colorScheme
                };
                var textViewBuffer = new TextView()
                {
                    X = 0,
                    Y = 0,
                    Width = Dim.Fill(),
                    Height = Dim.Fill() - 3,
                    ColorScheme = colorScheme,
                    Text = File.ReadAllText("Sample.js")
                };
                bufferWin.Add(textViewBuffer);
                editableSection.Add(bufferWin);

                EvenlySplitChildrenWidth(editableSection);
                return true;
            });
            top.KeyUp += eventArgs =>
            {
                if (eventArgs.KeyEvent.Key ==  (Key.Space | Key.CtrlMask) && !commandBar.HasFocus)
                {
                    eventArgs.Handled = true;
                    commandBar.SetFocus();
                    return;
                }
                else if (eventArgs.KeyEvent.Key ==  (Key.Space | Key.CtrlMask) && commandBar.HasFocus)
                {
                    eventArgs.Handled = true;
                    editableSection.FocusLast(); // Should retain information to pick the right one
                    return;
                }

                if (eventArgs.KeyEvent.Key == (Key.W | Key.CtrlMask))
                {
                    var index = editableSection.Subviews.IndexOf(editableSection.Focused);
                    editableSection.Remove(editableSection.Focused);
                    if (index == 0)
                    {
                        editableSection.FocusFirst();
                    }
                    else if (editableSection.Subviews.Any())
                    {
                        editableSection.Subviews[index - 1].SetFocus();
                    }
                    EvenlySplitChildrenWidth(editableSection);
                }
            };
            // top.KeyUp += eventArgs => 
            // {
            //     if (eventArgs.KeyEvent.Key == Key.Tab) //eventArgs.KeyEvent.IsCtrl && eventArgs.KeyEvent.Key == Key.Space )
            //     {
            //         editableSection.Add();
            //         var bufferWin = new Window("Sample.js")
            //         {
            //             X = Pos.Percent(50),
            //             Y = 0,
            //             Width = Dim.Percent(50),
            //             Height = Dim.Fill() - 1,
            //             ColorScheme = colorScheme
            //         };
            //         var textViewBuffer = new TextView()
            //         {
            //             X = 0,
            //             Y = 0,
            //             Width = Dim.Fill(),
            //             Height = Dim.Fill() - 3,
            //             ColorScheme = colorScheme,
            //             Text = File.ReadAllText("Buffer")
            //         };
            //         bufferWin.Add(textViewBuffer);
            //         editableSection.Add(bufferWin);
            //
            //         var winCount = editableSection.Subviews.Count;
            //         var percent = 100.0 / winCount ;
            //         foreach (var subview in editableSection.Subviews)
            //         {
            //             subview.Width = Dim.Percent((int)percent);
            //         }
            //         
            //     }
            // };
            winLeft.FocusFirst();
            
            // Application.AlternateForwardKey = Key.CtrlMask & Key.Space;
            
            
            // Application.RunLoop(new Application.RunState(top), true);
            Application.Run();
        }

        private static void EvenlySplitChildrenWidth(View view)
        {
            
            var winCount = view.Subviews.Count;
            if (winCount == 0) return;
            var percent = 100 / winCount ;
            for (var index = 0; index < view.Subviews.Count; index++)
            {
                var subview = view.Subviews[index];
                subview.Width = Dim.Percent(percent);
                subview.X = Pos.Percent(index * percent);
            }
        }
    }
}