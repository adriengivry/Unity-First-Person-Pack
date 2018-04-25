using UnityEngine;

/*
 * Here is a simple first person movement controller that helps you to move into your level.
 * You must add this script on your first person character
 */
public class FirstPersonMovement : MonoBehaviour
{
    private enum MovementMode
    {
        NORMAL,
        FLYING
    }

    private enum JumpMode
    {
        DISABLED,
        SINGLE,
        DOUBLE
    }

    [Header("MOVEMENT PARAMETERS")]
    [SerializeField] private MovementMode m_movementMode;
    [SerializeField] private bool m_allowSwitchingModeAtRuntime;
    [SerializeField] private bool m_canRun;
    [SerializeField] private float m_walkSpeed;
    [SerializeField] private float m_runSpeed;
    [SerializeField] private float m_smoothing;

    [Header("JUMP PARAMETERS")]
    [SerializeField] private JumpMode m_jumpMode;
    [SerializeField] private float m_jumpStrength;
    [SerializeField] private float m_doubleJumpStrength;

    [Header("INPUT BINDING")]
    [SerializeField] private string m_verticalAxisInput;
    [SerializeField] private string m_horizontalAxisInput;
    [SerializeField] private string m_upAxisInput;
    [SerializeField] private string m_runInput;
    [SerializeField] private string m_jumpInput;
    [SerializeField] private string m_switchingModeInput;

    private Detector m_detector;
    private Rigidbody m_rigidbody;
    private Vector3 m_velocity;
    private bool m_grounded;
    private bool m_running;
    private bool m_jumped;
    private bool m_doubleJumped;

    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        m_detector = GetComponent<Detector>();
    }

    private void Start()
    {
        ListenToDetector();
        SetMovementMode(m_movementMode);
        m_rigidbody.freezeRotation = true;
    }

    private void Update()
    {
        UpdateRun();
        HandleMoveInput();
        HandleJumpInput();
        HandleModeSwitch();
    }

    private void ListenToDetector()
    {
        m_detector.GroundedEvent.AddListener(OnGrounded);
        m_detector.NotGroundedEvent.AddListener(OnNotGrounded);
    }

    private void OnGrounded()
    {
        if (!m_grounded)
        {
            m_grounded = true;
            m_doubleJumped = false;
            m_jumped = false;
        }
    }

    private void OnNotGrounded()
    {
        if (m_grounded)
            m_grounded = false;
    }

    private void SetMovementMode(MovementMode p_newMode)
    {
        m_movementMode = p_newMode;

        if (m_movementMode == MovementMode.FLYING)
        {
            m_rigidbody.useGravity = false;
        }
        else
        {
            m_rigidbody.useGravity = true;
        }
    }

    private void ToggleMode()
    {
        if (m_movementMode == MovementMode.FLYING)
        {
            SetMovementMode(MovementMode.NORMAL);
        }
        else if (m_movementMode == MovementMode.NORMAL)
        {
            SetMovementMode(MovementMode.FLYING);
        }
    }
    private void HandleMoveInput()
    {
        Vector3 movement = new Vector3();

        movement += Camera.main.transform.right * Input.GetAxisRaw(m_horizontalAxisInput);

        if (m_movementMode == MovementMode.NORMAL)
        {
            movement += transform.forward * Input.GetAxisRaw(m_verticalAxisInput);
        }

        if (m_movementMode == MovementMode.FLYING)
        {
            movement += transform.up * Input.GetAxisRaw(m_upAxisInput);
            movement += Camera.main.transform.forward * Input.GetAxisRaw(m_verticalAxisInput);
        }

        movement.Normalize();
        movement *= m_running ? m_runSpeed : m_walkSpeed;

        Vector3 smoothMovement = Vector3.SmoothDamp(m_rigidbody.velocity, movement, ref m_velocity, m_smoothing);

        if (m_movementMode == MovementMode.NORMAL)
            smoothMovement.y = m_rigidbody.velocity.y;

        m_rigidbody.velocity = smoothMovement;
    }

    private void HandleJumpInput()
    {
        if (m_jumpMode == JumpMode.SINGLE || m_jumpMode == JumpMode.DOUBLE)
        {
            if (m_movementMode == MovementMode.NORMAL && Input.GetButtonDown(m_jumpInput))
            {
                if (m_grounded)
                {
                    Jump(m_jumpStrength);
                    m_jumped = true;
                }
                else if (!m_doubleJumped && m_jumped && m_jumpMode == JumpMode.DOUBLE)
                {
                    Jump(m_doubleJumpStrength);
                    m_doubleJumped = true;
                }
            }
        }
    }

    private void HandleModeSwitch()
    {
        if (m_allowSwitchingModeAtRuntime && Input.GetButtonDown(m_switchingModeInput))
            ToggleMode();
    }

    private void Jump(float p_strength)
    {
        m_rigidbody.velocity = new Vector3(m_rigidbody.velocity.x, p_strength, m_rigidbody.velocity.z);
    }

    private void UpdateRun()
    {
        if (!Input.GetButton(m_runInput))
            m_running = false;
        else if (m_grounded && m_canRun)
            m_running = true;
    }
}