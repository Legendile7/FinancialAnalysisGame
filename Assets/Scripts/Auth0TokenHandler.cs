using LootLocker.Requests;
using System.Runtime.InteropServices;
using UnityEngine;

public class Auth0TokenHandler : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern string GetAuthToken();

    public void ProcessAuthToken()
    {
        string token = GetAuthToken();
        if (!string.IsNullOrEmpty(token))
        {
            Debug.Log("Google Sign-In Success! Token: " + token);
            SendTokenToLootLocker(token);
        }
    }

    void SendTokenToLootLocker(string token)
    {
        LootLockerSDKManager.ConnectGoogleAccount(token, (response) =>
        {
            if (response.success)
            {
                Debug.Log("Successfully logged into LootLocker with Google");
            }
            else
            {
                Debug.LogError("LootLocker login failed: " + response.errorData);
            }
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
