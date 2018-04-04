using System.Collections.Generic;
using System.Configuration;

namespace HyperVAdmin.Utilities
{
    public class UserConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("", IsRequired = false, IsDefaultCollection = true)]
        public UserCollection Users
        {
            get { return (UserCollection)this[""]; }
            set { this[""] = value; }
        }
    }

    public class UserCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement() =>
            new UserInstanceElement();

        protected override object GetElementKey(ConfigurationElement element) =>
            ((UserInstanceElement)element).Username;
    }

    public class UserInstanceElement : ConfigurationElement
    {
        [ConfigurationProperty("username", IsKey = true, IsRequired = true)]
        public string Username
        {
            get { return (string)base["username"]; }
            set { base["username"] = value; }
        }

        [ConfigurationProperty("password", IsRequired = true)]
        public string Password
        {
            get { return (string)base["password"]; }
            set { base["password"] = value; }
        }

        [ConfigurationProperty("displayname", IsRequired = true)]
        public string Displayname
        {
            get
            {
                return base["displayname"] != null ? (string)base["displayname"] : (string)base["username"];
            }
            set { base["displayname"] = value; }
        }
    }

    public static class UserConfig
    {
        public static Dictionary<string, UserInstanceElement> UsersCollection = new Dictionary<string, UserInstanceElement>();

        static UserConfig()
        {
            UserConfigSection section = (UserConfigSection)ConfigurationManager.GetSection("userConfiguration");

            foreach (UserInstanceElement i in section.Users)
            {
                UsersCollection.Add(i.Username, i);
            }
        }

        public static UserInstanceElement Users(string user) =>
            UsersCollection.ContainsKey(user) ? UsersCollection[user] : null;
    }
}