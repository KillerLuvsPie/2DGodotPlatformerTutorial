using Godot;
using System.IO;
using System;

public static class SaveLoadManager
{
    private static readonly string FileDirectory = OS.GetUserDataDir() + "/saves";
    private const string FileName = "/save.sav";

    private static void CreateSaveFolder()
    {
        if(!Directory.Exists(OS.GetUserDataDir() + "/saves"))
            Directory.CreateDirectory(FileDirectory);
    }

    public static void Save()
    {
        CreateSaveFolder();
        FileStream fs = new FileStream(FileDirectory + FileName, FileMode.Create);
        BinaryWriter bw = new BinaryWriter(fs);

        //SETTINGS VARS
        bw.Write(Global.windowOptionIndex);
        bw.Write(Global.currentResolution);
        bw.Write(Global.musicVolumePercent);
        bw.Write(Global.sfxVolumePercent);

        //GAME VARS
        bw.Write(Global.UnlockedLevelIndex);
        foreach(double entry in Global.levelTimeRecords.Values)
                bw.Write(entry);
        foreach(int entry in Global.levelDeathRecords.Values)
            bw.Write(entry);

        bw.Close();
        fs.Close();
        GD.Print("Game Saved");
    }
    public static void Load()
    {
        FileStream fs = new FileStream(FileDirectory + FileName, FileMode.Open);
        BinaryReader br = new BinaryReader(fs);
        try
        {
            //SETTINGS VARS
            Global.windowOptionIndex = br.ReadInt32();
            Global.currentResolution = br.ReadString();
            Global.musicVolumePercent = br.ReadSingle();
            Global.sfxVolumePercent = br.ReadSingle();

            //GAME VARS
            Global.UnlockedLevelIndex = br.ReadInt32();
            for(int i = 1; i <= Global.levelTotal; i++)
                Global.levelTimeRecords.Add(i, br.ReadDouble());
            for(int i = 1; i <= Global.levelTotal; i++)
                Global.levelDeathRecords.Add(i, br.ReadInt32());
        }
        catch
        {
            GD.Print("Save file format mismatch");
        }
        br.Close();
        fs.Close();
        GD.Print("Game Loaded");
    }

    public static bool CheckIfSaveExists()
    {
        return File.Exists(FileDirectory + FileName);
    }
}
