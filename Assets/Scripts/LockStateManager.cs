using UnityEngine;

[RequireComponent(typeof(GameManager))]
public class LockStateManager : MonoBehaviour
{
    public CursorLockMode CursorLockMode { get { return m_cursorLockMode; } }
    public bool IsPaused { get { return m_isPaused; } }

    private CursorLockMode m_cursorLockMode = CursorLockMode.None;
    private bool m_isPaused = true;

    private void Update()
    {
        ProcessRegisterUserInput();
    }

    /// <summary>
    /// Registers the left and right click when clicked and sets corresponding CursorLockMode.
    /// </summary>
    private void ProcessRegisterUserInput()
    {
        if (Input.GetMouseButtonDown(1)) // Right click
        {
            if (m_cursorLockMode != CursorLockMode.None && !m_isPaused)
                SetLockState(CursorLockMode.None);
        }
        if (Input.GetMouseButtonDown(0)) // Left click
        {
            SetLockState(CursorLockMode.Locked);
        }
    }

    /// <summary>
    /// Status of the mouse cursor depends on the set LockMode.
    /// </summary>
    private void OnGUI()
    {
        Cursor.visible      = m_cursorLockMode != CursorLockMode.Locked;
        Cursor.lockState    = m_cursorLockMode;
    }

    /// <summary>
    /// Gets a LockMode and sets it. The application pauses when LockMode is set to None.
    /// </summary>
    /// <param name="_lockMode">CursorLockMode (Locked, None)</param>
    public void SetLockState(CursorLockMode _lockMode)
    {
        if (m_cursorLockMode == _lockMode) return;

        m_cursorLockMode    = _lockMode;
        m_isPaused          = m_cursorLockMode != CursorLockMode.Locked;

        //LogLockState(); // Logs the set lockMode.
    }

    private void LogLockState()
    {
        Debug.Log("CurrentLockState: " + m_cursorLockMode + "\nIs paused: " + m_isPaused);
    }
}
