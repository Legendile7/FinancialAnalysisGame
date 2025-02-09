using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class PinataUploader : MonoBehaviour
{
    private string pinataApiKey = "b2e47308eb8a70731ba1";
    private string pinataSecretApiKey = "d37f782a3c1bb727886c817012b985f9c6d72fbc19a3acde74d94dc4b47bbc43";
    private string pinataUploadUrl = "https://api.pinata.cloud/pinning/pinFileToIPFS";

    private void Start()
    {
        // This could be used for other setup if needed.
    }

    // Triggered by JavaScript when a file is uploaded
    public void OnFileUploaded(string base64FileData)
    {
        byte[] fileData = System.Convert.FromBase64String(base64FileData);
        string fileHash = GetSHA256Hash(fileData);  // Calculate the hash
        StartCoroutine(UploadFile(fileData, fileHash));
    }

    // Calculate the SHA-256 hash of the file data
    private string GetSHA256Hash(byte[] data)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hashBytes = sha256.ComputeHash(data);
            StringBuilder hashStringBuilder = new StringBuilder();
            foreach (byte b in hashBytes)
            {
                hashStringBuilder.Append(b.ToString("x2"));  // Convert byte to hex string
            }
            return hashStringBuilder.ToString();
        }
    }

    public IEnumerator UploadFile(byte[] fileData, string fileHash)
    {
        WWWForm form = new WWWForm();
        form.AddBinaryData("file", fileData, fileHash + ".jpg");  // Use the hash as the filename

        UnityWebRequest request = UnityWebRequest.Post(pinataUploadUrl, form);
        request.SetRequestHeader("pinata_api_key", pinataApiKey);
        request.SetRequestHeader("pinata_secret_api_key", pinataSecretApiKey);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Upload Successful: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Upload Failed: " + request.error);
        }
    }
    public void OpenFileExplorer()
    {
        Application.ExternalCall("openFileExplorer", "OnFileUploaded");
    }

}
