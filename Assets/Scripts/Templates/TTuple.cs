//-----------------------------------------------------------------------------
// TTuple.cs
//
// Class for Tuple Implementation
//
//
// Authors: Michael Stefan Holly
//          Michael Schiller
//          Christopher Schinnerl
//-----------------------------------------------------------------------------
//

using System;
using UnityEngine;

/// <summary>
/// 1-Tuple or singleton implementation
/// </summary>
public class Tuple<T1>
{
    public T1 item1 { get; private set; }
    internal Tuple(T1 it1)
    {
        item1 = it1;
    }
}

/// <summary>
/// 2-Tuple or pair implementation
/// </summary>
public class Tuple<T1, T2>
{
    public T1 item1 { get; private set; }
    public T2 item2 { get; private set; }
    internal Tuple(T1 it1, T2 it2)
    {
        item1 = it1;
        item2 = it2;
    }
}

/// <summary>
/// 3-Tuple or triple implementation
/// </summary>
public class Tuple<T1, T2, T3>
{
    public T1 item1 { get; private set; }
    public T2 item2 { get; private set; }
    public T3 item3 { get; private set; }
    internal Tuple(T1 it1, T2 it2, T3 it3)
    {
        item1 = it1;
        item2 = it2;
        item3 = it3;
    }
}

/// <summary>
/// 4-Tuple or quadruple implementation
/// </summary>
public class Tuple<T1, T2, T3, T4>
{
    public T1 item1 { get; private set; }
    public T2 item2 { get; private set; }
    public T3 item3 { get; private set; }
    public T4 item4 { get; private set; }
    internal Tuple(T1 it1, T2 it2, T3 it3, T4 it4)
    {
        item1 = it1;
        item2 = it2;
        item3 = it3;
        item4 = it4;
    }
}

/// <summary>
/// 5-Tuple or quadruple implementation
/// </summary>
public class Tuple<T1, T2, T3, T4, T5>
{
    public T1 item1 { get; private set; }
    public T2 item2 { get; private set; }
    public T3 item3 { get; private set; }
    public T4 item4 { get; private set; }
    public T5 item5 { get; private set; }
    internal Tuple(T1 it1, T2 it2, T3 it3, T4 it4, T5 it5)
    {
        item1 = it1;
        item2 = it2;
        item3 = it3;
        item4 = it4;
        item5 = it5;
    }
}

/// <summary>
/// 6-Tuple or quadruple implementation
/// </summary>
public class Tuple<T1, T2, T3, T4, T5, T6>
{
    public T1 item1 { get; private set; }
    public T2 item2 { get; private set; }
    public T3 item3 { get; private set; }
    public T4 item4 { get; private set; }
    public T5 item5 { get; private set; }
    public T6 item6 { get; private set; }
    internal Tuple(T1 it1, T2 it2, T3 it3, T4 it4, T5 it5, T6 it6)
    {
        item1 = it1;
        item2 = it2;
        item3 = it3;
        item4 = it4;
        item5 = it5;
        item6 = it6;
    }
}

/// <summary>
/// 7-Tuple or quadruple implementation
/// </summary>
public class Tuple<T1, T2, T3, T4, T5, T6, T7>
{
    public T1 item1 { get; private set; }
    public T2 item2 { get; private set; }
    public T3 item3 { get; private set; }
    public T4 item4 { get; private set; }
    public T5 item5 { get; private set; }
    public T6 item6 { get; private set; }
    public T7 item7 { get; private set; }
    internal Tuple(T1 it1, T2 it2, T3 it3, T4 it4, T5 it5, T6 it6, T7 it7)
    {
        item1 = it1;
        item2 = it2;
        item3 = it3;
        item4 = it4;
        item5 = it5;
        item6 = it6;
        item7 = it7;
    }
}

/// <summary>
/// 8-Tuple or quadruple implementation
/// </summary>
public class Tuple<T1, T2, T3, T4, T5, T6, T7, T8>
{
    public T1 item1 { get; private set; }
    public T2 item2 { get; private set; }
    public T3 item3 { get; private set; }
    public T4 item4 { get; private set; }
    public T5 item5 { get; private set; }
    public T6 item6 { get; private set; }
    public T7 item7 { get; private set; }
    public T8 item8 { get; private set; }
    internal Tuple(T1 it1, T2 it2, T3 it3, T4 it4, T5 it5, T6 it6, T7 it7, T8 it8)
    {
        item1 = it1;
        item2 = it2;
        item3 = it3;
        item4 = it4;
        item5 = it5;
        item6 = it6;
        item7 = it7;
        item8 = it8;
    }
}

/// <summary>
/// Static Tuple class that implements Tuple.Create(...)
/// </summary>
public static class Tuple
{
    /// <summary>
    /// 1-Tuple Create implementation
    /// </summary>
    /// <returns>1-Tuple</returns>
	public static Tuple<T1> Create<T1>(T1 it1)
    {
        return new Tuple<T1>(it1);
    }

    /// <summary>
    /// 2-Tuple Create implementation
    /// </summary>
    /// <returns>1-Tuple</returns>
    public static Tuple<T1, T2> Create<T1, T2>(T1 it1, T2 it2)
    {
        return new Tuple<T1, T2>(it1, it2);
    }

    /// <summary>
    /// 3-Tuple Create implementation
    /// </summary>
    /// <returns>1-Tuple</returns>
    public static Tuple<T1, T2, T3> Create<T1, T2, T3>(T1 it1, T2 it2, T3 it3)
    {
        return new Tuple<T1, T2, T3>(it1, it2, it3);
    }

    /// <summary>
    /// 4-Tuple Create implementation
    /// </summary>
    /// <returns>1-Tuple</returns>
    public static Tuple<T1, T2, T3, T4> Create<T1, T2, T3, T4>(T1 it1, T2 it2, T3 it3, T4 it4)
    {
        return new Tuple<T1, T2, T3, T4>(it1, it2, it3, it4);
    }

    /// <summary>
    /// 5-Tuple Create implementation
    /// </summary>
    /// <returns>1-Tuple</returns>
    public static Tuple<T1, T2, T3, T4, T5> Create<T1, T2, T3, T4, T5>(T1 it1, T2 it2, T3 it3, T4 it4, T5 it5)
    {
        return new Tuple<T1, T2, T3, T4, T5>(it1, it2, it3, it4, it5);
    }

    /// <summary>
    /// 6-Tuple Create implementation
    /// </summary>
    /// <returns>1-Tuple</returns>
    public static Tuple<T1, T2, T3, T4, T5, T6> Create<T1, T2, T3, T4, T5, T6>(T1 it1, T2 it2, T3 it3, T4 it4, T5 it5, T6 it6)
    {
        return new Tuple<T1, T2, T3, T4, T5, T6>(it1, it2, it3, it4, it5, it6);
    }

    /// <summary>
    /// 7-Tuple Create implementation
    /// </summary>
    /// <returns>1-Tuple</returns>
    public static Tuple<T1, T2, T3, T4, T5, T6, T7> Create<T1, T2, T3, T4, T5, T6, T7>(T1 it1, T2 it2, T3 it3, T4 it4, T5 it5, T6 it6, T7 it7)
    {
        return new Tuple<T1, T2, T3, T4, T5, T6, T7>(it1, it2, it3, it4, it5, it6, it7);
    }

    /// <summary>
    /// 8-Tuple Create implementation
    /// </summary>
    /// <returns>1-Tuple</returns>
    public static Tuple<T1, T2, T3, T4, T5, T6, T7, T8> Create<T1, T2, T3, T4, T5, T6, T7, T8>(T1 it1, T2 it2, T3 it3, T4 it4, T5 it5, T6 it6, T7 it7, T8 it8)
    {
        return new Tuple<T1, T2, T3, T4, T5, T6, T7, T8>(it1, it2, it3, it4, it5, it6, it7, it8);
    }
}