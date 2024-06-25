using System.Collections;
using JetBrains.Annotations;
using UnityEngine;

namespace CustomCoroutines.Example {
    public class CoroutinesServiceClientExample : MonoBehaviour {
        [SerializeField] private CoroutinesServiceInstallerExample coroutinesServiceInstaller;
        
        private const int COUNTER_MAX_VALUE = 5;

        private ICoroutinesService coroutinesService;
        private CoroutinePlaybackInfo? pressedButtonCoroutine;
        
        private void Start() {
            coroutinesService = coroutinesServiceInstaller.CoroutinesService;
        }

        [UsedImplicitly]
        public void OnButtonClick() {
            // stop previous coroutine if active
            coroutinesService.StopCoroutine(pressedButtonCoroutine);
            
            // start new coroutine playback
            pressedButtonCoroutine = coroutinesService.PlayCoroutine("TEST_COROUTINE_NAME", () => TestEnumeratorMethod(true),
                () => this);
        }

        private IEnumerator TestEnumeratorMethod(bool useDebugLogs) {
            for (var i = 0; i < COUNTER_MAX_VALUE; i++) {
                if (useDebugLogs) {
                    Debug.Log("Ticking: " + i);
                }
                yield return new WaitForSeconds(1f);
            }
        } 
    }
}