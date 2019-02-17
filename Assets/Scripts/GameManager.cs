using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private const string POI_DATA_RES_PATH          = "poi_data/poi_lib";

    public static GameManager Instance              => s_instance;
    private static GameManager s_instance           = null;

    public POIDataSet POIData                       => m_poiDataSet;
    private POIDataSet m_poiDataSet;

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
        LoadPOIData();
        SceneManager.LoadScene("Scene 01");
    }

    private void LoadPOIData()
    {
        try
        {
            var textAssitFile   = Resources.Load<TextAsset>(POI_DATA_RES_PATH);
            m_poiDataSet        = JsonUtility.FromJson<POIDataSet>(textAssitFile.text);
        }
        catch (System.Exception _e)
        {
            Debug.LogError($"Failed to load poi data set ({_e.GetType().Name}): {_e.Message}");
            return;
        }
    }

    public POIData GetPOIByID(int _ID)
    {
        //foreach(var dat in POIData.Data)
        //{
        //    if (dat.ID == _ID) return dat;
        //}
        //return null;
        return POIData.Data.FirstOrDefault(_object => _object.ID == _ID);
    }

    //public Texture2D[] LoadPOIImageData(POIData _data)
    //{
    //    var images = new List<Texture2D>();
    //    foreach (var path in _data.ImagePath)
    //    {
    //        var img = Resources.Load<Texture2D>("poi_images/" + path);
    //        if (img != null)
    //        {
    //            images.Add(img);
    //        }
    //    }
    //    Debug.Log(images.Count + " images loaded for POI " + _data.ID);
    //    return images.ToArray();
    //}

    private void OnDestroy()
    {
        if(s_instance == this)
            s_instance = null;
    }
}
