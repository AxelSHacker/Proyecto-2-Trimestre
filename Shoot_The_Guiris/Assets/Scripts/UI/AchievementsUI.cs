using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementsUI : CustomMonoBehaviour
{
   [SerializeField] Animator _animator;
   [SerializeField] TextMeshProUGUI _achievementsNameText;
   [SerializeField] Image _achievementIconImage;
   float _animationDuration = 3f;
   float lastAchievementTime;
   public override void EditorInit()
   {
      _animator = GetComponent<Animator>();
      _achievementsNameText = GetComponentInChildren<TextMeshProUGUI>();
   }
   void OnEnable()
   {
      AchievementManager.OnAchievementUnlock += SetandShow;
   }
   void OnDisable()
   {
      AchievementManager.OnAchievementUnlock -= SetandShow;
   }
   private void SetandShow(string name, string image)
   {
      if (lastAchievementTime > Time.time)
      {
         lastAchievementTime += _animationDuration;
      }
      else
      {
         lastAchievementTime = Time.time + _animationDuration;
      }
      StartCoroutine(DelayShow(name, image, lastAchievementTime));
   }
   private IEnumerator DelayShow(string name, string image, float shoTime)
   {
      while (Time.time < shoTime)
      {
         yield return null;
      }

      _achievementsNameText.text = name;
      _achievementIconImage.sprite = Resources.Load<Sprite>(image);
      _animator.SetTrigger("Show");
   }

}



