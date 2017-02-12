/*
 * Created by SharpDevelop.
 * User: GONCARJ3
 * Date: 12/02/2017
 * Time: 04:05
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;

public class NumericListComparer<T> : IComparer<T>
{
    static readonly NumericComparer _innerComparer = new NumericComparer();

    public int Compare(T x, T y)
    {
        return _innerComparer.Compare(x, y); // I'm guessing this is how NumericComparer works
    }
}
