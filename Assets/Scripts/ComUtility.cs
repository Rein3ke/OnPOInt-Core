using System.Runtime.InteropServices;
using UnityEngine;

using IntercomCallbacks = System.Collections.Generic.Dictionary<EProtocolObjectType, System.Action<ProtocolObject>>;

public class ComUtility : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void OnDataReceived(string _jsonString);

    private static IntercomCallbacks s_intercomCallbacks = new IntercomCallbacks();

    public static void Send(ProtocolObject _pObject)
    {
        TrySerializeAndCall(_pObject, OnDataReceived);
    }

    public static void RegisterIntercomCallback(EProtocolObjectType _type, System.Action<ProtocolObject> _callback)
    {
        if (s_intercomCallbacks.ContainsKey(_type)) throw new System.ArgumentException("_type provided is invalid as a callback has already been registered for it.");
        s_intercomCallbacks.Add(_type, _callback);
    }

    private static void TrySerializeAndCall(ProtocolObject _pObject, System.Action<string> _jsCallback)
    {
        try
        {
            if (_pObject == null) throw new System.ArgumentException("_pObject is null");
            if (_jsCallback == null) throw new System.ArgumentException("_jsCallback is null");

            var result = _pObject.Serialize();
            if (string.IsNullOrEmpty(result)) throw new System.Exception("Serialization data is null");
            Debug.Log("TrySerializeAndCall: " + result);
            _jsCallback(result);
        }
#if UNITY_EDITOR
        catch(System.EntryPointNotFoundException)
        {
            //Ignore, because we are in the editor
        }
#endif
        catch (System.Exception _e)
        {
            Debug.LogError("Serialization error occurred while serializing (" + _e.GetType().Name + "): " + _e.Message);
        }
    }

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private void OnWebDataReceived(string _jsonString)
    {
        var pObject = ProtocolObject.Deserialize(_jsonString);
        if (!s_intercomCallbacks.ContainsKey(pObject.Type))
        {
            Debug.LogWarning("Invalid PO type received: " + pObject.Type + " no callback registered for this type.");
            return;
        }
        s_intercomCallbacks[pObject.Type](pObject);
    }
}
