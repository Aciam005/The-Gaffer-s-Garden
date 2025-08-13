using UnityEngine;

namespace Game.Mechanics
{
    public class CameraController : MonoBehaviour
    {
        [Header("SETTINGS")]
        public float moveSpeed = 10f; // Speed of camera movement
        public float zoomSpeed = 20f; // Speed of camera zoom
        public float minZoom = 10f;   // Minimum camera height
        public float maxZoom = 100f;  // Maximum camera height

        [Header("REFERENCES")]
        [SerializeField] private Camera cam;
        private Vector3 targetPosition;
        private float targetZoom;

        [Header("Rotation Settings")]
        public float rotationSpeed = 5f; // Speed of camera rotation
        private float rotationX = 0f;
        private float rotationY = 0f;

        private void Awake()
        {
            targetPosition = transform.position;
            targetZoom = transform.position.y;
            // Initialize rotation from current transform
            rotationX = transform.eulerAngles.x;
            rotationY = transform.eulerAngles.y;
        }

        private void Update()
        {
            HandleMovement();
            HandleZoom();
            HandleRotation();
        }

        // Handles WASD movement parallel to the ground (XZ plane)
        private void HandleMovement()
        {
            float moveX = Input.GetAxis("Horizontal"); // A/D or Left/Right
            float moveZ = Input.GetAxis("Vertical");   // W/S or Up/Down

            Vector3 move = new Vector3(moveX, 0, moveZ);
            if (move.sqrMagnitude > 0.01f)
            {
                move = transform.TransformDirection(move);
                move.y = 0; // Ensure movement is parallel to ground
                targetPosition += move.normalized * moveSpeed * Time.deltaTime;
            }
            // Smoothly interpolate to target position
            transform.position = Vector3.Lerp(transform.position, targetPosition, 0.15f);
        }

        // Handles mouse wheel zoom (dolly zoom)
        private void HandleZoom()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.01f)
            {
                targetZoom += scroll * zoomSpeed;
                targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
            }
            // Smoothly interpolate camera height
            Vector3 pos = transform.position;
            pos.y = Mathf.Lerp(pos.y, targetZoom, 0.15f);
            transform.position = pos;
        }

        // Handles camera rotation when holding right mouse button
        private void HandleRotation()
        {
            if (Input.GetMouseButton(1)) // Right mouse button held
            {
                float mouseX = Input.GetAxis("Mouse X");
                float mouseY = Input.GetAxis("Mouse Y");

                rotationY += mouseX * rotationSpeed;
                rotationX -= mouseY * rotationSpeed;
                rotationX = Mathf.Clamp(rotationX, -80f, 80f); // Prevent flipping over

                transform.rotation = Quaternion.Euler(rotationX, rotationY, 0f);
            }
        }
    }
}
