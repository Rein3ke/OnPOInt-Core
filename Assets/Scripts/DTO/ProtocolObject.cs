using UnityEngine;

[System.Serializable]
public class ProtocolObject
{
    public EProtocolObjectType Type;

    public ProtocolObject() { }
    public ProtocolObject(EProtocolObjectType _type)
    {
        Type = _type;
    }

    public static ProtocolObject Deserialize(string _jsonString)
    {
        var obj = JsonUtility.FromJson<ProtocolObject>(_jsonString);
        switch(obj.Type)
        {
            default: throw new System.InvalidOperationException("UNITY ERROR: Invalid protocol type supplied!");
            case EProtocolObjectType.POI:
                return JsonUtility.FromJson<POIProtocolObject>(_jsonString);
            case EProtocolObjectType.SCENE_CHANGE:
                return JsonUtility.FromJson<SceneChangeProtocolObject>(_jsonString);
        }
    }

    public string Serialize()
    {
        var serializedObj = JsonUtility.ToJson(this, false);
        //Debug.Log(serializedObj);
        return serializedObj;
    }
}

/// <summary>
/// Geht vom unity build raus an javascript.
/// Unity -> JS
/// </summary>
[System.Serializable]
public class POIProtocolObject : ProtocolObject
{
    public int ID;

    public POIProtocolObject() { }
    public POIProtocolObject(int _ID)
    {
        ID = _ID;
    }

    public static POIProtocolObject None => 
    new POIProtocolObject()
    {
        Type = EProtocolObjectType.POI,
        ID = -1
    };
}

/// <summary>
/// Protocol for changing the present scene.
/// JS -> Unity
/// </summary>
[System.Serializable]
public class SceneChangeProtocolObject : ProtocolObject
{
    public string SceneID;
    public string SceneName;

    public SceneChangeProtocolObject() { }
    public SceneChangeProtocolObject(string _sceneID, string _sceneName)
        : base(EProtocolObjectType.SCENE_CHANGE)
    {
        SceneID     = _sceneID;
        SceneName   = _sceneName;
    }
}

/// <summary>
/// Protocol type. E.g. Point of Interest-Protocol
/// </summary>
[System.Serializable]
public enum EProtocolObjectType
{
    NONE            = 0,
    POI             = 1,
    SCENE_CHANGE    = 2,
}