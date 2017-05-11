using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinesense.Interfaces.Enum
{
    public class LocalisedEnum<T>
    {
        public LocalisedEnum(T e)
        {
            this.Entry = e;
        }

        public LocalisedEnum(T e, string res)
        {
            this.Entry = e;
            this.ResourceName = res;
        }

        public T Entry { get; set; }

        public string ResourceName { get; set; }

        public override string ToString()
        {
            if (EnumLocalizer.StringLocalizer == null)
                return Entry.ToString();
            else
            {
                string res = string.IsNullOrEmpty(ResourceName) ? Entry.ToString() : ResourceName;
                return EnumLocalizer.StringLocalizer(res);
            }
        }
    }
}
