using System;
using System.IO;
using UnityEngine;

public static class SaveSystem
{
    const string FolderName = "saves";

    static string Dir => Path.Combine(Application.persistentDataPath, FolderName);

    public static string GetPath(int slot) => Path.Combine(Dir, $"slot_{slot + 1}.json");

    public static bool Exists(int slot) => File.Exists(GetPath(slot));

    public static bool Write(SaveData data, int slot)
    {
        try
        {
            Directory.CreateDirectory(Dir);
            string json = JsonUtility.ToJson(data, true);
            string path = GetPath(slot);
            string tmp = path + ".tmp";
            File.WriteAllText(tmp, json);
            if (File.Exists(path)) File.Delete(path);
            File.Move(tmp, path);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveSystem] Falha ao salvar o slot {slot + 1}: {e.Message}");
            return false;
        }
    }

    public static SaveData Read(int slot)
    {
        if (!Exists(slot)) return null;
        try
        {
            string json = File.ReadAllText(GetPath(slot));
            return JsonUtility.FromJson<SaveData>(json);
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveSystem] Falha ao ler o slot {slot + 1}: {e.Message}");
            return null;
        }
    }

    public static void Delete(int slot)
    {
        try
        {
            if (Exists(slot)) File.Delete(GetPath(slot));
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveSystem] Falha ao apagar o slot {slot + 1}: {e.Message}");
        }
    }
}
