using System;
using TMPro;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    [SerializeField] WaveController _waveController;
    [SerializeField] GameObject[] _doors;
    

    void Update()
    {
        CheckDoors();
    }
    private void CheckDoors()
    {
        if (_waveController.tutorial)
        {
            for (int i = 0; i < _doors.Length; i++)
            {
                if (_doors[i] == null)
                {
                    _waveController.tutorial = false;
                    gameObject.SetActive(false);
                }
            }
        }
    }

}
