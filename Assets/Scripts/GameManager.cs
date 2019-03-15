using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance              => s_instance;
    private static GameManager s_instance           = null;

    public LockStateManager LockStateManager        => m_lockStateManager;
    private LockStateManager m_lockStateManager;

    public SceneLoader SceneLoader => m_sceneLoader;
    private SceneLoader m_sceneLoader;

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
        m_sceneLoader       = GetComponent<SceneLoader>();
    }

    private void Start()
    {
//#if UNITY_EDITOR
//        SceneManager.LoadScene("test_scene");
//#else
//        SceneManager.LoadScene("Welcome_Screen");
//#endif
        SceneManager.LoadScene("Welcome_Screen");
        m_sceneLoader.LoadScene(1);
    }

    public void SetLockState(int _lockMode)
    {
        if (_lockMode > 2 || _lockMode < 0) throw new System.ArgumentException("Invalid _lockMode supplied.");
        m_lockStateManager.SetLockState((CursorLockMode)_lockMode);
    }

    /// <summary>
    /// Recognize incomming scene change request
    /// </summary>
    /// <param name="_ID"></param>
    public void ReceiveSceneID(int _ID)
    {
        if(m_sceneLoader.IsLoading)
        {
            Debug.LogWarning("Loader is already busy on another scene!");
            return;
        }
        Debug.Log($"Loading Scene {_ID} ...");
        m_sceneLoader.LoadScene(_ID);
    }

    private void OnDestroy()
    {
        if(s_instance == this)
            s_instance = null;
    }
}
