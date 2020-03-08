using iTunesLib;
using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MusicRaitingSync
{
    public class RatingConnector
    {
        public static string[] compitableMusicTypes = { ".mp3", ".m4a", ".acc" };

        public static async Task<Dictionary<int, SongStructS>> GetFilesRating(Dictionary<int, SongStructS> givenList, CancellationToken ctx, IProgress<double> progress)
        {
            await Task.Run(() =>
            {
                for (int i = 0; i < givenList.Count; i++)
                {
                    if (ctx.IsCancellationRequested)
                    {
                        break;
                    }

                    givenList.Values.ElementAt(i).RatingFile = GetFileRating(givenList.Values.ElementAt(i).Location);
                    Report(progress, i, givenList.Count);
                }
            }, ctx);

            return givenList;
        }

        public static async Task<Dictionary<int, SongStructS>> SetFilesRating(Dictionary<int, SongStructS> givenList, CancellationToken ctx, IProgress<double> progress)
        {
            await Task.Run(() =>
            {
                for (int i = 0; i < givenList.Count; i++)
                {
                    if (ctx.IsCancellationRequested)
                    {
                        break;
                    }

                    var entry = givenList.Values.ElementAt(i);
                    if (RatingWriteFile(new FileInfo(entry.Location), entry.RatingiTunes))
                    {
                        entry.RatingFile = entry.RatingiTunes;
                    }

                    Report(progress, i, givenList.Count);
                }
            }, ctx);

            return givenList;
        }

        public static async Task<Dictionary<int, SongStructS>> ItunesRatingGet(Dictionary<int, SongStructS> givenList, iTunesAppClass _myiTunes, CancellationToken ctx, IProgress<double> progress)
        {
            var a = DateTime.Now;
            int i = 0;
            Dictionary<int, SongStructS> listOfDifferences = new Dictionary<int, SongStructS>();
            List<SongStructS> l = new List<SongStructS>();
            await Task.Run(() =>
            {
                foreach (IITTrack trac in _myiTunes.LibraryPlaylist.Tracks)
                {
                    if (ctx.IsCancellationRequested)
                    {
                        break;
                    }

                    l.Add(new SongStructS
                    {
                        ID = trac.TrackDatabaseID,
                        RatingiTunes = trac.Rating.ItunesRatingTo5(),
                    });

                    Report(progress, i++, givenList.Count);
                }

                i = 0;
                foreach (SongStructS track in l)
                {
                    if (ctx.IsCancellationRequested)
                    {
                        break;
                    }

                    SongStructS entry = (from _ in givenList.Values
                                         where _.ID == track.ID && _.RatingComputed != true
                                         select _).FirstOrDefault();
                    if (entry == null)
                        continue;

                    entry.RatingiTunes = track.RatingiTunes;
                    Report(progress, i++, givenList.Count);
                }
            }, ctx);

            var b = DateTime.Now - a;
            Console.WriteLine($"TIme: {b}");
            await Task.Delay(0);
            return givenList;
        }

        public static async Task<Dictionary<int, SongStructS>> ItunesRatingGet2(Dictionary<int, SongStructS> givenList, iTunesAppClass _myiTunes, CancellationToken ctx, IProgress<double> progress)
        {
            var a = DateTime.Now;
            Dictionary<int, SongStructS> listOfDifferences = new Dictionary<int, SongStructS>();
            await Task.Run(() =>
            {
                for (int i = 0; i < givenList.Count; i++)
                {
                    if (ctx.IsCancellationRequested)
                    {
                        break;
                    }


                    var entry = givenList.Values.ElementAt(i);
                    var tracks = _myiTunes.LibraryPlaylist.Search(entry.Name, ITPlaylistSearchField.ITPlaylistSearchFieldSongNames);

                    foreach (IITTrack track in tracks)
                    {
                        if (track.TrackDatabaseID == entry.ID)
                        {
                            entry.RatingiTunes = track.Rating.ItunesRatingTo5();
                            break;
                        }
                    }

                    Report(progress, i, givenList.Count);
                }
            }, ctx);

            var b = DateTime.Now - a;
            Console.WriteLine($"TIme: {b}");
            return givenList;
        }

        public static async Task<Dictionary<int, SongStructS>> ItunesRatingGet3(Dictionary<int, SongStructS> givenList, iTunesAppClass _myiTunes, CancellationToken ctx, IProgress<double> progress)
        {
            var a = DateTime.Now;
            Dictionary<int, SongStructS> listOfDifferences = new Dictionary<int, SongStructS>();

            List<SongStructS> l = new List<SongStructS>();
            await Task.Run(() =>
            {
                foreach (IITTrack track in _myiTunes.LibraryPlaylist.Tracks)
                {     
                    if (ctx.IsCancellationRequested)
                    {
                        break;
                    }

                    SongStructS entry = (from _ in givenList.Values
                                         where _.ID == track.TrackDatabaseID && _.RatingComputed != true
                                         select _).FirstOrDefault();
                    if (entry == null)
                        continue;

                    entry.RatingiTunes = track.Rating.ItunesRatingTo5();
                    Report(progress, 0, givenList.Count);
                }
            }, ctx);

            var b = DateTime.Now - a;
            Console.WriteLine($"TIme: {b}");
            return givenList;
        }

        public static async Task<Dictionary<int, SongStructS>> ItunesRatingSet(Dictionary<int, SongStructS> givenList, iTunesAppClass _myiTunes, CancellationToken ctx, IProgress<double> progress)
        {
            Dictionary<int, SongStructS> listOfDifferences = new Dictionary<int, SongStructS>();
            await Task.Run(() =>
            {
                for (int i = 0; i < givenList.Count; i++)
                {
                    if (ctx.IsCancellationRequested)
                    {
                        break;
                    }

                    // ToDo: Testen mit kleiner Bibliotek
                    var entry = givenList.Values.ElementAt(i);
                    var tracks = _myiTunes.LibraryPlaylist.Search(entry.Name, ITPlaylistSearchField.ITPlaylistSearchFieldSongNames);
                    foreach (var track in from IITTrack track in tracks
                                          where track.TrackDatabaseID == entry.ID
                                          select track)
                    {
                        track.Rating = entry.RatingFile.ToItunesRating();
                        entry.RatingiTunes = track.Rating.ItunesRatingTo5();
                        break;
                    }

                    Report(progress, i, givenList.Count);
                }
            }, ctx);

            return givenList;
        }

        #region read/write file rating
        public static int GetFileRating(string location)
        {
            if (!File.Exists(location))
            {
                return 0;
            }

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
                {
                    return false;
                }

                if (!file.Exists)
                {
                    return false;
                }

                so.Properties.System.Rating.Value = rating.ToFileRating();
                return (so.Properties.System.Rating.Value == null && rating != 0) ? false : true;
            }
            catch (Exception)
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
