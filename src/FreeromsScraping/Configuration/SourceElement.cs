using System.Configuration;

namespace FreeromsScraping.Configuration
{
    public class SourceElement : ConfigurationElement
    {
        public SourceElement()
        {
        }

        public SourceElement(string name, string url)
            : this()
        {
            Name = name;
            Url = url;
        }

        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("url", IsRequired = true)]
        public string Url
        {
            get { return (string)this["url"]; }
            set { this["url"] = value; }
        }
    }
}