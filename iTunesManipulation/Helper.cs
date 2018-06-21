using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAPICodePack.Shell;
using iTunesLib;
using System.Threading;

namespace iTunesManipulation
{
    public class Helper
    {
        public static string[] compitableMusicTypes = { ".mp3", ".m4a", ".acc" };
        public static double status = 0;

        /// <summary>
        /// Load FIleInfos from XML - without Rating
        /// </summary>
        /// <returns></returns>
        public static async Task<Dictionary<int, SongStruct> > GetFileRating(Dictionary<int, SongStruct>  givenList, CancellationToken ctx, IProgress<double> progress)
        {
            await Task.Run(() =>
            {
                for (int i = 0; i < givenList.Count; i++)
                {
                    if (ctx.IsCancellationRequested)
                        break;

                    givenList[i].RatingFile = RatingReadFile(givenList[i].Location);
                    Report(progress, i, givenList.Count);
                }
            }, ctx);

            return givenList;
        }

        public static async Task<Dictionary<int, SongStruct> > SetFileRating(Dictionary<int, SongStruct>  givenList, CancellationToken ctx, IProgress<double> progress)
        {
            await Task.Run(() =>
            {
                for (int i = 0; i < givenList.Count; i++)
                {
                    if (ctx.IsCancellationRequested)
                        break;

                    if (RatingWriteFile(new FileInfo(givenList[i].Location), givenList[i].RatingiTunes))
                        givenList[i].RatingFile = givenList[i].RatingiTunes;

                    Report(progress, i, givenList.Count);
                }
            }, ctx);

            return givenList;
        }

        public static Dictionary<int, SongStruct>  ItunesGetInformation(iTunesAppClass _myiTunes)
        {
            Dictionary<int, SongStruct>  fileList = new Dictionary<int, SongStruct> ();

            foreach (IITTrack track in _myiTunes.LibraryPlaylist.Tracks)
            {
                fileList.Add(new SongStruct
                {
                    Name = track.Name,
                    Track = track.trackID,
                    Album = track.Album,
                    //Location = track.,
                    Artist = track.Artist,
                    RatingiTunes = track.Rating,
                });
            }

            return fileList;
        }

        public static async Task<Dictionary<int, SongStruct> > ItunesRatingGet(Dictionary<int, SongStruct>  givenList, iTunesAppClass _myiTunes, CancellationToken ctx, IProgress<double> progress)
        {
            Dictionary<int, SongStruct>  listOfDifferences = new Dictionary<int, SongStruct> ();
            await Task.Run(() =>
            {
                //foreach (IITTrack item in _myiTunes.LibraryPlaylist.Tracks)
                //{
                //    var c = item.Rating;
                //}

                for (int i = 0; i < givenList.Count; i++)
                {
                    if (ctx.IsCancellationRequested)
                        break;

                    //IITTrack a =  _myiTunes.LibraryPlaylist.Tracks.ItemByName[givenList[i].Name] as IITTrack;

                    var tracks = _myiTunes.LibraryPlaylist.Search(givenList[i].Name, ITPlaylistSearchField.ITPlaylistSearchFieldSongNames);

                    foreach (IITTrack track in tracks)
                    {
                        if (track.TrackDatabaseID == givenList[i].ID)
                        {
                            givenList[i].RatingiTunes = track.Rating.ItunesRatingTo5();
                            break;
                        }
                    }
                    
                    Report(progress, i, givenList.Count);
                }
            }, ctx);

            return givenList;
        }

        public static async Task<Dictionary<int, SongStruct> > ItunesRatingSet(Dictionary<int, SongStruct>  givenList, iTunesAppClass _myiTunes, CancellationToken ctx, IProgress<double> progress)
        {
            Dictionary<int, SongStruct>  listOfDifferences = new Dictionary<int, SongStruct> ();
            await Task.Run(() =>
            {
                for (int i = 0; i < givenList.Count; i++)
                {
                    if (ctx.IsCancellationRequested)
                        break;

                    // ToDo: Testen mit kleiner Bibliotek
                    var tracks = _myiTunes.LibraryPlaylist.Search(givenList[i].Name, ITPlaylistSearchField.ITPlaylistSearchFieldSongNames);

                    foreach (IITTrack track in tracks)
                    {
                        if (track.TrackDatabaseID == givenList[i].ID)
                        {
                            track.Rating = givenList[i].RatingFile.ToItunesRating();
                            givenList[i].RatingiTunes = track.Rating.ItunesRatingTo5();
                            break;
                        }
                    }

                    Report(progress, i, givenList.Count);
                }
            }, ctx);

            return givenList;
        }

        public static async Task<Dictionary<int, SongStruct> > FilterDifferenzes(Dictionary<int, SongStruct>  givenList, CancellationToken ctx, IProgress<double> progress)
        {
            Dictionary<int, SongStruct>  listOfDifferences = new Dictionary<int, SongStruct> ();
            //ConcurrentBag<SongStruct> bagOfDifferences = new ConcurrentBag<SongStruct>();

            await Task.Run(() =>
            {
                for (int i = 0; i < givenList.Count; i++)
                {
                    if (ctx.IsCancellationRequested)
                        break;

                    if (givenList[i].RatingFile != givenList[i].RatingiTunes)
                    {
                        listOfDifferences.Add(givenList[i]);
                    }

                    Report(progress, i, givenList.Count);
                }
            }, ctx);

            return listOfDifferences;
        }

        #region read/write file rating
        private static int RatingReadFile(string location)
        {
            if (!File.Exists(location))
                return 0;

            ShellFile so = ShellFile.FromFilePath(location);

            if (!compitableMusicTypes.Contains<string>("." + location.Split('.').Last()) || so.Properties.System.Rating.Value == null)
            { return 0; }

            return so.Properties.System.Rating.Value.FileRatingTo5();
        }

        private static bool RatingWriteFile(FileInfo file, int rating)
        {
            try
            {
                ShellFile so = ShellFile.FromFilePath(file.FullName);

                if (!compitableMusicTypes.Contains<string>(file.Extension))
                    return false;

                if (!file.Exists)
                    return false;

                so.Properties.System.Rating.Value = rating.ToFileRating();
                return (so.Properties.System.Rating.Value == null && rating != 0) ? false : true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private static void Report(IProgress<double> progress, int value, int max)
        {
            progress.Report((value * 100.0) / max);
        }
        #endregion
    }
}
