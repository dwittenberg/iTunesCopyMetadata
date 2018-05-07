
namespace iTunesManipulation
{
    public class SongStruct
    {
        public SongStruct()
        {
        }
        // All informations of xml, iTunes & file
        public string Name, Artist, AlbumArtist, Album, Genre;
        public int TotalTime, BitRate, PlayCount, RatingiTunes, RatingFile, Track, Year;
        public bool RatingComputed = false;
        public int AlbumRating; // from 1 to 100
        public string Location; // where on disk

        public override string ToString()
        {
            return "song: " + Name;
        }
    }
}

