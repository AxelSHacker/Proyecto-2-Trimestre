using System;
using UnityEngine;

public class AchievementManager : CustomMonoBehaviour
{
   public static Action<string, string> OnAchievementUnlock;
   public override void EditorInit()
   {

   }
   void OnEnable()
   {
      WaveController.OnEnemyDead += () => IncreaseStatAnCheckAchievement("Kill", 1);
      WaveController.OnWaveIncrease += () => IncreaseStatAnCheckAchievement("Wave", 1);
      BlockingWalls.OnWallCounter += () => IncreaseStatAnCheckAchievement("Wall", 1);
   }
    private void IncreaseStatAnCheckAchievement(string code, int amount)
   {
      Stat stat = DataManager.Instance.GetStatWithCode(code);
      if (stat == null) return;
      stat.value += amount;
      Achievement[] achievements = DataManager.Instance.GetkAchievementsWithStat(stat.code);
      Debug.Log(achievements);
      for (int i = 0; i < achievements.Length; i++)
      {
         if (!achievements[i].unlocked && achievements[i].targetAmount <= stat.value)
         {
            //Logro completado
            achievements[i].unlocked = true;
            //Notificamos desbloqueo de logo pasamos la info necesaria a traves del evento
            OnAchievementUnlock?.Invoke(achievements[i].name, achievements[i].imageName);

            DataManager.Instance.Save();
         }
      }
   }
}



