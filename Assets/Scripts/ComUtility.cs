using System.Runtime.InteropServices;

public class ComUtility
{
    [DllImport("__Internal")]
    public static extern void OnPositionDataReceived(string _jsonString);
}
