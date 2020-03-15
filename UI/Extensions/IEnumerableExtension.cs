using DataLayer.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Extensions
{
    public static class IEnumerableExtension
    {
        public static IEnumerable<SelectListItem> ToSelectItem<T>(this IEnumerable<T> items, int SelectValue)
        {
            return from item in items
                   select new SelectListItem
                   {
                       Text = item.GetPropertyValue("Name"),
                       Value = item.GetPropertyValue("CategoryID"),
                       Selected = item.GetPropertyValue("CategoryID").Equals(SelectValue.ToString())
                   };
        }
    }
}
