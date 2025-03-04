using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStore
{
    public class Promotion
    {
        public int PromotionID { get; set; }
        public int CustomerID { get; set; }
        public decimal MinPurchase { get; set; }
        public decimal DiscountPercentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public Promotion()
        {
            // Пустой конструктор для сериализации
        }

        public Promotion(int customerID, decimal minPurchase, decimal discountPercentage)
        {
            CustomerID = customerID;
            MinPurchase = minPurchase;
            DiscountPercentage = discountPercentage;
            StartDate = DateTime.Now;
        }
    }
}
