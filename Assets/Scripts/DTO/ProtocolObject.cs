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
            case EProtocolObjectType.POS_UPDATE:
                return JsonUtility.FromJson<PositionProtocolObject>(_jsonString);
            case EProtocolObjectType.SPEED_CHANGE:
                return JsonUtility.FromJson<SpeedChangeProtocolObject>(_jsonString);
            case EProtocolObjectType.SCENE_CHANGE:
                return JsonUtility.FromJson<SpeedChangeProtocolObject>(_jsonString);
        }
    }

    public string Serialize()
    {
        var serializedObj = JsonUtility.ToJson(this, false);
        //Debug.Log(serializedObj);
        return serializedObj;
    }
}

[System.Serializable]
public class POIProtocolObject : ProtocolObject
{
    public int ID;
    public string Name;
    public string Description;
    public byte[] Image;
    public string[] Links;

    public POIProtocolObject() { }
    public POIProtocolObject(int _ID, string _name, string _description, byte[] _image, string[] _links)
        : base(EProtocolObjectType.POI)
    {
        ID          = _ID;
        Name        = _name;
        Description = _description;
        Image       = _image;
        Links       = _links;
    }

    public static POIProtocolObject None => 
    new POIProtocolObject()
    {
        Type = EProtocolObjectType.POI,
        ID = -1
    };
}

[System.Serializable]
public class PositionProtocolObject : ProtocolObject
{
    public Vector3 Position;
    public Vector3 Rotation;

    public PositionProtocolObject() { }
    public PositionProtocolObject(Vector3 _position, Quaternion _rotation)
        : base(EProtocolObjectType.POS_UPDATE)
    {
        Position = _position;
        Rotation = _rotation.eulerAngles;
    }
}

// TEST PROTOCOL
[System.Serializable]
public class SpeedChangeProtocolObject : ProtocolObject
{
    public float NewSpeed;
    
    public SpeedChangeProtocolObject() { }
    public SpeedChangeProtocolObject(float _newSpeed)
        : base(EProtocolObjectType.SPEED_CHANGE)
    {
        NewSpeed = _newSpeed;
    }
}

/// <summary>
/// Protocol for changing the present scene.
/// </summary>
[System.Serializable]
public class SceneChangeProtocolObject : ProtocolObject
{
    public string SceneName;

    public SceneChangeProtocolObject() { }
    public SceneChangeProtocolObject(string _sceneName)
        : base(EProtocolObjectType.SCENE_CHANGE)
    {
        SceneName = _sceneName;
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
    POS_UPDATE      = 2,
    SPEED_CHANGE    = 3,
    SCENE_CHANGE    = 4,
}