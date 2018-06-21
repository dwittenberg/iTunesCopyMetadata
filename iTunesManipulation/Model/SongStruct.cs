
namespace iTunesManipulation
{
    public class SongStruct
    {
        public SongStruct()
        {
            RatingComputed = false;
        }

        // All informations of xml, iTunes & file
        public string Name { get; set; }
        public string Artist { get; set; }
        public string AlbumArtist { get; set; }
        public string Album { get; set; }
        public string Genre { get; set; }
        public int ID { get; set; }
        public int TotalTime { get; set; }
        public int BitRate { get; set; }
        public int PlayCount { get; set; }
        public int RatingiTunes { get; set; }
        public int RatingFile { get; set; }
        public int Track { get; set; }
        public int Year { get; set; }
        public bool RatingComputed { get; set; }

        /// <summary>
        /// Value between 0 and 100
        /// </summary>
        public int AlbumRating { get; set; }

        /// <summary>
        /// Location of the file.
        /// </summary>
        public string Location { get; set; }

        public override string ToString()
        {
            return "song: " + Name;
        }
    }
}

