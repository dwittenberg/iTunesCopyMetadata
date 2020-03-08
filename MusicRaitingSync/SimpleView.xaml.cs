using iTunesLib;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace MusicRaitingSync
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SimpleView : Window
    {
        private Dictionary<int, SongStructS> _sourceSonglistFull;
        private Dictionary<int, SongStructS> _sourceSonglistLight;

        private readonly iTunesAppClass _myiTunes = new iTunesAppClass();
        private CancellationTokenSource cancelSource;
        private Progress<double> progress;
        private string _xmlPath = string.Empty;

        public SimpleView()
        {
            InitializeComponent();

            progress = new Progress<double>();
            progress.ProgressChanged += Progress_ProgressChanged;
            cancelSource = new CancellationTokenSource();
            _xmlPath = _myiTunes.LibraryXMLPath;
        }

        private int counter = 1;
        private void Progress_ProgressChanged(object sender, double e)
        {
            if (ProgressBar.Value > e)
                counter += 1;

            ProgressBar.Value = e;
            tbCounter.Text = $"Schritt {counter} | {Math.Round(e, 2)}";
        }

        #region new buttons
        private async void ReadRating(object sender, RoutedEventArgs e)
        {

            EnableButtons(false);
            _sourceSonglistFull = XmlHandler.LoadItunesXML(_xmlPath);

            if ((bool)CbFastMode.IsChecked)
            {
                if ((bool)RbFile.IsChecked)
                {
                    _sourceSonglistFull = await RatingConnector.GetFilesRating(_sourceSonglistFull, cancelSource.Token, progress);
                    _sourceSonglistLight = await Helper.FilterDifferenzes(_sourceSonglistFull, cancelSource.Token, progress);
                    _sourceSonglistLight = await RatingConnector.ItunesRatingGet(_sourceSonglistLight, _myiTunes, cancelSource.Token, progress);
                }

                if ((bool)RbItunes.IsChecked)
                {
                    _sourceSonglistFull = await RatingConnector.ItunesRatingGet(_sourceSonglistFull, _myiTunes, cancelSource.Token, progress);
                    _sourceSonglistLight = await Helper.FilterDifferenzes(_sourceSonglistFull, cancelSource.Token, progress);
                    _sourceSonglistLight = await RatingConnector.GetFilesRating(_sourceSonglistLight, cancelSource.Token, progress);
                }

                _sourceSonglistLight = await Helper.FilterDifferenzes(_sourceSonglistLight, cancelSource.Token, progress);
                CbFilter.IsChecked = true;
            }
            else
            {
                _sourceSonglistFull = await RatingConnector.ItunesRatingGet(_sourceSonglistFull, _myiTunes, cancelSource.Token, progress);
                _sourceSonglistFull = await RatingConnector.GetFilesRating(_sourceSonglistFull, cancelSource.Token, progress);
                _sourceSonglistLight = await Helper.FilterDifferenzes(_sourceSonglistFull, cancelSource.Token, progress);
            }

            EnableButtons(true);
        }

        private void btnToFile_Click(object sender, RoutedEventArgs e)
        {
            WriteRating(false);
        }

        private void btnToItunes_Click(object sender, RoutedEventArgs e)
        {
            WriteRating(true);
        }

        private async void WriteRating(bool ToiTunes)
        {
            EnableButtons(false);

            if (ToiTunes)
                _sourceSonglistFull = await RatingConnector.ItunesRatingSet(dgVisible.DataContext as Dictionary<int, SongStructS>, _myiTunes, cancelSource.Token, progress);
            else
                _sourceSonglistFull = await RatingConnector.SetFilesRating(dgVisible.DataContext as Dictionary<int, SongStructS>, cancelSource.Token, progress);

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
            btnToFile.IsEnabled = btnToItunes.IsEnabled = value && !(CbFastMode.IsChecked ?? false);
            btnWriteRating.IsEnabled = value && (CbFastMode.IsChecked ?? false);
            CbFastMode.IsEnabled = value;
            RbFile.IsEnabled = RbItunes.IsEnabled = value;
            btnReadXml.IsEnabled = value;
            //ProgressBar.Value = 1;
            Progress_ProgressChanged(new object(), 1.0);
        }

        private void CbFastMode_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                bool FastModeEnabled = CbFastMode.IsChecked ?? false;
                RbFile.IsEnabled = RbItunes.IsEnabled = FastModeEnabled;

                btnWriteRating.IsEnabled = FastModeEnabled;
                btnToItunes.IsEnabled = btnToFile.IsEnabled = !FastModeEnabled;
            }
            catch (Exception)
            { }
        }

        private void CbFilter_Checked(object sender, RoutedEventArgs e)
        {
            UpdateView();
        }

        private void UpdateView()
        {
            if (CbFilter.IsChecked ?? false)
                dgVisible.DataContext = _sourceSonglistLight;
            else
                dgVisible.DataContext = _sourceSonglistFull;
        }
    }
}
