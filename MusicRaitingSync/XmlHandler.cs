using Claunia.PropertyList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace MusicRaitingSync
{
    static class XmlHandler
    {
        public static Dictionary<int, SongStructS>  LoadItunesXML(string filePath)
        {
            if (!File.Exists(filePath))
            {
                MessageBox.Show("invalid filepath: '" + filePath + "'");
                return new Dictionary<int, SongStructS> ();
            }

            Dictionary<int, SongStructS> bibliotek = new Dictionary<int, SongStructS> ();
            NSDictionary tracks = (XmlPropertyListParser.Parse(new FileInfo(filePath)) as NSDictionary)["Tracks"] as NSDictionary;

            foreach (var entry in tracks)
            {
                NSDictionary track = (NSDictionary)entry.Value;
                SongStructS musicFile = new SongStructS();
                int id = 0;

                // Get importend informations
                if (track.ContainsKey("Location"))
                { musicFile.Location = Path2String(track["Location"].ToString()); }

                // ToDo: set Filter?
                // if (File.Exists(musicFile.Location))
                //    continue;

                // strings
                if (track.ContainsKey("Name"))
                { musicFile.Name = track["Name"].ToString(); }

                if (track.ContainsKey("Album"))
                { musicFile.Album = track["Album"].ToString(); }

                if (track.ContainsKey("Artist"))
                { musicFile.Artist = track["Artist"].ToString(); }

                if (track.ContainsKey("Album Artist"))
                { musicFile.AlbumArtist = track["Album Artist"].ToString(); }

                if (track.ContainsKey("Genre"))
                { musicFile.Genre = track["Genre"].ToString(); }

                // integer
                if (track.ContainsKey("Track ID"))
                { id = musicFile.ID = int.Parse(track["Track ID"].ToString()); }

                if (track.ContainsKey("Track Number"))
                { musicFile.Track = int.Parse(track["Track Number"].ToString()); }

                if (track.ContainsKey("Year"))
                { musicFile.Year = int.Parse(track["Year"].ToString()); }

                if (track.ContainsKey("Album Rating"))
                { musicFile.AlbumRating = int.Parse(track["Album Rating"].ToString()); }

                if (track.ContainsKey("Total Time"))
                { musicFile.TotalTime = int.Parse(track["Total Time"].ToString()); }

                if (track.ContainsKey("Bit Rate"))
                { musicFile.BitRate = int.Parse(track["Bit Rate"].ToString()); }

                if (track.ContainsKey("Play Count"))
                { musicFile.PlayCount = int.Parse(track["Play Count"].ToString()); }

                //// bool
                if (track.ContainsKey("Rating Computed"))
                { musicFile.RatingComputed = (track["Rating Computed"].ToString().ToLower() == "true") ? true : false; }

                // ToDo: TryCatch ??

                bibliotek.Add(id, musicFile);
            }
            return bibliotek;
        }

        public static string Path2String(string pathStr)
        {
            Uri pathUri = new Uri(pathStr);
            string path = pathUri.LocalPath;
            path = path.Replace("string: ", "");
            path = path.Replace("\\\\localhost\\", string.Empty);
            path = path.Replace(" file://localhost/", string.Empty);
            path = path.Replace("file://localhost/", string.Empty);

            return path;
        }

        public static string String2Path(string path)
        {
            path = path.Replace("string: ", "");
            path = path.Replace("file://localhost/", string.Empty);

            Uri pathUri = new Uri(path);
            string uri = pathUri.AbsoluteUri;
            uri = "string: " + uri.Replace("file:///", "file://localhost/");

            return uri;
        }
    }
}
