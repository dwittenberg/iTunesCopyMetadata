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
    public partial class SimpleView : Window
    {
        private Dictionary<int, SongStruct>  _sourceSonglistFull;
        private Dictionary<int, SongStruct>  _sourceSonglistLight;

        private readonly iTunesAppClass _myiTunes = new iTunesAppClass();
        private CancellationTokenSource cancelSource;
        private Progress<double> progress;

        // ToDo Caching der iTunes und File Daten
        public SimpleView()
        {
            InitializeComponent();

            progress = new Progress<double>();
            progress.ProgressChanged += Progress_ProgressChanged;
            cancelSource = new CancellationTokenSource();
            tbXmlPath.Text = _myiTunes.LibraryXMLPath;
        }

        private void Progress_ProgressChanged(object sender, double e)
        {
            ProgressBar.Value = e;
            tbCounter.Text = e.ToString();
        }

        #region new buttons
        private async void btnReadXml_Click(object sender, RoutedEventArgs e)
        {

            EnableButtons(false);
            _sourceSonglistFull = XmlHandler.LoadItunesXML(tbXmlPath.Text);

            if ((bool)CbFastMode.IsChecked)
            {
                if ((bool)RbFile.IsChecked)
                {
                    _sourceSonglistFull = await Helper.GetFileRating(_sourceSonglistFull, cancelSource.Token, progress);
                    _sourceSonglistLight = await Helper.FilterDifferenzes(_sourceSonglistFull, cancelSource.Token, progress);
                    _sourceSonglistLight = await Helper.ItunesRatingGet(_sourceSonglistLight, _myiTunes, cancelSource.Token, progress);
                }

                if ((bool)RbItunes.IsChecked)
                {
                    _sourceSonglistFull = await Helper.ItunesRatingGet(_sourceSonglistFull, _myiTunes, cancelSource.Token, progress);
                    _sourceSonglistLight = await Helper.FilterDifferenzes(_sourceSonglistFull, cancelSource.Token, progress);
                    _sourceSonglistLight = await Helper.GetFileRating(_sourceSonglistLight, cancelSource.Token, progress);
                }

                _sourceSonglistLight = await Helper.FilterDifferenzes(_sourceSonglistLight, cancelSource.Token, progress);
                CbFilter.IsChecked = true;
            }
            else
            {
                _sourceSonglistFull = await Helper.ItunesRatingGet(_sourceSonglistFull, _myiTunes, cancelSource.Token, progress);
                _sourceSonglistFull = await Helper.GetFileRating(_sourceSonglistFull, cancelSource.Token, progress);
                _sourceSonglistLight = await Helper.FilterDifferenzes(_sourceSonglistFull, cancelSource.Token, progress);
            }

            EnableButtons(true);
        }


        private async void btnCompareiTunes2File_Click(object sender, RoutedEventArgs e)
        {
            EnableButtons(false);
            _sourceSonglistFull = _sourceSonglistLight = await Helper.FilterDifferenzes(_sourceSonglistFull, cancelSource.Token, progress);
            EnableButtons(true);
        }

        private async void btnToFile_Click(object sender, RoutedEventArgs e)
        {
            EnableButtons(false);
            _sourceSonglistFull = await Helper.SetFileRating(dgVisible.DataContext as Dictionary<int, SongStruct> , cancelSource.Token, progress);
            _sourceSonglistLight = await Helper.FilterDifferenzes(_sourceSonglistFull, cancelSource.Token, progress);
            CbFilter.IsChecked = true;
            EnableButtons(true);
        }

        private async void btnToItunes_Click(object sender, RoutedEventArgs e)
        {
            EnableButtons(false);
            _sourceSonglistFull = await Helper.ItunesRatingSet(dgVisible.DataContext as Dictionary<int, SongStruct> , _myiTunes, cancelSource.Token, progress);
            _sourceSonglistLight = await Helper.FilterDifferenzes(_sourceSonglistFull, cancelSource.Token, progress);
            CbFilter.IsChecked = true;
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
            if (value)
            {
                UpdateView();
            }
            btnCancel.IsEnabled = !value;
            btnToFile.IsEnabled = value;
            btnToItunes.IsEnabled = value;
            btnReadXml.IsEnabled = value;
            ProgressBar.Value = 0;

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

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                RbFile.IsEnabled = RbItunes.IsEnabled = CbFastMode.IsChecked ?? false;
                CbFilter.IsChecked = CbFilter.IsEnabled = CbFastMode.IsChecked ?? false;
            }
            catch (Exception)
            { }
        }

        private void tbCounter_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void UpdateView()
        {
            if (CbFilter.IsChecked ?? false)
                dgVisible.DataContext = _sourceSonglistLight;
            else
                dgVisible.DataContext = _sourceSonglistFull;
        }

        private void CheckBox_Checked_1(object sender, RoutedEventArgs e)
        {
            UpdateView();
        }
    }
}
