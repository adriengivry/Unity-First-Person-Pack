using UnityEngine;

/*
 * A simple first person camera allowing you to look everywhere on your scene
 * You have to place this script on the camera associated to your first person character (The camera must be a child).
 */ 
public class FirstPersonCamera : MonoBehaviour
{
    [Header("MOUSE SETTINGS")]
    [SerializeField] [Range(0, 5)] private float m_mouseSensitivity;

    [Header("CAMERA MOVEMENT SETTINGS")]
    [SerializeField] [Range(-360, 360)] private float m_minimumVerticalAngle = -90.0f;
    [SerializeField] [Range(-360, 360)] private float m_maximumVerticalAngle = 90.0f;
    [SerializeField] private bool m_cameraVerticalLock;

    private Vector2 m_mouseLook;
    private Quaternion m_initialRotation;
    private Camera m_camera;

    private void Awake()
    {
        m_camera = GetComponentInChildren<Camera>();
    }

    private void Start()
    {
        LockCursor();
        HideCursor();

        m_initialRotation = transform.rotation;
    }

    private void Update()
    {
        UpdateMouseLook();
        UpdateCameraRotation();
    }

    private void LockCursor(bool p_state = true)
    {
        if (p_state)
            Cursor.lockState = CursorLockMode.Locked;
        else
            Cursor.lockState = CursorLockMode.None;
    }

    private void HideCursor(bool p_state = true)
    {
        if (p_state)
            Cursor.visible = false;
        else
            Cursor.visible = true;
    }

    private void UpdateMouseLook()
    {
        m_mouseLook += new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * m_mouseSensitivity;

        if (m_cameraVerticalLock)
            m_mouseLook.y = Mathf.Clamp(m_mouseLook.y, m_minimumVerticalAngle, m_maximumVerticalAngle);
    }

    private void UpdateCameraRotation()
    {
        m_camera.transform.localRotation = Quaternion.AngleAxis(-m_mouseLook.y, Vector3.right);
        transform.rotation = m_initialRotation * Quaternion.AngleAxis(m_mouseLook.x, transform.up);
    }
}