using System.IO;
using UnityEditor;
using UnityEngine;

public class CreateJSONDatabase
{
    private const string POI_DATA_PATH = "Assets/Resources/poi_data/poi_lib.json";

    [MenuItem("Tools/Create database")]
    private static void CreateDatabase()
    {
        POIDataSet data = 
        new POIDataSet(
            new []
            {
                new POIData(
                    0, 
                    "Awesome POI", 
                    "So awesome. Really awesome!",
                    new [] 
                    {
                        "test_image"
                    },
                    new []
                    {
                        "https://google.com"
                    }),
                new POIData(
                    1,
                    "Another POI",
                    "Not so awesome. But still quite cool!",
                    new []
                    {
                        "other_test_image"
                    },
                    new []
                    {
                        "https://revvis.com"
                    })
            });

        var jString = JsonUtility.ToJson(data);
        File.WriteAllText(POI_DATA_PATH, jString);
        AssetDatabase.Refresh();
        Debug.Log("POI database created @ " + POI_DATA_PATH);
    }
}
