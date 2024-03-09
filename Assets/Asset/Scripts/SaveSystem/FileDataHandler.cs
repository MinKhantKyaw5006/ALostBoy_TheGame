using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

/*
public class FileDataHandler 
{
    private string dataDirPath = "";
    private string dataFileName = "";

    public FileDataHandler(string dataDirPath, string dataFileName)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
    }

    public GameData Load()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        GameData loadedData = null;
        if (File.Exists(fullPath))
        {
            try
            {
                //load the serialized data from the file
                string dataToLoad = "";
                using(FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using(StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                //deserialize the data from json back into the C# object
                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch(Exception e)
            {
                Debug.LogError("Error occured  when trying to  load data from file: " + fullPath + "\n" + e);

            }

        }
        return loadedData;
    }

    public void Save(GameData data)
    {
        //use Path.Combine to account for different OS's having differnt path
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        try
        {
            //create the directory the file will be written to if it doesnt already exist
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            //serialize  the C# game data object into json
            string dataToStore = JsonUtility.ToJson(data, true);

            //write the serialized data to the file
            using(FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error occured when trying to save data to file" + fullPath + "\n" + e);
        }
    }
}
*/

public class FileDataHandler
{
    private string dataDirPath = "";
    private string dataFileName = "";

    public FileDataHandler(string dataDirPath, string dataFileName)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
    }

    public GameData Load(string profileId)
    {
        //base case - if the profileid is null, return right away
        if(profileId == null)
        {
            return null;
        }

        //use Path.Combine to account for different OS's having differnt path
        string fullPath = Path.Combine(dataDirPath, profileId, dataFileName);
        GameData loadedData = null;
        if (File.Exists(fullPath))
        {
            try
            {
                //load the serialized data from the file
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                //deserialize the data from json back into the C# object
                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError("Error occured  when trying to  load data from file: " + fullPath + "\n" + e);

            }

        }
        return loadedData;
    }

    public void Save(GameData data, string profileId)
    {
        //base case - if the profileid is null,  return right away
        if(profileId == null)
        {
            return;
        }

        //use Path.Combine to account for different OS's having differnt path
        string fullPath = Path.Combine(dataDirPath, profileId, dataFileName);
        try
        {
            //create the directory the file will be written to if it doesnt already exist
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            //serialize  the C# game data object into json
            string dataToStore = JsonUtility.ToJson(data, true);

            //write the serialized data to the file
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
            Debug.LogError("Error occured when trying to save data to file" + fullPath + "\n" + e);
        }
    }

    //load all profile method
    public Dictionary<string, GameData> LoadAllProfiles()
    {
        Dictionary<string, GameData> profileDictionary = new Dictionary<string, GameData>();

        //loop over all directory names in the data  directory path
        IEnumerable<DirectoryInfo> dirInfos = new DirectoryInfo(dataDirPath).EnumerateDirectories();
        foreach (DirectoryInfo dirInfo in dirInfos)
        {
            string profileId = dirInfo.Name;

            //defensive programming - check if the data file exists
            //if it doesn't, this this folder isn't a profile  and should be skipped
            string fullPath = Path.Combine(dataDirPath, profileId, dataFileName);
            if (!File.Exists(fullPath))
            {
                Debug.LogWarning("Skipping directory when loading all profiles because it does not contain data: " + profileId);
                //continue to next dir
                continue;
            }

            //load the game data for this profile and put it in directory
            GameData profileData = Load(profileId);

            //defensive programming - ensure the profile data isn't null,
            //because if it is then something went wrong and  we should let ourselves know
            if(profileData != null)
            {
                profileDictionary.Add(profileId, profileData);
            }
            else
            {
                Debug.LogError("Tried to load profile but something went wrong . ProfileId" + profileId);
            }
                
        }

        return profileDictionary;
    }

    public string GetMostRecentlyUpdatedProfile()
    {
        string mostRecentprofileId = null;

        Dictionary<string, GameData> profilesGameData = LoadAllProfiles();
        foreach(KeyValuePair<string, GameData> pair in profilesGameData)
        {
            string profileId = pair.Key;
            GameData gameData = pair.Value;

            //defensive check
            //skip this entry if the gamedata is null
            if(gameData == null)
            {
                continue;
            }

            //if this is the first data we've come across that exists, its the most recent so far
            if(mostRecentprofileId== null)
            {
                mostRecentprofileId = profileId;
            }
            //otherwise, compare to see which date is the most recent
            else
            {
                DateTime mostRecentDataTime = DateTime.FromBinary(profilesGameData[mostRecentprofileId].lastUpdated);
                DateTime newDateTime = DateTime.FromBinary(gameData.lastUpdated);
                //the greatest DateTime value is the most recent
                if(newDateTime > mostRecentDataTime)
                {
                    mostRecentprofileId = profileId;
                }
            }
            
        }
        return mostRecentprofileId;
    }

}