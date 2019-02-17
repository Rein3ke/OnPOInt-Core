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

    private void ProcessRegisterUserInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
        {
            if (m_cursorLockMode != CursorLockMode.None && !m_isPaused)
                SetLockState(CursorLockMode.None);
        }
        if (Input.GetMouseButtonDown(0))
        {
            SetLockState(CursorLockMode.Locked);
        }
    }

    private void OnGUI()
    {
        Cursor.visible      = m_cursorLockMode != CursorLockMode.Locked;
        Cursor.lockState    = m_cursorLockMode;
    }

    public void SetLockState(CursorLockMode _lockMode)
    {
        m_cursorLockMode    = _lockMode;
        m_isPaused          = m_cursorLockMode != CursorLockMode.Locked;

        LogLockState();
    }

    private void LogLockState()
    {
        Debug.Log("CurrentLockState: " + m_cursorLockMode + "\nPausiert: " + m_isPaused);
    }
}
