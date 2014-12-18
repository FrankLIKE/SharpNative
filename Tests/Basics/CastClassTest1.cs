﻿//CastClass Test 1 - Passes

using System;

namespace CsNativeVisual.Tests
{

    class A
    {
        public int F() { return 4; }
        public virtual int G() { return 2; }
        public int H() { return 10; }
    }
    class B : A
    {
        public new int F() { return 3; }
        public override int G() { return 4; }
        public new int H () {return 11;}
    }
    class Test
    {
        static public void Main()
        {
            int result = 0;
            B b = new B();
            A a = b;
            if (a.F() != 1)
                result |= 1 << 0;
            Console.WriteLine(a.F());
            if (b.F() != 3)
                result |= 1 << 1;
            Console.WriteLine(b.F());
            if (b.G() != 4)
                result |= 1 << 2;
            Console.WriteLine(b.G());
            if (a.G() != 4)
                result |= 1 << 3;
            Console.WriteLine(a.G());
            if (a.H() != 10)
                result |= 1 << 4;
            Console.WriteLine(a.H());
            if (b.H() != 11)
                result |= 1 << 5;
            Console.WriteLine(b.H());
            if (((A)b).H() != 10)
                result |= 1 << 6;
            Console.WriteLine(((A)b).H());
            if (((B)a).H() != 11)
                result |= 1 << 7;
            Console.WriteLine(((B)a).H());
        }
    };
};