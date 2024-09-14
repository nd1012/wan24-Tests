using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using wan24.Core;

namespace wan24.Tests
{
    /// <summary>
    /// Basic tests for list implementation
    /// </summary>
    public static class ListTests
    {
        /// <summary>
        /// Run list tests
        /// </summary>
        /// <typeparam name="tList">List type (must have a constructor which takes <c>items</c>)</typeparam>
        /// <typeparam name="tItem">Item type</typeparam>
        /// <param name="items">Items (at last 2 required)</param>
        /// <returns>List</returns>
        public static tList RunTests<tList, tItem>(params tItem[] items)
            where tList : class, IList<tItem>
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(items.Length, other: 2, nameof(items));

            // Construct
            ConstructorInfoExt? ci = TypeInfoExt.From(typeof(tList))
                .GetConstructors()
                .FirstOrDefault(
                    c => !c.Constructor.IsStatic &&
                        c.ParameterCount == 1 &&
                        c.GetParameters()[0].ParameterType.GetRealType().IsAssignableFrom(typeof(tItem[]))
                );
            Assert.IsNotNull(ci, "Constructor not found");
            Assert.IsNotNull(ci.Invoker, "Constructor can't be invoked");
            tList? list = ci.Invoker([items]) as tList;
            Assert.IsNotNull(list, $"Constructor didn't construct a {typeof(tList)}");

            // Count
            Assert.AreEqual(items.Length, list.Count);
            list.Clear();
            Assert.AreEqual(0, list.Count);

            return RunTests<tList, tItem>(list, items);
        }

        /// <summary>
        /// Run list tests
        /// </summary>
        /// <typeparam name="tList">List type</typeparam>
        /// <typeparam name="tItem">Item type</typeparam>
        /// <param name="list">List (must be empty)</param>
        /// <param name="items">Items (at last 2 required)</param>
        /// <returns>List</returns>
        public static tList RunTests<tList, tItem>(tList list, params tItem[] items)
            where tList : class, IList<tItem>
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(items.Length, other: 2, nameof(items));

            // Count
            Assert.AreEqual(0, list.Count);
            for (int i = 0; i < items.Length; i++)
                list.Add(items[i]);
            Assert.AreEqual(items.Length, list.Count);

            // IndexOf
            Assert.AreEqual(0, list.IndexOf(items[0]));
            Assert.AreEqual(1, list.IndexOf(items[1]));

            // RemoveAt
            list.RemoveAt(0);
            Assert.AreEqual(-1, list.IndexOf(items[0]));
            Assert.AreEqual(0, list.IndexOf(items[1]));

            // Insert
            list.Insert(0, items[0]);
            Assert.AreEqual(0, list.IndexOf(items[0]));

            // Index access
            for (int i = 0; i < items.Length; i++)
                Assert.AreEqual(items[i], list[i], $"Value #{i} mismatch");

            // Clear
            list.Clear();
            Assert.AreEqual(0, list.Count);

            return list;
        }

        /// <summary>
        /// Run list tests
        /// </summary>
        /// <typeparam name="T">List type</typeparam>
        /// <param name="items">Items (at last 2 required)</param>
        /// <returns>List</returns>
        public static T RunTests<T>(params object?[] items)
            where T : class, IList, new()
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(items.Length, other: 2, nameof(items));

            // Construct
            T list = new();
            for (int i = 0; i < items.Length; i++)
                list.Add(items[i]);

            // Count
            Assert.AreEqual(items.Length, list.Count);
            list.Clear();
            Assert.AreEqual(0, list.Count);

            return RunTests<T>(list, items);
        }

        /// <summary>
        /// Run list tests
        /// </summary>
        /// <typeparam name="T">List type</typeparam>
        /// <param name="list">List (must be empty)</param>
        /// <param name="items">Items (at last 2 required)</param>
        /// <returns>List</returns>
        public static T RunTests<T>(T list, params object?[] items)
            where T : class, IList
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(items.Length, other: 2, nameof(items));

            // Fixed size
            Assert.IsFalse(list.IsFixedSize, "List size is fixed");

            // Read-only
            Assert.IsFalse(list.IsReadOnly, "Read-only");

            // Fill items
            Assert.AreEqual(0, list.Count);
            for (int i = 0; i < items.Length; i++)
                list.Add(items[i]);
            Assert.AreEqual(items.Length, list.Count);

            // Clear
            list.Clear();
            Assert.AreEqual(0, list.Count);

            // Index access
            for (int i = 0; i < items.Length; i++)
                list.Add(items[i]);
            for (int i = 0; i < items.Length; i++)
                Assert.AreEqual(items[i], list[i], $"Value #{i} mismatch");

            // Contains
            Assert.IsTrue(list.Contains(items[0]), "Contains failed");
            Assert.IsTrue(list.Contains(items[1]), "Contains failed 2");

            // Remove
            list.Remove(items[1]);
            Assert.IsFalse(list.Contains(items[1]), "Remove failed");
            list.Add(items[1]);


            // IndexOf
            Assert.AreEqual(0, list.IndexOf(items[0]));
            Assert.AreEqual(1, list.IndexOf(items[1]));

            // RemoveAt
            list.RemoveAt(0);
            Assert.AreEqual(-1, list.IndexOf(items[0]));
            Assert.AreEqual(0, list.IndexOf(items[1]));

            // Insert
            list.Insert(0, items[0]);
            Assert.AreEqual(0, list.IndexOf(items[0]));

            return list;
        }
    }
}
