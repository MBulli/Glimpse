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
    /// Interaction logic for WindowsPreviewHostView.xaml
    /// </summary>
    public partial class WindowsPreviewHostView : UserControl
    {
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(string), typeof(WindowsPreviewHostView),
                                        new PropertyMetadata(propertyChangedCallback: SourcePropertyChanged));

        public string Source
        {
            get { return (string)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        private Interop.WindowsPreviewControl windowsPreview;        

        public WindowsPreviewHostView()
        {
            InitializeComponent();

            windowsPreview = new Interop.WindowsPreviewControl();
            this.formsHost.Child = windowsPreview;
        }

        private static void SourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var winPreviewHost = (WindowsPreviewHostView)d;

            string source = e.NewValue as string;

            System.Windows.Threading.Dispatcher.CurrentDispatcher.InvokeAsync(() =>
            {
                winPreviewHost.windowsPreview.Open(source);
            });          
        }
    }
}
