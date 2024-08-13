using DealSeekerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DealSeekerApp.Services
{
    public class TwitterService : IDealsSeeker
    {
        private readonly List<DealTableEntity> dealsOfTheDay = new List<DealTableEntity>()
        {
            new DealTableEntity()
            {
                Description = "SAMSUNG Galaxy Tab S6 Lite (2024) 10.4\" 64GB WiFi Android Tablet, S Pen Included, Gaming Ready, Long Battery Life, Slim Metal Design, Expandable Storage, US Version, Oxford Gray, Amazon Exclusive",
                Price = 209.98,
                Url = "https://www.amazon.com/SAMSUNG-Android-Included-Expandable-Exclusive/dp/B0CWS8MNW1"
            },

            new DealTableEntity()
            {
                Description = "Deluxe 14PC Nonstick Cookware Sets, DUXANO Freshness-Maintained Pots and Pans with 9H Hardness 2-Layer Ceramic Coating, True Cool Handles, PFAS Free, Dishwasher Safe, All Cooktops & Induction Ready",
                Price = 198.89,
                Url = "https://www.amazon.com/Nonstick-DUXANO-Freshness-Maintained-Dishwasher-Induction/dp/B0D25GGFLS"
            },

            new DealTableEntity()
            {
                Description = "Quiet Hybrid Spectrum 20W Bug Zapper Fruit Fly Traps for Indoors Mosquito Zapper Electric Fly Zapper for Home Mosquito Repellent Moth Light Gnat Insect Killer with 2 Replacement Bulbs",
                Price = 39.98,
                Url = "https://www.amazon.com/Spectrum-Mosquito-Electric-Repellent-Replacement/dp/B0BVKGC941"
            },
        };

        public async Task<IEnumerable<DealTableEntity>> GetDealsOfTheDay()
        {
            /* Simulate wait time for api call */
            await Task.Delay(1000);

            return dealsOfTheDay;
        }
    }
}
