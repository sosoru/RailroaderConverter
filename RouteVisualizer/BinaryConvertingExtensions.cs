using System;
using System.Runtime.InteropServices;

namespace RouteVisualizer
{
    public static class BinaryConvertingExtensions
    {
        public static T ToObject<T>(this Array arr)
        {
            var handle = GCHandle.Alloc(arr, GCHandleType.Pinned);
            try
            {
                var ptr = Marshal.UnsafeAddrOfPinnedArrayElement(arr, 0);
                var ret = (T)Marshal.PtrToStructure(ptr, typeof(T));
                return ret;
            }
            finally
            {
                handle.Free();
            }

        }
        public static byte [] ToByteArray(this object obj)
        {
            byte[] buffer = new byte [Marshal.SizeOf(obj)];
            var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            try
            {
                // obj  will be destroyed
                Marshal.StructureToPtr(obj, handle.AddrOfPinnedObject(), true);
                return buffer;
            }
            finally
            {
                handle.Free();
            }
        }

        public static T RestoreObject<T>(this T obj, byte [] array)
        {
            var handle = GCHandle.Alloc(obj, GCHandleType.Pinned);
            try
            {
                Marshal.Copy(array, 0, handle.AddrOfPinnedObject(), array.Length);
                return obj;
            }
            finally
            {
                handle.Free();
            }
        }

    }

}