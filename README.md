# CoroutinesService

## Overview
A service for registering the playbacks of coroutines on a separate empty game object. 

The idea was to keep all active coroutines in one place, allowing them to be viewed in a custom editor tool. For this purpose, a wrapping class was made - `CoroutinePlayback`. This also made it possible to add some statistics and a label to the started coroutine. Later, the ability to attach callbacks in different contexts of stopping the coroutine was introduced.

## Install
You can download a unity package from [the latest release](../../releases).

## Usage
Samples are provided in the package. 

`CoroutinesService` is a simple class without inheritance from `MonoBehaviour`. The task of introducing this dependency into your project (by using *Singleton*, *DI*, etc.) is your concern; the samples provide an example of creating and using a service instance.

The `CoroutinesService` has two main methods for starting/stopping coroutines:
```cs
CoroutinePlaybackInfo PlayCoroutine(string coroutineDebugName, Func<IEnumerator> coroutineCallback,
    Func<bool> checkCallback, CoroutinePlaybackData data = default);

void StopCoroutine(CoroutinePlaybackInfo? coroutinePlaybackInfo);
```

To start a coroutine, you need to specify a label (string), a coroutine method, and a validation method.

** Validation Method **

The validation method is designed to stop the coroutine in the player loop cycle if the method returns `false`. For example, check that the game object is still alive or that the `Boolean` variable is still set to `true`. If you want to omit the verification method, you can make an anonymous method that returns `true`.

```cs
coroutinesService.PlayCoroutine("SAMPLE_LABEL", SampleCoroutineMethod, () => true);
...

private IEnumerator SampleCoroutineMethod() {
    yield return new WaitForEndOfFrame();
}
```

If you need to be able to manually stop a running coroutine at a specific moment, then cache returned `CoroutinePlaybackInfo` instance in a variable and pass it to the `StopCoroutine` method.

```cs
private CoroutinePlaybackInfo? examplePlaybackInfo;
...

coroutinesService.StopCoroutine(examplePlaybackInfo);
```

It is more convenient to mark a variable as *nullable*. The service is responsible for checking that the value is not `null` and that the coroutine can be stopped.

** Stopping Callbacks **
An optional parameter is the `CoroutinePlaybackData` structure, which provides fields with callbacks for different coroutine stopping contexts:
* `onBreakCallback` - Is called if a validation method fails (returns `false`).
* `onFinishedCallback` - Is called only when the coroutine finishes execution without breaking/stopping.
* `onEndCallback` - Is called whenever coroutine stops/breakes/finishes.

## Last Notes
This service became unnecessary as soon as the need for more complex asynchronous interactions arose. This functionality is provided by Task-s. For Unity, the supported analogue is UniTask-s.
