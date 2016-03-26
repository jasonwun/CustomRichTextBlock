using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Markup;

namespace CustomRichTextBlock
{
    public class BindingHelper : DependencyObject
    {
        static StringBuilder builder = App.builder;
        static Dictionary<string, string> emojiDict = App.emojiDict;

        public static string GetText(DependencyObject obj)
        {
            return (string)obj.GetValue(TextProperty);
        }

        public static void SetText(DependencyObject obj, string value)
        {
            obj.SetValue(TextProperty, value);
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.RegisterAttached("Text", typeof(string), typeof(BindingHelper), new PropertyMetadata(String.Empty, OnTextChanged));

        private static void OnTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            
            var control = sender as RichTextBlock;
            if (control != null)
            {
                control.Blocks.Clear();
                string value = e.NewValue.ToString();


                value = value.Replace("<", "&lt;").Replace(">", "&gt;").Replace("&", "&amp;");


                Regex urlRx = new Regex(@"(?<url>(http:[/][/]t.cn[/]\w{7}))", RegexOptions.IgnoreCase);
                MatchCollection matches = urlRx.Matches(value);
                var r = new Regex(builder.ToString());
                var mc = r.Matches(value);


                foreach (Match m in mc)
                {
                    value = value.Replace(m.Value, string.Format(@"<InlineUIContainer><Border><StackPanel Margin = ""2,0,2,0"" Width = ""19"" Height = ""19""><Image Source =""/Assets/Emoji/{0}.png""/></StackPanel></Border></InlineUIContainer> ", emojiDict[m.Value]));
                }
                foreach (Match match in matches)
                {
                    string url = match.Groups["url"].Value;
                    value = value.Replace(url,
                        string.Format(@"<InlineUIContainer><Border><HyperlinkButton Margin=""0,0,0,-4"" Padding=""0,2,0,0"" NavigateUri =""{0}""><StackPanel HorizontalAlignment=""Center"" Height=""23"" Width=""87"" Background=""#FFB8E9FF"" Orientation = ""Horizontal"">
                        <Image Margin=""5,0,0,0"" Source = ""ms-appx:///Assets/Icons/Link.png"" Width = ""15"" Height = ""15""/><TextBlock Margin=""4,1.5,0,0"" Text=""网页链接"" Foreground=""White"" FontFamily=""Microsoft YaHei UI"" FontSize=""14"" FontWeight=""Bold""/>
                    </StackPanel>
                </HyperlinkButton>
            </Border>
        </InlineUIContainer>", url));
                }


                var xaml = string.Format(@"<Paragraph 
                                        xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
                                        xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
                                    <Paragraph.Inlines>
                                    <Run></Run>
                                        {0}
                                    </Paragraph.Inlines>
                                </Paragraph>", value);
                var p = (Paragraph)XamlReader.Load(xaml);
                control.Blocks.Add(p);
            }
        }
    }
}
