using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Glimpse.Views
{
    /// <summary>
    /// Interaction logic for RichTextView.xaml
    /// </summary>
    public partial class RichTextView : UserControl
    {
        public static readonly DependencyProperty RtfTextProperty =
            DependencyProperty.Register("RtfText", typeof(string), typeof(RichTextView),
                                        new PropertyMetadata(propertyChangedCallback: RtfPropertyChanged));

        public string RtfText
        {
            get { return (string)GetValue(RtfTextProperty); }
            set { SetValue(RtfTextProperty, value); }
        }

        private readonly System.Windows.Forms.RichTextBox formsRichTextBox;

        public RichTextView()
        {
            InitializeComponent();

            // We're using the win forms RichTextBox because it has a way better rtf render performance than the wpf one...
            this.formsRichTextBox = new System.Windows.Forms.RichTextBox();
            this.formsRichTextBox.ReadOnly = true;
            this.formsRichTextBox.BackColor = System.Drawing.Color.White;
            this.formsRichTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;

            this.formsHost.Child = formsRichTextBox;
        }

        private static void RtfPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var richTextView = (RichTextView)d;
            richTextView.formsRichTextBox.Rtf = e.NewValue as string;
        }
    }
}
