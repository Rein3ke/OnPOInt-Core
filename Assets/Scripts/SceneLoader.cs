using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(GameManager))]
public class SceneLoader : MonoBehaviour
{
    public const string LOADER_SCENE_NAME   = "Loader";
    public const string BUNDLE_ROOT         = "http://localhost:3000/data/scene_bundles";

    public bool IsLoading => m_runningRequest != null;

    private int m_loadingSceneID;
    private UnityWebRequestAsyncOperation m_runningRequest = null;

    private Text m_loadingText;

    public void LoadScene(int _ID)
    {
        if (IsLoading) return;
        m_loadingSceneID = _ID;
        var currScene = SceneManager.GetActiveScene();
        if (currScene.name != LOADER_SCENE_NAME)
        {
            SceneManager.LoadScene(LOADER_SCENE_NAME);
        }
        DoSceneLoad();
    }

    private void Update()
    {
        if (m_runningRequest == null) return;
        if (m_runningRequest.isDone)
        {
            if(m_runningRequest.webRequest.isNetworkError || m_runningRequest.webRequest.isHttpError)
            {
                OnDownloadFailed(m_runningRequest.webRequest.error, m_runningRequest.webRequest.responseCode);
            }
            else
            {
                AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(m_runningRequest.webRequest);
                OnBundleDownloaded(bundle);
            }

            m_runningRequest    = null;
            m_loadingText       = null;
            return;
        }

        if(m_loadingText != null && m_runningRequest.progress > 0f)
        {
            m_loadingText.text = $"Loading scene {m_loadingSceneID} ... {(int)(m_runningRequest.progress * 100f)}%";
        }
    }

    private void DoSceneLoad()
    {
        m_loadingText = FindObjectOfType<Text>();
        if (m_loadingText)
        {
            m_loadingText.color = Color.black;
            m_loadingText.text = "Initializing download...";
        }
        var url = $"{BUNDLE_ROOT}/scene_{m_loadingSceneID}";
        Debug.Log("Requesting bundle from " + url);
        var request = UnityWebRequestAssetBundle.GetAssetBundle(url);
        m_runningRequest = request.SendWebRequest();
        m_runningRequest.allowSceneActivation = true;
    }

    private void ShowError(string _error)
    {
        m_loadingText.text = _error;
        m_loadingText.color = Color.red;
    }

    private void OnBundleDownloaded(AssetBundle _bundle)
    {
        if (_bundle == null)
        {
            ShowError("Failed to load bundle!");
            return;
        }
        Debug.Log("Scene downloaded. Loading scene...");
        SceneManager.LoadScene("scene_" + m_loadingSceneID);
    }

    private void OnDownloadFailed(string _err, long _responseCode)
    {
        if (m_runningRequest.webRequest.responseCode == 404)
        {
            ShowError("I really tried! But couldn't find anything for this ID...");
        }
        else
        {
            ShowError("Oh noes :( An error occured: " + m_runningRequest.webRequest.error);
        }
    }
}
