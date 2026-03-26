
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class NewSaveData
{
    //public string savedSceneName;

    //public void SaveCurrentScene(int saveNum, string savedSceneName)
    //{
    //    NewSaveData savedData = new NewSaveData();
    //    string json = JsonUtility.ToJson(savedData);
    //    System.IO.File.WriteAllText(Application.persistentDataPath +"/"+savedSceneName+".json", json);
    //    Debug.Log("Saved scene to "+ Application.persistentDataPath + "/" + savedSceneName + ".json");
    //}

    //public void LoadSceneSaveData(int saveNum, string sceneName)
    //{
    //    string jsonData = System.IO.File.ReadAllText(Application.persistentDataPath + "/" + sceneName + ".json");
    //    NewSaveData savedData = JsonUtility.FromJson<NewSaveData>(jsonData);
    //    SceneManager.LoadSceneAsync(savedData.savedSceneName);
    //}

}