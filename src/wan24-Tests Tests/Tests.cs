using System.Collections;
using wan24.Core;
using wan24.Tests;

namespace wan24_Tests_Tests
{
    [TestClass]
    public class Tests : TestBase
    {
        private bool Initialized = false;

        [TestMethod]
        public void General_Tests()
        {
            Assert.IsNotNull(TestsInitialization.LoggerFactory);
            Assert.IsNotNull(TestsInitialization.Options);

            Assert.IsTrue(Initialized);
            Assert.IsNotNull(TestContext);
            Assert.IsNotNull(Logger);
        }

        [TestInitialize]
        public override void InitTests()
        {
            base.InitTests();
            Initialized = true;
        }

        [TestMethod]
        public void Dictionary_Tests()
        {
            DictionaryTests.RunTests<OrderedDictionary<string, string>, string, string>(
                new KeyValuePair<string, string>("a", "a"),
                new KeyValuePair<string, string>("b", "b")
                );
            DictionaryTests.RunTests<OrderedDictionary<string, string>>(
                new KeyValuePair<object, object?>("a", "a"),
                new KeyValuePair<object, object?>("b", "b")
                );
        }

        [TestMethod]
        public void Collection_Tests()
        {
            CollectionTests.RunTests<List<string>, string>(
                "a",
                "b"
                );
            CollectionTests.RunTests<List<string>>(new List<string>()
                {
                    "a",
                    "b"
                },
                "a",
                "b"
                );
        }

        [TestMethod]
        public void List_Tests()
        {
            ListTests.RunTests<List<string>, string>(
                "a",
                "b"
                );
            ListTests.RunTests<List<string>>(
                "a",
                "b"
                );
        }

        [TestMethod]
        public void Enumerable_Tests()
        {
            EnumerableTests.RunTests<string>(
                new List<string>()
                {
                    "a",
                    "b"
                },
                canReset: true,
                count: 2
                );
            EnumerableTests.RunTests(
                (IEnumerable)new List<string>()
                {
                    "a",
                    "b"
                },
                canReset: true,
                count: 2
                );
        }

        [TestMethod]
        public async Task Enumerable_TestsAsync()
        {
            async IAsyncEnumerable<string> Enumerable()
            {
                await Task.Yield();
                yield return "a";
                yield return "b";
            };

            await EnumerableTests.RunTestsAsync<string>(
                Enumerable(),
                canReUse: true,
                count: 2
                );
        }
    }
}
