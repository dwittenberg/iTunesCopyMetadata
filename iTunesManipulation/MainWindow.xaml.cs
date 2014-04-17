using System.IO;
using System.Threading.Tasks;
using System.Windows.Threading;
using iTunesLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;

namespace iTunesManipulation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly iTunesAppClass _myiTunes = new iTunesAppClass();

        private List<SongStruct> _sourceSonglist;
        private string _path = "";
        private string Path
        {
            get { return _path; }
            set
            {
                if (_path == value || OutputTextBox == null)
                    return;
                _path = value;
                if (OutputTextBox != null && OutputTextBox.IsInitialized)
                    OutputTextBox.Text += "source file: " + value + "\n";
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            if (App.args.Length > 0)
                SourceBox.Text = SourceBox.Text.Insert(0, App.args[0] + "\n");

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (_myiTunes.SelectedTracks == null) return;

            ThreadPool.QueueUserWorkItem(ThreadPoolCallback);
            DaButton.IsEnabled = false;
        }

        private void LogLineAsync(string output)
        {
            OutputTextBox.Dispatcher.Invoke(
                    DispatcherPriority.Normal,
                 (ThreadStart)delegate
                 {
                     OutputTextBox.Text += output + "\n";
                 }
            );
        }

        private double _indirectProgressValue = 0;
        public void ThreadPoolCallback(Object threadContext)
        {
            _sourceSonglist = ParseItunesXmlFile(Path);

            var selectedTracks = _myiTunes.SelectedTracks;
            if (selectedTracks == null) return;

            LogLineAsync("# found old songs: " + _sourceSonglist.Count);
            LogLineAsync("# selected tracks: " + selectedTracks.Count);

            //_sourceSonglist = ParseItunesXmlFile(SourceBox.Text);
            uint i = 0;
            uint nrColitions = 0;
            //var looper = _sourceSonglist.ToArray();
            //Parallel.For(0, looper.Count(), itterator =>
            foreach (var oldTrack in _sourceSonglist)
            {
                //var songStruct = looper[itterator];
                ++i;
                _indirectProgressValue = (double)i / _sourceSonglist.Count * 100.0;

                if (oldTrack.Name == null) continue; // unnamed song
                var livingTrack = selectedTracks.ItemByName[oldTrack.Name];
                if (livingTrack == null) continue;


                // ' livingTrack (from current iTunes) '  ==  ' oldTrack (from other iTunes XML) '
                ++nrColitions;
                livingTrack.PlayedCount = Math.Max(livingTrack.PlayedCount, oldTrack.PlayCount);
                if (!oldTrack.RatingComputed && oldTrack.Rating != 0)
                {
                    livingTrack.Rating = Math.Max(oldTrack.Rating, livingTrack.Rating);
                }


            }//);
            LogLineAsync("# colitions: "+nrColitions);
        }
        private void ProgressBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ProgressBar.Value = _indirectProgressValue;
        }

        List<SongStruct> ParseItunesXmlFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                MessageBox.Show("invalid filepath: '" + filePath + "'");
                return new List<SongStruct>();
            }
            var songList = new List<SongStruct>();
            songList.Capacity = 2000;
            // source file, gelezen door basic XML reader
            var reader = new XmlTextReader(filePath);
            // SongStruct tmpSong;

            // Read in the source File
            while (reader.Read())
            {
                switch (reader.Value)
                {
                    case "Track ID":
                        songList.Add(new SongStruct());
                        break;
                    case "Name":
                        reader.Read();
                        reader.Read();
                        reader.Read(); // 3 thing further
                        songList.ElementAt(songList.Count - 1).Name = reader.Value;
                        break;
                    case "Artist":
                        reader.Read();
                        reader.Read();
                        reader.Read(); // 3 thing further
                        songList.ElementAt(songList.Count - 1).Artist = reader.Value;
                        break;
                    case "Album Artist":
                        reader.Read();
                        reader.Read();
                        reader.Read(); // 3 thing further
                        songList.ElementAt(songList.Count - 1).AlbumArtist = reader.Value;
                        break;
                    case "Album":
                        reader.Read();
                        reader.Read();
                        reader.Read(); // 3 thing further
                        songList.ElementAt(songList.Count - 1).Album = reader.Value;
                        break;
                    case "Play Count":
                        reader.Read();
                        reader.Read();
                        reader.Read(); // 3 thing further
                        songList.ElementAt(songList.Count - 1).PlayCount = Convert.ToInt32(reader.Value);
                        break;
                    case "Rating":
                        reader.Read();
                        reader.Read();
                        reader.Read(); // 3 thing further
                        songList.ElementAt(songList.Count - 1).Rating = Convert.ToInt32(reader.Value);
                        break;
                    case "Rating Computed":
                        reader.Read();
                        reader.Read();
                        if (reader.Name == "true")
                            songList.ElementAt(songList.Count - 1).RatingComputed = true;
                        break;
                    case "Album Rating":
                        reader.Read();
                        reader.Read();
                        reader.Read(); // 3 thing further
                        songList.ElementAt(songList.Count - 1).AlbumRating = Convert.ToInt32(reader.Value);
                        break;
                    case "Location":
                        reader.Read();
                        reader.Read();
                        reader.Read(); // 3 thing further
                        songList.ElementAt(songList.Count - 1).Location = reader.Value;
                        break;
                }
                //if (songList.Count > 100) break; 
                //System.Threading.Thread.Sleep(50);
            }
            return songList;
        }

        private void _daButton_Drop(object sender, DragEventArgs e)
        {
            int haha;
        }

        private void SourceBox_Drop(object sender, DragEventArgs e)
        {
            string filename;
            GetFilename(out filename, e);
            SourceBox.Text = filename;
        }
        protected bool GetFilename(out string filename, DragEventArgs e)
        {
            const bool ret = false;
            filename = String.Empty;

            if ((e.AllowedEffects & DragDropEffects.Copy) != DragDropEffects.Copy) return ret;

            Array data = ((IDataObject)e.Data).GetData("FileName") as Array;
            if (data == null) return ret;
            if ((data.Length == 1) && (data.GetValue(0) is String))
            {
                filename = ((string[])data)[0];
                //string ext = Path.GetExtension(filename).ToLower();
                //if ((ext == ".xml") || (ext == ".png") || (ext == ".bmp"))
                //{
                //    ret = true;
                //}
            }
            return ret;
        }

        private void SourceBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var box = (TextBox)sender;

            var pathsToChose = new List<string>(SourceBox.Text.Split('\n'));
            Path = pathsToChose.Find(File.Exists);
        }

    }
}
