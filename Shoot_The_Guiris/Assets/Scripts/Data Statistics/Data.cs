using System.Collections.Generic;
using UnityEngine;
[System.Serializable]

public class Data
{
    [SerializeField] Stat[] _statistics;
    [SerializeField] Achievement[] _achievements;
    public IReadOnlyList<Stat> Statistics => _statistics;
    public IReadOnlyList<Achievement> Achievements => _achievements;
}
