using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public List<string> objectData = new List<string>();
}

public class SaveGameManager : MonoBehaviour
{
    private static SaveGameManager instance;
    public List<SaveableObject> SaveableObejcts { get; private set; }
    public static SaveGameManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<SaveGameManager>();
            return instance;
        }
    }

    void Awake()
    {
        SaveableObejcts = new List<SaveableObject>();
    }

    public void Save()
    {
        PlayerPrefs.SetInt("ObjectCount", SaveableObejcts.Count);
        for (int i = 0; i < SaveableObejcts.Count; i++)
        {
            SaveableObejcts[i].Save(i);
        }

        Debug.Log("Saved " + SaveableObejcts.Count + " Objects!");
    }

    public void Load()
    {
        foreach (SaveableObject obj in SaveableObejcts)
        {
            if (obj != null)
            {
                Destroy(obj.gameObject);
            }
        }
        SaveableObejcts.Clear();

        int objectCount = PlayerPrefs.GetInt("ObjectCount");
        for (int i = 0; i < objectCount; i++)
        {
            string[] values = PlayerPrefs.GetString(i.ToString()).Split("~");
            GameObject tmp = Instantiate(Resources.Load(values[0]) as GameObject);
            tmp.GetComponent<SaveableObject>().Load(values);
        }

        Debug.Log("Loaded " + objectCount + " Objects!");
    }

    public void SaveAsJSON(StringReference name)
    {
        if (name.value == "") return;
        string saveFile = Application.persistentDataPath + "/" + name.value + ".json";
        SaveData save = new SaveData();

        PlayerPrefs.SetInt("ObjectCount", SaveableObejcts.Count);
        for (int i = 0; i < SaveableObejcts.Count; i++)
        {
            save.objectData.Add(SaveableObejcts[i].Save(i));
        }
        Debug.Log("Saved " + SaveableObejcts.Count + " Objects!");

        string jsonString = JsonUtility.ToJson(save);
        File.WriteAllText(saveFile, jsonString);
    }

    public void LoadJSON(StringReference name)
    {
        if (name.value == "") return;
        string saveFile = Application.persistentDataPath + "/" + name.value + ".json";
        if (File.Exists(saveFile))
        {
            string fileContents = File.ReadAllText(saveFile);

            SaveData save = JsonUtility.FromJson<SaveData>(fileContents);

            foreach (SaveableObject obj in SaveableObejcts)
            {
                if (obj != null)
                {
                    Destroy(obj.gameObject);
                }
            }
            SaveableObejcts.Clear();

            foreach (string data in save.objectData)
            {
                string[] values = data.Split("~");
                Debug.LogWarning(values[0]);
                GameObject tmp = Instantiate(Resources.Load(values[0]) as GameObject);
                tmp.GetComponent<SaveableObject>().Load(values);
            }
            Debug.Log("Loaded " + save.objectData.Count + " Objects!");
        }
        else
        {
            Debug.LogError("No such save file! " + name.value + ".json");
        }
    }

    public Vector3 StringToVector(string value)
    {
        value = value.Trim(new char[] { '(', ')' });
        string[] components = value.Split(',');
        return new Vector3(
            float.Parse(components[0]),
            float.Parse(components[1]),
            float.Parse(components[2])
        );
    }

    public Quaternion StringToQuaternion(string value)
    {
        value = value.Trim(new char[] { '(', ')' });
        string[] components = value.Split(',');
        return new Quaternion(
            float.Parse(components[0]),
            float.Parse(components[1]),
            float.Parse(components[2]),
            float.Parse(components[3])
        );
    }
}
