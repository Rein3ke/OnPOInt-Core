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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_jsonString"></param>
    /// <returns></returns>
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

    /// <summary>
    /// A method to serialize the previously declared ProtocolObject.
    /// </summary>
    /// <returns>The serialzied ProtocolObject as a JSON string</returns>
    public string Serialize()
    {
        var serializedObj = JsonUtility.ToJson(this, false);
        return serializedObj;
    }
}

/// <summary>
/// Association of the ProtocolObject. Classifies the protocol as a POI-object and contains the ID of the POI.
/// Intended for communication with JavaScript
/// </summary>
[System.Serializable]
public class POIProtocolObject : ProtocolObject
{
    public int ID;

    public POIProtocolObject() { }
    public POIProtocolObject(int _ID)
        : base(EProtocolObjectType.POI)
    {
        ID = _ID;
    }

    public static POIProtocolObject None => new POIProtocolObject(-1);
}

/// <summary>
/// Association of the ProtocolObject. Classifies the protocol as a Scene-object and contains the ID and Name of the Scene.
/// Intended for communication with Unity
/// </summary>
[System.Serializable]
public class SceneChangeProtocolObject : ProtocolObject
{
    public int SceneID;

    public SceneChangeProtocolObject() { }
    public SceneChangeProtocolObject(int _sceneID)
        : base(EProtocolObjectType.SCENE_CHANGE)
    {
        SceneID     = _sceneID;
    }
}

/// <summary>
/// Classify the protocol types into categories (e.g. POI Protocol)
/// </summary>
[System.Serializable]
public enum EProtocolObjectType
{
    NONE            = 0,
    POI             = 1,
    SCENE_CHANGE    = 2,
}