using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveableObject : MonoBehaviour
{
    protected string save;
    [SerializeField]
    public string prefabLocation;

    void Start()
    {
        if(prefabLocation != "")
            SaveGameManager.Instance.SaveableObejcts.Add(this);
        else {
            Debug.LogWarning("Missing Prefab Location! " + name);
        }
    }

    public virtual string Save(int id){
        string objectData = 
            prefabLocation + "~" + 
            name + "~" + 
            transform.position.ToString() + "~" + 
            transform.rotation.ToString();
        PlayerPrefs.SetString( id.ToString(), objectData );
        return objectData;
    }
    public virtual void Load(string[] values){
        name = values[1];
        transform.position = SaveGameManager.Instance.StringToVector(values[2]);
        transform.rotation = SaveGameManager.Instance.StringToQuaternion(values[3]);
    }
    public virtual void DestroySaveable(){
        SaveGameManager.Instance.SaveableObejcts.Remove(this);
        Destroy(this);
    }

}
