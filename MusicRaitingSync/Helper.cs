using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAPICodePack.Shell;
using iTunesLib;
using System.Threading;

namespace MusicRaitingSync
{
    public class Helper
    {

        public static double status = 0;

        /// <summary>
        /// Load FIleInfos from XML - without Rating
        /// </summary>
        /// <returns></returns>


        public static Dictionary<int, SongStructS> ItunesGetInformation(iTunesAppClass _myiTunes)
        {
            Dictionary<int, SongStructS> fileList = new Dictionary<int, SongStructS>();

            foreach (IITTrack track in _myiTunes.LibraryPlaylist.Tracks)
            {
                fileList.Add(track.trackID, new SongStructS
                {
                    Name = track.Name,
                    Track = track.trackID,
                    Album = track.Album,
                    //Location = track.,
                    Artist = track.Artist,
                    RatingiTunes = track.Rating,
                    ID = track.trackID,
                });
            }

            return fileList;
        }



        public static async Task<Dictionary<int, SongStructS>> FilterDifferenzes(Dictionary<int, SongStructS> givenList, CancellationToken ctx, IProgress<double> progress)
        {
            Dictionary<int, SongStructS> listOfDifferences = new Dictionary<int, SongStructS>();
            //ConcurrentBag<SongStruct> bagOfDifferences = new ConcurrentBag<SongStruct>();

            await Task.Run(() =>
            {
                foreach (var song in from SongStructS _ in givenList.Values
                                     where _.RatingFile != _.RatingiTunes
                                     select _)
                {
                    listOfDifferences.Add(song.ID, song);
                }
            }, ctx);

            return listOfDifferences;
        }

        private static void Report(IProgress<double> progress, int value, int max)
        {
            progress.Report((value * 100.0) / max);
        }
    }
}
