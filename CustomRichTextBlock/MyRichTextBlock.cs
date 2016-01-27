using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.RegularExpressions;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace CustomRichTextBlock
{
    public sealed class MyRichTextBlock : Control
    {
        public MyRichTextBlock()
        {
            this.DefaultStyleKey = typeof(MyRichTextBlock);
            foreach (var key in emojiDict.Keys)
            {
                builder.Append(key.Replace("[", @"\[").Replace("]", @"\]"));//将字典中的[ 和 ] 符号转换成\]和\[,因为在正则表达式中[
                                                                            // 和 ] 符号有特殊含义。
                builder.Append("|");
            }
            builder.Remove(builder.Length - 1, 1); //去掉最后一个“|”符号，否则匹配时会多出一个""字符。
        }

        RichTextBlock _richTextBlock;
        StringBuilder builder = new StringBuilder();
        Regex urlRx = new Regex(@"(?<url>(http:[/][/]|www.)([a-z]|[A-Z]|[0-9]|[/.]|[~])*)", RegexOptions.IgnoreCase);

        private readonly Dictionary<string, string> emojiDict = new Dictionary<string, string>
    {
        {"[哈哈]", "[哈哈]"},
        {"[嘻嘻]", "[嘻嘻]"},
        {"[微笑]", "[微笑]"},
        {"[爱你]", "[爱你]"}
    };

        protected override void OnApplyTemplate()
        {
            _richTextBlock = GetTemplateChild("ChildRichTextBlock") as RichTextBlock;
            SetRichTextBlock(Text);
        }


        public String Text
        {
            get { return (String)GetValue(TextProperty); }
            set
            {
                SetValue(TextProperty, value);}
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(String), typeof(MyRichTextBlock), new PropertyMetadata(""));

        private void SetRichTextBlock(string value)
        {
            MatchCollection matches = urlRx.Matches(value);
            var r = new Regex(builder.ToString()); 
            var mc = r.Matches(value); 
            foreach (Match m in mc) 
            {
                value = value.Replace(m.Value, string.Format(@"<InlineUIContainer><Border><Image Source=""ms-appx:///Assets/Emoji/{0}.png"" Margin=""2,0,2,0"" Width=""30"" Height=""30""/></Border></InlineUIContainer>", emojiDict[m.Value]));
            }
            foreach (Match match in matches)
            {
                string url = match.Groups["url"].Value;
                //string newurl = "";
                //foreach(string i in url)
                //{
                //    newurl.ad
                //}
                value = value.Replace(url, 
                    string.Format("<InlineUIContainer><Border><HyperlinkButton Margin=\"0,0,0,-4\" Padding=\"0,2,0,0\" NavigateUri =\"{0}\"><StackPanel HorizontalAlignment=\"Center\" Height=\"25\" Width=\"90\" Background=\"#FFB8E9FF\" Orientation = \"Horizontal\"><Image Margin=\"5,0,0,0\" Source = \"/Assets/Emoji/[哈哈].png\" Width = \"15\" Height = \"15\"/><TextBlock Margin=\"4,2.5,0,0\" Text=\"网页链接\" Foreground=\"White\" FontFamily=\"Microsoft YaHei UI\" FontSize=\"14\" FontWeight=\"Bold\"/></StackPanel ></HyperlinkButton></Border></InlineUIContainer>", url));
            }
            value = value.Replace("\r\n", "<LineBreak/>"); //将换行符转换成<LineBreak/>,用于实现换行。
            

            var xaml = string.Format(@"<Paragraph 
                                        xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
                                        xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
                                    <Paragraph.Inlines>
                                    <Run></Run>
                                      {0}
                                    </Paragraph.Inlines>
                                </Paragraph>", value);
            var p = (Paragraph)XamlReader.Load(xaml);
            _richTextBlock.Blocks.Add(p);
        }

        




    }
}
