using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    public float        Sensitivity = 3.0f;
    public float        Smoothing = 2.0f;

    public Camera       Camera { get; private set; }

    private Vector2     m_mouseLook;
    private Vector2     m_smoothV;
    private Vector2     m_moveDirection;

    private GameObject  m_playerController;

    private void Awake()
    {
        m_playerController  = this.transform.parent.gameObject;
        Camera              = GetComponent<Camera>();
    }

    /// <summary>
    /// Allows the camera to rotate if the application is not paused.
    /// </summary>
    private void Update()
    {
        if (GameManager.Instance?.LockStateManager.IsPaused ?? true)
        {
            return;
        }
        ProcessMove();
    }

    /// <summary>
    /// Based on the mouse movement, the camera rotates to the calculated vector.
    /// </summary>
    private void ProcessMove()
    {
        m_moveDirection = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        m_moveDirection = Vector2.Scale(m_moveDirection, new Vector2(Sensitivity * Smoothing, Sensitivity * Smoothing));
        m_smoothV.x     = Mathf.Lerp(m_smoothV.x, m_moveDirection.x, 1f / Smoothing);
        m_smoothV.y     = Mathf.Lerp(m_smoothV.y, m_moveDirection.y, 1f / Smoothing);
        m_mouseLook     += m_smoothV;

        m_mouseLook.y   = Mathf.Clamp(m_mouseLook.y, -90f, 90f);

        transform.localRotation                     = Quaternion.AngleAxis(-m_mouseLook.y, Vector3.right);
        m_playerController.transform.localRotation  = Quaternion.AngleAxis(m_mouseLook.x, m_playerController.transform.up);
    }
}
