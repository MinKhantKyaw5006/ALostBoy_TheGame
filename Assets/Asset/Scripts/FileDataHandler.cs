using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class FileDataHandler 
{
    private string dataDirPath = "";
    private string dataFileName = "";

    private readonly string backupExtension =  ".bak";

    public FileDataHandler(string dataDirPath, string dataFileName)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
            
    }

    public GameData Load(string profileId, bool allowRestoreFromBackup = true)
    {
        //base case - if the profileId is null, return right away
        if (profileId == null)
        {
            return null;
        }

        string fullPath = Path.Combine(dataDirPath,profileId, dataFileName);
        GameData loadedData = null;
            if (File.Exists(fullPath))
        {
            try
            {
                //load serialied data to file
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();    
                    }
                }

                //deserialize the data to C#
                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch(Exception e)
            {
                //since we are calling load(..) recursively, we need to account for the case where
                //the rollback succeeds, but the data is still failing to load for some other reason,   
                //which without this check may cause an infinite recursion loop.    
                if (allowRestoreFromBackup)
                {
                    Debug.LogWarning("Failed to load data file. Attempting to roll back.\n" + e);
                    bool rollbackSuccess = AttemptRollBack(fullPath);
                    if (rollbackSuccess)
                    {
                        //try to load again recursively
                        loadedData = Load(profileId, false);
                    }
                }
                //if we hit this else block, one possibility is that the backup file is also corrupt
                else
                {
                    Debug.LogError("Error occured when trying to load file at path"
                        + fullPath + " and backup did not work.\n" + e);
                }
              
            }
            
        }
        return loadedData;
    }

    public void Save(GameData data, string profileId)
    {

        //base case - if the profileId is null, return right away
        if (profileId == null)
        {
            return ;
        }

        string fullPath = Path.Combine(dataDirPath,profileId, dataFileName);
        string backupFilePath = fullPath + backupExtension;
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            //seriallize
            string dataToStore = JsonUtility.ToJson(data, true);

            //write serialize data to file
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }

            //verify the newly saved file can be loaded successfully
            GameData verifiedGameData = Load(profileId);
            // if the data can be verified, back it up
            if (verifiedGameData != null)
            {
                File.Copy(fullPath, backupFilePath, true);
            }
            //otherwise, something went wrong and we should throw an exception
            else
            {
                throw new Exception("Save file could not be verified and backup could not  be  created.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error  occured when trying to save the data to file:" + fullPath + "\n" + e);

        }
    }

    public Dictionary<string, GameData> LoadAllProfiles()
    {
        Dictionary<string, GameData> profileDictionary = new Dictionary<string, GameData>();

        //loop over all directory names in  the data  dir path
        IEnumerable<DirectoryInfo> dirInfos = new DirectoryInfo(dataDirPath).EnumerateDirectories();
        foreach (DirectoryInfo dirInfo in dirInfos)
        {
            string profileId = dirInfo.Name;
            //defensive programming - check if   the data file exist;
            //if it doesn't, then this folder isn't a profile and should be skipped;
            string fullPath = Path.Combine(dataDirPath, profileId, dataFileName);
            if (!File.Exists(fullPath))
            {
                Debug.LogWarning("Skipping directory when loading all profiles  because  it does not contain  data:"
                    + profileId);
                continue;

            }

            //load the game data for this profile and put it in the directory
            GameData profileData = Load(profileId);
            // defensive programming - ensure the profile data isn't null,
            // because if it is then something went wrong and we should let ourselves know
            if (profileData != null)
            {
                profileDictionary.Add(profileId, profileData);
            }
            else
            {
                Debug.LogError("Tried to load profile but something went wrong: profileid " + profileId);
            }
        }

        return profileDictionary;
    }

    public string GetMostRecentlyUpdatedProfileId()
    {
        string mostRecentProfileId = null;

        Dictionary<string, GameData> profilesGameData = LoadAllProfiles();
        foreach (KeyValuePair<string, GameData> pair in profilesGameData)
        {
            string profileId = pair.Key;
            GameData gameData = pair.Value;

            //skip this game entry if game data is null
            if (gameData == null)
            {
                continue;
            }

            //if this is the first data we've come across that exists, its the most recent so far
            if(mostRecentProfileId == null)
            {
                mostRecentProfileId = profileId;
            }
            //otherwise, compare to see which date is the most recent
            else
            {
                DateTime mostRecentDateTime = DateTime.FromBinary(profilesGameData[mostRecentProfileId].lastUpdated);
                DateTime newDateTime = DateTime.FromBinary(gameData.lastUpdated);
                //the greatest DateTime value is  the most recent
                if (newDateTime> mostRecentDateTime)
                {
                    mostRecentProfileId = profileId;
                }
            }
        }
        return mostRecentProfileId;
    }

    public void Delete(string profileId)
    {
        //base case - if  the profileId is  null, return right away
        if(profileId == null)
        {
            return;
        }

        string fullPath = Path.Combine(dataDirPath, profileId, dataFileName);
        try
        {
            //ensure the data file exists at this path before deleting the directory
            if (File.Exists(fullPath))
            {
                Directory.Delete(Path.GetDirectoryName(fullPath), true);
            }
            else
            {
                Debug.LogWarning("Tried to delete the data but data was not found at path:" + fullPath);
            }
            
        }
       
        catch (Exception e)
        {
            Debug.LogError("Failed to delete  profile data for profileId" + profileId + "at path: " + fullPath + "\n" + e);
        }
    }

    private bool AttemptRollBack(string fullPath)
    {
        bool success = false;
        string backupFilePath = fullPath + backupExtension;
        try
        {
            //if the file exists, attempt to roll back to it by overwriting the original file    
            if (File.Exists(backupFilePath))
            {
                File.Copy(backupFilePath, fullPath, true);
                success = true;
                Debug.LogWarning("had to roll back to  backup file at" + backupFilePath);
            }
            //otherwise, we don't yet have a backup file - so  there's nothing to roll back to 
            else
            {
                throw new Exception("Tried to roll back, but no backup file exists to roll back to.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error occured when trying to roll back to back file at: " + backupFilePath + "\n" + e);
        }

        return success;
    }


}
