# wan24-Tests

This library contains some test project helpers.

## Usage

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
| `Logger` | An `ILogger` instance |

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
ass the file `Initialization.cs` to your test project:

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
