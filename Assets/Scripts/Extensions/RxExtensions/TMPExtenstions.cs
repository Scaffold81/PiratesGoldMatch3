using System;
using System.Collections.Generic;
using TMPro;

namespace Core.Extensions
{
    public static class TMPExtenstions
    {
        public static void SetOptionsFromEnum<T>(this TMP_Dropdown dropdown) where T : Enum
        {
            var enumValues = Enum.GetValues(typeof(T));
            var enumDescriptions = new List<string>();
            foreach (T v in enumValues)
            {
                enumDescriptions.Add(v.GetDescription());
            }

            dropdown.options.Clear();
            foreach (var description in enumDescriptions)
            {
                var data = new TMP_Dropdown.OptionData(description);
                dropdown.options.Add(data);
            }
        }
    }
}
