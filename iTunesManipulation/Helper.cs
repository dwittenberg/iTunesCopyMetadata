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
        

        public static async Task<List<SongStruct>> GetFileRating(List<SongStruct> givenList, CancellationToken ctx, IProgress<double> progress)
        {
            List<SongStruct> listOfDifferences = new List<SongStruct>();
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

            return listOfDifferences;
        }

        public static async Task<List<SongStruct>> GetItunesRating(List<SongStruct> givenList, iTunesAppClass _myiTunes, CancellationToken ctx, IProgress<double> progress)
        {
            List<SongStruct> listOfDifferences = new List<SongStruct>();
            await Task.Run(() =>
            {
                for (int i = 0; i < givenList.Count; i++)
                {
                    if (ctx.IsCancellationRequested)
                        break;

                    // ToDo: Testen mit kleiner Bibliotek
                    var a = _myiTunes.LibraryPlaylist.Tracks.ItemByName[givenList[i].Name] as IITTrack;
                    givenList[i].RatingiTunes = a.Rating.ItunesRatingTo5();
                    Report(progress, i, givenList.Count);
                }
            }, ctx);

            return givenList;
        }

        public static async Task<List<SongStruct>> FilterDifferenzes(List<SongStruct> givenList, CancellationToken ctx, IProgress<double> progress)
        {
            List<SongStruct> listOfDifferences = new List<SongStruct>();
            //ConcurrentBag<SongStruct> bagOfDifferences = new ConcurrentBag<SongStruct>();

            await Task.Run(() =>
            {
                //givenList.AsParallel().ForAll(song =>
                //{
                //    if (ctx.IsCancellationRequested)
                //        return;

                //    if ((song.RatingiTunes == 0 &&
                //        song.RatingFile != 0) |
                //        song.RatingFile != song.RatingiTunes)
                //    {
                //        bagOfDifferences.Add(song);
                //    }

                //    progress.Report(1);
                //});
                for (int i = 0; i < givenList.Count; i++)
                {

                    if (ctx.IsCancellationRequested)
                        break;

                    if ((givenList[i].RatingiTunes == 0 &&
                        givenList[i].RatingFile != 0) |
                        givenList[i].RatingFile != givenList[i].RatingiTunes)
                    {
                        listOfDifferences.Add(givenList[i]);
                    }

                    Report(progress, i, givenList.Count);
                }
            }, ctx);

            return listOfDifferences;
        }

        #region read/write file rating
        public static int RatingReadFile(string location)
        {
            ShellFile so = ShellFile.FromFilePath(location);

            if (!compitableMusicTypes.Contains<string>(location.Split('.').Last()) | so.Properties.System.Rating.Value == null)
            { return 0; }

            return so.Properties.System.Rating.Value.FileRatingTo5();
        }

        public static async Task<bool> Test(CancellationToken ctx, IProgress<double> progress)
        {

            for (int i = 0; i < 100; i++)
            {
                if (ctx.IsCancellationRequested)
                    break;
                await Task.Delay(25);
                Report(progress, i, 100);
            }

            return true;
        }


        public static bool RatingWrite(FileInfo file, int rating)
        {
            try
            {
                ShellFile so = ShellFile.FromFilePath(file.FullName);

                if (!compitableMusicTypes.Contains<string>(file.Extension))
                    return false;

                so.Properties.System.Rating.Value = rating.ToFileRating();
                return true;
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
