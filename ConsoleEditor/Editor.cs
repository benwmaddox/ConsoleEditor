using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleEditor
{



    public class Pane
    {
        // positioning and size
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public AttachPosition AttachPosition { get; set; }
        public List<object> Children { get; set; } // Pane, document, etc. Rendered IN ORDER 
        
    }

    public enum AttachPosition
    {
        Top,
        Bottom,
        Left,
        Right
    }

    public class SelectList
    {
        
    }

    public class StatusBar
    {
        
    }

    public class CommandBar
    {
        
    }
    
    
    
    
    
    
    
    
    
    public enum EditorMode
    {
        Command,
        Input
    }

    public static class EditorCommands
    {
        public static Action<Document> MoveDown = doc =>
        {
            if (doc.Text.Count() > doc.VirtualTop)
            {
                doc.VirtualTop++;
                doc.VirtualLeft = Math.Min(doc.VirtualLeft, doc.Text[doc.VirtualTop].Length);
            }
        };

        public static Action<Document> MoveRight = doc =>
        {
            var line = doc.Text[doc.VirtualTop];
            doc.VirtualLeft = Math.Min(doc.VirtualLeft + 1, line.Length);
        };

        public static Action<Document> MoveUp = doc =>
        {
            if (doc.VirtualTop > 0)
            {
                doc.VirtualTop--;
                doc.VirtualLeft = Math.Min(doc.VirtualLeft, doc.Text[doc.VirtualTop].Length);
            }
        };

        public static Action<Document> MoveLeft = doc =>
        {
            if (doc.VirtualLeft > 0) doc.VirtualLeft--;
        };

        public static Action<Document> MoveToStart = doc =>
        {
            doc.VirtualTop = 0;
            doc.VirtualLeft = 0;
        };

        public static Action<Document> MoveToEnd = doc =>
        {
            doc.VirtualTop = doc.Text.Count() - 1;
            doc.VirtualLeft = doc.Text[doc.VirtualTop].Length;
        };

        public static Action<Document> MoveToLineStart = doc => { doc.VirtualLeft = 0; };

        public static Action<Document> MoveToLineEnd = doc => { doc.VirtualLeft = doc.Text[doc.VirtualTop].Length; };

        public static Action<Document> MoveToNextWord = doc =>
        {
            var line = doc.Text[doc.VirtualTop];
            var word = line.Substring(doc.VirtualLeft);
            var nextWord = word.Split(' ').FirstOrDefault();
            if (nextWord != null) doc.VirtualLeft += nextWord.Length;
        };

        public static Action<Document> MoveToPreviousWord = doc =>
        {
            var line = doc.Text[doc.VirtualTop];
            var word = line.Substring(0, doc.VirtualLeft);
            var previousWord = word.Split(' ').LastOrDefault();
            if (previousWord != null) doc.VirtualLeft -= previousWord.Length;

        };

        public static Action<Document> SelectAll = doc => { };

        public static Action<Document> Backspace = doc =>
        {
            if (doc.VirtualLeft > 0)
            {
                var line = doc.Text[doc.VirtualTop];
                doc.Text[doc.VirtualTop] = line.Substring(0, doc.VirtualLeft - 1) + line.Substring(doc.VirtualLeft);
                doc.VirtualLeft--;
            }
            else if (doc.VirtualTop > 0)
            {
                var line = doc.Text[doc.VirtualTop];
                doc.VirtualLeft = doc.Text[doc.VirtualTop - 1].Length;
                doc.Text[doc.VirtualTop - 1] += line;
                doc.VirtualTop--;
                doc.Text.RemoveAt(doc.VirtualTop + 1);
            }
        };

        public static Action<Document> Delete = doc =>
        {
            if (doc.VirtualLeft < doc.Text[doc.VirtualTop].Length)
            {
                var line = doc.Text[doc.VirtualTop];
                doc.Text[doc.VirtualTop] = line.Substring(0, doc.VirtualLeft) + line.Substring(doc.VirtualLeft + 1);
            }
            else if (doc.VirtualTop < doc.Text.Count() - 1)
            {
                var line = doc.Text[doc.VirtualTop];
                doc.Text[doc.VirtualTop] = line.Substring(doc.VirtualLeft);
                doc.VirtualTop++;
                doc.VirtualLeft = 0;
            }
        };

        public static Action<Document> NewLine = doc =>
        {
            var line = doc.Text[doc.VirtualTop];
            doc.Text[doc.VirtualTop] = line.Substring(0, doc.VirtualLeft);
            doc.Text.Insert(doc.VirtualTop + 1, line.Substring(doc.VirtualLeft));
            doc.VirtualTop++;
            doc.VirtualLeft = 0;
        };

        public static Action<Document> Undo = doc => { };
        public static Action<Document> Redo = doc => { };

        public static Action<Document> Cut = doc => { };
        public static Action<Document> Copy = doc => { };
        public static Action<Document> Paste = doc => { };



    }

    public static class Editor
    {
        public static List<Pane> Panes = new();
        public static Pane ActivePane { get; set; }
        
        
        // Active document?
        public static Document document = new();

        public static Dictionary<(ConsoleKey, ConsoleModifiers), Action<Document>> KeyboardMapping = new()
        {
            { (ConsoleKey.DownArrow, 0x0), EditorCommands.MoveDown },
            { (ConsoleKey.RightArrow, 0x0), EditorCommands.MoveRight },
            { (ConsoleKey.UpArrow, 0x0), EditorCommands.MoveUp },
            { (ConsoleKey.LeftArrow, 0x0), EditorCommands.MoveLeft },
            { (ConsoleKey.LeftArrow, ConsoleModifiers.Control), EditorCommands.MoveToPreviousWord },
            { (ConsoleKey.RightArrow, ConsoleModifiers.Control), EditorCommands.MoveToNextWord },
            { (ConsoleKey.Home, 0x0), EditorCommands.MoveToLineStart },
            { (ConsoleKey.End, 0x0), EditorCommands.MoveToLineEnd },
            { (ConsoleKey.Home, ConsoleModifiers.Control), EditorCommands.MoveToStart },
            { (ConsoleKey.End, ConsoleModifiers.Control), EditorCommands.MoveToEnd },
            { (ConsoleKey.Delete, 0x0), EditorCommands.Delete },
            { (ConsoleKey.Backspace, 0x0), EditorCommands.Backspace },
            { (ConsoleKey.Enter, 0x0), EditorCommands.NewLine },
            // { (ConsoleKey.Tab, 0x0), EditorV2Commands.Tab},
            // { (ConsoleKey.Tab, ConsoleModifiers.Shift), EditorV2Commands.Untab},
            { (ConsoleKey.A, ConsoleModifiers.Control), EditorCommands.SelectAll },
            { (ConsoleKey.C, ConsoleModifiers.Control), EditorCommands.Copy },
            { (ConsoleKey.V, ConsoleModifiers.Control), EditorCommands.Paste },
            { (ConsoleKey.X, ConsoleModifiers.Control), EditorCommands.Cut },
            { (ConsoleKey.Z, ConsoleModifiers.Control), EditorCommands.Undo },
            { (ConsoleKey.Y, ConsoleModifiers.Control), EditorCommands.Redo },

        };

        static Editor()
        {
            Console.Clear();
        }

        public static void Run()
        {
            var r = Console.ReadKey();
            while (true)
            {
                var keyMap = (r.Key, r.Modifiers);
                if (KeyboardMapping.ContainsKey(keyMap))
                    KeyboardMapping[keyMap].Invoke(document);
                else
                    InsertCharacter(r.KeyChar);

                RenderText();
                Console.SetCursorPosition(document.VirtualLeft, document.VirtualTop);
                r = Console.ReadKey();
            }
        }

        public static void Apply(Action<Document> action)
        {
            action.Invoke(document);
        }

        public static void InsertCharacter(char character)
        {
            var line = document.Text[document.VirtualTop];
            document.Text[document.VirtualTop] =
                line.Insert(Math.Min(document.VirtualLeft, line.Length), character.ToString());
            Apply(EditorCommands.MoveRight);
        }

        public static void LoadFile(IEnumerable<string> readLines)
        {
            document.Text = readLines.ToList();
            // if (Text.Any())
            // {
            //     TextHistory.Add(this.Text);
            // } 
            // Text = readLines.ToImmutableList();
            RenderText();
            Console.SetCursorPosition(0, 0);
        }

        public static void RenderText()
        {
            for (var i = 0; i < document.Text.Count && i < Console.WindowHeight - 1; i++)
            {
                Console.SetCursorPosition(0, i);
                Console.WriteLine(document.Text[i].PadRight(Console.WindowWidth - 1));
            }
        }
    }

    public class Document
    {
        public List<string> Text { get; set; } = new();
        public int VirtualLeft { get; set; }
        public int VirtualTop { get; set; }

        // public List<ImmutableList<string>> TextHistory = new List<ImmutableList<string>>();
        // public ImmutableList<string> Text { get; set; } = ImmutableList<string>.Empty;
        // public int LeftPos { get; set; } = 0;
        // public int TopPos { get; set; } = 0;
        // public List<Func<Editor, bool>> Actions = new List<Func<Editor, bool>>();
        // public ConsoleKeyInfo? LastKey = null;
        // public string CommandText = "";
        // public EditorMode Mode = EditorMode.Input;
        // public void RunCustomConsole()
        // {
        //     (LeftPos, TopPos) = Console.GetCursorPosition();
        //     while (true)
        //     {
        //         LastKey = Console.ReadKey(true)!;
        //
        //         for (int i = Actions.Count - 1; i >= 0; i--)
        //         {
        //             var action = Actions[i];
        //             var ran = action.Invoke(this);
        //             if (ran)
        //             {
        //                 break;
        //             }
        //         }
        //         using (var stdOut = Console.OpenStandardOutput())
        //         {
        //             var outputString = string.Join("", this.Text.Select(x => x.PadRight(Console.BufferWidth)));
        //             var outputBytes = Encoding.UTF8.GetBytes(outputString);
        //             Console.SetCursorPosition(0,0);
        //             stdOut.Write(outputBytes, 0, outputBytes.Length);
        //                 
        //         }
        //
        //
        //         Console.SetCursorPosition(LeftPos, TopPos);
        //          
        //         if (LastKey.Value.Key == ConsoleKey.Enter)
        //         {
        //             Console.SetCursorPosition(0, Console.GetCursorPosition().Top+1);
        //             LeftPos = 0;
        //             TopPos++;
        //         }
        //         // else if (LastKey.Value.Key == ConsoleKey.Escape)
        //         // {
        //         //     Console.Write("[esc]");
        //         //     LeftPos+=5;
        //         // }
        //         // else
        //         // {
        //         //     Console.Write(LastKey.Value.KeyChar);
        //         //     LeftPos++;
        //         // }
        //         
        //         
        //         WriteStatusBar(LastKey.Value);
        //         
        //     }
        // }

        // private static void WriteStatusBar(ConsoleKeyInfo consoleKeyInfo)
        // {
        //     var currentPos = Console.GetCursorPosition();
        //     var status = $"Key: {consoleKeyInfo.Key.ToString().PadRight(10)} Pos: {currentPos} ";
        //     Console.SetCursorPosition(0, Console.WindowHeight-1);
        //     
        //     Console.Write(status.PadRight(Console.WindowWidth));
        //     Console.SetCursorPosition(currentPos.Left, currentPos.Top);
        // }
    }

    // public static class BaseLibrary
    // {
    //     public static List<Func<Editor, bool>> ArrowActions = new List<Func<Editor, bool>>()
    //     {
    //         editor =>
    //         {
    //             if (editor.LastKey?.Key != ConsoleKey.DownArrow) return false;
    //             editor.TopPos = editor.TopPos + 1;
    //             return true;
    //         },
    //         editor =>
    //         {
    //             if (editor.LastKey?.Key != ConsoleKey.UpArrow) return false;
    //             editor.TopPos = Math.Max(0, editor.TopPos - 1);
    //             return true;
    //         },
    //         editor =>
    //         {
    //             if (editor.LastKey?.Key != ConsoleKey.RightArrow) return false;
    //             editor.LeftPos += 1;
    //             return true;
    //         },
    //         editor =>
    //         {
    //             if (editor.LastKey?.Key != ConsoleKey.LeftArrow) return false;
    //             editor.LeftPos = Math.Max(0, editor.LeftPos - 1);
    //             return true;
    //         }
    //     };
    //
    //
    //     public static List<Func<Editor, bool>> InputCharacters = new List<Func<Editor, bool>>()
    //     {
    //         editor =>
    //         {
    //             if (editor.LastKey?.Key == null || editor.Mode != EditorMode.Input) return false;
    //             // Console.Write(editor.LastKey.Value.KeyChar);
    //
    //             editor.TextHistory.Add(editor.Text);
    //             // if (!editor.Text.Contains(editor.TopPos))
    //             // {
    //             //     // editor.Text[editor.TopPos] = "";
    //             // }
    //
    //
    //             var newLine = editor.Text[editor.TopPos]
    //                 // .PadRight(Console.WindowWidth)
    //                 .Insert(editor.LeftPos, editor.LastKey.Value.KeyChar.ToString());
    //             editor.Text = editor.Text.RemoveAt(editor.TopPos);
    //             editor.Text = editor.Text.Insert(editor.TopPos, newLine);
    //
    //             // Console.SetCursorPosition(0, editor.TopPos);
    //             // Console.WriteLine(editor.Text[editor.TopPos]);
    //
    //             editor.LeftPos += 1;
    //             Console.SetCursorPosition(editor.LeftPos, editor.TopPos);
    //             Console.OutputEncoding = Encoding.UTF8;
    //             //
    //             // using (var stdOut = Console.OpenStandardOutput())
    //             // {
    //             //     var outputString = string.Join("", editor.Text.Select(x => x.PadRight(Console.BufferWidth)));
    //             //     var outputBytes = Encoding.UTF8.GetBytes(outputString);
    //             //     stdOut.Write(outputBytes, 0, outputBytes.Length);
    //             //         
    //             // }
    //             //
    //
    //
    //             return true;
    //         }
    //     };
    // }
}