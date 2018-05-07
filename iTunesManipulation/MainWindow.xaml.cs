using iTunesLib;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace iTunesManipulation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<SongStruct> _sourceSonglist;

        private readonly iTunesAppClass _myiTunes = new iTunesAppClass();
        private CancellationTokenSource cancelSource;
        private Progress<double> progress;

        // ToDo Caching der iTunes und File Daten
        public MainWindow()
        {
            InitializeComponent();

            progress = new Progress<double>();
            progress.ProgressChanged += Progress_ProgressChanged;
            cancelSource = new CancellationTokenSource();
            DataContext = new List<SongStruct>();//_sourceSonglist;
        }

        private void Progress_ProgressChanged(object sender, double e)
        {
            ProgressBar.Value = e;
            tbCounter.Text = e.ToString();
        }

        #region new buttons
        private void btnReadXml_Click(object sender, RoutedEventArgs e)
        {

            EnableButtons(false);
            //await Helper.Test(cancelSource.Token, progress);
            _sourceSonglist =  XmlHandler.LoadItunesXML(tbXmlPath.Text);
            EnableButtons(true);
        }

        private async void btnReadItunesRating_Click(object sender, RoutedEventArgs e)
        {
            EnableButtons(false);
            _sourceSonglist = await Helper.GetItunesRating(_sourceSonglist, _myiTunes, cancelSource.Token, progress);
            EnableButtons(true);
        }

        private async void btnFileRating_Click(object sender, RoutedEventArgs e)
        {
            EnableButtons(false);
            _sourceSonglist = await Helper.GetFileRating(_sourceSonglist, cancelSource.Token, progress);
            EnableButtons(true);
        }

        private async void btnCompareiTunes2File_Click(object sender, RoutedEventArgs e)
        {
            EnableButtons(false);
            _sourceSonglist = await Helper.FilterDifferenzes(_sourceSonglist, cancelSource.Token, progress);
            EnableButtons(true);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            btnCancel.IsEnabled = false;
            cancelSource.Cancel();
            cancelSource = new CancellationTokenSource();
        }
        #endregion

        private void EnableButtons(bool value)
        {
            btnCancel.IsEnabled = !value;
            btnCompareiTunes2File.IsEnabled = value;
            btnFileRating.IsEnabled = value;
            btnReadItunesRating.IsEnabled = value;
            btnReadXml.IsEnabled = value;
        }

        private void btnSelectFile_Click(object sender, RoutedEventArgs e)
        {
            // Displays an OpenFileDialog so the user can select a Cursor.  
            System.Windows.Forms.OpenFileDialog openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            openFileDialog1.Filter = "Cursor Files|*.xml";
            openFileDialog1.Title = "Select a iTunes File";

            // Show the Dialog.  
            // If the user clicked OK in the dialog and  
            // a .CUR file was selected, open it.  
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // Assign the cursor in the Stream to the Form's Cursor property.  
                tbXmlPath.Text = openFileDialog1.FileName;
            }
        }
    }
}
