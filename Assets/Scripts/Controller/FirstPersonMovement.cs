using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

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
    [SerializeField] private float m_walkSpeed;
    [SerializeField] private float m_runSpeed;
    [SerializeField] private float m_smoothing;

    [Header("JUMP PARAMETERS")]
    [SerializeField] private JumpMode m_jumpMode;
    [SerializeField] private float m_jumpStrength;
    [SerializeField] private float m_doubleJumpStrength;
    [SerializeField] private float m_allowJumpDuringFallInSeconds;

    [Header("INPUT BINDING")]
    [SerializeField] private string m_verticalAxisInput;
    [SerializeField] private string m_horizontalAxisInput;
    [SerializeField] private string m_upAxisInput;
    [SerializeField] private string m_runInput;
    [SerializeField] private string m_jumpInput;

    private Rigidbody m_rigidbody;
    private Vector3 m_velocity;
    private float m_distanceToGround;
    private float m_jumpDelay;
    private bool m_grounded;
    private bool m_running;
    private float m_secondsSinceNotGrounded;
    private bool m_doubleJumped;

    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        if (m_rigidbody)
            m_rigidbody.freezeRotation = true;

        m_distanceToGround = GetComponent<Collider>().bounds.extents.y;

        SetMovementMode(m_movementMode);
    }

    private void Update()
    {
        UpdateRun();
        UpdateGrounded();
        HandleMoveInput();
        HandleJumpInput();
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
            m_jumpDelay += Time.deltaTime;

            if (m_movementMode == MovementMode.NORMAL && Input.GetButtonDown(m_jumpInput))
            {
                if (m_grounded || m_secondsSinceNotGrounded <= m_allowJumpDuringFallInSeconds)
                {
                    Jump(m_jumpStrength);
                }
                else if (!m_doubleJumped && m_jumpMode == JumpMode.DOUBLE)
                {
                    Jump(m_doubleJumpStrength);
                    m_doubleJumped = true;
                }
            }
        }
    }

    private void Jump(float p_strength)
    {
        m_rigidbody.velocity.Scale(new Vector3(1.0f, 0.0f, 1.0f));
        m_rigidbody.velocity += transform.up * p_strength;
    }

    private void UpdateGrounded()
    {
        m_grounded = false;

        if (Physics.Raycast(transform.position, -Vector3.up, m_distanceToGround + 0.1f))
        {
            m_doubleJumped = false;
            m_grounded = true;
            m_secondsSinceNotGrounded = 0.0f;
        }
        else
        {
            m_secondsSinceNotGrounded += Time.deltaTime;
            m_grounded = false;
        }
    }

    private void UpdateRun()
    {
        if (!Input.GetButton(m_runInput))
            m_running = false;
        else if (m_grounded)
            m_running = true;
    }
}