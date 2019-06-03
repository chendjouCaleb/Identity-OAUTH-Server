using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace Everest.Identity.Core.Infrastructure
{
    public class MapConfiguration : IConfiguration
    {
        protected Dictionary<String, string> Items = new Dictionary<string, string>();
        public string this[string key]
        {
            get => Items[key];
            set => Items[key] = value;
        }

        public virtual IEnumerable<IConfigurationSection> GetChildren()
        {
            throw new NotImplementedException();
        }

        public virtual IChangeToken GetReloadToken()
        {
            throw new NotImplementedException();
        }

        public virtual IConfigurationSection GetSection(string key)
        {
            throw new NotImplementedException();
        }
    }
}
