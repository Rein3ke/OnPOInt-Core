using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(GameManager))]
public class SceneLoader : MonoBehaviour
{
    public const string LOADER_SCENE_NAME   = "Loader";
    public const string BUNDLE_ROOT         = "https://marvin.petesplace.de/data/scene_bundles";

    private const string LOAD_SCENE_PREFIX  = "scene_";

    public bool IsLoading => m_runningRequest != null;

    private int m_loadingSceneID = -1;
    private string m_loadSceneName;
    private UnityWebRequestAsyncOperation m_runningRequest = null;
    private AssetBundle m_currentlyLoadedSceneBundle;

    private Text m_loadingText;

    private void Awake()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    /// <summary>
    /// Public function that triggers the loading process.
    /// The transmitted id defines a path to the bundle.
    /// </summary>
    /// <param name="_ID">Scene ID</param>
    public void LoadScene(int _ID)
    {
        if (IsLoading || !string.IsNullOrEmpty(m_loadSceneName)) return;

        m_loadingSceneID = _ID;
        m_loadSceneName = LOAD_SCENE_PREFIX + m_loadingSceneID;

        var currScene = SceneManager.GetActiveScene();

        if (currScene.name != LOADER_SCENE_NAME)
        {
            SceneManager.LoadScene(LOADER_SCENE_NAME);
            return;
        }

        DoSceneDownload();
    }

    /// <summary>
    /// Process which monitors the current request and reacts after completion.
    /// </summary>
    private void Update()
    {
        if (m_runningRequest == null) return;
        if (m_runningRequest.isDone)
        {
            try
            {
                // If the request was terminated by an error the OnDownloadFailed method is called.
                if (m_runningRequest.webRequest.isNetworkError || m_runningRequest.webRequest.isHttpError)
                {
                    OnDownloadFailed(m_runningRequest.webRequest.error, m_runningRequest.webRequest.responseCode);
                }
                else
                {
                    // Writes the byte code of the response as AssetBundle.
                    AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(m_runningRequest.webRequest);
                    OnBundleDownloaded(bundle);
                }
            }
            finally
            {
                m_runningRequest = null;
                m_loadingText = null;
            }
                        
            return;
        }

        // The status of the process is transmitted to the text element of the loader scene.
        if (m_loadingText != null && m_runningRequest.progress > 0f)
        {
            m_loadingText.text = $"Loading scene {m_loadingSceneID} ... {(int)(m_runningRequest.progress * 100f)}%";
        }
    }

    /// <summary>
    /// Will generate and start a WebRequest.
    /// </summary>
    private void DoSceneDownload()
    {
        m_loadingText = FindObjectOfType<Text>();
        if (m_loadingText)
        {
            m_loadingText.color = Color.black;
            m_loadingText.text = "Initializing download...";
        }

        // Unloading cached scene bundle.
        if (m_currentlyLoadedSceneBundle != null)
        {
            m_currentlyLoadedSceneBundle.Unload(true);
            m_currentlyLoadedSceneBundle = null;
        }

        var url = $"{BUNDLE_ROOT}/{LOAD_SCENE_PREFIX}{m_loadingSceneID}";

        // Requesting bundle from url
        var request = UnityWebRequestAssetBundle.GetAssetBundle(url);

        m_runningRequest = request.SendWebRequest();
        // TODO: Check documentation if necessary
        m_runningRequest.allowSceneActivation = true;
    }

    /// <summary>
    /// Replaces the content of the text element with the passed string.
    /// </summary>
    /// <param name="_error">An error, process or warning message as string.</param>
    private void ShowError(string _error)
    {
        m_loadingText.text  = _error;
        m_loadingText.color = Color.red;
    }

    /// <summary>
    /// Called after a bundle has been successfully downloaded and registered.
    /// Loads the scene of the bundle with the SceneManager.
    /// </summary>
    /// <param name="_bundle">Bundle that has been successfully downloaded and registered.</param>
    private void OnBundleDownloaded(AssetBundle _bundle)
    {
        // Bundle downloaded...
        if (_bundle == null)
        {
            ShowError("Failed to load scene!");
            return;
        }
        m_currentlyLoadedSceneBundle = _bundle;
        // Loading scene...
        SceneManager.LoadScene(m_loadSceneName);
    }

    /// <summary>
    /// Called after a request was terminated with an error.
    /// Prints a detailed error message.
    /// </summary>
    /// <param name="_err">String - Error message</param>
    /// <param name="_responseCode">Response code of webrequest</param>
    private void OnDownloadFailed(string _err, long _responseCode)
    {
        Debug.LogWarning("OnDownloadFailed: Failed to load scene!");
        if (m_runningRequest.webRequest.responseCode == 404)
        {
            ShowError($"Scene {m_loadSceneName} requested, but not found on {BUNDLE_ROOT}!");
        }
        else
        {
            ShowError("An unexpected error occured: " + m_runningRequest.webRequest.error);
        }
    }

    /// <summary>
    /// Called after a scene was loaded.
    /// If current loaded scene is the LoaderScene, loading process will be startet via DoSceneDownload().
    /// If current loaded scene is from the AssetBundle, will call OnBundleSceneLoaded function of the GameManager.
    /// </summary>
    /// <param name="_loaded">Current loaded scene</param>
    /// <param name="_mode">Unused parameter</param>
    private void OnSceneLoaded(Scene _loaded, LoadSceneMode _mode)
    {
        if (string.IsNullOrEmpty(m_loadSceneName)) return;

        if(_loaded.name == LOADER_SCENE_NAME)
        {
            DoSceneDownload();
            return;
        }

        if(_loaded.name != m_loadSceneName)
        {
            if(_loaded.buildIndex == -1)
                ShowError($"Requested scene {m_loadSceneName} but got {_loaded.name}. Check your config!");
            return;
        }

        try
        {
            GameManager.Instance?.OnBundleSceneLoaded();
        }
        finally
        {
            m_loadSceneName = string.Empty;
        }
    }
}
