using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomCoroutines {
    public interface ICoroutinesService {
        /// <summary>
        /// Gets all coroutine playbacks.
        /// </summary>
        /// <returns>All executing coroutine playbacks.</returns>
        List<CoroutinePlaybackInfo> GetAllCoroutinePlaybacks();
        
        /// <summary>
        /// Registers new coroutine playback.
        /// Stops previous one if it's not null.
        /// </summary>
        /// <param name="coroutinePlaybackInfo">Previous coroutine playback. Can be null.</param>
        /// <param name="coroutineDebugName">Name for coroutine that will be displayed in debug menu.</param>
        /// <param name="coroutineCallback">Method that contains coroutine logic.</param>
        /// <param name="checkCallback">Callback that will stop playback if returns false. For example, if game object or component was destroyed (() => this).</param>
        /// <param name="data">Optional settings.</param>
        /// <returns>Coroutine playback info.</returns>
        CoroutinePlaybackInfo PlayCoroutine(CoroutinePlaybackInfo? coroutinePlaybackInfo, string coroutineDebugName, Func<IEnumerator> coroutineCallback, Func<bool> checkCallback, CoroutinePlaybackData data = default);
        
        /// <summary>
        /// Registers new coroutine playback.
        /// </summary>
        /// <param name="coroutineDebugName">Name for coroutine that will be displayed in debug menu.</param>
        /// <param name="coroutineCallback">Method that contains coroutine logic.</param>
        /// <param name="checkCallback">Callback that will stop playback if returns false. For example, if game object or component was destroyed (() => this).</param>
        /// <param name="data">Optional settings.</param>
        /// <returns>Coroutine playback info.</returns>
        CoroutinePlaybackInfo PlayCoroutine(string coroutineDebugName, Func<IEnumerator> coroutineCallback, Func<bool> checkCallback, CoroutinePlaybackData data = default);
        
        /// <summary>
        /// Stops coroutine's playback. Ignores null object.
        /// </summary>
        /// <param name="coroutinePlaybackInfo">Coroutine playback. Can be null.</param>
        void StopCoroutine(CoroutinePlaybackInfo? coroutinePlaybackInfo);
    }

    public class CoroutinesService : ICoroutinesService {
        private readonly List<CoroutinePlaybackInfo> coroutinePlaybacks = new();
        private readonly MonoBehaviour globalCoroutineParent;

        public CoroutinesService() {
            globalCoroutineParent = new GameObject("GLOBAL_COROUTINE_PARENT").AddComponent<EmptyMonoBehaviour>();
        }

        public List<CoroutinePlaybackInfo> GetAllCoroutinePlaybacks() => coroutinePlaybacks;

        public CoroutinePlaybackInfo PlayCoroutine(CoroutinePlaybackInfo? coroutinePlaybackInfo, string coroutineDebugName, Func<IEnumerator> coroutineCallback, Func<bool> checkCallback, CoroutinePlaybackData data = default) {
            if (coroutinePlaybackInfo != null) {
                StopCoroutine(coroutinePlaybackInfo);
            }
            
            return PlayCoroutine(coroutineDebugName, coroutineCallback, checkCallback, data);
        }
        
        public CoroutinePlaybackInfo PlayCoroutine(string coroutineDebugName, Func<IEnumerator> coroutineCallback, Func<bool> checkCallback, CoroutinePlaybackData data = default) {
            var coroutinePlayback = new CoroutinePlayback(coroutineDebugName, coroutineCallback, checkCallback, data);
            var coroutinePlaybackInfo = new CoroutinePlaybackInfo(coroutinePlayback);
            coroutinePlayback.Executed += () => StopCoroutine(coroutinePlaybackInfo);
            
            coroutinePlaybacks.Add(coroutinePlaybackInfo);
            coroutinePlayback.StartCoroutine(globalCoroutineParent);
            return coroutinePlaybackInfo;
        }

        public void StopCoroutine(CoroutinePlaybackInfo? coroutinePlaybackInfo) {
            if (coroutinePlaybackInfo == null) {
                return;
            }
            
            coroutinePlaybackInfo.Value.coroutinePlayback.StopCoroutine(globalCoroutineParent);
            
            if (coroutinePlaybacks.Contains(coroutinePlaybackInfo.Value)) {
                coroutinePlaybacks.Remove(coroutinePlaybackInfo.Value);
            }
        }

        public void Update(float deltaTime) {
            for (var i = coroutinePlaybacks.Count - 1; i >= 0; i--) {
                var coroutinePlaybackInfo = coroutinePlaybacks[i];
                coroutinePlaybackInfo.coroutinePlayback.executionTimer += deltaTime;
                if (coroutinePlaybackInfo.coroutinePlayback.checkCallback.Invoke()) {
                    continue;
                }
                StopCoroutine(coroutinePlaybackInfo);
                coroutinePlaybackInfo.coroutinePlayback.data.onBreakCallback?.Invoke();
            }
        }
        
        public void FixedUpdate(float fixedDeltaTime) {
            for (var i = coroutinePlaybacks.Count - 1; i >= 0; i--) {
                var coroutinePlaybackInfo = coroutinePlaybacks[i];
                if (coroutinePlaybackInfo.coroutinePlayback.checkCallback.Invoke()) {
                    continue;
                }
                StopCoroutine(coroutinePlaybackInfo);
                coroutinePlaybackInfo.coroutinePlayback.data.onBreakCallback?.Invoke();
            }
        }
    }
}