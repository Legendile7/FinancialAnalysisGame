using UnityEngine;
using LootLocker;

public class GoogleLoginHandler : MonoBehaviour
{
    public void OnGoogleLogin(string idToken)
    {
        // Here you use the idToken with LootLocker API
        LootLockerAPIManager.sesson(idToken, OnLoginSuccess, OnLoginFailure);
    }

    void OnLoginSuccess()
    {
        // Handle successful login
        Debug.Log("Successfully logged in!");
    }

    void OnLoginFailure(string error)
    {
        // Handle login failure
        Debug.LogError("Login failed: " + error);
    }
}
