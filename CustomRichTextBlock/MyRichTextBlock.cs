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

        #region Field

        RichTextBlock _richTextBlock;
        StringBuilder builder = new StringBuilder();
        Dictionary<string, string> emojiDict = App.emojiDict;
        Regex urlRx = new Regex(@"(?<url>(http:[/][/]t.cn[/]\w{7}))", RegexOptions.IgnoreCase);//Regular expression of hyperlink, this regular expression is specific for Sina Weibo link

        #endregion

        #region Property

        public String Text
        {
            get { return (String)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(String), typeof(MyRichTextBlock), new PropertyMetadata("", OnTextChange));

        #endregion

        #region Override

        protected override void OnApplyTemplate()
        {
            _richTextBlock = GetTemplateChild("ChildRichTextBlock") as RichTextBlock;
        }

        #endregion

        #region Handler
        //Will be triggered every time the text property is changed
        private static void OnTextChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var rtb = (MyRichTextBlock)d;
            rtb.SetRichTextBlock(e.NewValue.ToString());
        }

        #endregion

        #region Events

        private void SetRichTextBlock(string value)
        {
            //Replace all the symbols that will make xaml reader throws exception
            value = value.Replace("<", "&lt;").Replace(">", "&gt;").Replace(" &", "&amp;");
            if (_richTextBlock == null)
                return;

            MatchCollection matches = urlRx.Matches(value);
            var r = new Regex(builder.ToString());
            var mc = r.Matches(value);

            //Convert every single block of plain text that matches the rule of the stringbuilder to a image respectively.
            foreach (Match m in mc)
            {
                value = value.Replace(m.Value, string.Format(@"<InlineUIContainer><Border><StackPanel Margin = ""2,0,2,0"" Width = ""19"" Height = ""19""><Image Source =""/Assets/Emoji/{0}.png""/></StackPanel></Border></InlineUIContainer> ", emojiDict[m.Value]));
            }

            //Convert every single block of plain text that matches the rule of the stringbuilder to a visualize button respectively. You shall change the text of the buttom based on your demand
            foreach (Match match in matches)
            {
                string url = match.Groups["url"].Value;
                value = value.Replace(url,
                    string.Format(@"<InlineUIContainer><Border><HyperlinkButton Margin=""0,0,0,-4"" Padding=""0,2,0,0"" NavigateUri =""{0}""><StackPanel HorizontalAlignment=""Center"" Height=""23"" Width=""87"" Background=""#FFB8E9FF"" Orientation = ""Horizontal"">
                        <Image Margin=""5,0,0,0"" Source = ""ms-appx:///Assets/Link.png"" Width = ""15"" Height = ""15""/><TextBlock Margin=""4,1.5,0,0"" Text=""网页链接"" Foreground=""White"" FontFamily=""Microsoft YaHei UI"" FontSize=""14"" FontWeight=""Bold""/>
                    </StackPanel>
                </HyperlinkButton>
            </Border>
        </InlineUIContainer>", url));
            }

            //Convert the plain text into a block of xaml code
            var xaml = string.Format(@"<Paragraph 
                                        xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
                                        xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
                                    <Paragraph.Inlines>
                                    <Run></Run>
                                      {0}
                                    </Paragraph.Inlines>
                                </Paragraph>", value);
            var p = (Paragraph)XamlReader.Load(xaml);
            _richTextBlock.Blocks.Clear();//clear the richtextblock to avoid duplicated text
            _richTextBlock.Blocks.Add(p);

        }

        #endregion

        public MyRichTextBlock()
        {
            this.DefaultStyleKey = typeof(MyRichTextBlock);

            builder = App._textbuilder;
        }
  
    }
}
