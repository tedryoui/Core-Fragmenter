using UnityEngine;
using UnityEngine.UIElements;

namespace _.Scripts.Utilities
{
    [RequireComponent(typeof(UIDocument))]
    public class UserInterfaceFaceCamera : MonoBehaviour
    {
        private Camera _activeCamera;

        private void LateUpdate()
        {
            var camera = GetActiveCamera();
            if (camera == null)
                return;

            transform.forward = camera.transform.forward;
        }

        private Camera GetActiveCamera()
        {
            if (_activeCamera != null && _activeCamera.isActiveAndEnabled)
                return _activeCamera;

            _activeCamera = FindActiveCamera();
            return _activeCamera;
        }

        private static Camera FindActiveCamera()
        {
            if (Camera.main != null && Camera.main.isActiveAndEnabled)
                return Camera.main;

            var cameras = FindObjectsByType<Camera>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

            Camera best = null;
            foreach (var camera in cameras)
            {
                if (!camera.isActiveAndEnabled)
                    continue;

                if (best == null || camera.depth > best.depth)
                    best = camera;
            }

            return best;
        }
    }
}
