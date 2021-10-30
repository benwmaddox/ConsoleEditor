using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Linq;

namespace ConsoleEditor
{

    public enum EditorV2Mode
    {
        Command,
        Input
    }

    public static class EditorV2Commands
    {
        public static Action<Document> MoveDown = (Document doc) =>
        {
            if (doc.Text.Count() > doc.VirtualTop)
            {
                doc.VirtualTop++;
                doc.VirtualLeft = Math.Min(doc.VirtualLeft, doc.Text[doc.VirtualTop].Length);
            }
        };
        public static Action<Document> MoveRight = (Document doc) =>
        {
            var line = doc.Text[doc.VirtualTop];
            doc.VirtualLeft = Math.Min(doc.VirtualLeft + 1, line.Length);
        };

        public static Action<Document> MoveUp = (Document doc) =>
        {
            if (doc.VirtualTop > 0)
            {
                doc.VirtualTop--;
                doc.VirtualLeft = Math.Min(doc.VirtualLeft, doc.Text[doc.VirtualTop].Length);
            }
        };
        
        public static Action<Document> MoveLeft = (Document doc) =>
        {
            if (doc.VirtualLeft > 0)
            {
                doc.VirtualLeft--;
            }
        };
        
        public static Action<Document> MoveToStart = (Document doc) =>
        {
            doc.VirtualTop = 0;
            doc.VirtualLeft = 0;
        };
        public static Action<Document> MoveToEnd = (Document doc) =>
        {
            doc.VirtualTop = doc.Text.Count() - 1;
            doc.VirtualLeft = doc.Text[doc.VirtualTop].Length;
        };
        
        public static Action<Document> MoveToLineStart = (Document doc) =>
        {
            doc.VirtualLeft = 0;
        };
        public static Action<Document> MoveToLineEnd = (Document doc) =>
        {
            doc.VirtualLeft = doc.Text[doc.VirtualTop].Length;
        };
        public static Action<Document> MoveToNextWord = (Document doc) =>
        {
            var line = doc.Text[doc.VirtualTop];
            var word = line.Substring(doc.VirtualLeft);
            var nextWord = word.Split(' ').FirstOrDefault();
            if (nextWord != null)
            {
                doc.VirtualLeft += nextWord.Length;
            }
        };
        public static Action<Document> MoveToPreviousWord = (Document doc) =>
        {
            var line = doc.Text[doc.VirtualTop];
            var word = line.Substring(0, doc.VirtualLeft);
            var previousWord = word.Split(' ').LastOrDefault();
            if (previousWord != null)
            {
                doc.VirtualLeft -= previousWord.Length;
            }
        };
        
        // select console text
        public static Action<Document> SelectAll = (Document doc) =>
        {
            
        };
        
        
        //public static Action<
        
        
    }

    public class EditorV2
    {
        public EditorV2()
        {
            // var r = Console.ReadKey();
            Console.Clear();
        }

        private Document document = new Document();

        public void Run()
        {
            var r = Console.ReadKey();
            while (true)
            {
                var keyMap = (r.Key, r.Modifiers);
                if (KeyboardMapping.ContainsKey(keyMap))
                {
                    KeyboardMapping[keyMap].Invoke(this.document);
                }
                else
                {
                    InsertCharacter(r.KeyChar);
                }

                RenderText();
                Console.SetCursorPosition(document.VirtualLeft, document.VirtualTop);
                r = Console.ReadKey();
            }
        }

        public void Apply(Action<Document> action)
        {
            action.Invoke(document);
        }

        public void InsertCharacter(char character)
        {
            var line = document.Text[document.VirtualTop];
            document.Text[document.VirtualTop] = line.Insert(Math.Min(document.VirtualLeft, line.Length), character.ToString());
            Apply(EditorV2Commands.MoveRight);
        }
        public void LoadFile(IEnumerable<string> readLines)
        {
            this.document.Text = readLines.ToList();
            // if (Text.Any())
            // {
            //     TextHistory.Add(this.Text);
            // } 
            // Text = readLines.ToImmutableList();
            this.RenderText();
            Console.SetCursorPosition(0, 0);
        }

        public void RenderText()
        {
            for (var i = 0; i < this.document.Text.Count && i < Console.WindowHeight - 1; i++)
            {
                Console.SetCursorPosition(0, i);
                Console.WriteLine(this.document.Text[i]);
            }

        }

        public Dictionary<(ConsoleKey, ConsoleModifiers), Action<Document>> KeyboardMapping = new Dictionary<(ConsoleKey, ConsoleModifiers), Action<Document>>()
        {
            { (ConsoleKey.DownArrow, 0x0 ), EditorV2Commands.MoveDown },
            { (ConsoleKey.RightArrow, 0x0 ), EditorV2Commands.MoveRight },
            { (ConsoleKey.UpArrow, 0x0 ), EditorV2Commands.MoveUp },
            { (ConsoleKey.LeftArrow, 0x0 ), EditorV2Commands.MoveLeft },
            { (ConsoleKey.LeftArrow, ConsoleModifiers.Control), EditorV2Commands.MoveToPreviousWord},
            { (ConsoleKey.RightArrow, ConsoleModifiers.Control), EditorV2Commands.MoveToNextWord},
            { (ConsoleKey.Home, 0x0), EditorV2Commands.MoveToLineStart},
            { (ConsoleKey.End, 0x0), EditorV2Commands.MoveToLineEnd},
            { (ConsoleKey.Home, ConsoleModifiers.Control), EditorV2Commands.MoveToStart},
            { (ConsoleKey.End, ConsoleModifiers.Control), EditorV2Commands.MoveToEnd},
            // { (ConsoleKey.Delete, 0x0), EditorV2Commands.Delete},
            // { (ConsoleKey.Backspace, 0x0), EditorV2Commands.Backspace},
            // { (ConsoleKey.Enter, 0x0), EditorV2Commands.NewLine},
            // { (ConsoleKey.Tab, 0x0), EditorV2Commands.Tab},
            // { (ConsoleKey.Tab, ConsoleModifiers.Shift), EditorV2Commands.Untab},
            { (ConsoleKey.A, ConsoleModifiers.Control), EditorV2Commands.SelectAll},
            // { (ConsoleKey.C, ConsoleModifiers.Control), EditorV2Commands.Copy},
            // { (ConsoleKey.V, ConsoleModifiers.Control), EditorV2Commands.Paste},
            // { (ConsoleKey.X, ConsoleModifiers.Control), EditorV2Commands.Cut},
            // { (ConsoleKey.Z, ConsoleModifiers.Control), EditorV2Commands.Undo},
            // { (ConsoleKey.Y, ConsoleModifiers.Control), EditorV2Commands.Redo},
            
            
        };
    }
    public class Document
    {

        public List<string> Text { get; set; } = new List<string>();
        public int VirtualLeft { get; set; }
        public int VirtualTop { get; set; }




        public Document()
        {
            // Actions = new List<Func<Editor, bool>>();
            // Actions.AddRange(BaseLibrary.InputCharacters);
            // Actions.AddRange(BaseLibrary.ArrowActions);
        }

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