using System;
using System.Collections;
using System.Collections.Generic;
using VGDevs;
using UnityEngine;
using Object = System.Object;

public static class ListExtensions
{
    public static bool AddUnique<T>(this List<T> collection, T item)
    {
        if (collection.Contains(item))
            return false;

        collection.Add(item);
        return true;
    }
}
