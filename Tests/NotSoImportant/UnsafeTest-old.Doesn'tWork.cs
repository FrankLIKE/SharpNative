using System;
using System.Runtime.InteropServices;

namespace CSharpPlayground
{


    //Tests abstract classes, abstract and overidden properties too
//Fix is supposed to be 
/*     union {
    public long Bgra;


    struct {
    public byte B;
    public byte G;
    public byte R;
    public byte A;
    }
}
*/

    [StructLayout(LayoutKind.Explicit)]
    public struct ColorBgra
    {
        [FieldOffset(0)]
        public byte B;

        [FieldOffset(1)]
        public byte G;

        [FieldOffset(2)]
        public byte R;

        [FieldOffset(3)]
        public byte A;

        /// <summary>
        /// Lets you change B, G, R, and A at the same time.
        /// </summary>
        [FieldOffset(0)]
        public uint Bgra;

        public  override    string  ToString()
        {
            return "["+B+","+G+","+R+","+A+"]";
        }
    }

    unsafe class TestShapes
    {
        public static ColorBgra Apply(ColorBgra lhs, ColorBgra rhs)
        {
            var result = new ColorBgra
            {
                Bgra = 16777215
            };
            return result;
        }

        public static unsafe void Apply(ColorBgra* dst, ColorBgra* lhs, ColorBgra* rhs, int length)
        {
            while (length > 0)
            {
                *dst = Apply(*lhs, *rhs);
                ++dst;
                ++lhs;
                ++rhs;
                --length;
            }
        }

        private static void Main()
        {
            var sourcePixels = new ColorBgra[1024];
            var destPixels = new ColorBgra[1024];
            var targetPixels = new ColorBgra[1024];
            fixed (ColorBgra* src = sourcePixels)
            fixed (ColorBgra* dst = destPixels)
            fixed (ColorBgra* target = targetPixels)
            {
                Apply(src, dst, target, 1024);

            }

            for(int i=0;i<1024;i++)
            {
                Console.WriteLine(i+": s:"+ sourcePixels[i] + "d:" + destPixels[i]+ "t:"+ targetPixels[i]);
            }
        }
    }
}
