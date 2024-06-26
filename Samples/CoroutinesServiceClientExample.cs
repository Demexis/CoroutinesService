using System.Collections;
using JetBrains.Annotations;
using UnityEngine;

namespace CustomCoroutines.Example {
    public class CoroutinesServiceClientExample : MonoBehaviour {
        [SerializeField] private CoroutinesServiceInstallerExample coroutinesServiceInstaller;
        
        private const int COUNTER_MAX_VALUE = 3;

        private ICoroutinesService coroutinesService;
        private CoroutinePlaybackInfo? pressedButtonCoroutine;

        private int buttonClicksCounter;
        
        private void Start() {
            coroutinesService = coroutinesServiceInstaller.CoroutinesService;
        }

        [UsedImplicitly]
        public void OnButtonClick() {
            // stop previous coroutine if active
            coroutinesService.StopCoroutine(pressedButtonCoroutine);
            
            // increase clicks counter by 1
            buttonClicksCounter++;
            Debug.Log($"<color=yellow>Button pressed.</color> Total clicks: <color=yellow>{buttonClicksCounter}</color>");
            
            // start new coroutine playback
            pressedButtonCoroutine = coroutinesService.PlayCoroutine("TEST_COROUTINE_NAME", () => TestEnumeratorMethod(true),
                () => FailOnThirdClick(), new CoroutinePlaybackData() {
                    onBreakCallback = () => Debug.Log("<color=#FF0000>Check-callback failed, that caused playback to stop.</color>"),
                    onFinishedCallback = () => Debug.Log("<color=#00FF00>Coroutine playback successfully finished.</color>"),
                    onEndCallback = () => Debug.Log("<color=#00BAFF>Coroutine playback ended.</color>")
                });
        }
        
        private IEnumerator TestEnumeratorMethod(bool useDebugLogs) {
            for (var i = 0; i < COUNTER_MAX_VALUE; i++) {
                if (useDebugLogs) {
                    Debug.Log($"Ticking: <color=yellow>{i}</color>");
                }
                yield return new WaitForSeconds(1f);
            }
        }

        private bool FailOnThirdClick() {
            if (!this) {
                // GameObject / Component has been destroyed. Coroutine needs to be manually stopped. Just return 'false'.
                return false;
            }
            
            return buttonClicksCounter % 3 != 0;
        }
    }
}
