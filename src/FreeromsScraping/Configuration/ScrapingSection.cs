using System.Configuration;

namespace FreeromsScraping.Configuration
{
    public class ScrapingSection : ConfigurationSection
    {
        [ConfigurationProperty("destinationFolder", IsRequired = true)]
        public string DestinationFolder
        {
            get { return (string)this["destinationFolder"]; }
            set { this["destinationFolder"] = value; }
        }

        [ConfigurationProperty("sources", IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(SourceElementCollection), AddItemName = "add", ClearItemsName = "clear", RemoveItemName = "remove")]
        public SourceElementCollection Sources => (SourceElementCollection)base["sources"];
    }
}
