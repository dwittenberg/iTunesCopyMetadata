using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTunesManipulation
{
    public static class MyExtentions
    {
        public static int ToItunesRating(this int rating)
        {
            return rating * 20;
        }

        public static int ItunesRatingTo5(this int rating)
        {
            if (rating == 0)
                return 0;
            else
                return rating / 20;
        }

        public static uint? ToFileRating(this int rating)
        {
            switch (rating)
            {
                case 0:
                    return null;
                case 1:
                    return 1;
                case 2:
                    return 25;
                case 3:
                    return 50;
                case 4:
                    return 75;
                case 5:
                    return 99;
                default:
                    return 200;
            }
        }

        public static int FileRatingTo5(this uint? rating)
        {
            switch (rating)
            {
                case 0:
                    return 0;
                case 1:
                    return 1;
                case 25:
                    return 2;
                case 50:
                    return 3;
                case 75:
                    return 4;
                case 99:
                    return 5;
                default:
                    return 200;
            }
        }
    }
}
