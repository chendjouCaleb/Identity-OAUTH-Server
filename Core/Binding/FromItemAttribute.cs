using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;

namespace Everest.Identity.Core.Binding
{
    public class FromItem : Attribute, IBindingSourceMetadata
    {
        public FromItem(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
        public BindingSource BindingSource => new BindingSource("FromItem", "FromItem", true, false);
    }
}
