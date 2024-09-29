using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Cryptography;
using wan24.Core;

namespace wan24.Tests
{
    /// <summary>
    /// Basic tests for a <see cref="Stream"/>
    /// </summary>
    public class StreamTests
    {
        /// <summary>
        /// Test data
        /// </summary>
        private static readonly byte[] TestData = RandomNumberGenerator.GetBytes(200000);

        /// <summary>
        /// Synchronous stream testing
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>Stream</returns>
        public static T RunSyncTests<T>(T stream) where T : Stream
        {
            stream.WriteByte(0);
            stream.Write(TestData);
            stream.Position = 0;
            using MemoryStream ms = new();
            stream.CopyTo(ms);
            byte[] data = ms.ToArray();
            Assert.AreEqual((byte)0, data[0]);
            Assert.IsTrue(data.Skip(1).SequenceEqual(TestData));

            stream.Write(TestData);
            stream.Position = TestData.Length + 1;
            ms.SetLength(0);
            stream.CopyExactlyPartialTo(ms, 200000);
            data = ms.ToArray();
            Assert.IsTrue(data.SequenceEqual(TestData));

            stream.SetLength(TestData.Length + 1);
            stream.Seek(1, SeekOrigin.Begin);
            Assert.AreEqual(TestData.Length, stream.Read(data));
            Assert.IsTrue(data.SequenceEqual(TestData));

            stream.Position = 0;
            Assert.AreEqual(0, stream.ReadByte());

            return stream;
        }

        /// <summary>
        /// Asynchronous stream testing
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>Stream</returns>
        public static async Task<T> RunAsyncTests<T>(T stream) where T : Stream
        {
            stream.WriteByte(0);
            await stream.WriteAsync(TestData);
            stream.Position = 0;
            using MemoryStream ms = new();
            await stream.CopyToAsync(ms);
            byte[] data = ms.ToArray();
            Assert.AreEqual((byte)0, data[0]);
            Assert.IsTrue(data.Skip(1).SequenceEqual(TestData));

            await stream.WriteAsync(TestData);
            stream.Position = TestData.Length + 1;
            ms.SetLength(0);
            await stream.CopyExactlyPartialToAsync(ms, 200000);
            data = ms.ToArray();
            Assert.IsTrue(data.SequenceEqual(TestData));

            stream.SetLength(TestData.Length + 1);
            stream.Seek(1, SeekOrigin.Begin);
            Assert.AreEqual(TestData.Length, await stream.ReadAsync(data));
            Assert.IsTrue(data.SequenceEqual(TestData));

            stream.Position = 0;
            Assert.AreEqual(0, stream.ReadByte());

            return stream;
        }
    }
}
