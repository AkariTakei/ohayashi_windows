using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class SaveDataManager
{
    [System.Serializable]
    public class SaveData
    {
        public SongData[] songData;
    }

    [System.Serializable]
    public class SongData
    {
        public string songName;

        public InstrumentData[] instrumentData;
    }

    [System.Serializable]
    public class InstrumentData
    {
        public string instrumentName;
        public LevelData[] levelData;
    }

    [System.Serializable]
    public class LevelData
    {
        public string levelName;
        public int playNum;
        public int combo;
        public int score;
        public int fullComboNum;
    }

    public static void Save(SaveData data)
    {
        StreamWriter writer;
        string jsonstr = JsonUtility.ToJson(data);

        writer = new StreamWriter(Application.persistentDataPath + "/savedata.json", false);
        Debug.Log(Application.persistentDataPath + "/savedata.jsonにセーブしました。");
        writer.Write(jsonstr);
        writer.Flush();
        writer.Close();
    }

    public static SaveData Load()
    {
        string data = "";
        StreamReader reader;

        reader = new StreamReader(Application.persistentDataPath + "/savedata.json");
        data = reader.ReadToEnd();
        reader.Close();

        Debug.Log(Application.persistentDataPath + "/savedata.jsonをロードしました。");

        return JsonUtility.FromJson<SaveData>(data);
    }


    public static void ResetData()
    {
        // 初期状態のデータをセーブ
        SaveData data = new SaveData();
        data.songData = new SongData[4];// 4曲分のデータを用意
        data.songData[0] = new SongData(); // 1曲目のデータを用意
        data.songData[0].songName = "ninnba";
        data.songData[1] = new SongData(); // 2曲目のデータを用意
        data.songData[1].songName = "yatai";
        data.songData[2] = new SongData(); // 3曲目のデータを用意
        data.songData[2].songName = "nakanokiri";
        data.songData[3] = new SongData(); // 4曲目のデータを用意
        data.songData[3].songName = "ti-hyaitoro";

        for (int i = 0; i < data.songData.Length; i++)
        {
            data.songData[i].instrumentData = new InstrumentData[3];// 3楽器分のデータを用意
            data.songData[i].instrumentData[0] = new InstrumentData(); // 1楽器目のデータを用意
            data.songData[i].instrumentData[0].instrumentName = "tuke";
            data.songData[i].instrumentData[1] = new InstrumentData(); // 2楽器目のデータを用意
            data.songData[i].instrumentData[1].instrumentName = "ookan";
            data.songData[i].instrumentData[2] = new InstrumentData(); // 3楽器目のデータを用意
            data.songData[i].instrumentData[2].instrumentName = "kane";

            for (int j = 0; j < data.songData[i].instrumentData.Length; j++)
            {
                data.songData[i].instrumentData[j].levelData = new LevelData[3];// 3難易度分のデータを用意
                data.songData[i].instrumentData[j].levelData[0] = new LevelData(); // 1難易度目のデータを用意
                data.songData[i].instrumentData[j].levelData[0].levelName = "easy";
                data.songData[i].instrumentData[j].levelData[1] = new LevelData(); // 2難易度目のデータを用意
                data.songData[i].instrumentData[j].levelData[1].levelName = "normal";
                data.songData[i].instrumentData[j].levelData[2] = new LevelData(); // 3難易度目のデータを用意
                data.songData[i].instrumentData[j].levelData[2].levelName = "hard";

                for (int k = 0; k < data.songData[i].instrumentData[j].levelData.Length; k++)
                {
                    data.songData[i].instrumentData[j].levelData[k].playNum = 0;
                    data.songData[i].instrumentData[j].levelData[k].combo = 0;
                    data.songData[i].instrumentData[j].levelData[k].score = 0;
                    data.songData[i].instrumentData[j].levelData[k].fullComboNum = 0;
                }
            }
        }
        Save(data);
    }

}
