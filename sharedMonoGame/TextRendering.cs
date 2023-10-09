
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Sprites;
using shared;
using SharpDX.Direct3D9;
using SharpDX.MediaFoundation;
using SharpDX.XAPO.Fx;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using FontStashSharp;
using static System.Net.Mime.MediaTypeNames;
//using SharpDX.Direct2D1;

/// This is a very complex class as it has been extended a bunch of times
/// to handle memory issues and multiple attempts at rendering hindi.
/// It handles the code to render text in different colours by chopping the text into sections 
/// of the same colour and across multiple lines.
namespace SharedMonoGame
{
    public struct TextStyle
    {
        /// <summary>
        /// Create a rich paragraph style instruction to change font style, color, or other properties.
        /// </summary>
        /// <param name="fillColor">Set fill color.</param>
        /// <param name="fontStyle">Set font style.</param>
        public TextStyle(string name, Color? fillColor = null)//, FontStyle? fontStyle = null)
        {
            FillColor = fillColor;
            Name = name;
            //            FontStyle = fontStyle;
        }

        /// <summary>
        /// Will change text fill color.
        /// </summary>
        public Color? FillColor { get; private set; }
        public string Name { get; internal set; }

        /// <summary>
        /// Will change font style.
        /// </summary>
        //      public FontStyle? FontStyle { get; private set; }
    }

    public enum TextAlignment
    {
        left,
        right,
        center,
        justified
    }

    public class TextSection
    {
        TextStyle _textStyle;
        string _text;

        public TextStyle TextStyle { get { return _textStyle; } }
        public string Text { get { return _text; } }

        public TextSection(string text, TextStyle textStyle)
        {
            _textStyle = textStyle;
            _text = text;
        }

        public void Clear()
        {
            _text = null;
        }
    }

    public class TextRow
    {
        public List<TextSection> _sections = new List<TextSection>();

        /// <summary>
        /// This is used to record whether the row alignment changes
        /// Helpful for the advisor code
        /// </summary>
        public float AlignmentOverrideX { get; internal set; } = 0;
        public bool AlignmentOverride { get; internal set; } = false;

        public TextStyle GetFinalTextStyle()
        {
            if (_sections.Count > 0)
            {
                return _sections.Last().TextStyle;
            }
            else
            {
                return new TextStyle( name:"PINK", fillColor: Color.HotPink);
            }
        }

        public void Add(TextSection section)
        {
            _sections.Add(section);
        }

        public Vector2 MaxSize(FontStashSharp.SpriteFontBase font)
        {
            float totalX = 0;
            float maxY = font.MeasureString("ygXT").Y;

            foreach (TextSection section in _sections)
            {
                var text = section.Text;
                try
                {
                    var sectionSize = font.MeasureString(section.Text);

                    totalX += sectionSize.X;
                    if (sectionSize.Y > maxY) maxY = sectionSize.Y;
                }
                catch (Exception e)
                {
                    // this is some additional code to help figure out which character can't be rendered correctly
                    if (e is System.ArgumentException)
                    {
                        for (int i = 0; i < text.Length; ++i)
                        {
                            char c = text[i];
                            try
                            {
                                var measureTest = font.MeasureString(c.ToString());
                            }
                            catch (Exception e2)
                            {
                                DebugOutput.Instance.WriteError("Problem rendering character: " + c + " ID: " + i);
                            }
                        }
                    }
                }
            }

            return new Vector2(totalX, maxY);
        }

        public Vector2 MaxSize(SpriteFont font)
        {
            float totalX = 0;
            float maxY = font.MeasureString("ygXT").Y;

            foreach (TextSection section in _sections)
            {
                var text = section.Text;
                try
                {
                    var sectionSize = font.MeasureString(section.Text);

                    totalX += sectionSize.X;
                    if (sectionSize.Y > maxY) maxY = sectionSize.Y;
                }
                catch(Exception e)
                {
                    // this is some additional code to help figure out which character can't be rendered correctly

                    if ( e is System.ArgumentException)
                    {
                        for(int i = 0; i< text.Length; ++i)
//                        foreach( char c in text)
                        {
                            char c = text[i];
                            try
                            {
                                var measureTest = font.MeasureString(c.ToString());
                            }
                            catch (Exception e2)
                            {
                                DebugOutput.Instance.WriteError("Problem rendering character: " + c  + " ID: " + i);
                            }
                        }
                    }
                }
                    
            }

            return new Vector2(totalX, maxY);
        }

        public string CombineSectionsToString()
        {
            string result = "";
            foreach (var section in _sections)
            {
                result += section.Text;
            }
            return result;
        }

        public void Render(SpriteBatch sprite, SpriteFont font, float startX, float centreY)
        {
            float x = startX;

            foreach (var section in _sections)
            {
                var sectionSize = font.MeasureString(section.Text);
                Vector2 position = new Vector2(x, centreY - (sectionSize.Y / 2));
                sprite.DrawString(font, section.Text, position, section.TextStyle.FillColor.Value);

                x += sectionSize.X;
            }
        }

        public void Render(SpriteBatch sprite, FontStashSharp.SpriteFontBase font, float startX, float centreY)
        {
            float x = startX;

            foreach (var section in _sections)
            {
                var sectionSize = font.MeasureString(section.Text);
                Vector2 position = new Vector2(x, centreY - (sectionSize.Y / 2));
                sprite.DrawString(font, section.Text, position, section.TextStyle.FillColor.Value);

                //font.DrawText(sprite, section.Text, position, section.TextStyle.FillColor.Value);

                x += sectionSize.X;
            }
        }

        public int TotalLength()
        {
            return CombineSectionsToString().Length;
        }

        string Spaces(int count)
        {
            string spaces = "";

            for (int i = 0; i < count; ++i)
            {
                spaces += " ";

            }
            return spaces;
        }

        public List<TextRow> SplitToMultiple(int maxLineLength)
        {
            // a row is made of multiple sections...

            // this is fiddly
            // sections need to be added together
            // we might need to split sections apart
            // I think split into individual word sections
            // then rejoin them

            List<TextRow> returnRows = new List<TextRow>();
            List<TextSection> sectionWords = new List<TextSection>();

            // split into words
            foreach (var section in _sections)
            {
                var text = section.Text;

//                var words = section.Text.Split(' ');

                int beginning = text.Length - text.TrimStart().Length;
                int end = text.Length - text.TrimEnd().Length;
                text = text.Trim();

                // count first empty
                // count end empty
                // Trimming then add

                var words = text.Split(' ');

                if (beginning > 0)
                {
                    // create a string x long
                    words[0] = Spaces(beginning) + words[0];
                }

                if (end > 0)
                {
                    int index = words.Length - 1;
                    words[index] = words[index] + Spaces(end);
                }

                foreach (var word in words)
                {
                    sectionWords.Add(new TextSection(word, section.TextStyle));
                }
            }

            Color EMPTY_COLOR = new Color(0, 0, 0, 0);

            var previousFillColour = EMPTY_COLOR;
            var previousFillName = "BLACK";

            TextRow currentRow = new TextRow();

            string totalLine = null;
            string thisSection = null;

            // join them together
            foreach (var section in sectionWords)
            {

                //                string join = "";
                string temp;
                if (string.IsNullOrEmpty(totalLine))
                {
                    temp = "" + section.Text;
                }
                else
                {
                    temp = totalLine + " " + section.Text;
                }

                // WORKING HERE!!!
                // or if the sections types don't match...
                // need a total line length to decide on new lines
                // but need different section when the style changes...

                var currentFillColour = section.TextStyle.FillColor.Value;
                var currentFillName = section.TextStyle.Name;


                if (temp.Length <= maxLineLength)
                {
                    if (previousFillColour != EMPTY_COLOR && previousFillColour != currentFillColour)
                    {
                        // this is a section change
                        TextSection textSection = new TextSection(thisSection, new TextStyle(previousFillName,previousFillColour));
                        currentRow.Add(textSection);
                        thisSection = "" + section.Text;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(thisSection))
                        {
                            thisSection = "" + section.Text;
                        }
                        else
                        {
                            thisSection += " " + section.Text;
                        }
                    }

                    totalLine = temp;
                }
                else
                {
                    TextSection textSection = new TextSection(thisSection, new TextStyle(previousFillName, previousFillColour));
                    currentRow.Add(textSection);
                    returnRows.Add(currentRow);
                    currentRow = new TextRow();

                    thisSection = "" + section.Text;
                    totalLine = "" + section.Text;

                    //TextRow row = new TextRow(join, section.TextStyle);
                }

                previousFillColour = currentFillColour;
                previousFillName = currentFillName;
            }

            if (!string.IsNullOrEmpty(thisSection))
            {
                TextSection textSection = new TextSection(thisSection, new TextStyle(previousFillName, previousFillColour));
                currentRow.Add(textSection);
                returnRows.Add(currentRow);
                currentRow = null;
            }

            return returnRows;
        }

        public string ConvertToText()
        {
            string originalText = "";

            foreach(var section in _sections)
            {
                originalText += "{{" + section.TextStyle.Name + "}}"+section.Text;
            }

            return originalText;
        }

        public void Clear()
        {
            foreach( var section in _sections)
            {
                section.Clear();
            }

            _sections.Clear();
            _sections = null;
        }

        public string ConvertToTextWithoutColours()
        {
            string originalText = "";

            foreach (var section in _sections)
            {
                originalText += section.Text;
            }

            return originalText;
        }
    }

    /// <summary>
    /// Paragraph - made up of rows. 
    /// Each row has a bunch of sections.
    /// Each section contains text and colour
    /// </summary>
    public class TextParagraph
    {
        TextAlignment _textAlignment = TextAlignment.center;
        List<TextRow> _rows = new List<TextRow>();

        public List<TextRow> Rows { get { return _rows; } }

        public float RowCount { get { return _rows.Count; } }

        public TextAlignment TextAlignment { set { _textAlignment = value; } }

        public void Add(TextRow textRow)
        {
            _rows.Add(textRow);
        }

        public Vector2 MaxSize(SpriteFont font)
        {
            float maxX = 0;
            float totalY = 0;

            foreach (var row in _rows)
            {
                Vector2 rowSize = row.MaxSize(font);
                if (row.AlignmentOverride) rowSize.X += row.AlignmentOverrideX;

                if (rowSize.X > maxX) maxX = rowSize.X;
                totalY += rowSize.Y;
            }

            return new Vector2(maxX, totalY);
        }

        public Vector2 MaxSize(FontStashSharp.SpriteFontBase font)
        {
            float maxX = 0;
            float totalY = 0;

            foreach (var row in _rows)
            {
                Vector2 rowSize = row.MaxSize(font);
                if (row.AlignmentOverride) rowSize.X += row.AlignmentOverrideX;

                if (rowSize.X > maxX) maxX = rowSize.X;
                totalY += rowSize.Y;
            }

            return new Vector2(maxX, totalY);
        }

        public string GetCombinedRow(int rowID)
        {
            var row = _rows[rowID];
            return row.CombineSectionsToString();
        }

        public void Render(SpriteBatch sprite, FontStashSharp.SpriteFontBase font, Vector2 maxLineExtents, float offsetX = 0, float offsetY = 0)
        {
            float lineHeight = maxLineExtents.Y;
            var centreY = lineHeight / 2; // this is the central point at which it is rendered

            foreach (var row in _rows)
            {
                float startX = 0;

                if (row.AlignmentOverride)
                {
                    startX = row.AlignmentOverrideX;
                }
                else
                {
                    switch (_textAlignment)
                    {
                        case TextAlignment.left:
                            startX = 0;
                            break;
                        case TextAlignment.right:
                            startX = maxLineExtents.X - row.MaxSize(font).X;
                            break;
                        case TextAlignment.center:
                            startX = (maxLineExtents.X - row.MaxSize(font).X) / 2;
                            break;
                        case TextAlignment.justified:
                            break;
                        default:
                            break;
                    }

                    // Store the X alignment
                    // this is a special case to help with the advisor text rendering
                    row.AlignmentOverrideX = startX;
                }

                row.Render(sprite, font, startX + offsetX, centreY + offsetY);
                centreY += lineHeight;
            }
        }

        public void Render(SpriteBatch sprite, SpriteFont font, Vector2 maxLineExtents, float offsetX=0, float offsetY=0)
        {
            float lineHeight = maxLineExtents.Y;
            var centreY = lineHeight / 2; // this is the central point at which it is rendered

            foreach (var row in _rows)
            {
                float startX = 0;

                if (row.AlignmentOverride)
                {
                    startX = row.AlignmentOverrideX;
                }
                else
                { 
                    switch (_textAlignment)
                    {
                        case TextAlignment.left:
                            startX = 0;
                            break;
                        case TextAlignment.right:
                            startX = maxLineExtents.X - row.MaxSize(font).X;
                            break;
                        case TextAlignment.center:
                            startX = (maxLineExtents.X - row.MaxSize(font).X) / 2;
                            break;
                        case TextAlignment.justified:
                            break;
                        default:
                            break;
                    }

                    // Store the X alignment
                    // this is a special case to help with the advisor text rendering
                    row.AlignmentOverrideX = startX;
                }

                row.Render(sprite, font, startX + offsetX, centreY + offsetY);
                centreY += lineHeight;
            }
        }

        public void CalculateLineIndents(SpriteFontBase font, Vector2 maxLineExtents)
        {
            float lineHeight = maxLineExtents.Y;
            var centreY = lineHeight / 2; // this is the central point at which it is rendered

            foreach (var row in _rows)
            {
                float startX = 0;

                if (row.AlignmentOverride)
                {
                    startX = row.AlignmentOverrideX;
                }
                else
                {
                    switch (_textAlignment)
                    {
                        case TextAlignment.left:
                            startX = 0;
                            break;
                        case TextAlignment.right:
                            startX = maxLineExtents.X - row.MaxSize(font).X;
                            break;
                        case TextAlignment.center:
                            startX = (maxLineExtents.X - row.MaxSize(font).X) / 2;
                            break;
                        case TextAlignment.justified:
                            break;
                        default:
                            break;
                    }

                    // Store the X alignment
                    // this is a special case to help with the advisor text rendering
                    row.AlignmentOverrideX = startX;
                }

                centreY += lineHeight;
            }
        }

        public void CalculateLineIndents(SpriteFont font, Vector2 maxLineExtents)
        {
            float lineHeight = maxLineExtents.Y;
            var centreY = lineHeight / 2; // this is the central point at which it is rendered

            foreach (var row in _rows)
            {
                float startX = 0;

                if (row.AlignmentOverride)
                {
                    startX = row.AlignmentOverrideX;
                }
                else
                {
                    switch (_textAlignment)
                    {
                        case TextAlignment.left:
                            startX = 0;
                            break;
                        case TextAlignment.right:
                            startX = maxLineExtents.X - row.MaxSize(font).X;
                            break;
                        case TextAlignment.center:
                            startX = (maxLineExtents.X - row.MaxSize(font).X) / 2;
                            break;
                        case TextAlignment.justified:
                            break;
                        default:
                            break;
                    }

                    // Store the X alignment
                    // this is a special case to help with the advisor text rendering
                    row.AlignmentOverrideX = startX;
                }

                centreY += lineHeight;
            }
        }

        /// <summary>
        /// This takes the individual rows and decides if they need to be split
        /// It won't ever join together rows
        /// </summary>
        /// <param name="maxLineLength"></param>
        public void SplitTooLongLines(int maxLineLength)
        {
            // hmmm - we can't use a foreach if we're changing the lines...
            // probably need to add to a new paragraph and then copy it back?

            List<TextRow> outputRows = new List<TextRow>();

            foreach (var row in _rows)
            {
                if (row.TotalLength() > maxLineLength)
                {
                    List<TextRow> newRows = row.SplitToMultiple(maxLineLength);
                    outputRows.AddRange(newRows);
                }
                else
                {
                    // just copy it across
                    outputRows.Add(row);
                }
            }

            _rows = outputRows;
        }

        public string ConvertBackToTextWithCRs()
        {
            string textWithCRs = "";

            foreach (var row in _rows)
            {
                textWithCRs += row.ConvertToText() + "\n";
            }

            return textWithCRs;
        }

        public void Clear()
        {
            foreach(var row in _rows)
            {
                row.Clear();
            }

            _rows.Clear();
        }

        public string ConvertBackToTextWithoutColours()
        {
            string text = "";

            foreach (var row in _rows)
            {
                text += row.ConvertToTextWithoutColours()+ " ";
            }

            return text;
        }
    }

    public sealed class TextRendering
    {
        private static readonly TextRendering _instance = new TextRendering();

        public static TextRendering Instance
        {
            get { return _instance; }
        }

        public TextRendering()
        {
        }

        MyGameBase _game;

        Dictionary<string, TextStyle> _instructions = new Dictionary<string, TextStyle>();
        // regex to find color instructions
        System.Text.RegularExpressions.Regex _styleInstructionsRegex;
        string _styleInstructionsOpening = "{{";
        string _styleInstructionsClosing = "}}";

        public void Initialise(MyGameBase game)
        {
            _game = game;
            _instructions = new Dictionary<string, TextStyle>();
            _styleInstructionsRegex = new System.Text.RegularExpressions.Regex(_styleInstructionsOpening + "[^{}]*" + _styleInstructionsClosing);

            //AddInstruction("DEFAULT", new RichParagraphStyleInstruction(resetStyles: true));

            // add color-changing instructions
            AddInstruction("RED", new TextStyle(name:"RED", fillColor: Color.Red));
            AddInstruction("BLUE", new TextStyle(name: "BLUE", fillColor: Color.Blue));
            AddInstruction("GREEN", new TextStyle(name: "GREEN", fillColor: Color.Green));
            AddInstruction("YELLOW", new TextStyle(name: "YELLOW", fillColor: Color.Yellow));
            AddInstruction("BROWN", new TextStyle(name: "BROWN", fillColor: Color.Brown));
            AddInstruction("BLACK", new TextStyle(name: "BLACK", fillColor: Color.Black));
            AddInstruction("WHITE", new TextStyle(name: "WHITE", fillColor: Color.White));
            AddInstruction("CYAN", new TextStyle(name: "CYAN", fillColor: Color.Cyan));
            AddInstruction("PINK", new TextStyle(name: "PINK", fillColor: Color.Pink));
            AddInstruction("HOT_PINK", new TextStyle(name: "HOT_PINK", fillColor: Color.HotPink));
            AddInstruction("GRAY", new TextStyle(name: "GRAY", fillColor: Color.Gray));
            AddInstruction("MAGENTA", new TextStyle(name: "MAGENTA", fillColor: Color.Magenta));
            AddInstruction("ORANGE", new TextStyle(name: "ORANGE", fillColor: Color.Orange));
            AddInstruction("PURPLE", new TextStyle(name: "PURPLE", fillColor: Color.Purple));
            AddInstruction("SILVER", new TextStyle(name: "SILVER", fillColor: Color.Silver));
            AddInstruction("GOLD", new TextStyle(name: "GOLD", fillColor: Color.Gold));
            AddInstruction("TEAL", new TextStyle(name: "TEAL", fillColor: Color.Teal));
            AddInstruction("NAVY", new TextStyle(name: "NAVY", fillColor: Color.Navy));
            AddInstruction("L_RED", new TextStyle(name: "L_RED", fillColor: new Color(1f, 0.35f, 0.25f)));
            AddInstruction("L_BLUE", new TextStyle(name: "L_BLUE", fillColor: new Color(0.25f, 0.35f, 1f)));
            AddInstruction("L_GREEN", new TextStyle(name: "L_GREEN", fillColor: Color.LawnGreen));
            AddInstruction("L_YELLOW", new TextStyle(name: "L_YELLOW", fillColor: Color.LightYellow));
            AddInstruction("L_BROWN", new TextStyle(name: "L_BROWN", fillColor: Color.RosyBrown));
            AddInstruction("L_CYAN", new TextStyle(name: "L_CYAN", fillColor: Color.LightCyan));
            AddInstruction("L_PINK", new TextStyle(name: "L_PINK", fillColor: Color.LightPink));
            AddInstruction("L_GRAY", new TextStyle(name: "L_GRAY", fillColor: Color.LightGray));
            AddInstruction("L_GOLD", new TextStyle(name: "L_GOLD", fillColor: Color.LightGoldenrodYellow));
            AddInstruction("D_RED", new TextStyle(name: "D_RED", fillColor: Color.DarkRed));
            AddInstruction("D_BLUE", new TextStyle(name: "D_BLUE", fillColor: Color.DarkBlue));
            AddInstruction("D_GREEN", new TextStyle(name: "D_GREEN", fillColor: Color.ForestGreen));
            AddInstruction("D_YELLOW", new TextStyle(name: "D_YELLOW", fillColor: Color.GreenYellow));
            AddInstruction("D_BROWN", new TextStyle(name: "D_BROWN", fillColor: Color.SaddleBrown));
            AddInstruction("D_CYAN", new TextStyle(name: "D_CYAN", fillColor: Color.DarkCyan));
            AddInstruction("D_PINK", new TextStyle(name: "D_PINK", fillColor: Color.DeepPink));
            AddInstruction("D_GRAY", new TextStyle(name: "D_GRAY", fillColor: Color.DarkGray));
            AddInstruction("D_GOLD", new TextStyle(name: "D_GOLD", fillColor: Color.DarkGoldenrod));
        }

        private void AddInstruction(string name, TextStyle richParagraphStyleInstruction)
        {
            _instructions.Add(name, richParagraphStyleInstruction);
        }

        public TextParagraph ConvertTextToParagraph(string inputText, TextAlignment textAlignment, int maxLineLength = 60)
        {
            //if (inputText.Contains("elcome"))
            //{
            //    bool bug = true;
            //}

            TextParagraph paragraph = new TextParagraph();
            paragraph.TextAlignment = textAlignment;

            inputText = FixNewLinesAcrossNetwork(inputText);

            var lines = SplitTextToMultiLinesSimple(inputText);

            TextStyle currentStyle = new TextStyle(name: "WHITE", fillColor: Color.White);

            foreach (var line in lines)
            {
                var textRow = CreateRowFromLine(line, currentStyle);
                paragraph.Add(textRow);
                currentStyle = textRow.GetFinalTextStyle();
            }

            paragraph.SplitTooLongLines(maxLineLength);

            return paragraph;
        }

        /// <summary>
        /// When some strings are sent across the network (or possibly from the google sheets)
        /// The \n \r is broken.
        /// 
        /// </summary>
        /// <param name="inputText"></param>
        /// <exception cref="NotImplementedException"></exception>
        private string FixNewLinesAcrossNetwork(string inputText)
        {
            string newlineBroken = "\\" + "n";
            string carriageReturnBroken = "\\" + "r";

            string newline = "\n";
            string carriageReturn = "\r";

            return inputText.Replace(newlineBroken, newline).Replace(carriageReturnBroken, carriageReturn);
        }

        public TextRow CreateRowFromLine(string text, TextStyle startingStyle)
        {
            TextRow textRow = new TextRow();
            TextStyle currentStyle = startingStyle;

            var openingDenote = _styleInstructionsOpening;
            var closingDenote = _styleInstructionsClosing;
            var denotesLength = (openingDenote.Length + closingDenote.Length);

            // find and parse color instructions
            if (text.Contains(openingDenote))
            {
                int iLastPosition = 0;

                System.Text.RegularExpressions.MatchCollection oMatches = _styleInstructionsRegex.Matches(text);
                foreach (System.Text.RegularExpressions.Match oMatch in oMatches)
                {
                    var sectionText = text.Substring(iLastPosition, oMatch.Index - iLastPosition);

                    if (!string.IsNullOrEmpty(sectionText))
                    {
                        TextSection section = new TextSection(sectionText, currentStyle);
                        textRow.Add(section);
                    }

                    string key = oMatch.Value.Substring(openingDenote.Length, oMatch.Value.Length - denotesLength).ToUpper();

                    if (_instructions.ContainsKey(key))
                    {
                        currentStyle = _instructions[key];
                    }
                    else
                    {
                        currentStyle = _instructions.First().Value;
                    }
                    
                    
                    iLastPosition = oMatch.Index + oMatch.Value.Length;
                }

                var remainingText = text.Substring(iLastPosition);
                if (!string.IsNullOrWhiteSpace(remainingText))
                {
                    TextSection section = new TextSection(remainingText, currentStyle);
                    textRow.Add(section);
                }
                // remove all the style instructions from text so it won't be shown
                //text = _styleInstructionsRegex.Replace(text, string.Empty);
            }
            else
            {
                // if there's no commands - then do still copy the text across
                TextSection section = new TextSection(text, currentStyle);
                textRow.Add(section);
            }

            return textRow;
        }

        public List<string> SplitTextToMultiLinesSimple(string text)
        {
            text = text.Replace('\n', '|').Trim();
            var lines = text.Split('|');
            List<string> listLines = new List<string>(lines);
            return listLines;
        }

        /// <summary>
        /// This is only used by Draw Down Card generation
        /// </summary>
        /// <param name="textParagraph"></param>
        /// <param name="font"></param>
        /// <returns></returns>
        public Texture2D RenderParagraphToTexture(TextParagraph textParagraph, SpriteFont font)
        {
            if (textParagraph.RowCount == 1)
            {
                string combined = textParagraph.GetCombinedRow(0);

                if (string.IsNullOrWhiteSpace(combined))
                {
                    TextRow textRow = new TextRow();
                    textRow.Add(new TextSection(" ", new TextStyle(name: "WHITE", fillColor: Color.White)));
                    textParagraph.Add(textRow);
                }
            }

            Vector2 maxExtents = textParagraph.MaxSize(font);
            Vector2 position = Vector2.Zero;

            Vector2 maxLineExtents = new Vector2(maxExtents.X, maxExtents.Y / textParagraph.RowCount);

            if (maxExtents.X <= 0 || maxExtents.Y <= 0) return null;

            Vector2 renderTargetSize = new Vector2(maxExtents.X, maxExtents.Y);

            try
            {
                PresentationParameters pp = _game.GraphicsDevice.PresentationParameters;
                RenderTarget2D renderTarget
                    = new RenderTarget2D(_game.GraphicsDevice, (int)renderTargetSize.X, (int)renderTargetSize.Y, false, _game.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24);

                Color clearColour = new Color(0, 0, 0, 0);

                var targets = _game.GraphicsDevice.GetRenderTargets();

                RenderTarget2D previousRenderTarget = null;
                if (targets.Length > 0)
                {
                    var renderTargetBinding = targets[0];
                    previousRenderTarget = renderTargetBinding.RenderTarget as RenderTarget2D;
                }

                _game.GraphicsDevice.SetRenderTarget(renderTarget);
                _game.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, clearColour, 1.0f, 0);

                using (SpriteBatch sprite = new SpriteBatch(_game.GraphicsDevice))
                {
                    sprite.Begin();
                    textParagraph.Render(sprite, font, maxLineExtents);
                    sprite.End();
                }

                if (targets.Length > 0)
                {
                    _game.GraphicsDevice.SetRenderTarget(previousRenderTarget);
                }

                Texture2D returnTexture = (Texture2D)renderTarget;
                return returnTexture;
            }
            catch(Exception e)
            {
                DebugOutput.Instance.WriteError("Text Rendering 1: Exception in the Monogame. Just catch it for now and move on\n" + e);
                
            }

            return null;
        }

        
        public Vector2 RenderParagraphToExistingRenderTarget(RenderTarget2D renderTarget, TextParagraph textParagraph, SpriteFontBase font)
        {
            if (renderTarget == null) return Vector2.Zero;

            if (textParagraph.RowCount == 1)
            {
                string combined = textParagraph.GetCombinedRow(0);

                if (string.IsNullOrWhiteSpace(combined))
                {
                    TextRow textRow = new TextRow();
                    textRow.Add(new TextSection(" ", new TextStyle(name: "WHITE", fillColor: Color.White)));
                    textParagraph.Add(textRow);
                }
            }

            Vector2 maxExtents = textParagraph.MaxSize(font);
            Vector2 position = Vector2.Zero;

            //Vector2 maxLineExtents = new Vector2(maxExtents.X, maxExtents.Y / textParagraph.RowCount);
            Vector2 maxLineExtents = new Vector2(maxExtents.X, maxExtents.Y / textParagraph.RowCount);


            if (maxExtents.X <= 0 || maxExtents.Y <= 0) return Vector2.Zero;

            Vector2 renderTargetSize = new Vector2(maxExtents.X, maxExtents.Y);

            try
            {
                //PresentationParameters pp = _game.GraphicsDevice.PresentationParameters;
                //RenderTarget2D renderTarget
                //    = new RenderTarget2D(_game.GraphicsDevice, (int)renderTargetSize.X, (int)renderTargetSize.Y, false, _game.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24);

                Vector2 offsetIntoFixedTexture = (new Vector2(renderTarget.Width, renderTarget.Height) - renderTargetSize) / 2;
                Color clearColour = new Color(0, 0, 0, 0);

                var targets = _game.GraphicsDevice.GetRenderTargets();

                RenderTarget2D previousRenderTarget = null;
                if (targets.Length > 0)
                {
                    var renderTargetBinding = targets[0];
                    previousRenderTarget = renderTargetBinding.RenderTarget as RenderTarget2D;
                }

                _game.GraphicsDevice.SetRenderTarget(renderTarget);
                _game.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, clearColour, 1.0f, 0);

                using (SpriteBatch sprite = new SpriteBatch(_game.GraphicsDevice))
                {
                    sprite.Begin();
                    textParagraph.Render(sprite, font, maxLineExtents, offsetIntoFixedTexture.X, offsetIntoFixedTexture.Y);
                    sprite.End();
                }

                if (targets.Length > 0)
                {
                    _game.GraphicsDevice.SetRenderTarget(previousRenderTarget);
                }

                //Texture2D returnTexture = (Texture2D)renderTarget;
                //return returnTexture;
            }
            catch (Exception e)
            {
                DebugOutput.Instance.WriteError("Text Rendering 2: Exception in the Monogame. Just catch it for now and move on\n" + e);
            }

            return renderTargetSize;
        }

        public Vector2 RenderParagraphToExistingRenderTarget(RenderTarget2D renderTarget, TextParagraph textParagraph, SpriteFont font)
        {
            if (renderTarget == null) return Vector2.Zero;

            if (textParagraph.RowCount == 1)
            {
                string combined = textParagraph.GetCombinedRow(0);

                if (string.IsNullOrWhiteSpace(combined))
                {
                    TextRow textRow = new TextRow();
                    textRow.Add(new TextSection(" ", new TextStyle(name: "WHITE", fillColor: Color.White)));
                    textParagraph.Add(textRow);
                }
            }

            Vector2 maxExtents = textParagraph.MaxSize(font);
            Vector2 position = Vector2.Zero;

            //Vector2 maxLineExtents = new Vector2(maxExtents.X, maxExtents.Y / textParagraph.RowCount);
            Vector2 maxLineExtents = new Vector2(maxExtents.X, maxExtents.Y / textParagraph.RowCount);


            if (maxExtents.X <= 0 || maxExtents.Y <= 0) return Vector2.Zero;

            Vector2 renderTargetSize = new Vector2(maxExtents.X, maxExtents.Y);

            try
            {
                //PresentationParameters pp = _game.GraphicsDevice.PresentationParameters;
                //RenderTarget2D renderTarget
                //    = new RenderTarget2D(_game.GraphicsDevice, (int)renderTargetSize.X, (int)renderTargetSize.Y, false, _game.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24);

                Vector2 offsetIntoFixedTexture = (new Vector2(renderTarget.Width, renderTarget.Height) - renderTargetSize) / 2;

                Color clearColour = new Color(0, 0, 0, 0);

                var targets = _game.GraphicsDevice.GetRenderTargets();

                RenderTarget2D previousRenderTarget = null;
                if (targets.Length > 0)
                {
                    var renderTargetBinding = targets[0];
                    previousRenderTarget = renderTargetBinding.RenderTarget as RenderTarget2D;
                }

                _game.GraphicsDevice.SetRenderTarget(renderTarget);
                _game.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, clearColour, 1.0f, 0);

                using (SpriteBatch sprite = new SpriteBatch(_game.GraphicsDevice))
                {
                    sprite.Begin();
                    textParagraph.Render(sprite, font, maxLineExtents, offsetIntoFixedTexture.X, offsetIntoFixedTexture.Y);
                    sprite.End();
                }

                if (targets.Length > 0)
                {
                    _game.GraphicsDevice.SetRenderTarget(previousRenderTarget);
                }

                //Texture2D returnTexture = (Texture2D)renderTarget;
                //return returnTexture;
            }
            catch (Exception e)
            {
                DebugOutput.Instance.WriteError("Text Rendering 2: Exception in the Monogame. Just catch it for now and move on\n" + e);
            }

            return renderTargetSize;
        }

        public Vector2 RenderParagraphExtents(TextParagraph textParagraph, SpriteFontBase font)
        {
            if (textParagraph.RowCount == 1)
            {
                string combined = textParagraph.GetCombinedRow(0);

                if (string.IsNullOrWhiteSpace(combined))
                {
                    TextRow textRow = new TextRow();
                    textRow.Add(new TextSection(" ", new TextStyle(name: "WHITE", fillColor: Color.White)));
                    textParagraph.Add(textRow);
                }
            }

            Vector2 maxExtents = textParagraph.MaxSize(font);
            Vector2 position = Vector2.Zero;

            Vector2 maxLineExtents = new Vector2(maxExtents.X, maxExtents.Y / textParagraph.RowCount);

            if (maxExtents.X <= 0 || maxExtents.Y <= 0) return Vector2.Zero;

            Vector2 renderTargetSize = new Vector2(maxExtents.X, maxExtents.Y);

            textParagraph.CalculateLineIndents(font, maxLineExtents);

            return renderTargetSize;
        }

        public Vector2 RenderParagraphExtents(TextParagraph textParagraph, SpriteFont font)
        {
            if (textParagraph.RowCount == 1)
            {
                string combined = textParagraph.GetCombinedRow(0);

                if (string.IsNullOrWhiteSpace(combined))
                {
                    TextRow textRow = new TextRow();
                    textRow.Add(new TextSection(" ", new TextStyle(name: "WHITE", fillColor: Color.White)));
                    textParagraph.Add(textRow);
                }
            }

            Vector2 maxExtents = textParagraph.MaxSize(font);
            Vector2 position = Vector2.Zero;

            Vector2 maxLineExtents = new Vector2(maxExtents.X, maxExtents.Y / textParagraph.RowCount);

            if (maxExtents.X <= 0 || maxExtents.Y <= 0) return Vector2.Zero;

            Vector2 renderTargetSize = new Vector2(maxExtents.X, maxExtents.Y);

            textParagraph.CalculateLineIndents(font, maxLineExtents);

            return renderTargetSize;
        }

        public Texture2D RenderParagraphToTexture(TextParagraph textParagraph, FontStashSharp.SpriteFontBase font)
        {
            if (textParagraph.RowCount == 1)
            {
                string combined = textParagraph.GetCombinedRow(0);

                if (string.IsNullOrWhiteSpace(combined))
                {
                    TextRow textRow = new TextRow();
                    textRow.Add(new TextSection(" ", new TextStyle(name: "WHITE", fillColor: Color.White)));
                    textParagraph.Add(textRow);
                }
            }

            Vector2 maxExtents = textParagraph.MaxSize(font);
            Vector2 position = Vector2.Zero;

            Vector2 maxLineExtents = new Vector2(maxExtents.X, maxExtents.Y / textParagraph.RowCount);

            if (maxExtents.X <= 0 || maxExtents.Y <= 0) return null;

            Vector2 renderTargetSize = new Vector2(maxExtents.X, maxExtents.Y);

            try
            {
                PresentationParameters pp = _game.GraphicsDevice.PresentationParameters;
                RenderTarget2D renderTarget
                    = new RenderTarget2D(_game.GraphicsDevice, (int)renderTargetSize.X, (int)renderTargetSize.Y, false, _game.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24);

                Color clearColour = new Color(0, 0, 0, 0);

                var targets = _game.GraphicsDevice.GetRenderTargets();

                RenderTarget2D previousRenderTarget = null;
                if (targets.Length > 0)
                {
                    var renderTargetBinding = targets[0];
                    previousRenderTarget = renderTargetBinding.RenderTarget as RenderTarget2D;
                }

                _game.GraphicsDevice.SetRenderTarget(renderTarget);
                _game.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, clearColour, 1.0f, 0);

                using (SpriteBatch sprite = new SpriteBatch(_game.GraphicsDevice))
                {
                    sprite.Begin();
                    textParagraph.Render(sprite, font, maxLineExtents);
                    sprite.End();
                }

                if (targets.Length > 0)
                {
                    _game.GraphicsDevice.SetRenderTarget(previousRenderTarget);
                }

                Texture2D returnTexture = (Texture2D)renderTarget;
                return returnTexture;
            }
            catch (Exception e)
            {
                DebugOutput.Instance.WriteError("Text Rendering 4: Exception in the Monogame. Just catch it for now and move on\n" + e);
            }

            return null;
        }

 




        //        public RenderTarget2D RenderParagraphToRenderTarget(TextParagraph textParagraph, SpriteFont font)
        //        {
        //            if (textParagraph.RowCount == 1)
        //            {
        //                string combined = textParagraph.GetCombinedRow(0);

        //                if (string.IsNullOrWhiteSpace(combined))
        //                {
        //                    TextRow textRow = new TextRow();
        //                    textRow.Add(new TextSection(" ", new TextStyle(name: "WHITE", fillColor: Color.White)));
        //                    textParagraph.Add(textRow);
        //                }
        //            }

        //            Vector2 maxExtents = textParagraph.MaxSize(font);
        //            Vector2 position = Vector2.Zero;

        //            Vector2 maxLineExtents = new Vector2(maxExtents.X, maxExtents.Y / textParagraph.RowCount);

        //            if (maxExtents.X <= 0 || maxExtents.Y <= 0) return null;

        //            Vector2 renderTargetSize = new Vector2(maxExtents.X, maxExtents.Y);

        //            try
        //            {
        //                //PresentationParameters pp = _game.GraphicsDevice.PresentationParameters;
        //                RenderTarget2D renderTarget
        //                    = new RenderTarget2D(_game.GraphicsDevice, (int)renderTargetSize.X, (int)renderTargetSize.Y, false, _game.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24);

        //                Color clearColour = new Color(0, 0, 0, 0);

        //                var targets = _game.GraphicsDevice.GetRenderTargets();

        //                RenderTarget2D previousRenderTarget = null;
        //                if (targets.Length > 0)
        //                {
        //                    var renderTargetBinding = targets[0];
        //                    previousRenderTarget = renderTargetBinding.RenderTarget as RenderTarget2D;
        //                }

        //                _game.GraphicsDevice.SetRenderTarget(renderTarget);
        //                _game.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, clearColour, 1.0f, 0);

        //                using (SpriteBatch sprite = new SpriteBatch(_game.GraphicsDevice))
        //                {
        //                    sprite.Begin();
        //                    textParagraph.Render(sprite, font, maxLineExtents);
        //                    sprite.End();
        //                }

        //                if (targets.Length > 0)
        //                {
        //                    _game.GraphicsDevice.SetRenderTarget(previousRenderTarget);
        //                }

        ////                Texture2D returnTexture = (Texture2D)renderTarget;
        //                return renderTarget;
        //            }
        //            catch (Exception e)
        //            {
        //                DebugOutput.Instance.WriteError("Text Rendering 3: Exception in the Monogame. Just catch it for now and move on\n" + e);
        //            }

        //            return null;
        //        }
    }
}