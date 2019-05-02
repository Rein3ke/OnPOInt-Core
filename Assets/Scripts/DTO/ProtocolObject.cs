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
    /// A static method for deserializing a JSON string as a ProtocolObject.
    /// The ProtocolObject will then be returned.
    /// </summary>
    /// <param name="_jsonString">JSON string that defines a ProtocolObject</param>
    /// <returns>Deserialized ProtocolObject</returns>
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
            case EProtocolObjectType.SPEED_CHANGE:
                return JsonUtility.FromJson<SpeedChangeProtocolObject>(_jsonString);
            case EProtocolObjectType.SENS_CHANGE:
                return JsonUtility.FromJson<SensibilityChangeProtocolObject>(_jsonString);
        }
    }

    /// <summary>
    /// A method to serialize the previously declared ProtocolObject.
    /// </summary>
    /// <returns>The serialized ProtocolObject as a JSON string</returns>
    public string Serialize()
    {
        var serializedObj = JsonUtility.ToJson(this, false);
        return serializedObj;
    }
}

/// <summary>
/// Association of the ProtocolObject. Classifies the protocol as a POIProtocolObject and contains the ID of the POI.
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
/// Association of the ProtocolObject. Classifies the protocol as a SceneChangeProtocolObject and contains the ID and Name of the Scene.
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
/// Association of the ProtocolObject. Classifies the protocol as a SpeedChangeProtocolObject and contains the speed value of the player.
/// Intended for communication with Unity
/// </summary>
[System.Serializable]
public class SpeedChangeProtocolObject : ProtocolObject
{
    public int Value;

    public SpeedChangeProtocolObject() { }
    public SpeedChangeProtocolObject(int _value)
        : base(EProtocolObjectType.SPEED_CHANGE)
    {
        Value = _value;
    }
}

/// <summary>
/// Association of the ProtocolObject. Classifies the protocol as a SensibilityChangeProtocolObject and contains the sensibility value.
/// Intended for communication with Unity
/// </summary>
[System.Serializable]
public class SensibilityChangeProtocolObject : ProtocolObject
{
    public int Value;

    public SensibilityChangeProtocolObject() { }
    public SensibilityChangeProtocolObject(int _value)
        : base(EProtocolObjectType.SENS_CHANGE)
    {
        Value = _value;
    }
}

/// <summary>
/// Classify the protocol types into categories (e.g. POI-Protocol)
/// </summary>
[System.Serializable]
public enum EProtocolObjectType
{
    NONE            = 0,
    POI             = 1,
    SCENE_CHANGE    = 2,
    SPEED_CHANGE    = 3,
    SENS_CHANGE     = 4
}