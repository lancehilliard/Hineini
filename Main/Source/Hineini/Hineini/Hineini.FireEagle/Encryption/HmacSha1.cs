using System;
using System.IO;
using System.Security.Cryptography;

/*
 * NOTE: The Compact Framework doesn't have keyed hashing build in, and none of the 
 * HMACSHA1 implementations I found work correctly with .NET CF (issues with key generation).
 * 
 * This is a direct copy of the .Net 3.5 cryptography from Reflector.
 */

namespace System.Security.Cryptography {
    public abstract class HashAlgorithm : IDisposable {
        protected int HashSizeValue;
        protected internal byte[] HashValue;
        private bool m_bDisposed;
        protected int State;

        public virtual bool CanReuseTransform {
            get { return true; }
        }

        public virtual bool CanTransformMultipleBlocks {
            get { return true; }
        }

        public virtual byte[] Hash {
            get { return (byte[])HashValue.Clone(); }
        }

        public virtual int HashSize {
            get { return HashSizeValue; }
        }

        public virtual int InputBlockSize {
            get { return 1; }
        }

        public virtual int OutputBlockSize {
            get { return 1; }
        }

        #region IDisposable Members

        void IDisposable.Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        public void Clear() {
            ((IDisposable)this).Dispose();
        }

        public byte[] ComputeHash(byte[] buffer) {
            HashCore(buffer, 0, buffer.Length);
            HashValue = HashFinal();
            byte[] buffer2 = (byte[])HashValue.Clone();
            Initialize();
            return buffer2;
        }

        public byte[] ComputeHash(Stream inputStream) {
            int num;

            byte[] buffer = new byte[0x1000];
            do {
                num = inputStream.Read(buffer, 0, 0x1000);
                if (num > 0) {
                    HashCore(buffer, 0, num);
                }
            } while (num > 0);
            HashValue = HashFinal();
            byte[] buffer2 = (byte[])HashValue.Clone();
            Initialize();
            return buffer2;
        }

        public byte[] ComputeHash(byte[] buffer, int offset, int count) {
            HashCore(buffer, offset, count);
            HashValue = HashFinal();
            byte[] buffer2 = (byte[])HashValue.Clone();
            Initialize();
            return buffer2;
        }

        public static HashAlgorithm Create() {
            return Create("System.Security.Cryptography.HashAlgorithm");
        }

        public static HashAlgorithm Create(string hashName) {
            return (HashAlgorithm)CryptoConfig.CreateFromName(hashName);
        }

        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                if (HashValue != null) {
                    Array.Clear(HashValue, 0, HashValue.Length);
                }
                HashValue = null;
                m_bDisposed = true;
            }
        }

        protected abstract void HashCore(byte[] array, int ibStart, int cbSize);

        protected abstract byte[] HashFinal();

        public abstract void Initialize();

        public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset) {
            State = 1;
            HashCore(inputBuffer, inputOffset, inputCount);
            if ((outputBuffer != null) && ((inputBuffer != outputBuffer) || (inputOffset != outputOffset))) {
                Buffer.BlockCopy(inputBuffer, inputOffset, outputBuffer, outputOffset, inputCount);
            }
            return inputCount;
        }

        public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount) {
            HashCore(inputBuffer, inputOffset, inputCount);
            HashValue = HashFinal();
            byte[] dst = new byte[inputCount];
            if (inputCount != 0) {
                Buffer.BlockCopy(inputBuffer, inputOffset, dst, 0, inputCount);
            }
            State = 0;
            return dst;
        }
    }

    public abstract class KeyedHashAlgorithm : HashAlgorithm {
        protected byte[] KeyValue;

        public virtual byte[] Key {
            get { return (byte[])KeyValue.Clone(); }
            set { KeyValue = (byte[])value.Clone(); }
        }

        public static KeyedHashAlgorithm Create() {
            return Create("System.Security.Cryptography.KeyedHashAlgorithm");
        }

        public static KeyedHashAlgorithm Create(string algName) {
            return (KeyedHashAlgorithm)CryptoConfig.CreateFromName(algName);
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                if (KeyValue != null) {
                    Array.Clear(KeyValue, 0, KeyValue.Length);
                }
                KeyValue = null;
            }
            base.Dispose(disposing);
        }
    }

    public abstract class HMAC : KeyedHashAlgorithm {
        private int blockSizeValue = 0x40;
        internal HashAlgorithm m_hash1;
        internal HashAlgorithm m_hash2;
        private bool m_hashing;
        internal string m_hashName;
        private byte[] m_inner;
        private byte[] m_outer;

        protected int BlockSizeValue {
            get { return blockSizeValue; }
            set { blockSizeValue = value; }
        }

        public string HashName {
            get { return m_hashName; }
            set {
                m_hashName = value;
                m_hash1 = HashAlgorithm.Create(m_hashName);
                m_hash2 = HashAlgorithm.Create(m_hashName);
            }
        }

        public override byte[] Key {
            get { return (byte[])base.KeyValue.Clone(); }
            set { InitializeKey(value); }
        }

        public static HMAC Create() {
            return Create("System.Security.Cryptography.HMAC");
        }

        public static HMAC Create(string algorithmName) {
            return (HMAC)CryptoConfig.CreateFromName(algorithmName);
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                if (m_hash1 != null) {
                    m_hash1.Clear();
                }
                if (m_hash2 != null) {
                    m_hash2.Clear();
                }
                if (m_inner != null) {
                    Array.Clear(m_inner, 0, m_inner.Length);
                }
                if (m_outer != null) {
                    Array.Clear(m_outer, 0, m_outer.Length);
                }
            }
            base.Dispose(disposing);
        }

        protected override void HashCore(byte[] rgb, int ib, int cb) {
            if (!m_hashing) {
                m_hash1.TransformBlock(m_inner, 0, m_inner.Length, m_inner, 0);
                m_hashing = true;
            }
            m_hash1.TransformBlock(rgb, ib, cb, rgb, ib);
        }

        protected override byte[] HashFinal() {
            if (!m_hashing) {
                m_hash1.TransformBlock(m_inner, 0, m_inner.Length, m_inner, 0);
                m_hashing = true;
            }
            m_hash1.TransformFinalBlock(new byte[0], 0, 0);
            byte[] hashValue = m_hash1.HashValue;
            m_hash2.TransformBlock(m_outer, 0, m_outer.Length, m_outer, 0);
            m_hash2.TransformBlock(hashValue, 0, hashValue.Length, hashValue, 0);
            m_hashing = false;
            m_hash2.TransformFinalBlock(new byte[0], 0, 0);
            return m_hash2.HashValue;
        }

        public override void Initialize() {
            m_hash1.Initialize();
            m_hash2.Initialize();
            m_hashing = false;
        }

        internal void InitializeKey(byte[] key) {
            m_inner = null;
            m_outer = null;
            if (key.Length > BlockSizeValue) {
                base.KeyValue = m_hash1.ComputeHash(key);
            }
            else {
                base.KeyValue = (byte[])key.Clone();
            }
            UpdateIOPadBuffers();
        }

        private void UpdateIOPadBuffers() {
            int num;
            if (m_inner == null) {
                m_inner = new byte[BlockSizeValue];
            }
            if (m_outer == null) {
                m_outer = new byte[BlockSizeValue];
            }
            for (num = 0; num < BlockSizeValue; num++) {
                m_inner[num] = 0x36;
                m_outer[num] = 0x5c;
            }
            for (num = 0; num < base.KeyValue.Length; num++) {
                m_inner[num] = (byte)(m_inner[num] ^ base.KeyValue[num]);
                m_outer[num] = (byte)(m_outer[num] ^ base.KeyValue[num]);
            }
        }

        // Properties
    }

    public class HmacSha1 : HMAC {
        // Methods
        public HmacSha1()
            : this(null) { }

        public HmacSha1(byte[] key)
            : this(key, false) { }

        public HmacSha1(byte[] key, bool useManagedSha1) {
            if (key == null) {
                key = new byte[64];

                Random random = new Random();
                random.NextBytes(key);
            }

            base.m_hashName = "SHA1";
            base.m_hash1 = new SHA1Managed();
            base.m_hash2 = new SHA1Managed();
            base.HashSizeValue = 160;
            base.InitializeKey(key);
        }
    }

    public class SHA1Managed : SHA1 {
        private readonly byte[] _buffer;
        private readonly uint[] _expandedBuffer;
        private readonly uint[] _stateSHA1;
        private long _count;

        public SHA1Managed() {
            _stateSHA1 = new uint[5];
            _buffer = new byte[0x40];
            _expandedBuffer = new uint[80];
            InitializeState();
        }

        private byte[] _EndHash() {
            byte[] block = new byte[20];
            int num = 0x40 - ((int)(_count & 0x3fL));
            if (num <= 8) {
                num += 0x40;
            }
            byte[] partIn = new byte[num];
            partIn[0] = 0x80;
            long num2 = _count * 8L;
            partIn[num - 8] = (byte)((num2 >> 0x38) & 0xffL);
            partIn[num - 7] = (byte)((num2 >> 0x30) & 0xffL);
            partIn[num - 6] = (byte)((num2 >> 40) & 0xffL);
            partIn[num - 5] = (byte)((num2 >> 0x20) & 0xffL);
            partIn[num - 4] = (byte)((num2 >> 0x18) & 0xffL);
            partIn[num - 3] = (byte)((num2 >> 0x10) & 0xffL);
            partIn[num - 2] = (byte)((num2 >> 8) & 0xffL);
            partIn[num - 1] = (byte)(num2 & 0xffL);
            _HashData(partIn, 0, partIn.Length);
            DWORDToBigEndian(block, _stateSHA1, 5);
            base.HashValue = block;
            return block;
        }

        internal static void DWORDToBigEndian(byte[] block, uint[] x, int digits) {
            int index = 0;
            for (int i = 0; index < digits; i += 4) {
                block[i] = (byte)((x[index] >> 0x18) & 0xff);
                block[i + 1] = (byte)((x[index] >> 0x10) & 0xff);
                block[i + 2] = (byte)((x[index] >> 8) & 0xff);
                block[i + 3] = (byte)(x[index] & 0xff);
                index++;
            }
        }

        internal static unsafe void DWORDFromBigEndian(uint* x, int digits, byte* block) {
            int index = 0;
            for (int i = 0; index < digits; i += 4) {
                x[index] = (uint)((((block[i] << 0x18) | (block[i + 1] << 0x10)) | (block[i + 2] << 8)) | block[i + 3]);
                index++;
            }
        }

        private unsafe void _HashData(byte[] partIn, int ibStart, int cbSize) {
            int count = cbSize;
            int srcOffset = ibStart;
            int dstOffset = (int)(_count & 0x3fL);
            _count += count;
            fixed (uint* numRef = _stateSHA1) {
                fixed (byte* numRef2 = _buffer) {
                    fixed (uint* numRef3 = _expandedBuffer) {
                        if ((dstOffset > 0) && ((dstOffset + count) >= 0x40)) {
                            Buffer.BlockCopy(partIn, srcOffset, _buffer, dstOffset, 0x40 - dstOffset);
                            srcOffset += 0x40 - dstOffset;
                            count -= 0x40 - dstOffset;
                            SHATransform(numRef3, numRef, numRef2);
                            dstOffset = 0;
                        }
                        while (count >= 0x40) {
                            Buffer.BlockCopy(partIn, srcOffset, _buffer, 0, 0x40);
                            srcOffset += 0x40;
                            count -= 0x40;
                            SHATransform(numRef3, numRef, numRef2);
                        }
                        if (count > 0) {
                            Buffer.BlockCopy(partIn, srcOffset, _buffer, dstOffset, count);
                        }
                    }
                }
            }
        }

        protected override void HashCore(byte[] rgb, int ibStart, int cbSize) {
            _HashData(rgb, ibStart, cbSize);
        }

        protected override byte[] HashFinal() {
            return _EndHash();
        }

        public override void Initialize() {
            InitializeState();
            Array.Clear(_buffer, 0, _buffer.Length);
            Array.Clear(_expandedBuffer, 0, _expandedBuffer.Length);
        }

        private void InitializeState() {
            _count = 0L;
            _stateSHA1[0] = 0x67452301;
            _stateSHA1[1] = 0xefcdab89;
            _stateSHA1[2] = 0x98badcfe;
            _stateSHA1[3] = 0x10325476;
            _stateSHA1[4] = 0xc3d2e1f0;
        }

        private static unsafe void SHAExpand(uint* x) {
            for (int i = 0x10; i < 80; i++) {
                uint num2 = ((x[i - 3] ^ x[i - 8]) ^ x[i - 14]) ^ x[i - 0x10];
                x[i] = (num2 << 1) | (num2 >> 0x1f);
            }
        }

        private static unsafe void SHATransform(uint* expandedBuffer, uint* state, byte* block) {
            uint num = state[0];
            uint num2 = state[1];
            uint num3 = state[2];
            uint num4 = state[3];
            uint num5 = state[4];
            DWORDFromBigEndian(expandedBuffer, 0x10, block);
            SHAExpand(expandedBuffer);
            int index = 0;
            while (index < 20) {
                num5 += ((((num << 5) | (num >> 0x1b)) + (num4 ^ (num2 & (num3 ^ num4)))) + expandedBuffer[index]) + 0x5a827999;
                num2 = (num2 << 30) | (num2 >> 2);
                num4 += ((((num5 << 5) | (num5 >> 0x1b)) + (num3 ^ (num & (num2 ^ num3)))) + expandedBuffer[index + 1]) + 0x5a827999;
                num = (num << 30) | (num >> 2);
                num3 += ((((num4 << 5) | (num4 >> 0x1b)) + (num2 ^ (num5 & (num ^ num2)))) + expandedBuffer[index + 2]) + 0x5a827999;
                num5 = (num5 << 30) | (num5 >> 2);
                num2 += ((((num3 << 5) | (num3 >> 0x1b)) + (num ^ (num4 & (num5 ^ num)))) + expandedBuffer[index + 3]) + 0x5a827999;
                num4 = (num4 << 30) | (num4 >> 2);
                num += ((((num2 << 5) | (num2 >> 0x1b)) + (num5 ^ (num3 & (num4 ^ num5)))) + expandedBuffer[index + 4]) + 0x5a827999;
                num3 = (num3 << 30) | (num3 >> 2);
                index += 5;
            }
            while (index < 40) {
                num5 += ((((num << 5) | (num >> 0x1b)) + ((num2 ^ num3) ^ num4)) + expandedBuffer[index]) + 0x6ed9eba1;
                num2 = (num2 << 30) | (num2 >> 2);
                num4 += ((((num5 << 5) | (num5 >> 0x1b)) + ((num ^ num2) ^ num3)) + expandedBuffer[index + 1]) + 0x6ed9eba1;
                num = (num << 30) | (num >> 2);
                num3 += ((((num4 << 5) | (num4 >> 0x1b)) + ((num5 ^ num) ^ num2)) + expandedBuffer[index + 2]) + 0x6ed9eba1;
                num5 = (num5 << 30) | (num5 >> 2);
                num2 += ((((num3 << 5) | (num3 >> 0x1b)) + ((num4 ^ num5) ^ num)) + expandedBuffer[index + 3]) + 0x6ed9eba1;
                num4 = (num4 << 30) | (num4 >> 2);
                num += ((((num2 << 5) | (num2 >> 0x1b)) + ((num3 ^ num4) ^ num5)) + expandedBuffer[index + 4]) + 0x6ed9eba1;
                num3 = (num3 << 30) | (num3 >> 2);
                index += 5;
            }
            while (index < 60) {
                num5 += ((((num << 5) | (num >> 0x1b)) + ((num2 & num3) | (num4 & (num2 | num3)))) + expandedBuffer[index]) + 0x8f1bbcdc;
                num2 = (num2 << 30) | (num2 >> 2);
                num4 += ((((num5 << 5) | (num5 >> 0x1b)) + ((num & num2) | (num3 & (num | num2)))) + expandedBuffer[index + 1]) + 0x8f1bbcdc;
                num = (num << 30) | (num >> 2);
                num3 += ((((num4 << 5) | (num4 >> 0x1b)) + ((num5 & num) | (num2 & (num5 | num)))) + expandedBuffer[index + 2]) + 0x8f1bbcdc;
                num5 = (num5 << 30) | (num5 >> 2);
                num2 += ((((num3 << 5) | (num3 >> 0x1b)) + ((num4 & num5) | (num & (num4 | num5)))) + expandedBuffer[index + 3]) + 0x8f1bbcdc;
                num4 = (num4 << 30) | (num4 >> 2);
                num += ((((num2 << 5) | (num2 >> 0x1b)) + ((num3 & num4) | (num5 & (num3 | num4)))) + expandedBuffer[index + 4]) + 0x8f1bbcdc;
                num3 = (num3 << 30) | (num3 >> 2);
                index += 5;
            }
            while (index < 80) {
                num5 += ((((num << 5) | (num >> 0x1b)) + ((num2 ^ num3) ^ num4)) + expandedBuffer[index]) + 0xca62c1d6;
                num2 = (num2 << 30) | (num2 >> 2);
                num4 += ((((num5 << 5) | (num5 >> 0x1b)) + ((num ^ num2) ^ num3)) + expandedBuffer[index + 1]) + 0xca62c1d6;
                num = (num << 30) | (num >> 2);
                num3 += ((((num4 << 5) | (num4 >> 0x1b)) + ((num5 ^ num) ^ num2)) + expandedBuffer[index + 2]) + 0xca62c1d6;
                num5 = (num5 << 30) | (num5 >> 2);
                num2 += ((((num3 << 5) | (num3 >> 0x1b)) + ((num4 ^ num5) ^ num)) + expandedBuffer[index + 3]) + 0xca62c1d6;
                num4 = (num4 << 30) | (num4 >> 2);
                num += ((((num2 << 5) | (num2 >> 0x1b)) + ((num3 ^ num4) ^ num5)) + expandedBuffer[index + 4]) + 0xca62c1d6;
                num3 = (num3 << 30) | (num3 >> 2);
                index += 5;
            }
            state[0] += num;
            uint* numPtr1 = state + 1;
            numPtr1[0] += num2;
            uint* numPtr2 = state + 2;
            numPtr2[0] += num3;
            uint* numPtr3 = state + 3;
            numPtr3[0] += num4;
            uint* numPtr4 = state + 4;
            numPtr4[0] += num5;
        }
    }

    public abstract class SHA1 : HashAlgorithm {
        // Methods
        protected SHA1() {
            base.HashSizeValue = 160;
        }

        public static SHA1 Create() {
            return Create("System.Security.Cryptography.SHA1");
        }

        public static SHA1 Create(string hashName) {
            return (SHA1)CryptoConfig.CreateFromName(hashName);
        }
    }
}