using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class DataManager : CustomMonoBehaviour
{
   private static DataManager _instance;
   public static DataManager Instance => _instance;
   [SerializeField] Data _data;
   string _fileName = "data.dat";
   string _dataPath;
   public override void EditorInit()
   {

   }
   void Awake()
   {
      if (_instance == null)
      {
         _instance = this;
         DontDestroyOnLoad(gameObject);
      }
      else
      {
         Destroy(this);
      }

      _dataPath = Application.persistentDataPath + "/" + _fileName;

      Load();
   }
   public void Save()
   {
      BinaryFormatter bf = new BinaryFormatter();
      FileStream file = File.Create(_dataPath);
      bf.Serialize(file, _data);
      file.Close();
   }
   private void Load()
   {
      if (!File.Exists(_dataPath)) return;
      BinaryFormatter bf = new BinaryFormatter();
      FileStream file = File.Open(_dataPath, FileMode.Open);
      _data = bf.Deserialize(file) as Data;
      file.Close();
   }
   [ContextMenu("Delete Data")]
   private void DeleteData()
   {
      File.Delete(_dataPath);
   }
   public Stat GetStatWithCode(string code)
   {
      for(int i=0; i < _data.Statistics.Count; i++)
      {
         if (_data.Statistics[i].code == code)
         {
            return _data.Statistics[i];
         }
      }
      return null;
   }
   public Achievement[] GetkAchievementsWithStat(string code)
   {
      return _data.Achievements.Where(a => a.statCode == code).ToArray();
   }
}



