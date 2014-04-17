
namespace iTunesManipulation
{
    class SongStruct
    {
        public SongStruct()
        {
        }
        // Belangrijke informatie uit de xml
        public string Name, Artist, AlbumArtist, Album;
        public int TotalTime, BitRate, PlayCount, Rating;
        public bool RatingComputed = false;
        public int AlbumRating; // from 1 to 100
        public string Location; // where on disk
        public override string ToString()
        {
            return "song: " + Name;
        }
    }
}

