using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

public class PinataUploader : MonoBehaviour
{
    private string pinataApiKey = "b2e47308eb8a70731ba1";
    private string pinataSecretApiKey = "d37f782a3c1bb727886c817012b985f9c6d72fbc19a3acde74d94dc4b47bbc43";
    private string pinataUploadUrl = "https://api.pinata.cloud/pinning/pinFileToIPFS";

    public void Upload()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "myfile.txt");
        File.WriteAllText(filePath, "This is a test file.");
        StartCoroutine(UploadFile(filePath));
    }

    public IEnumerator UploadFile(string filePath)
    {
        byte[] fileData = File.ReadAllBytes(filePath);
        WWWForm form = new WWWForm();
        form.AddBinaryData("file", fileData, Path.GetFileName(filePath));

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
}
