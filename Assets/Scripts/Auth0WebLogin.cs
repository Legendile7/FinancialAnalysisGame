using UnityEngine;

public class Auth0WebLogin : MonoBehaviour
{
    private string auth0Domain = "dev-jew4dmb244nn63gs.us.auth0.com";
    private string clientId = "dev-jew4dmb244nn63gs.us.auth0.com";
    private string redirectUri = "http://localhost:8000/callback"; 

    public void LoginWithGoogle()
    {
        string authUrl = $"https://{auth0Domain}/authorize?client_id={clientId}&response_type=token&scope=openid%20profile%20email&redirect_uri={redirectUri}&connection=google-oauth2";
        Application.OpenURL(authUrl);
    }
}
