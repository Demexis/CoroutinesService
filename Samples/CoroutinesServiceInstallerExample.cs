using UnityEngine;

namespace CustomCoroutines.Example {
    public class CoroutinesServiceInstallerExample : MonoBehaviour {
        public CoroutinesService CoroutinesService { get; private set; }

        private void Awake() {
            CoroutinesService = new CoroutinesService();
        }

        private void Update() {
            CoroutinesService.Update(Time.deltaTime);
        }

        private void FixedUpdate() {
            CoroutinesService.FixedUpdate(Time.fixedDeltaTime);
        }
    }
}