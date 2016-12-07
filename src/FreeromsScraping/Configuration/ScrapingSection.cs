using System.Configuration;

namespace FreeromsScraping.Configuration
{
    public class ScrapingSection : ConfigurationSection
    {
        [ConfigurationProperty("sources", IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(SourceElementCollection), AddItemName = "add", ClearItemsName = "clear", RemoveItemName = "remove")]
        public SourceElementCollection Sources => (SourceElementCollection)base["sources"];
    }
}
