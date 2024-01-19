using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extensions
{
    public static class CloneExtensions
    {

        public static T? Clone<T>(this T obj)
        {
            if (obj is null) return default(T);

            var newRecord = Activator.CreateInstance<T>();

            foreach(var prpInfo in newRecord!.GetType().GetProperties().Where(prpInfo => prpInfo.CanWrite))
            {
                prpInfo.SetValue(newRecord, obj.GetType().GetProperty(prpInfo.Name)!.GetValue(obj, null));
            }

            return newRecord;
        }

    }
}
