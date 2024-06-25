using JetBrains.Annotations;
using System;
using System.Collections;
using UnityEngine;

namespace CustomCoroutines {
    public struct CoroutinePlaybackData {
        /// <summary>
        /// Is called if checking-callback fails.
        /// </summary>
        [CanBeNull] public Action onBreakCallback;
        
        /// <summary>
        /// Is called only when the coroutine finishes execution without stopping.
        /// </summary>
        [CanBeNull] public Action onFinishedCallback;
        
        /// <summary>
        /// Is called whenever coroutine stops or finishes.
        /// </summary>
        [CanBeNull] public Action onEndCallback;
    }
    
    public class CoroutinePlayback {
        /// <summary>
        /// Playback finished executing enumerator callback.
        /// </summary>
        public event Action Executed = delegate { };
        
        public readonly string coroutineDebugName;
        public readonly Func<IEnumerator> coroutineCallback;
        public readonly Func<bool> checkCallback;

        public readonly CoroutinePlaybackData data;

        public float executionTimer;
        public bool executed;

        private IEnumerator enumerator;
        
        public CoroutinePlayback(string coroutineDebugName, Func<IEnumerator> coroutineCallback, Func<bool> checkCallback, CoroutinePlaybackData data) {
            this.coroutineDebugName = coroutineDebugName;
            this.coroutineCallback = coroutineCallback;
            this.checkCallback = checkCallback;
            this.data = data;
        }

        /// <summary>
        /// Starts coroutine callback using given MonoBehaviour.
        /// </summary>
        /// <param name="monoBehaviour">MonoBehaviour that will be used to call StartCoroutine().</param>
        public void StartCoroutine(MonoBehaviour monoBehaviour) {
            StopCoroutine(monoBehaviour);
            enumerator = CoroutinePlaybackWrap(monoBehaviour);
            monoBehaviour.StartCoroutine(enumerator);
        }
        
        /// <summary>
        /// Waits for coroutine callback to finish executing. Will stop coroutine in the end and invoke 'Executed' event. 
        /// </summary>
        /// <param name="monoBehaviour">MonoBehaviour that will be used to call StopCoroutine() when callback will be executed.</param>
        /// <returns>enumerator</returns>
        private IEnumerator CoroutinePlaybackWrap(MonoBehaviour monoBehaviour) {
            yield return coroutineCallback.Invoke();
            data.onFinishedCallback?.Invoke();
            StopCoroutine(monoBehaviour);
            executed = true;
            Executed.Invoke();
        }

        /// <summary>
        /// Stops coroutine callback using given MonoBehaviour.
        /// </summary>
        /// <param name="monoBehaviour">MonoBehaviour that will be used to call StopCoroutine().</param>
        public void StopCoroutine(MonoBehaviour monoBehaviour) {
            if (enumerator == null) {
                return;
            }

            if (!monoBehaviour) {
                Debug.LogWarning("MonoBehaviour was destroyed at the moment when the coroutine was about to stop. "
                    + "That may happen when stopping/changing scene.");
                enumerator = null;
                return;
            }
            
            monoBehaviour.StopCoroutine(enumerator);
            enumerator = null;
            data.onEndCallback?.Invoke();
        }
    }
}