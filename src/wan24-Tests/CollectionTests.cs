using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using wan24.Core;

namespace wan24.Tests
{
    /// <summary>
    /// Basic tests for collection implementation
    /// </summary>
    public static class CollectionTests
    {
        /// <summary>
        /// Run collection tests
        /// </summary>
        /// <typeparam name="tCollection">Collection type (must have a constructor which takes <c>items</c>)</typeparam>
        /// <typeparam name="tItem">Item type</typeparam>
        /// <param name="items">Items (at last 2 required)</param>
        /// <returns>Collection</returns>
        public static tCollection RunTests<tCollection, tItem>(params tItem[] items)
            where tCollection : class, ICollection<tItem>
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(items.Length, other: 2, nameof(items));

            // Construct
            ConstructorInfoExt? ci = TypeInfoExt.From(typeof(tCollection))
                .GetConstructors()
                .FirstOrDefault(
                    c => !c.Constructor.IsStatic &&
                        c.ParameterCount == 1 &&
                        c.GetParameters()[0].ParameterType.GetRealType().IsAssignableFrom(typeof(tItem[]))
                );
            Assert.IsNotNull(ci, "Constructor not found");
            Assert.IsNotNull(ci.Invoker, "Constructor can't be invoked");
            tCollection? collection = ci.Invoker([items]) as tCollection;
            Assert.IsNotNull(collection, $"Constructor didn't construct a {typeof(tCollection)}");

            // Count
            Assert.AreEqual(items.Length, collection.Count);
            collection.Clear();
            Assert.AreEqual(0, collection.Count);

            return RunTests<tCollection, tItem>(collection, items);
        }

        /// <summary>
        /// Run collection tests
        /// </summary>
        /// <typeparam name="tCollection">Collection type</typeparam>
        /// <typeparam name="tItem">Item type</typeparam>
        /// <param name="collection">Collection (must be empty)</param>
        /// <param name="items">Items (at last 2 required)</param>
        /// <returns>Collection</returns>
        public static tCollection RunTests<tCollection, tItem>(tCollection collection, params tItem[] items)
            where tCollection : class, ICollection<tItem>
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(items.Length, other: 2, nameof(items));

            // Ensure not read-only
            Assert.IsFalse(collection.IsReadOnly, "Read-only");

            // Fill items
            Assert.AreEqual(0, collection.Count);
            for (int i = 0; i < items.Length; i++)
                collection.Add(items[i]);
            Assert.AreEqual(items.Length, collection.Count);

            // Remove
            Assert.IsTrue(collection.Remove(items[0]), "Remove failed");
            Assert.AreEqual(items.Length - 1, collection.Count);

            // Contains
            Assert.IsTrue(collection.Contains(items[1]), "Contains failed");
            Assert.IsFalse(collection.Contains(items[0]), "Contains failed 2");

            // Copy
            tItem[] arr = new tItem[items.Length];
            collection.CopyTo(arr, arrayIndex: 1);
            Assert.IsNull(arr[0]);
            Assert.AreEqual(items[1], arr[1]);

            // Clear
            collection.Clear();
            Assert.AreEqual(0, collection.Count);

            return collection;
        }

        /// <summary>
        /// Run collection tests
        /// </summary>
        /// <typeparam name="T">Collection type (must have a constructor which takes <c>items</c>)</typeparam>
        /// <param name="items">Items (at last 2 required)</param>
        /// <returns>Collection</returns>
        public static T RunTests<T>(params object?[] items)
            where T : class, ICollection
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(items.Length, other: 2, nameof(items));

            // Construct
            ConstructorInfoExt? ci = TypeInfoExt.From(typeof(T))
                .GetConstructors()
                .FirstOrDefault(
                    c => !c.Constructor.IsStatic &&
                        c.ParameterCount == 1 &&
                        c.GetParameters()[0].ParameterType.GetRealType().IsAssignableFrom(typeof(object?[]))
                );
            Assert.IsNotNull(ci, "Constructor not found");
            Assert.IsNotNull(ci.Invoker, "Constructor can't be invoked");
            T? collection = ci.Invoker([items]) as T;
            Assert.IsNotNull(collection, $"Constructor didn't construct a {typeof(T)}");

            return RunTests<T>(collection, items);
        }

        /// <summary>
        /// Run collection tests
        /// </summary>
        /// <typeparam name="T">Collection type</typeparam>
        /// <param name="collection">Collection (must contain <c>items</c>)</param>
        /// <param name="items">Items (at last 2 required)</param>
        /// <returns>Collection</returns>
        public static T RunTests<T>(T collection, params object?[] items)
            where T : class, ICollection
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(items.Length, other: 2, nameof(items));

            // Properties
            _ = collection.IsSynchronized;
            _ = collection.SyncRoot;

            // Count
            Assert.AreEqual(items.Length, collection.Count);

            // Copy
            object?[] arr = new object?[items.Length + 1];
            collection.CopyTo(arr, index: 1);
            Assert.IsNull(arr[0]);
            Assert.AreEqual(items[0], arr[1]);

            return collection;
        }
    }
}
