using System;
using System.Collections.Generic;
using System.Linq;

namespace com.clusterrr.hakchi_gui.data
{
    public class Region
    {
        public string LocalizedName => Properties.Resources.ResourceManager.GetString(LocalizedNameKey);

        public string DesktopName { get; set; }
        public string LocalizedNameKey { get; set; }
        public string[] DefaultNames { get; set; } = new string[] { };  //Considered from database when importing (and possible scraping?).

        //DefaultNames have been populated with ones seen in the database but really this should be properly data driven...
        public static IReadOnlyList<Region> RegionList = new List<Region>()
        {
            new Region(){
                DesktopName = "us",
                LocalizedNameKey = "UnitedStates",
                DefaultNames = new string[] { "United States", "US", "USA" }
            },
            new Region(){
                DesktopName = "eu",
                LocalizedNameKey = "Europe",
                DefaultNames = new string[] { "Europe", "France", "Germany", "Spain", "Italy", "Sweden", "Netherlands", "Scandinavia", "United Kingdom", "UK" }
            },
            new Region(){
                DesktopName = "jp",
                LocalizedNameKey = "Japan",
                DefaultNames = new string[] { "Japan" }
            }
        };
        public static Dictionary<string, Region> _RegionDictionary = null;
        public static IReadOnlyDictionary<string, Region> RegionDictionary
        {
            get
            {
                if (_RegionDictionary == null)
                {
                    _RegionDictionary = new Dictionary<string, Region>();

                    foreach (var region in RegionList.OrderBy(e => e.DesktopName))
                    {
                        _RegionDictionary[region.DesktopName] = region;
                    }
                }

                return _RegionDictionary;
            }
        }
    }
}
