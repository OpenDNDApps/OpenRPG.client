using System.Collections.Generic;

namespace ORC
{
    public static class ScriptableDataExtensions
    {
        public static T GetByID<T>(this List<T> list, string id) where T : ScriptableData
        {
            return list.Find(item => item.ID.Equals(id));
        }
    }
}