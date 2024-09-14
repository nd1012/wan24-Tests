using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using wan24.Core;

namespace wan24.Tests
{
    /// <summary>
    /// Basic tests for enumerable implementation
    /// </summary>
    public static class EnumerableTests
    {
        /// <summary>
        /// Run enumerable tests
        /// </summary>
        /// <typeparam name="T">Item type</typeparam>
        /// <param name="enumerable">Enumerable (shouldn't contain <see langword="null"/>)</param>
        /// <param name="canReset">If an enumerator can reset</param>
        /// <param name="count">Expected count</param>
        /// <returns>Enumerable</returns>
        public static IEnumerable<T> RunTests<T>(IEnumerable<T> enumerable, bool canReset, int count)
        {
            if (canReset)
            {
                // Get an array
                T[] arr = [.. enumerable];

                // Count
                Assert.AreEqual(count, arr.Length);
            }

            // Enumerate
            using IEnumerator<T> enumerator = enumerable.GetEnumerator();
            for(int i = 0; i < count; i++)
            {
                Assert.IsTrue(enumerator.MoveNext(), $"Move to #{i} failed");
                Assert.IsNotNull(enumerator.Current);
            }
            Assert.IsFalse(enumerator.MoveNext(), "Move next at the end");

            if (canReset)
            {
                // Reset
                enumerator.Reset();
                for (int i = 0; i < count; i++)
                {
                    Assert.IsTrue(enumerator.MoveNext(), $"Move to #{i} failed");
                    Assert.IsNotNull(enumerator.Current);
                }
                Assert.IsFalse(enumerator.MoveNext(), "Move next at the end");
            }

            return enumerable;
        }

        /// <summary>
        /// Run enumerable tests
        /// </summary>
        /// <param name="enumerable">Enumerable (shouldn't contain <see langword="null"/>)</param>
        /// <param name="canReset">If an enumerator can reset</param>
        /// <param name="count">Expected count</param>
        /// <returns>Enumerable</returns>
        public static IEnumerable RunTests(IEnumerable enumerable, bool canReset, int count)
        {
            if (canReset)
            {
                // Get an array
                object?[] arr = [.. enumerable];

                // Count
                Assert.AreEqual(count, arr.Length);
            }

            // Enumerate
            IEnumerator enumerator = enumerable.GetEnumerator();
            try
            {
                for (int i = 0; i < count; i++)
                {
                    Assert.IsTrue(enumerator.MoveNext(), $"Move to #{i} failed");
                    Assert.IsNotNull(enumerator.Current);
                }
                Assert.IsFalse(enumerator.MoveNext(), "Move next at the end");

                if (canReset)
                {
                    // Reset
                    enumerator.Reset();
                    for (int i = 0; i < count; i++)
                    {
                        Assert.IsTrue(enumerator.MoveNext(), $"Move to #{i} failed");
                        Assert.IsNotNull(enumerator.Current);
                    }
                    Assert.IsFalse(enumerator.MoveNext(), "Move next at the end");
                }
            }
            finally
            {
                enumerator.TryDispose();
            }

            return enumerable;
        }

        /// <summary>
        /// Run enumerable tests
        /// </summary>
        /// <typeparam name="T">Item type</typeparam>
        /// <param name="enumerable">Enumerable (shouldn't contain <see langword="null"/>)</param>
        /// <param name="canReUse">If die enumerable can be re-used</param>
        /// <param name="count">Expected count</param>
        /// <returns>Enumerable</returns>
        public static async Task<IAsyncEnumerable<T>> RunTestsAsync<T>(IAsyncEnumerable<T> enumerable, bool canReUse, int count)
        {
            if (canReUse)
            {
                // Get an array
                T[] arr = await enumerable.ToArrayAsync();

                // Count
                Assert.AreEqual(count, arr.Length);
            }

            // Enumerate
            IAsyncEnumerator<T> enumerator = enumerable.GetAsyncEnumerator();
            await using (enumerator)
            {
                for (int i = 0; i < count; i++)
                {
                    Assert.IsTrue(await enumerator.MoveNextAsync(), $"Move to #{i} failed");
                    Assert.IsNotNull(enumerator.Current);
                }
                Assert.IsFalse(await enumerator.MoveNextAsync(), "Move next at the end");
            }
            return enumerable;
        }
    }
}
