/*
 * FILE: PriorityQueue__test.cs
 * AUTHOR: Sean T. McBeth
 * DATE: JAN-24-2007
 */

using System;
using System.Collections;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Juniper.Collections.Tests
{

    internal class MockComparer : IComparer<int>
    {
        public int Compare(int obj1, int obj2)
        {
            return 0;
        }
    }
}
