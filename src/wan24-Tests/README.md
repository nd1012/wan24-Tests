# wan24-Tests

This library contains some test project helpers.

## Usage

### How to get it

This package is available as 
[NuGet package `wan24-Tests`](https://www.nuget.org/packages/wan24-Tests/).

### Add options using an assembly attribute

Create the file `Attributes.cs` with this content:

```cs
using wan24.Tests;

[assembly: TestsOptions(...)]
```

You can set several options using the attribute properties. You may also 
implement a custom attribute which extends `TestsOptions` and may override any 
event handling method:

| Method | Description |
| ------ | ----------- |
| `OnBeforeInitialization` | Run before global initialization |
| `OnAfterInitialization` | Run after global initialization |
| `OnBeforeTestsInitialization` | Run before tests initialization |
| `OnBeforeTestsInitialization` | Run before tests initialization |

### Tests initialization

The tests initialization does the following for you:

- Configure `wan24-Core` NuGet package logging (using a log file and the 
console)
- Configure the MS test project logging
- Logging background errors
- Logging object validation errors of the `ObjectValidation` NuGet package
- Optional enable creating a construction stack information for disposable 
types which extend `wan24.Core.Disposable(Record)Base`
- Booting `wan24-Core`

It needs to run before any tests are being executed. To achive this, please 
add the file `Initialization.cs` to your test project:

```cs
namespace wan24.Tests
{
    [TestClass]
    public class Initialization
    {
        [AssemblyInitialize]
        public static void Init(TestContext tc) => TestsInitialization.Init(tc);
    }
}
```

The `TestsInitialization` type exports some static properties:

| Property | Description |
| -------- | ----------- |
| `LoggerFactory` | A logger factory |
| `Options` | The used tests options (`TestsOptionsAttribute`) |

### Using `TestBase` as base class for a test

Example test:

```cs
[TestClass]
public class YourTests : TestBase
{
	...
}
```

The `TestBase` will log the currently running test, which makes it more easy 
to see where tests failed, if you're running them from  the CLI. For running 
initialization code after the `TestOptionsAttribute` event handlers you may 
override the `InitTests` method:

```cs
[TestInitialize]
public override void InitTests()
{
	base.InitTests();
	// Your tests initialization code here
}
```

The `TestBase` type defines these properties:

| Property | Description |
| -------- | ----------- |
| `TestContext` | The current test context |
| `Logger` | An `ILogger` instance, using `Logging.Logger` or a new created one from `TestsInitialization.LoggerFactory` |

### Interface testing

There are some static test methods for types which implement interfaces:

| Interface | Test method |
| --------- | ----------- |
| `IDictionary(<tKey, tValue>)` | `DictionaryTests.RunTests` |
| `ICollection(<T>)` | `CollectionTests.RunTests` |
| `IList(<T>)` | `ListTests.RunTests` |
| `I(Async)Enumerable(<T>)` | `EnumerableTests.RunTests(Async)` |

Since it's impossible to define a fixed test data set which works well for all 
types which implement those interfaces (-> but we never know _how_ a type 
implemented an interface _for which data type_), those tests do only cover a 
very basic set of functional tests, and you'll have to provide a test data set 
for each method (see `Tests.cs` in the source code). Example:

```cs
DictionaryTests.RunTests<OrderedDictionary<string, string>, string, string>(
    new KeyValuePair<string, string>("a", "a"),
    new KeyValuePair<string, string>("b", "b")
    );
```

In case the tested type can't be constructed from the static test methods, 
there are overloads which accept an existing instance. A test data set must 
contain at last two items, while an existing instance may contain test data 
or not - that depends on the interface (please see the DocComments for each 
test methods requirements).

More detailed tests need to be implemented by yourself. However, I'm open for 
adding more basic tests, if they could be applied to all types which implement 
an interface. Don't hesitate to contribute.
