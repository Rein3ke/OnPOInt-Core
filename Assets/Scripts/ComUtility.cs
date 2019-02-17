using System.Runtime.InteropServices;
using UnityEngine;

public class ComUtility
{
    [DllImport("__Internal")]
    private static extern void OnDataReceived(string _jsonString);

    public static void Send(ProtocolObject _pObject)
    {
        TrySerializeAndCall(_pObject, OnDataReceived);
    }

    private static void TrySerializeAndCall(ProtocolObject _pObject, System.Action<string> _jsCallback)
    {
        try
        {
            if (_pObject == null) throw new System.ArgumentException("_pObject is null");
            if (_jsCallback == null) throw new System.ArgumentException("_jsCallback is null");

            var result = _pObject.Serialize();
            if (string.IsNullOrEmpty(result)) throw new System.Exception("Serialization data is null");
            _jsCallback(result);
        }
        catch (System.Exception _e)
        {
            Debug.LogError("Serialization error occurred while serializing " + _pObject.GetType().Name + ": " + _e.Message);
        }
    }
}
