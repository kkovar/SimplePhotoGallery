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
using System.IO;

namespace ImageExplorer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void DoImageSearch(object sender, RoutedEventArgs e)
        {
            var finder = new FileFinder();
            finder.FindFiles("c:\\", "jpg");
            foreach (var f in finder.FoundFilesDictionary)
            {
                FilesList.AppendText(f.Key);

                /*
                 * create a hyperlink to the directories where pictures are.
                 * rank them by size of pics... a directory with large pics (over 1MB or so)
                 * would be ranked higher than one with smaller images, as this would probably be
                 * non-user generated content. when the user clicks on a directory link, the page will
                 * show the file name and metadata like exif, plus a thumb.
                 */

            }
        }
    }
}
