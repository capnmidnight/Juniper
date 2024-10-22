/************************************************************************************
Copyright : Copyright (c) Facebook Technologies, LLC and its affiliates. All rights reserved.

Licensed under the Oculus Utilities SDK License Version 1.31 (the "License"); you may not use
the Utilities SDK except in compliance with the License, which is provided at the time of installation
or download, or which otherwise accompanies this software in either electronic or hard copy form.

You may obtain a copy of the License at
https://developer.oculus.com/licenses/utilities-1.31

Unless required by applicable law or agreed to in writing, the Utilities SDK distributed
under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
ANY KIND, either express or implied. See the License for the specific language governing
permissions and limitations under the License.
************************************************************************************/

using System;
using System.Runtime.InteropServices;

namespace Oculus.VR
{
    /// <summary>
    /// Encapsulates an 8-byte-aligned of unmanaged memory.
    /// </summary>
    public class NativeBuffer : IDisposable
    {
        private bool disposed = false;
        private int m_numBytes = 0;
        private IntPtr m_ptr = IntPtr.Zero;

        /// <summary>
        /// Creates a buffer of the specified size.
        /// </summary>
        public NativeBuffer(int numBytes)
        {
            Reallocate(numBytes);
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the <see cref="NativeBuffer"/> is
        /// reclaimed by garbage collection.
        /// </summary>
        ~NativeBuffer()
        {
            Dispose(false);
        }

        /// <summary>
        /// Reallocates the buffer with the specified new size.
        /// </summary>
        public void Reset(int numBytes)
        {
            Reallocate(numBytes);
        }

        /// <summary>
        /// The current number of bytes in the buffer.
        /// </summary>
        public int GetCapacity()
        {
            return m_numBytes;
        }

        /// <summary>
        /// A pointer to the unmanaged memory in the buffer, starting at the given offset in bytes.
        /// </summary>
        public IntPtr GetPointer(int byteOffset = 0)
        {
            if (byteOffset < 0 || byteOffset >= m_numBytes)
            {
                return IntPtr.Zero;
            }

            return (byteOffset == 0) ? m_ptr : new IntPtr(m_ptr.ToInt64() + byteOffset);
        }

        /// <summary>
        /// Releases all resource used by the <see cref="NativeBuffer"/> object.
        /// </summary>
        /// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="NativeBuffer"/>. The <see cref="Dispose"/>
        /// method leaves the <see cref="NativeBuffer"/> in an unusable state. After calling <see cref="Dispose"/>, you must
        /// release all references to the <see cref="NativeBuffer"/> so the garbage collector can reclaim the memory that
        /// the <see cref="NativeBuffer"/> was occupying.</remarks>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                // dispose managed resources
            }

            // dispose unmanaged resources
            Release();

            disposed = true;
        }

        private void Reallocate(int numBytes)
        {
            Release();

            if (numBytes > 0)
            {
                m_ptr = Marshal.AllocHGlobal(numBytes);
                m_numBytes = numBytes;
            }
            else
            {
                m_ptr = IntPtr.Zero;
                m_numBytes = 0;
            }
        }

        private void Release()
        {
            if (m_ptr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(m_ptr);
                m_ptr = IntPtr.Zero;
                m_numBytes = 0;
            }
        }
    }
}