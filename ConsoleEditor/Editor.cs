using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleEditor
{



    public abstract class Pane
    {
        // positioning and size
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; } 
        public AttachPosition AttachPosition { get; set; }
    }

    public interface IPaneElement
    {
        
    }
    public class ElementPane<T> : Pane where T : IPaneElement  
    {
        public ElementPane(T element)
        {
            Element = element;
        }
        public T Element { get; set; }
    }

    public class ContainerPane : Pane
    {
        public List<Pane> Panes { get; set; } = new List<Pane>();
    }
    public enum AttachPosition
    {
        Top,
        Bottom,
        Left,
        Right
    }

    public class SelectList : IPaneElement
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
            // { (ConsoleKey.Spacebar, ConsoleModifiers.Control), EditorCommands.OpenCommand }

        };

        static Editor()
        {
            Console.Clear();
        }

        public static void Run()
        {
            RenderPanes();
            
            var r = Console.ReadKey();
            while (true)
            {
                var keyMap = (r.Key, r.Modifiers);

                
                LayoutPanes();
                
                if (ActivePane is ElementPane<Document> docPane)
                {
                    if (KeyboardMapping.ContainsKey(keyMap))
                        KeyboardMapping[keyMap].Invoke(docPane.Element);
                    else
                        InsertCharacter(r.KeyChar, docPane.Element);
                    
                    Console.SetCursorPosition(docPane.Element.VirtualLeft + docPane.X, docPane.Element.VirtualTop + docPane.Y);
                }
                else
                {
                    // TODO more types
                }

                RenderPanes();

                r = Console.ReadKey();
                
            }
        }

        public static void Apply(Action<Document> action, Document doc)
        {
            action.Invoke(doc);
        }

        public static void InsertCharacter(char character, Document doc)
        {
            var line = doc.Text[doc.VirtualTop];
            doc.Text[doc.VirtualTop] =
                line.Insert(Math.Min(doc.VirtualLeft, line.Length), character.ToString());
            Apply(EditorCommands.MoveRight, doc);
        }

        public static void LoadFile(IEnumerable<string> readLines)
        {
            
            // document.Text = readLines.ToList();
            ActivePane = new ElementPane<Document>(new Document(readLines));
            Panes.Add(ActivePane);
            // if (Text.Any())
            // {
            //     TextHistory.Add(this.Text);
            // } 
            // Text = readLines.ToImmutableList();
            LayoutPanes();
            // RenderText(((ActivePane as ElementPane<Document>)!).Element);
            // Console.SetCursorPosition(0, 0);
        }

        private static void LayoutPanes()
        {
            var maxWidth = Console.WindowWidth;
            var maxHeight = Console.WindowHeight-2;
            var paneWidth = maxWidth / Panes.Count;
            var paneHeight = maxHeight;
            var paneIndex = 0;
            foreach (var pane in Panes)
            {
                pane.X = paneIndex * paneWidth;
                pane.Y = 0;
                pane.Width = paneWidth;
                pane.Height = paneHeight;
                paneIndex++;
            }
        }

        public static void RenderPanes()
        {
            Console.Clear();
            foreach (var pane in Panes)
            {
                if (pane is ElementPane<Document> docPane)
                {
                    RenderText(docPane);
                }
            }
            
            if (ActivePane is ElementPane<Document> ap)
            {
                Console.SetCursorPosition(ap.X + ap.Element.VirtualLeft, ap.Y + ap.Element.VirtualTop);
            } 
        }
        public static void RenderText(ElementPane<Document> docPane)
        {
            for (var i = 0; i < docPane.Height; i++)
            {
                Console.SetCursorPosition(docPane.X, i);
                if (i < docPane.Element.Text.Count)
                { 
                    Console.WriteLine(docPane.Element.Text[i].PadRight(docPane.Width));
                }
                else 
                {
                    Console.WriteLine("~" + string.Join("", Enumerable.Repeat(" ", docPane.Width - 1)));
                }
            } 
        }
        public static void RenderText(Document doc)
        {
            for (var i = 0; i < Console.WindowHeight - 1; i++)
            {
                Console.SetCursorPosition(0, i);
                if (i < doc.Text.Count)
                {
                    Console.WriteLine(doc.Text[i].PadRight(Console.WindowWidth - 1));
                }
                else
                {
                    Console.WriteLine("~" + string.Join("", Enumerable.Repeat(" ", Console.WindowWidth - 2)));
                }
            }
        }
    }

    public class Document : IPaneElement
    {
        public List<string> Text { get; set; } = new();
        public int VirtualLeft { get; set; }
        public int VirtualTop { get; set; }

        public Document(IEnumerable<string> text)
        {
            Text = text.ToList();
        }
    }

}