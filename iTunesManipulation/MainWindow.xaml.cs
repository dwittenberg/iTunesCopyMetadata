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
        private Dictionary<int, SongStruct>  _sourceSonglist;

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
            _sourceSonglist = new Dictionary<int, SongStruct> 
            {
                //new SongStruct { Name = "hh" },
                //new SongStruct { Name = "ha" },
                //new SongStruct { Name = "hhs" },
                //new SongStruct { Name = "hhd" },
                //new SongStruct { Name = "hhg"},
            };
            dgVisible.DataContext = _sourceSonglist;
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
            dgVisible.DataContext = _sourceSonglist = XmlHandler.LoadItunesXML(tbXmlPath.Text);
            EnableButtons(true);
        }

        private async void btnReadItunesRating_Click(object sender, RoutedEventArgs e)
        {
            EnableButtons(false);
            dgVisible.DataContext = _sourceSonglist = await Helper.ItunesRatingSet(_sourceSonglist, _myiTunes, cancelSource.Token, progress);
            EnableButtons(true);
        }

        private async void btnFileRating_Click(object sender, RoutedEventArgs e)
        {
            EnableButtons(false);
            dgVisible.DataContext = _sourceSonglist = await Helper.GetFileRating(_sourceSonglist, cancelSource.Token, progress);
            EnableButtons(true);
        }

        private async void btnCompareiTunes2File_Click(object sender, RoutedEventArgs e)
        {
            EnableButtons(false);
            dgVisible.DataContext = _sourceSonglist = await Helper.FilterDifferenzes(_sourceSonglist, cancelSource.Token, progress);
            EnableButtons(true);
        }

        private async void btnToFile_Click(object sender, RoutedEventArgs e)
        {
            EnableButtons(false);
            dgVisible.DataContext = _sourceSonglist = await Helper.SetFileRating(_sourceSonglist, cancelSource.Token, progress);
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

        private void btnEasyMode_Click(object sender, RoutedEventArgs e)
        {
            SimpleView sv = new SimpleView();
            sv.Show();
            this.Close();
        }
    }
}
