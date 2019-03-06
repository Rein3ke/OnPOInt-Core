using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance              => s_instance;
    private static GameManager s_instance           = null;

    public LockStateManager LockStateManager        => m_lockStateManager;
    private LockStateManager m_lockStateManager;

    private void Awake()
    {
        // Delete the instance, if already one is existing to prevent multiple gamemanagers
        if (s_instance != null)
        {
            Destroy(gameObject);

            return;
        }

        DontDestroyOnLoad(this);

        s_instance          = this;
        m_lockStateManager  = GetComponent<LockStateManager>();
    }

    private void Start()
    {
        //LoadPOIData();
        SceneManager.LoadScene("scene_1");
        //SceneManager.LoadScene("Welcome_Screen");
    }

    public void SetLockState(int _lockMode)
    {
        if (_lockMode > 2 || _lockMode < 0) throw new System.ArgumentException("Invalid _lockMode supplied.");
        m_lockStateManager.SetLockState((CursorLockMode)_lockMode);
    }

    public void ReceiveSceneID(int _ID)
    {
        Debug.Log($"Loading Scene {_ID}.");
    }

    private void OnDestroy()
    {
        if(s_instance == this)
            s_instance = null;
    }
}
