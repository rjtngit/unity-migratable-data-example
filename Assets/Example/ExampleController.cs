using System.Collections;
using System.Collections.Generic;
using RJ;
using UnityEngine;

public class ExampleController : MonoBehaviour
{
    public TextAsset dataBefore;
    
    void Start()
    { 
        Debug.Log($"before: {dataBefore.text}"); 
        var dataObject = MigratableDataProcessor.FromJson<ExampleData.Example>(dataBefore.text);
        var dataAfter = MigratableDataProcessor.ToJson(dataObject);
        Debug.Log($"after: {dataAfter}");
    }

}
