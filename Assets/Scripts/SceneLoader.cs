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

    private Text m_loadingText;

    [SerializeField] private GameObject playerPrefab;

    private void Awake()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

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

    private void Update()
    {
        if (m_runningRequest == null) return;
        if (m_runningRequest.isDone)
        {
            try
            {
                if (m_runningRequest.webRequest.isNetworkError || m_runningRequest.webRequest.isHttpError)
                {
                    OnDownloadFailed(m_runningRequest.webRequest.error, m_runningRequest.webRequest.responseCode);
                }
                else
                {
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

        if(m_loadingText != null && m_runningRequest.progress > 0f)
        {
            m_loadingText.text = $"Loading scene {m_loadingSceneID} ... {(int)(m_runningRequest.progress * 100f)}%";
        }
    }

    private void DoSceneDownload()
    {
        m_loadingText = FindObjectOfType<Text>();
        if (m_loadingText)
        {
            m_loadingText.color = Color.black;
            m_loadingText.text = "Initializing download...";
        }

        var url = $"{BUNDLE_ROOT}/{LOAD_SCENE_PREFIX}{m_loadingSceneID}";

        Debug.Log("Requesting bundle from " + url);
        var request = UnityWebRequestAssetBundle.GetAssetBundle(url);

        m_runningRequest = request.SendWebRequest();
        // TODO: Check documentation if necessary
        m_runningRequest.allowSceneActivation = true;
    }

    private void ShowError(string _error)
    {
        m_loadingText.text  = _error;
        m_loadingText.color = Color.red;
    }

    private void OnBundleDownloaded(AssetBundle _bundle)
    {
        Debug.Log("OnBundleDownloaded: Bundle downloaded!");
        if (_bundle == null)
        {
            ShowError("Failed to load scene!");
            return;
        }

        Debug.Log("Loading scene...");
        SceneManager.LoadScene(m_loadSceneName);
    }

    private void OnDownloadFailed(string _err, long _responseCode)
    {
        Debug.LogWarning("OnDownloadFailed: Failed to load scene!");
        if (m_runningRequest.webRequest.responseCode == 404)
        {
            ShowError($"Scene {m_loadSceneName} requested, but not found on {BUNDLE_ROOT}!");
        }
        else
        {
            ShowError("Oh noes :( An unexpected error occured: " + m_runningRequest.webRequest.error);
        }
    }

    private void OnSceneLoaded(Scene _loaded, LoadSceneMode _mode)
    {
        if (string.IsNullOrEmpty(m_loadSceneName)) return;
        Debug.Log($"OnSceneLoaded: Szene {_loaded.name} [{_loaded.buildIndex}]");

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
            Debug.Log("Scene " + _loaded.name + " loaded!");
            GameObject respawn = GameObject.FindGameObjectWithTag("Respawn");
            Instantiate(playerPrefab, respawn.transform.position, respawn.transform.rotation);

            // TODO: Check if it's really necessary
            //var rend        = GameObject.FindObjectsOfType<MeshRenderer>();
            //var defShader   = Shader.Find("Standard");
            //foreach (var r in rend)
            //{
            //    r.material.shader = defShader;
            //}
        }
        finally
        {
            m_loadSceneName = string.Empty;
        }
    }
}
