using System.Collections.Generic;
using System;
using System.Linq;

public static class ListExtension
{
    private static System.Random rng = new System.Random();

    /// <summary>
    /// Shuffles the specified list.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list">The list.</param>
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    /// <summary>
    /// Clone the specified list.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="listToClone"></param>
    /// <returns></returns>
    public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
    {
        return listToClone.Select(item => (T)item.Clone()).ToList();
    }
}
