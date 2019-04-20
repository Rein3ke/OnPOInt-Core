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
        ComUtility.RegisterIntercomCallback(EProtocolObjectType.SPEED_CHANGE, OnSpeedChangeReceived);
        ComUtility.RegisterIntercomCallback(EProtocolObjectType.SENS_CHANGE, OnSensibilityChangeReceived);
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
    private void OnSpeedChangeReceived(ProtocolObject _poObject)
    {
        var speedChangeObject = (_poObject as SpeedChangeProtocolObject) ?? throw new System.ArgumentException("Received PO object was not of type speed change!");
        PlayerController player = FindObjectOfType<PlayerController>();

        if (player != null)
        {
            player.MoveSpeed = speedChangeObject.Value;
            Debug.Log($"Changend Player movement speed to {speedChangeObject.Value} ...");
        }
    }
    private void OnSensibilityChangeReceived(ProtocolObject _poObject)
    {
        var sensChangeObject = (_poObject as SensibilityChangeProtocolObject) ?? throw new System.ArgumentException("Received PO object was not of type sensibility change!");
        CameraController camera = FindObjectOfType<CameraController>();

        if (camera != null)
        {
            camera.Sensitivity = sensChangeObject.Value;
            Debug.Log($"Changend Camera sensibility to {sensChangeObject.Value} ...");
        }
    }
    #endregion
}
