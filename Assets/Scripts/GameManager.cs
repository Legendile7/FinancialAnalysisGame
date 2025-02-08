using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LootLocker.Requests;
using TMPro;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class GameManager : MonoBehaviour
{
    
    public GameObject LoginPanel;
    public GameObject SignUpWindow;
    public GameObject LoginWindow;
    public GameObject ResetWindow;
    public TMP_InputField signupEmail;
    public TMP_InputField signupPassword;
    public TMP_InputField loginEmail;
    public TMP_InputField loginPassword;
    public Toggle rememberMe;
    public TMP_InputField resetEmail;

    public GameObject successNotification;
    public TMP_Text successNotificationText;
    public GameObject errorNotification;
    public TMP_Text errorNotificationText;

    public void NewUser()
    {
        string email = this.signupEmail.text;
        string password = this.signupPassword.text;

        void Error(string error)
        {
            Debug.LogError(error);
            //Display Error
            errorNotification.SetActive(true);
            errorNotificationText.text = error;
        }

        if (!IsValidEmail(email))
        {
            Error("Invalid Email");
            return;
        }

        LootLockerSDKManager.WhiteLabelSignUp(email, password, (response) =>
        {
            if (!response.success)
            {
                Error(response.errorData.message);
                return;
            }
            else
            {
                Debug.Log("Sign up Successful");
                successNotification.SetActive(true);
                successNotificationText.text = "Verification email sent to " + email;
            }
        });
    }

    public void Start()
    {
        LootLockerSDKManager.CheckWhiteLabelSession(response =>
        {
            if (response)
            {
                // Start a new session
                Debug.Log("session is valid, you can start a game session");
                LootLockerSDKManager.StartWhiteLabelSession((response) =>
                {
                    if (response.success)
                    {
                        Debug.Log("Session started successfully");
                        LootLockerSDKManager.GetPlayerName((response) =>
                        {
                            if (response.success)
                            {
                                Debug.Log("Successfully got player name: " + response.name);
                                successNotification.SetActive(true);
                                successNotificationText.text = "Welcome back " + response.name + "!";
                                PlayerPrefs.SetString("playerName", response.name);
                            }
                            else
                            {
                                Debug.Log("Error getting player name: " + response.errorData);
                            }
                        });
                    }
                    else
                    {
                        Debug.Log("Error starting session: " + response.errorData.message);
                    }
                });
                LoginPanel.SetActive(false);
            }
            else
            {
                // Show login form here
                Debug.Log("session is NOT valid, we should show the login form");
                LoginPanel.SetActive(true);
            }
        });
    }

    public void Login()
    {
        LootLockerSDKManager.WhiteLabelLoginAndStartSession(loginEmail.text, loginPassword.text, rememberMe, response =>
        {
            Debug.Log(rememberMe.isOn);
            if (!response.success)
            {
                if (!response.LoginResponse.success)
                {
                    Debug.Log("error while logging in");
                    errorNotification.SetActive(true);
                    errorNotificationText.text = response.errorData.message;
                }
                else if (!response.SessionResponse.success)
                {
                    Debug.Log("error while starting session");
                    errorNotification.SetActive(true);
                    errorNotificationText.text = response.errorData.message;
                }
                return;
            }

            // Handle Returning Player
            LootLockerSDKManager.GetPlayerName((response) =>
            {
                if (response.success && response.name.Length >= 2)
                {
                    Debug.Log("Successfully got player name: " + response.name);
                    successNotification.SetActive(true);
                    successNotificationText.text = "Welcome back " + response.name + "!";
                    PlayerPrefs.SetString("playerName", response.name);
                }
                else
                {
                    Debug.Log("Error getting player name");
                    string username = loginEmail.text.Substring(0, loginEmail.text.IndexOf('@'));
                    LootLockerSDKManager.SetPlayerName(username, (response) =>
                    {
                        if (response.success)
                        {
                            Debug.Log("Successfully set player name to: " + response.name);
                            successNotification.SetActive(true);
                            successNotificationText.text = "Welcome back " + response.name + "!";
                        }
                        else
                        {
                            Debug.Log("Error setting player name:" + response.errorData);
                        }
                    });
                }
            });
            LoginPanel.SetActive(false);
        });
    }

    public void ResetPassword()
    {
        LootLockerSDKManager.WhiteLabelRequestPassword(resetEmail.text, (response) =>
        {
            if (response.success)
            {
                Debug.Log("Password reset email sent to " + resetEmail.text);
                successNotification.SetActive(true);
                successNotificationText.text = "Password reset email sent to " + resetEmail.text;
            }
            else
            {
                Debug.Log("Error sending password reset email: " + response.errorData.message);
                errorNotification.SetActive(true);
                errorNotificationText.text = response.errorData.message;
            }
        });
    }

    public bool IsValidEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
            return false;

        string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, pattern);
    }

    public void ToggleSignUp()
    {
        SignUpWindow.SetActive(true);
        LoginWindow.SetActive(false);
        ResetWindow.SetActive(false);
    }

    public void ToggleLogin()
    {
        SignUpWindow.SetActive(false);
        LoginWindow.SetActive(true);
        ResetWindow.SetActive(false);
    }

    public void ToggleReset()
    {
        SignUpWindow.SetActive(false);
        LoginWindow.SetActive(false);
        ResetWindow.SetActive(true);
    }

    


}