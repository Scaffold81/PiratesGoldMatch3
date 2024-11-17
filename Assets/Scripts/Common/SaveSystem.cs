using UnityEngine;

public static class SaveSystem
{
    public static bool HasKeyData(string saveSlot)
    {
        return PlayerPrefs.HasKey(saveSlot);
    }

    public static void SaveData(string saveSlot, string data)
    {
        PlayerPrefs.SetString(saveSlot, data);
    }
    
    public static string LoadData(string saveSlot)
    {
        if (PlayerPrefs.HasKey(saveSlot))
        {
            return PlayerPrefs.GetString(saveSlot);
        }
        else
        {
            Debug.LogError(saveSlot +" "+ "PlayerPrefs.HasKey value is not valid");
            return null;
        }
    }

    public static void DeleteData(string saveSlot)
    {
        PlayerPrefs.DeleteKey(saveSlot);
    }

   
}
