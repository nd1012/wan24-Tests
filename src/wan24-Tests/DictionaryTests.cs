using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using wan24.Core;

namespace wan24.Tests
{
    /// <summary>
    /// Basic tests for dictionary implementations
    /// </summary>
    public static class DictionaryTests
    {
        /// <summary>
        /// Run dictionary tests
        /// </summary>
        /// <typeparam name="tDict">Dictionary type (must have a constructor which can take <c>items</c>)</typeparam>
        /// <typeparam name="tKey">Key type</typeparam>
        /// <typeparam name="tValue">Value type</typeparam>
        /// <param name="items">Items (2 required, values should differ and not contain <see langword="null"/>)</param>
        /// <returns>Dictionary</returns>
        public static tDict RunTests<tDict, tKey, tValue>(params KeyValuePair<tKey, tValue>[] items)
            where tDict : class, IDictionary<tKey, tValue>
            where tKey : notnull
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(items.Length, other: 2, nameof(items));

            // Construct
            ConstructorInfoExt? ci = TypeInfoExt.From(typeof(tDict))
                .GetConstructors()
                .FirstOrDefault(
                    c => !c.Constructor.IsStatic &&
                        c.ParameterCount == 1 &&
                        c.GetParameters()[0].ParameterType.GetRealType().IsAssignableFrom(typeof(KeyValuePair<tKey, tValue>[]))
                );
            Assert.IsNotNull(ci, "Constructor not found");
            Assert.IsNotNull(ci.Invoker, "Constructor can't be invoked");
            tDict? dict = ci.Invoker([items]) as tDict;
            Assert.IsNotNull(dict, $"Constructor didn't construct a {typeof(tDict)}");

            // Count
            Assert.AreEqual(items.Length, dict.Count);
            dict.Clear();
            Assert.AreEqual(0, dict.Count);

            return RunTests<tDict, tKey, tValue>(dict, items);
        }

        /// <summary>
        /// Run dictionary tests
        /// </summary>
        /// <typeparam name="tDict">Dictionary type</typeparam>
        /// <typeparam name="tKey">Key type</typeparam>
        /// <typeparam name="tValue">Value type</typeparam>
        /// <param name="dict">Dictionary (must be empty)</param>
        /// <param name="items">Items (2 required, values should differ and not contain <see langword="null"/>)</param>
        /// <returns>Dictionary</returns>
        public static tDict RunTests<tDict, tKey, tValue>(tDict dict, params KeyValuePair<tKey, tValue>[] items)
            where tDict : class, IDictionary<tKey, tValue>
            where tKey : notnull
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(items.Length, other: 2, nameof(items));

            // Fill items
            Assert.AreEqual(0, dict.Count);
            for (int i = 0; i < items.Length; i++)
                dict[items[i].Key] = items[i].Value;

            // Keys/values
            Assert.IsTrue(items.Select(i => i.Key).OrderBy(k => k.GetHashCode()).SequenceEqual(dict.Keys.OrderBy(k => k.GetHashCode())));
            Assert.IsTrue(items.Select(i => i.Value).OrderBy(v => v?.GetHashCode() ?? 0).SequenceEqual(dict.Values.OrderBy(v => v?.GetHashCode() ?? 0)));

            // Indexed access
            Assert.AreEqual(items[0].Value, dict[items[0].Key], "Key/value mismatch");
            Assert.AreNotEqual(items[0].Value, dict[items[1].Key], "Key/value mismatch 2");

            // Add double key
            try
            {
                dict.Add(items[0].Key, items[0].Value);
                Assert.Fail("Double keys possible");
            }
            catch
            {
            }

            // Ensure all items exist and values are equal
            for (int i = 0; i < items.Length; i++)
            {
                Assert.IsTrue(dict.ContainsKey(items[i].Key), $"Key #{i} not found (ContainsKey)");
                Assert.AreEqual(items[i].Value, dict[items[i].Key], $"Value #{i} mismatch");
            }
            for (int i = 0; i < items.Length; i++)
                Assert.IsTrue(dict.ContainsValue(items[i].Value), $"Value #{i} not found (ContainsValue)");
            for (int i = 0; i < items.Length; i++)
                Assert.IsTrue(dict.Contains(items[i]), $"Key/value #{i} not found (Contains)");

            // Remove
            Assert.IsTrue(dict.Remove(items[0].Key), "Remove failed");
            Assert.IsFalse(dict.ContainsKey(items[0].Key), "Key not removed");
            Assert.IsFalse(dict.ContainsValue(items[0].Value), "Value not removed");
            Assert.IsFalse(dict.Contains(items[0]), "Key/value not removed");

            // Add
            dict.Add(items[0].Key, items[0].Value);
            Assert.IsTrue(dict.ContainsKey(items[0].Key), "Key not added");
            Assert.IsTrue(dict.ContainsValue(items[0].Value), "Value not added");
            Assert.IsTrue(dict.Contains(items[0]), "Key/value not added");
            Assert.IsTrue(dict.Remove(items[0].Key), "Remove failed 2");

            // Add key/value
            dict.Add(items[0]);
            Assert.IsTrue(dict.ContainsKey(items[0].Key), "Key not added 2");
            Assert.IsTrue(dict.ContainsValue(items[0].Value), "Value not added 2");
            Assert.IsTrue(dict.Contains(items[0]), "Key/value not added 2");

            // Clear
            dict.Clear();
            Assert.AreEqual(0, dict.Count);
            Assert.IsFalse(dict.ContainsKey(items[0].Key), "Key not cleared");
            Assert.IsFalse(dict.ContainsValue(items[0].Value), "Value not cleared");
            Assert.IsFalse(dict.Contains(items[0]), "Key/value not cleared");

            // TryGetValue
            dict.AddRange(items);
            Assert.AreEqual(items.Length, dict.Count);
            Assert.IsTrue(dict.TryGetValue(items[0].Key, out tValue? value), "TryGetValue failed");
            Assert.AreEqual(value, items[0].Value, "TryGetValue value mismatch");
            Assert.IsTrue(dict.Remove(items[0].Key), "Remove failed 3");
            Assert.IsFalse(dict.TryGetValue(items[0].Key, out value), "TryGetValue failed 2");
            Assert.IsNull(value, "TryGetValue value is not NULL");

            // Enumerator
            using IEnumerator<KeyValuePair<tKey, tValue>> enumerator = dict.GetEnumerator();
            for (int i = 0; i < items.Length - 1; i++)
            {
                Assert.IsTrue(enumerator.MoveNext(), $"No {i + 1}st/nd/th item");
                Assert.IsNotNull(enumerator.Current, $"{i + 1}st/nd/th item is NULL");
            }
            Assert.IsFalse(enumerator.MoveNext(), "Has 3rd item");

            return dict;
        }

        /// <summary>
        /// Run dictionary tests
        /// </summary>
        /// <typeparam name="T">Dictionary type</typeparam>
        /// <param name="items">Items (2 required, values should differ and not contain <see langword="null"/>)</param>
        /// <returns>Dictionary</returns>
        public static T RunTests<T>(params KeyValuePair<object, object?>[] items)
            where T : class, IDictionary, new()
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(items.Length, other: 2, nameof(items));

            // Construct
            T dict = new();
            for (int i = 0; i < items.Length; i++)
                dict[items[i].Key] = items[i].Value;

            // Count
            Assert.AreEqual(items.Length, dict.Count);
            dict.Clear();
            Assert.AreEqual(0, dict.Count);

            return RunTests<T>(dict, items);
        }

        /// <summary>
        /// Run dictionary tests
        /// </summary>
        /// <typeparam name="T">Dictionary type</typeparam>
        /// <param name="dict">Dictionary (must be empty)</param>
        /// <param name="items">Items (2 required, values should differ and not contain <see langword="null"/>)</param>
        /// <returns>Dictionary</returns>
        public static T RunTests<T>(T dict, params KeyValuePair<object, object?>[] items)
            where T : class, IDictionary
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(items.Length, other: 2, nameof(items));

            // Fill items
            Assert.AreEqual(0, dict.Count);
            for (int i = 0; i < items.Length; i++)
                dict[items[i].Key] = items[i].Value;

            // Ensure not read-only and not fixed size
            Assert.IsFalse(dict.IsReadOnly, "Writable dictionary expected");
            Assert.IsFalse(dict.IsFixedSize, "Is fixed size");

            // Keys/values
            List<object> keys = new(dict.Count);
            keys.AddRange((IEnumerable<object>)dict.Keys);
            Assert.IsTrue(items.Select(i => i.Key).OrderBy(k => k.GetHashCode()).SequenceEqual(keys.OrderBy(k => k.GetHashCode())));
            List<object?> values = new(dict.Count);
            values.AddRange((IEnumerable<object?>)dict.Values);
            Assert.IsTrue(items.Select(i => i.Value).OrderBy(v => v?.GetHashCode() ?? 0).SequenceEqual(values.OrderBy(v => v?.GetHashCode() ?? 0)));

            // Indexed access
            Assert.AreEqual(items[0].Value, dict[items[0].Key], "Key/value mismatch");
            Assert.AreNotEqual(items[0].Value, dict[items[1].Key], "Key/value mismatch 2");

            // Add double key
            try
            {
                dict.Add(items[0].Key, items[0].Value);
                Assert.Fail("Double keys possible");
            }
            catch
            {
            }

            // Ensure all items exist
            for (int i = 0; i < items.Length; i++)
            {
                Assert.IsTrue(dict.Contains(items[i].Key), $"Key #{i} not found (Contains)");
                Assert.AreEqual(items[i].Value, dict[items[i].Key], $"Value #{i} mismatch");
            }

            // Remove
            dict.Remove(items[0].Key);
            Assert.IsFalse(dict.Contains(items[0].Key), "Key not removed");

            // Add
            dict.Add(items[0].Key, items[0].Value);
            Assert.IsTrue(dict.Contains(items[0].Key), "Key not added");
            dict.Remove(items[0].Key);

            // Enumerator
            IEnumerator enumerator = dict.GetEnumerator();
            try
            {
                for (int i = 0; i < items.Length - 1; i++)
                {
                    Assert.IsTrue(enumerator.MoveNext(), $"No {i + 1}st/nd/th item");
                    Assert.IsNotNull(enumerator.Current, $"{i + 1}st/nd/th item is NULL");
                }
                Assert.IsFalse(enumerator.MoveNext(), "Has 3rd item");
            }
            finally
            {
                enumerator.TryDispose();
            }

            // Clear
            dict.Clear();
            Assert.AreEqual(0, dict.Count);
            Assert.IsFalse(dict.Contains(items[0].Key), "Key not cleared");

            return dict;
        }
    }
}
