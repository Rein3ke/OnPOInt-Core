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

    private const string START_SCENE = "Welcome_Screen";

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

        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;

        ComUtility.RegisterIntercomCallback(EProtocolObjectType.SCENE_CHANGE, OnSceneChangeReceived);
    }

    private void Start()
    {
        SceneManager.LoadScene(START_SCENE);
    }

    public void SetLockState(int _lockMode)
    {
        if (_lockMode > 2 || _lockMode < 0) throw new System.ArgumentException("Invalid _lockMode supplied.");
        m_lockStateManager.SetLockState((CursorLockMode)_lockMode);
    }

    private void OnDestroy()
    {
        if(s_instance == this)
            s_instance = null;
    }

    private void OnSceneLoaded(Scene _loaded, LoadSceneMode _mode)
    {
        if (_loaded.name != START_SCENE) return;
        Debug.Log($"GameManager: OnSceneLoaded: Scene >> {_loaded.name}");
        // TODO: Remove Test Request and send control unlock (scene switch enable) to frontend
        OnSceneChangeReceived(new SceneChangeProtocolObject(1));
        // END TEST REQUEST
    }

    #region INTERCOM_CALLBACKS
    private void OnSceneChangeReceived(ProtocolObject _poObject)
    {
        var sceneChangeObject = (_poObject as SceneChangeProtocolObject) ?? throw new System.ArgumentException("Received PO object was not of type scene change!");
        if (m_sceneLoader.IsLoading)
        {
            Debug.LogWarning("Loader is already busy on another scene!");
            return;
        }
        Debug.Log($"Loading Scene {sceneChangeObject.SceneID} ...");
        m_sceneLoader.LoadScene(sceneChangeObject.SceneID);
    }
    #endregion
}
