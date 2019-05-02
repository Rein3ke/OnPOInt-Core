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

    public PlayerController PlayerController        => m_playerController;
    private PlayerController m_playerController;

    public SceneLoader SceneLoader                  => m_sceneLoader;
    private SceneLoader m_sceneLoader;

    // START_SCENE defines the scene to be loaded after instantiation of the GameManager
    private const string START_SCENE = "Welcome_Screen";

    // A GameObject declared via the Inspector.
    [SerializeField] private GameObject playerPrefab;

    /// <summary>
    /// Is called first after instantiation - Sets the member variables,
    /// registers OnSceneLoaded in SceneManager,
    /// and assigns ProtocolObjects and Functions to the Callback-Register.
    /// </summary>
    private void Awake()
    {
        // Deletes the instance, if already one is existing to prevent multiple gamemanagers
        if (s_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        // Prevents the deletion after loading a new scene
        DontDestroyOnLoad(this);
        s_instance          = this;

        m_lockStateManager  = GetComponent<LockStateManager>();
        m_sceneLoader       = GetComponent<SceneLoader>();

#if UNITY_EDITOR
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
#endif

        ComUtility.RegisterIntercomCallback(EProtocolObjectType.SCENE_CHANGE, OnSceneChangeReceived);
        ComUtility.RegisterIntercomCallback(EProtocolObjectType.SPEED_CHANGE, OnSpeedChangeReceived);
        ComUtility.RegisterIntercomCallback(EProtocolObjectType.SENS_CHANGE, OnSensibilityChangeReceived);
    }

    /// <summary>
    /// Function that gets called after Awake method - Loads the startup scene via Unity's SceneManager
    /// </summary>
    private void Start()
    {
        SceneManager.LoadScene(START_SCENE);
    }

    /// <summary>
    /// Function to set the lockstate via the lockstate manager
    /// </summary>
    /// <param name="_lockMode">Number representing the lockstate between 0 - 2</param>
    public void SetLockState(int _lockMode)
    {
        if (_lockMode > 2 || _lockMode < 0) throw new System.ArgumentException("Invalid _lockMode supplied.");
        m_lockStateManager.SetLockState((CursorLockMode)_lockMode);
    }

    /// <summary>
    /// A function that is called automatically after the instance is deleted.
    /// </summary>
    private void OnDestroy()
    {
        if(s_instance == this)
            s_instance = null;
    }

#if UNITY_EDITOR
    private void OnSceneLoaded(Scene _loaded, LoadSceneMode _mode)
    {
        // Emulated SceneChangeRequest for testing purposes in the Unity Editor
        if (_loaded.name != START_SCENE) return;
        OnSceneChangeReceived(new SceneChangeProtocolObject(1));
    }
#endif

    /// <summary>
    /// A function called by the ComUtility class.
    /// Finds the spawn point in the scene and places the player on it.
    /// </summary>
    public void OnBundleSceneLoaded()
    {
        GameObject respawn = GameObject.FindGameObjectWithTag("Respawn");
        GameObject player = Instantiate(playerPrefab, respawn.transform.position, respawn.transform.rotation);
        m_playerController = player.GetComponent<PlayerController>();
    }

    #region INTERCOM_CALLBACKS
    // At this point, the callback functions are stored.

    /// <summary>
    /// This function is passed a ProtocolObject and translates it to a SceneChangeProtocolObject.
    /// Picks out the id of the scene and submits it to the SceneLoader-Method LoadScene.
    /// </summary>
    /// <param name="_poObject">ProtocolObject as SceneChangeProtocolObject</param>
    private void OnSceneChangeReceived(ProtocolObject _poObject)
    {
        var sceneChangeObject = (_poObject as SceneChangeProtocolObject) ?? throw new System.ArgumentException("Received PO object was not of type scene change!");
        if (m_sceneLoader.IsLoading)
        {
            Debug.LogWarning("Loader is already busy on another scene!");
            return;
        }
        m_sceneLoader.LoadScene(sceneChangeObject.SceneID);
    }

    /// <summary>
    /// This function is passed a ProtocolObject and translates it to a SpeedChangeProtocolObject.
    /// Transmits the value for the speed to the PlayerController.
    /// </summary>
    /// <param name="_poObject">ProtocolObject as SpeedChangeProtocolObject</param>
    private void OnSpeedChangeReceived(ProtocolObject _poObject)
    {
        var speedChangeObject = (_poObject as SpeedChangeProtocolObject) ?? throw new System.ArgumentException("Received PO object was not of type speed change!");

        if (m_playerController != null)
        {
            m_playerController.MoveSpeed = speedChangeObject.Value;
        } else
        {
            Debug.LogWarning($"Tried to set speed value, while no player was spawned!");
        }
    }

    /// <summary>
    /// This function is passed a ProtocolObject and translates it to a SensibilityChangeProtocolObject.
    /// Transmits the value for mouse sensitivity to the CameraController of the PlayerController.
    /// </summary>
    /// <param name="_poObject">ProtocolObject as SensibilityChangeProtocolObject</param>
    private void OnSensibilityChangeReceived(ProtocolObject _poObject)
    {
        var sensChangeObject = (_poObject as SensibilityChangeProtocolObject) ?? throw new System.ArgumentException("Received PO object was not of type sensibility change!");

        if (m_playerController != null && m_playerController.CameraController != null)
        {
            m_playerController.CameraController.Sensitivity = sensChangeObject.Value;
        }
        else
        {
            Debug.LogWarning($"Tried to set sensitivity value, while no player was spawned!");
        }
    }
    #endregion
}
