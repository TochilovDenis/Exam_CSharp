using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStore
{
    public class Album
    {
        public int AlbumID { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Label { get; set; }
        public int TrackCount { get; set; }
        public int GenreID { get; set; }
        public int YearReleased { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SellingPrice { get; set; }

        public Album()
        {
            // Пустой конструктор для сериализации
        }

        public Album(string title, string artist, decimal sellingPrice)
        {
            Title = title;
            Artist = artist;
            SellingPrice = sellingPrice;
        }
    }
}
