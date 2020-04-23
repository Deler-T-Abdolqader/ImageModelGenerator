using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageModelGenerator.Helper
{
    public static class ListExtention
    {
        public static T Pop<T>(this List<T> list)
        {
            if (list!=null && list.Any())
            {
                var lastIndex = list.Count - 1;
                T r = list[lastIndex];
                list.RemoveAt(lastIndex);
                return r;
            }
            else
            {
                return default(T);
            }
        }
    }
}
