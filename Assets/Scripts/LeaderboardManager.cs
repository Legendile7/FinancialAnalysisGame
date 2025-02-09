using LootLocker.Requests;
using TMPro;
using UnityEngine;

public class LeaderboardManager : MonoBehaviour
{
    public TMP_Text LeaderboardText;
    private string leaderboardKey = "lb";

    void OnEnable()
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
                leaderboardText += currentEntry.score + " XP";
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
