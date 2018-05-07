using Claunia.PropertyList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace iTunesManipulation
{
    static class XmlHandler
    {
        public static List<SongStruct> LoadItunesXML(string filePath)
        {
            if (!File.Exists(filePath))
            {
                MessageBox.Show("invalid filepath: '" + filePath + "'");
                return new List<SongStruct>();
            }

            List<SongStruct> bibliotek = new List<SongStruct>();
            NSDictionary tracks = (XmlPropertyListParser.Parse(new FileInfo(filePath)) as NSDictionary)["Tracks"] as NSDictionary;

            foreach (var entry in tracks)
            {
                NSDictionary track = (NSDictionary)entry.Value;
                SongStruct musicFile = new SongStruct();

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
                { musicFile.Album = track["Artist"].ToString(); }

                if (track.ContainsKey("AlbumA rtist"))
                { musicFile.Album = track["Album Artist"].ToString(); }

                if (track.ContainsKey("Genre"))
                { musicFile.Genre = track["Genre"].ToString(); }

                // integer
                if (track.ContainsKey("Track Number"))
                { musicFile.Track = int.Parse(track["Track Number"].ToString()); }

                if (track.ContainsKey("Year"))
                { musicFile.Year = int.Parse(track["Year"].ToString()); }

                if (track.ContainsKey("Album Rating"))
                { musicFile.Year = int.Parse(track["Album Rating"].ToString()); }

                if (track.ContainsKey("Total Time"))
                { musicFile.Year = int.Parse(track["Total Time"].ToString()); }

                if (track.ContainsKey("Bit Rate"))
                { musicFile.Year = int.Parse(track["Bit Rate"].ToString()); }

                if (track.ContainsKey("Play Count"))
                { musicFile.Year = int.Parse(track["Play Count"].ToString()); }

                //// bool
                //if (track.ContainsKey("Rating Computed"))
                //{ musicFile.RatingComputed = (track["Rating Computed"].ToString() == "true") ? true : false; }

                // ToDo: TryCatch ??

                bibliotek.Add(musicFile);
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
