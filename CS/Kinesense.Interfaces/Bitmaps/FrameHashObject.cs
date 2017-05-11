using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Kinesense.Interfaces.Bitmaps
{
    public class FrameHashObject : IDisposable, ICloneable
    {
        public FrameHashObject()
        { }

        public FrameHashObject(byte[] publichash, string publichashalgoname, bool calculateperFrame)
        {
            ExpectedEncodedDataHash = publichash;
            HashAlgorithmName = publichashalgoname;
            CalculateHashPerFrame = calculateperFrame;
        }

        public FrameHashObject(byte[] publichash, byte[] privatehash, string publichashalgoname, bool calculateperFrame)
        {
            ExpectedEncodedDataHash = publichash;
            ExpectedPrivateHash = privatehash;
            HashAlgorithmName = publichashalgoname;
            CalculateHashPerFrame = calculateperFrame; 
        }

        public virtual byte[] ExpectedEncodedDataHash { get; set; }
        public virtual string HashAlgorithmName { get; set; }
        public virtual byte[] ExpectedPrivateHash { get; set; }
        public bool CalculateHashPerFrame { get; set; }

        public string SourceFilePath { get; set; }

        public byte[] ComputePrivateHash(KeyedHashAlgorithm algorithm, DateTime dbTime)
        {
            if (this.ExpectedEncodedDataHash == null)
                return new byte[0];
            string simulateWrite = dbTime.ToString(VideoFrame.TimeStampFormat);
            DateTime simulateRead = DateTime.ParseExact(simulateWrite, VideoFrame.TimeStampFormat, System.Globalization.CultureInfo.InvariantCulture);
            byte[] data = new byte[this.ExpectedEncodedDataHash.Length + 8];
            Array.Copy(this.ExpectedEncodedDataHash, data, this.ExpectedEncodedDataHash.Length);
            Array.Copy(System.BitConverter.GetBytes(simulateRead.ToBinary()), 0, data, this.ExpectedEncodedDataHash.Length, 8);
            return algorithm.ComputeHash(data);
        }

        public byte[] ComputeEncodedDataHash(IHashAlgorithmFactory hashAlgorithmFactory, byte[] encodedData)
        {
            if (string.IsNullOrEmpty(this.HashAlgorithmName))
            {
                this.HashAlgorithmName = "MD5";
                DebugMessageLogger.LogEvent("Frame HashAlgorithmName not set!");
            }
            HashAlgorithm hashAlgorithm = hashAlgorithmFactory.GetHashAlgorithm(this.HashAlgorithmName);
            return hashAlgorithm != null ? hashAlgorithm.ComputeHash(encodedData) : new byte[] { };
        }

        public static byte[] ComputePrivateHash(byte[] publichash, byte[] timesTable, KeyedHashAlgorithm algorithm)
        {
            byte[] data = new byte[publichash.Length + timesTable.Length];
            Array.Copy(publichash, data, publichash.Length);
            Array.Copy(timesTable, 0, data, publichash.Length, timesTable.Length);

            return algorithm.ComputeHash(data);
        }

        public static byte[] ComputeEncodedDataHash(HashAlgorithm hashAlgorithm, byte[] data)
        {
            //HashAlgorithm hashAlgorithm = this.HashAlgorithmFactory.GetHashAlgorithm(H264VideoEncoder.HashAlgorithmName);
            return hashAlgorithm != null ? hashAlgorithm.ComputeHash(data) : new byte[] { };
        }

        public void Dispose()
        {
            ExpectedEncodedDataHash = null;
            ExpectedPrivateHash = null;
        }

        public object Clone()
        {
            return new FrameHashObject(
                (this.ExpectedEncodedDataHash == null ? null : this.ExpectedEncodedDataHash.Clone() as byte[]),
                (this.ExpectedPrivateHash == null ? null : this.ExpectedPrivateHash.Clone() as byte[]),
                this.HashAlgorithmName,
                this.CalculateHashPerFrame)
            { SourceFilePath = this.SourceFilePath
            };
        }
    }
}
