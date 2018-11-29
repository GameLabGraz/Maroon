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
using System.Reflection;

/// <summary>
/// 1-Tuple or singleton implementation
/// </summary>
public class Tuple<T1> : IEquatable<Tuple<T1>>
{
    readonly T1 item1;

    public T1 Item1 { get { return item1; } }

    internal Tuple(T1 item1)
    {
        this.item1 = item1;
    }

    public override int GetHashCode()
    {
        return item1.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        return Equals((Tuple<T1>)obj);
    }

    public bool Equals(Tuple<T1> other)
    {
        return other.item1.Equals(item1);
    }
}

/// <summary>
/// 2-Tuple or pair implementation
/// </summary>
public class Tuple<T1, T2> : IEquatable<Tuple<T1, T2>>
{
    readonly T1 item1;
    readonly T2 item2;

    public T1 Item1 { get { return item1; } }
    public T2 Item2 { get { return item2; } }

    internal Tuple(T1 item1, T2 item2)
    {
        this.item1 = item1;
        this.item2 = item2;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + item1.GetHashCode();
            hash = hash * 23 + item2.GetHashCode();
            return hash;
        }
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        return Equals((Tuple<T1, T2>)obj);
    }

    public bool Equals(Tuple<T1, T2> other)
    {
        return other.item1.Equals(item1) && other.item2.Equals(item2);
    }
}

/// <summary>
/// 3-Tuple or triple implementation
/// </summary>
public class Tuple<T1, T2, T3> : IEquatable<Tuple<T1, T2, T3>>
{
    readonly T1 item1;
    readonly T2 item2;
    readonly T3 item3;

    public T1 Item1 { get { return item1; } }
    public T2 Item2 { get { return item2; } }
    public T3 Item3 { get { return item3; } }

    internal Tuple(T1 item1, T2 item2, T3 item3)
    {
        this.item1 = item1;
        this.item2 = item2;
        this.item3 = item3;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + item1.GetHashCode();
            hash = hash * 23 + item2.GetHashCode();
            hash = hash * 23 + item3.GetHashCode();
            return hash;
        };
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        return Equals((Tuple<T1, T2, T3>)obj);
    }

    public bool Equals(Tuple<T1, T2, T3> other)
    {
        return other.item1.Equals(item1) && other.item2.Equals(item2) && other.item3.Equals(item3);
    }
}

/// <summary>
/// 4-Tuple or quadruple implementation
/// </summary>
public class Tuple<T1, T2, T3, T4> : IEquatable<Tuple<T1, T2, T3, T4>>
{
    readonly T1 item1;
    readonly T2 item2;
    readonly T3 item3;
    readonly T4 item4;

    public T1 Item1 { get { return item1; } }
    public T2 Item2 { get { return item2; } }
    public T3 Item3 { get { return item3; } }
    public T4 Item4 { get { return item4; } }


    internal Tuple(T1 item1, T2 item2, T3 item3, T4 item4)
    {
        this.item1 = item1;
        this.item2 = item2;
        this.item3 = item3;
        this.item4 = item4;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + item1.GetHashCode();
            hash = hash * 23 + item2.GetHashCode();
            hash = hash * 23 + item3.GetHashCode();
            hash = hash * 23 + item4.GetHashCode();
            return hash;
        };
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        return Equals((Tuple<T1, T2, T3, T4>)obj);
    }

    public bool Equals(Tuple<T1, T2, T3, T4> other)
    {
        return other.item1.Equals(item1) && other.item2.Equals(item2) && other.item3.Equals(item3) && other.item4.Equals(item4);
    }
}

/// <summary>
/// 5-Tuple or quadruple implementation
/// </summary>
public class Tuple<T1, T2, T3, T4, T5> : IEquatable<Tuple<T1, T2, T3, T4, T5>>
{
    readonly T1 item1;
    readonly T2 item2;
    readonly T3 item3;
    readonly T4 item4;
    readonly T5 item5;

    public T1 Item1 { get { return item1; } }
    public T2 Item2 { get { return item2; } }
    public T3 Item3 { get { return item3; } }
    public T4 Item4 { get { return item4; } }
    public T5 Item5 { get { return item5; } }

    internal Tuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5)
    {
        this.item1 = item1;
        this.item2 = item2;
        this.item3 = item3;
        this.item4 = item4;
        this.item5 = item5;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + item1.GetHashCode();
            hash = hash * 23 + item2.GetHashCode();
            hash = hash * 23 + item3.GetHashCode();
            hash = hash * 23 + item4.GetHashCode();
            hash = hash * 23 + item5.GetHashCode();
            return hash;
        };
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        return Equals((Tuple<T1, T2, T3, T4, T5>)obj);
    }

    public bool Equals(Tuple<T1, T2, T3, T4, T5> other)
    {
        return other.item1.Equals(item1) && other.item2.Equals(item2) && other.item3.Equals(item3) && other.item4.Equals(item4) && other.item5.Equals(item5);
    }
}

/// <summary>
/// 6-Tuple or quadruple implementation
/// </summary>
public class Tuple<T1, T2, T3, T4, T5, T6> : IEquatable<Tuple<T1, T2, T3, T4, T5, T6>>
{
    readonly T1 item1;
    readonly T2 item2;
    readonly T3 item3;
    readonly T4 item4;
    readonly T5 item5;
    readonly T6 item6;

    public T1 Item1 { get { return item1; } }
    public T2 Item2 { get { return item2; } }
    public T3 Item3 { get { return item3; } }
    public T4 Item4 { get { return item4; } }
    public T5 Item5 { get { return item5; } }
    public T6 Item6 { get { return item6; } }

    internal Tuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6)
    {
        this.item1 = item1;
        this.item2 = item2;
        this.item3 = item3;
        this.item4 = item4;
        this.item5 = item5;
        this.item6 = item6;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + item1.GetHashCode();
            hash = hash * 23 + item2.GetHashCode();
            hash = hash * 23 + item3.GetHashCode();
            hash = hash * 23 + item4.GetHashCode();
            hash = hash * 23 + item5.GetHashCode();
            hash = hash * 23 + item6.GetHashCode();
            return hash;
        };
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        return Equals((Tuple<T1, T2, T3, T4, T5, T6>)obj);
    }

    public bool Equals(Tuple<T1, T2, T3, T4, T5, T6> other)
    {
        return other.item1.Equals(item1) && other.item2.Equals(item2) && other.item3.Equals(item3) && other.item4.Equals(item4) && other.item5.Equals(item5) && other.item6.Equals(item6);
    }
}

/// <summary>
/// 7-Tuple or quadruple implementation
/// </summary>
public class Tuple<T1, T2, T3, T4, T5, T6, T7> : IEquatable<Tuple<T1, T2, T3, T4, T5, T6, T7>>
{
    readonly T1 item1;
    readonly T2 item2;
    readonly T3 item3;
    readonly T4 item4;
    readonly T5 item5;
    readonly T6 item6;
    readonly T7 item7;

    public T1 Item1 { get { return item1; } }
    public T2 Item2 { get { return item2; } }
    public T3 Item3 { get { return item3; } }
    public T4 Item4 { get { return item4; } }
    public T5 Item5 { get { return item5; } }
    public T6 Item6 { get { return item6; } }
    public T7 Item7 { get { return item7; } }

    internal Tuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7)
    {
        this.item1 = item1;
        this.item2 = item2;
        this.item3 = item3;
        this.item4 = item4;
        this.item5 = item5;
        this.item6 = item6;
        this.item7 = item7;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + item1.GetHashCode();
            hash = hash * 23 + item2.GetHashCode();
            hash = hash * 23 + item3.GetHashCode();
            hash = hash * 23 + item4.GetHashCode();
            hash = hash * 23 + item5.GetHashCode();
            hash = hash * 23 + item6.GetHashCode();
            hash = hash * 23 + item7.GetHashCode();
            return hash;
        };
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        return Equals((Tuple<T1, T2, T3, T4, T5, T6, T7>)obj);
    }

    public bool Equals(Tuple<T1, T2, T3, T4, T5, T6, T7> other)
    {
        return other.item1.Equals(item1) && other.item2.Equals(item2) && other.item3.Equals(item3) && other.item4.Equals(item4) && other.item5.Equals(item5) && other.item6.Equals(item6) && other.item7.Equals(item7);
    }
}

/// <summary>
/// 8-Tuple or quadruple implementation
/// </summary>
public class Tuple<T1, T2, T3, T4, T5, T6, T7, T8> : IEquatable<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>>
{
    readonly T1 item1;
    readonly T2 item2;
    readonly T3 item3;
    readonly T4 item4;
    readonly T5 item5;
    readonly T6 item6;
    readonly T7 item7;
    readonly T8 item8;

    public T1 Item1 { get { return item1; } }
    public T2 Item2 { get { return item2; } }
    public T3 Item3 { get { return item3; } }
    public T4 Item4 { get { return item4; } }
    public T5 Item5 { get { return item5; } }
    public T6 Item6 { get { return item6; } }
    public T7 Item7 { get { return item7; } }
    public T8 Item8 { get { return item8; } }

    internal Tuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, T8 item8)
    {
        this.item1 = item1;
        this.item2 = item2;
        this.item3 = item3;
        this.item4 = item4;
        this.item5 = item5;
        this.item6 = item6;
        this.item7 = item7;
        this.item8 = item8;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + item1.GetHashCode();
            hash = hash * 23 + item2.GetHashCode();
            hash = hash * 23 + item3.GetHashCode();
            hash = hash * 23 + item4.GetHashCode();
            hash = hash * 23 + item5.GetHashCode();
            hash = hash * 23 + item6.GetHashCode();
            hash = hash * 23 + item7.GetHashCode();
            hash = hash * 23 + item8.GetHashCode();
            return hash;
        };
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        return Equals((Tuple<T1, T2, T3, T4, T5, T6, T7, T8>)obj);
    }

    public bool Equals(Tuple<T1, T2, T3, T4, T5, T6, T7, T8> other)
    {
        return other.item1.Equals(item1) && other.item2.Equals(item2) && other.item3.Equals(item3) && other.item4.Equals(item4) && other.item5.Equals(item5) && other.item6.Equals(item6) && other.item7.Equals(item7) && other.item8.Equals(item8);
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