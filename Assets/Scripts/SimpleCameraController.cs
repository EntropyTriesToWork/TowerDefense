using UnityEngine;
using Sirenix.OdinInspector;

namespace Entropy.TD.Camera
{
    /// <summary>
    /// This camera controller was mostly copied from the CameraController in Unity's default URP project's example scene with the exception of the rotation.
    /// </summary>
    public class SimpleCameraController : MonoBehaviour
    {
        #region Variables
        CameraState m_TargetCameraState = new CameraState();
        CameraState m_InterpolatingCameraState = new CameraState();

        [BoxGroup("Required")] public Transform cameraTransform;

        [BoxGroup("Movement Settings")] public float boost = 3.5f;
        [BoxGroup("Movement Settings")] public float boostMultipler = 5f;

        [BoxGroup("Movement Settings")]
        [Tooltip("Time it takes to interpolate camera position 99% of the way to the target."), Range(0.001f, 1f)]
        public float positionLerpTime = 0.2f;

        [BoxGroup("Rotation Settings")]
        [Tooltip("X = Change in mouse position.\nY = Multiplicative factor for camera rotation.")]
        public AnimationCurve mouseSensitivityCurve = new AnimationCurve(new Keyframe(0f, 0.5f, 0f, 5f), new Keyframe(1f, 2.5f, 0f, 0f));

        [BoxGroup("Rotation Settings")]
        [Tooltip("Time it takes to interpolate camera rotation 99% of the way to the target."), Range(0.001f, 1f)]
        public float rotationLerpTime = 0.01f;

        [BoxGroup("Zoom Settings")] [Tooltip("Whether or not to invert our Y axis for mouse input to rotation.")] public float zoomSpeed = 3f;
        [BoxGroup("Zoom Settings")] [PropertyRange(0.001f, 1f)] public float zoomLerpTime = 0.2f;
        [BoxGroup("Zoom Settings")] public float minZoom = 3f;
        [BoxGroup("Zoom Settings")] public float maxZoom = 15f;

        private Vector3 _cameraZoomPos;
        #endregion

        #region Messages
        void OnEnable()
        {
            m_TargetCameraState.SetFromTransform(transform);
            m_InterpolatingCameraState.SetFromTransform(transform);
            _cameraZoomPos = cameraTransform.localPosition;
        }
        void Update()
        {
            // Exit Sample  
            if (IsEscapePressed())
            {
                Application.Quit();
				#if UNITY_EDITOR
				UnityEditor.EditorApplication.isPlaying = false; 
				#endif
            }

            // Hide and lock cursor when right mouse button pressed
            if (IsRightMouseButtonDown())
            {
                Cursor.lockState = CursorLockMode.Locked;
            }

            // Unlock and show cursor when right mouse button released
            if (IsRightMouseButtonUp())
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }

            // Rotation
            if (IsCameraRotationAllowed())
            {
                var mouseMovement = GetInputLookRotation() * Time.deltaTime * 5;
                
                var mouseSensitivityFactor = mouseSensitivityCurve.Evaluate(mouseMovement.magnitude);

                m_TargetCameraState.yaw += mouseMovement.x * mouseSensitivityFactor;
                //m_TargetCameraState.pitch += mouseMovement.y * mouseSensitivityFactor;
            }
            
            // Translation
            var translation = GetInputTranslationDirection() * Time.deltaTime;

            // Speed up movement when shift key held
            if (IsBoostPressed())
            {
                translation *= boostMultipler;
            }

            translation *= Mathf.Pow(2.0f, boost);

            m_TargetCameraState.Translate(translation);

            // Framerate-independent interpolation
            // Calculate the lerp amount, such that we get 99% of the way to our target in the specified time
            var positionLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / positionLerpTime) * Time.deltaTime);
            var rotationLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / rotationLerpTime) * Time.deltaTime);
            m_InterpolatingCameraState.LerpTowards(m_TargetCameraState, positionLerpPct, rotationLerpPct);

            m_InterpolatingCameraState.UpdateTransform(transform);

            // Zoom
            if (GetScrollWheelValue() != 0f)
            {
                var dist = cameraTransform.localPosition.magnitude;
                var zoomDist = Mathf.Clamp(dist + //Add local position offset distance.
                    -GetScrollWheelValue() * zoomSpeed //Calculate distance to move.
                    * dist / maxZoom, //Increase zoom speed based on current distance from local Vector3.zero. 
                    minZoom, maxZoom);
                _cameraZoomPos = -zoomDist * cameraTransform.InverseTransformDirection(cameraTransform.forward);
            }
            cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, _cameraZoomPos, zoomLerpTime * Time.deltaTime * 50);
        }
        #endregion

        #region Public
        [Button("Reset Camera")]
        public void ResetCamera()
        {
            _cameraZoomPos = new Vector3(0, 0, (maxZoom + minZoom) / 2f);
        }
        #endregion

        #region Getters
        //Input
        Vector3 GetInputTranslationDirection()
        {
            Vector3 direction = Vector3.zero;
            if (Input.GetKey(KeyCode.W))
            {
                direction += Vector3.forward;
            }
            if (Input.GetKey(KeyCode.S))
            {
                direction += Vector3.back;
            }
            if (Input.GetKey(KeyCode.A))
            {
                direction += Vector3.left;
            }
            if (Input.GetKey(KeyCode.D))
            {
                direction += Vector3.right;
            }
            return direction;
        }
        float GetScrollWheelValue() => Input.mouseScrollDelta.y;
        Vector2 GetInputLookRotation() => new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * 10;
        bool IsBoostPressed() => Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        bool IsEscapePressed() => Input.GetKey(KeyCode.Escape);
        bool IsCameraRotationAllowed() => Input.GetMouseButton(1);
        bool IsRightMouseButtonDown() => Input.GetMouseButtonDown(1);
        bool IsRightMouseButtonUp() => Input.GetMouseButtonUp(1);
    }
    #endregion

    class CameraState
    {
        public float yaw;
        public float pitch;
        public float roll;
        public float x;
        public float y;
        public float z;

        public void SetFromTransform(Transform t)
        {
            pitch = t.eulerAngles.x;
            yaw = t.eulerAngles.y;
            roll = t.eulerAngles.z;
            x = t.position.x;
            y = t.position.y;
            z = t.position.z;
        }

        public void Translate(Vector3 translation)
        {
            Vector3 rotatedTranslation = Quaternion.Euler(pitch, yaw, roll) * translation;

            x += rotatedTranslation.x;
            y += rotatedTranslation.y;
            z += rotatedTranslation.z;
        }

        public void LerpTowards(CameraState target, float positionLerpPct, float rotationLerpPct)
        {
            yaw = Mathf.Lerp(yaw, target.yaw, rotationLerpPct);
            pitch = Mathf.Lerp(pitch, target.pitch, rotationLerpPct);
            roll = Mathf.Lerp(roll, target.roll, rotationLerpPct);

            x = Mathf.Lerp(x, target.x, positionLerpPct);
            y = Mathf.Lerp(y, target.y, positionLerpPct);
            z = Mathf.Lerp(z, target.z, positionLerpPct);
        }

        public void UpdateTransform(Transform t)
        {
            t.eulerAngles = new Vector3(pitch, yaw, roll);
            t.position = new Vector3(x, y, z);
        }
    }
}