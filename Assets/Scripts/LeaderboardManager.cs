using LootLocker.Requests;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardManager : MonoBehaviour
{
    public TMP_Text LeaderboardText;
    private string leaderboardKey = "lb";

    void Start()
    {
        FetchLeaderboard();
    }

    void FetchLeaderboard()
    {
        LootLockerSDKManager.GetScoreList(leaderboardKey, 10, (response) =>
        {
            if (!response.success)
            {
                Debug.LogError("Failed to fetch leaderboard: " + response.errorData);
                return;
            }
            string leaderboardText = "";
            for (int i = 0; i < response.items.Length; i++)
            {
                LootLockerLeaderboardMember currentEntry = response.items[i];
                leaderboardText += currentEntry.rank + ".";
                leaderboardText += currentEntry.player.name;
                leaderboardText += " - ";
                leaderboardText += currentEntry.score;
                leaderboardText += "\n";
            }
            LeaderboardText.text = leaderboardText;
        });
    }


    public void SubmitScore(int xp)
    {
        LootLockerSDKManager.SubmitScore("", xp, leaderboardKey, (response) =>
        {
            if (!response.success)
            {
                Debug.LogError("Failed to submit score: " + response.errorData);
                return;
            }
            Debug.Log("Score submitted successfully!");
        });
    }
}
