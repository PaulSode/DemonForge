using UnityEngine;
using System;
using System.IO;
using Path = System.IO.Path;

public class FileDataHandler
{
    private string dataDirPath = "";

    private string dataFileName = "";

    public FileDataHandler(string dirPath, string fileName)
    {
        dataDirPath = dirPath;
        dataFileName = fileName;
    }

    public GameData Load()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        GameData gameData = null;

        if (File.Exists(fullPath))
        {
            try
            {
                string loadedData = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        loadedData = reader.ReadToEnd();
                        Debug.Log(loadedData);
                    }
                }

                gameData = JsonUtility.FromJson<GameData>(loadedData);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to load from " + fullPath + " : " + e);
                throw;
            }
        }

        return gameData;
    }

    public void Save(GameData data)
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            string dataToStore = JsonUtility.ToJson(data, true);

            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error while trying to save to " + fullPath + " : " + e);
            throw;
        }
    }
}
