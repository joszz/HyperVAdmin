using System.Collections.Generic;
using System.Configuration;

namespace HyperVAdmin.ConfigurationSections
{
    public class WeblinkConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("", IsRequired = false, IsDefaultCollection = true)]
        public WeblinkCollection Weblinks
        {
            get { return (WeblinkCollection)this[""]; }
            set { this[""] = value; }
        }
    }

    public class WeblinkCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement() =>
            new WeblinkInstanceElement();

        protected override object GetElementKey(ConfigurationElement element) =>
            ((WeblinkInstanceElement)element).Name;
    }

    public class WeblinkInstanceElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return (string)base["name"]; }
            set { base["name"] = value; }
        }

        [ConfigurationProperty("url", IsRequired = true)]
        public string Url
        {
            get { return (string)base["url"]; }
            set { base["url"] = value; }
        }

        [ConfigurationProperty("target", IsRequired = false)]
        public string Target
        {
            get
            {
                return base["target"] != null ? (string)base["target"] : "";
            }
            set { base["target"] = value; }
        }
    }

    public static class WeblinkConfig
    {
        public static Dictionary<string, WeblinkInstanceElement> WeblinksCollection = new Dictionary<string, WeblinkInstanceElement>();

        static WeblinkConfig()
        {
            WeblinkConfigSection section = (WeblinkConfigSection)ConfigurationManager.GetSection("weblinks");

            foreach (WeblinkInstanceElement link in section.Weblinks)
            {
                WeblinksCollection.Add(link.Name, link);
            }
        }

        public static WeblinkInstanceElement Link(string link) =>
            WeblinksCollection.ContainsKey(link) ? WeblinksCollection[link] : null;
    }
}