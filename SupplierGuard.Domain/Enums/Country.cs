namespace SupplierGuard.Domain.Enums
{
    /// <summary>
    /// List of countries supported by the system.
    /// </summary>
    public enum Country
    {
        Argentina,
        Bolivia,
        Brazil,
        Chile,
        Colombia,
        Ecuador,
        Paraguay,
        Peru,
        Uruguay,
        Venezuela,

        Canada,
        Mexico,
        UnitedStates,

        CostaRica,
        Cuba,
        DominicanRepublic,
        ElSalvador,
        Guatemala,
        Honduras,
        Nicaragua,
        Panama,

        Austria,
        Belgium,
        Denmark,
        Finland,
        France,
        Germany,
        Greece,
        Ireland,
        Italy,
        Netherlands,
        Norway,
        Poland,
        Portugal,
        Spain,
        Sweden,
        Switzerland,
        UnitedKingdom,

        China,
        India,
        Indonesia,
        Japan,
        Malaysia,
        Philippines,
        Singapore,
        SouthKorea,
        Taiwan,
        Thailand,
        Vietnam,
 
        Australia,
        NewZealand,

        Egypt,
        Kenya,
        Nigeria,
        SouthAfrica,

        Israel,
        SaudiArabia,
        UnitedArabEmirates,

        Other
    }

    /// <summary>
    /// Extension methods for the Country enum.
    /// </summary>
    public static class CountryExtensions
    {
        private static readonly Dictionary<Country, string> CountryNames = new()
    {
        
        { Country.Argentina, "Argentina" },
        { Country.Bolivia, "Bolivia" },
        { Country.Brazil, "Brazil" },
        { Country.Chile, "Chile" },
        { Country.Colombia, "Colombia" },
        { Country.Ecuador, "Ecuador" },
        { Country.Paraguay, "Paraguay" },
        { Country.Peru, "Peru" },
        { Country.Uruguay, "Uruguay" },
        { Country.Venezuela, "Venezuela" },

       
        { Country.Canada, "Canada" },
        { Country.Mexico, "Mexico" },
        { Country.UnitedStates, "United States" },

        
        { Country.CostaRica, "Costa Rica" },
        { Country.Cuba, "Cuba" },
        { Country.DominicanRepublic, "Dominican Republic" },
        { Country.ElSalvador, "El Salvador" },
        { Country.Guatemala, "Guatemala" },
        { Country.Honduras, "Honduras" },
        { Country.Nicaragua, "Nicaragua" },
        { Country.Panama, "Panama" },

       
        { Country.Austria, "Austria" },
        { Country.Belgium, "Belgium" },
        { Country.Denmark, "Denmark" },
        { Country.Finland, "Finland" },
        { Country.France, "France" },
        { Country.Germany, "Germany" },
        { Country.Greece, "Greece" },
        { Country.Ireland, "Ireland" },
        { Country.Italy, "Italy" },
        { Country.Netherlands, "Netherlands" },
        { Country.Norway, "Norway" },
        { Country.Poland, "Poland" },
        { Country.Portugal, "Portugal" },
        { Country.Spain, "Spain" },
        { Country.Sweden, "Sweden" },
        { Country.Switzerland, "Switzerland" },
        { Country.UnitedKingdom, "United Kingdom" },

        
        { Country.China, "China" },
        { Country.India, "India" },
        { Country.Indonesia, "Indonesia" },
        { Country.Japan, "Japan" },
        { Country.Malaysia, "Malaysia" },
        { Country.Philippines, "Philippines" },
        { Country.Singapore, "Singapore" },
        { Country.SouthKorea, "South Korea" },
        { Country.Taiwan, "Taiwan" },
        { Country.Thailand, "Thailand" },
        { Country.Vietnam, "Vietnam" },

        
        { Country.Australia, "Australia" },
        { Country.NewZealand, "New Zealand" },

       
        { Country.Egypt, "Egypt" },
        { Country.Kenya, "Kenya" },
        { Country.Nigeria, "Nigeria" },
        { Country.SouthAfrica, "South Africa" },

        
        { Country.Israel, "Israel" },
        { Country.SaudiArabia, "Saudi Arabia" },
        { Country.UnitedArabEmirates, "United Arab Emirates" },

        
        { Country.Other, "Other" }
    };

        /// <summary>
        /// It obtains the country's legible name.
        /// </summary>
        public static string GetDisplayName(this Country country)
        {
            return CountryNames.TryGetValue(country, out var name) ? name : country.ToString();
        }

        /// <summary>
        /// Converts a string to the corresponding Country enum.
        /// </summary>
        public static Country Parse(string countryName)
        {
            if (string.IsNullOrWhiteSpace(countryName))
                return Country.Other;

            if (Enum.TryParse<Country>(countryName.Replace(" ", ""), true, out var enumValue))
                return enumValue;

            
            var kvp = CountryNames.FirstOrDefault(x =>
                x.Value.Equals(countryName, StringComparison.OrdinalIgnoreCase));

            return kvp.Key != default ? kvp.Key : Country.Other;
        }

        /// <summary>
        /// Gets all countries as a list of tuples (enum, displayName).
        /// </summary>
        public static List<(Country Country, string DisplayName)> GetAll()
        {
            return CountryNames.Select(kvp => (kvp.Key, kvp.Value)).ToList();
        }
    }
}
