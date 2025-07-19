using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TournamentData", menuName = "Scriptable Objects/TournamentData")]
public class TournamentData : ScriptableObject
{
    [System.Serializable]
    public class Match
    {
        public int currencyReward = 2;
        public int playerCount = 4;
    }

    public int startingCurrency = 2;
    public int startingLives = 3;

    public List<Match> matches = new();
}
