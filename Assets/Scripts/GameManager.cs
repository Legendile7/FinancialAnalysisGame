using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LootLocker.Requests;

public class GameManager : MonoBehaviour
{
    public void StartGuestSession()
    {
        LootLockerSDKManager.StartGuestSession((response) =>
        {
            if (!response.success)
            {
                Debug.Log("error starting LootLocker session");

                return;
            }

            Debug.Log("successfully started LootLocker session");
        });
    }

    public void ConvertGuestToGoogle(string googleIdToken)
    {
        LootLockerSDKManager.ConnectGoogleAccount(googleIdToken, (response) =>
        {
            if (!response.success)
            {
                Debug.Log("error connecting Google account");
                return;
            }
            Debug.Log("successfully connected Google account");
        });
    }
}