using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace ConsoleEditor
{
    public enum EditorMode
    {
        Command,
        Input
    }

    public class Editor
    {
        public List<Func<Editor, bool>> Actions = new();
        public string CommandText = "";
        public ConsoleKeyInfo? LastKey;
        public EditorMode Mode = EditorMode.Input;

        public List<ImmutableList<string>> TextHistory = new();

        public Editor()
        {
            Console.Clear();
            Actions = new List<Func<Editor, bool>>();
            Actions.AddRange(BaseLibrary.InputCharacters);
            Actions.AddRange(BaseLibrary.ArrowActions);
        }

        public ImmutableList<string> Text { get; set; } = ImmutableList<string>.Empty;
        public int LeftPos { get; set; }
        public int TopPos { get; set; }

        public void RunCustomConsole()
        {
            (LeftPos, TopPos) = Console.GetCursorPosition();
            while (true)
            {
                LastKey = Console.ReadKey(true)!;

                for (var i = Actions.Count - 1; i >= 0; i--)
                {
                    var action = Actions[i];
                    var ran = action.Invoke(this);
                    if (ran) break;
                }

                using (var stdOut = Console.OpenStandardOutput())
                {
                    var outputString = string.Join("", Text.Select(x => x.PadRight(Console.BufferWidth)));
                    var outputBytes = Encoding.UTF8.GetBytes(outputString);
                    Console.SetCursorPosition(0, 0);
                    stdOut.Write(outputBytes, 0, outputBytes.Length);
                }


                Console.SetCursorPosition(LeftPos, TopPos);

                if (LastKey.Value.Key == ConsoleKey.Enter)
                {
                    Console.SetCursorPosition(0, Console.GetCursorPosition().Top + 1);
                    LeftPos = 0;
                    TopPos++;
                }
                // else if (LastKey.Value.Key == ConsoleKey.Escape)
                // {
                //     Console.Write("[esc]");
                //     LeftPos+=5;
                // }
                // else
                // {
                //     Console.Write(LastKey.Value.KeyChar);
                //     LeftPos++;
                // }


                WriteStatusBar(LastKey.Value);
            }
        }

        private static void WriteStatusBar(ConsoleKeyInfo consoleKeyInfo)
        {
            var currentPos = Console.GetCursorPosition();
            var status = $"Key: {consoleKeyInfo.Key.ToString().PadRight(10)} Pos: {currentPos} ";
            Console.SetCursorPosition(0, Console.WindowHeight - 1);

            Console.Write(status.PadRight(Console.WindowWidth));
            Console.SetCursorPosition(currentPos.Left, currentPos.Top);
        }

        public void LoadFile(IEnumerable<string> readLines)
        {
            if (Text.Any()) TextHistory.Add(Text);
            Text = readLines.ToImmutableList();

            for (var i = 0; i < Text.Count && i < Console.WindowHeight - 1; i++)
            {
                Console.SetCursorPosition(0, i);
                Console.WriteLine(Text[i]);
            }

            Console.SetCursorPosition(0, 0);
        }
    }

    public static class BaseLibrary
    {
        public static List<Func<Editor, bool>> ArrowActions = new()
        {
            editor =>
            {
                if (editor.LastKey?.Key != ConsoleKey.DownArrow) return false;
                editor.TopPos = editor.TopPos + 1;
                return true;
            },
            editor =>
            {
                if (editor.LastKey?.Key != ConsoleKey.UpArrow) return false;
                editor.TopPos = Math.Max(0, editor.TopPos - 1);
                return true;
            },
            editor =>
            {
                if (editor.LastKey?.Key != ConsoleKey.RightArrow) return false;
                editor.LeftPos += 1;
                return true;
            },
            editor =>
            {
                if (editor.LastKey?.Key != ConsoleKey.LeftArrow) return false;
                editor.LeftPos = Math.Max(0, editor.LeftPos - 1);
                return true;
            }
        };


        public static List<Func<Editor, bool>> InputCharacters = new()
        {
            editor =>
            {
                if (editor.LastKey?.Key == null || editor.Mode != EditorMode.Input) return false;
                // Console.Write(editor.LastKey.Value.KeyChar);

                editor.TextHistory.Add(editor.Text);
                // if (!editor.Text.Contains(editor.TopPos))
                // {
                //     // editor.Text[editor.TopPos] = "";
                // }


                var newLine = editor.Text[editor.TopPos]
                    // .PadRight(Console.WindowWidth)
                    .Insert(editor.LeftPos, editor.LastKey.Value.KeyChar.ToString());
                editor.Text = editor.Text.RemoveAt(editor.TopPos);
                editor.Text = editor.Text.Insert(editor.TopPos, newLine);

                // Console.SetCursorPosition(0, editor.TopPos);
                // Console.WriteLine(editor.Text[editor.TopPos]);

                editor.LeftPos += 1;
                Console.SetCursorPosition(editor.LeftPos, editor.TopPos);
                Console.OutputEncoding = Encoding.UTF8;
                //
                // using (var stdOut = Console.OpenStandardOutput())
                // {
                //     var outputString = string.Join("", editor.Text.Select(x => x.PadRight(Console.BufferWidth)));
                //     var outputBytes = Encoding.UTF8.GetBytes(outputString);
                //     stdOut.Write(outputBytes, 0, outputBytes.Length);
                //         
                // }
                //


                return true;
            }
        };
    }
}