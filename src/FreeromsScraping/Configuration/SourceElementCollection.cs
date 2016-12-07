using System.Configuration;

namespace FreeromsScraping.Configuration
{
    public class SourceElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new SourceElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((SourceElement)element).Name;
        }
    }
}