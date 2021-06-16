using System;

namespace DBDataGenLibrary
{
    public class PlaceNameGenerator
    {
        private string[] first = {"Chelm", "Elm", "El", "Bur", "En", "Eg", "Pem", "Pen", "Edg", "Sud", "Sod", "Hors", "Dur", "Sun", "Nort", "Brad", "Farn", "Barn", "Dart", "Hart", "South", "Shaft", "Blan", "Rock", "Alf", "Wy", "Marl", "Staf", "Wet", "Cas", "Stain", "Whit", "Stap", "Brom", "Wych", "Watch", "Win", "Horn", "Mel", "Cook", "Hurst", "Ald", "Shriv", "Kings", "Clere", "Maiden", "Leather", "Brack","Brain", "Walt", "Prest", "Wen", "Flit", "Ash"};
        private string[] doubles = {"Bass", "Chipp", "Sodd", "Sudd", "Ell", "Burr", "Egg", "Emm", "Hamm", "Hann", "Cann", "Camm", "Camb", "Sund", "Pend", "End", "Warr", "Worr", "Hamp", "Roth", "Both", "Sir", "Cir", "Redd", "Wolv", "Mill", "Kett", "Ribb", "Dribb", "Fald", "Skell", "Chedd", "Chill", "Tipp", "Full", "Todd", "Abb", "Booth"};
        private string[] postdoubles = {"ing", "en", "er"};
        private string[] mid = {"bas", "ber", "stan", "ring", "den", "-under-", " on ", "en", "re", "rens", "comp", "mer", "sey", "mans"};
        private string[] last = {"ford", "stoke", "ley", "ney",  "don", "den", "ton", "bury", "well", "beck", "ham", "borough", "side", "wick", "hampton", "wich", "cester", "chester", "ling", "moor", "wood", "brook", "port", "wold", "mere", "castle", "hall", "bridge", "combe", "smith", "field", "ditch", "wang", "over", "worth", "by", "brough", "low", "grove", "avon", "sted", "bourne", "borne", "thorne", "lake", "shot", "bage", "head", "ey", "nell", "tree", "down"};

        public string Generate()
        {
            var rand = new Random();

            string finishedName = "";
            char c;
            int pd = 0, i;

            if (rand.Next(100) > 40)
            {
                finishedName += doubles[rand.Next(doubles.Length)];
                if (rand.Next(100) > 60)
                {
                    finishedName += postdoubles[rand.Next(postdoubles.Length)];
                    pd++;
                } else
                    finishedName.TrimEnd();
            } else
                finishedName += first[rand.Next(first.Length)];

            c = finishedName[finishedName.Length - 1];

            if (rand.Next(100) > 50 && pd == 0)
            {
                if (c == 'r' || c == 'b')
                {
                    if (rand.Next(100) > 40)
                        finishedName += "ble";
                    else
                        finishedName += "gle";
                } else if (c == 'n' || c == 'd')
                {
                    finishedName += "dle";
                } else if (c == 's')
                {
                    finishedName += "tle";
                }
            }

            c = finishedName[finishedName.Length - 1];

            if (rand.Next(100) > 70 && c == 'e')
            {
                if (finishedName[finishedName.Length - 2] == 'l')
                    finishedName += "s";
            } else if (rand.Next(100) > 50)
            {
                if (c == 'n')
                {
                    if (rand.Next(100) > 50)
                        finishedName += "s";
                    else 
                        finishedName += "d";
                } else if (c == 'm')
                    finishedName += "s";
            }

            if (rand.Next(100) > 70)
                finishedName += mid[rand.Next(mid.Length)];
            finishedName += last[rand.Next(last.Length)];

            for (i = finishedName.Length - 1; i >= 0; i--)
            {
                if (finishedName[i] == ' ')
                {
                    NameGenerator.CapitalizeAt(0, ref finishedName);
                }
            }

            for (i = finishedName.Length - 1; i >= 0; i--)
            {
                if (finishedName[i] == '-')
                {
                    NameGenerator.CapitalizeAt(0, ref finishedName);
                }
            }

            return finishedName;
        }
    }
}