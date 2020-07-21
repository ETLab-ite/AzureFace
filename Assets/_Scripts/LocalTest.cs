using Microsoft.Azure.CognitiveServices.Vision.Face;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityAsync;
using System.Threading.Tasks;
using System;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Networking;

public class LocalTest : MonoBehaviour
{
    public Image image;
    Sprite sprite;
    string path;
    Texture2D texture;

    // Start is called before the first frame update
    void Start()
    {
        texture = new Texture2D(1000, 1000);

#if UNITY_EDITOR
        path = Path.Combine(Application.streamingAssetsPath, "image", "miyu.jpg");
        byte[] bytes = File.ReadAllBytes(path);
        texture.LoadImage(bytes);
        texture.Apply();

        image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

        Stream stream = new MemoryStream(bytes);
        _ = testAzureApi(stream);

#elif UNITY_ANDROID
        path = string.Format("jar:file://{0}!/assets/image/miyu.jpg", Application.dataPath);
        _ = loadImageAsync(path);
#endif


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    async Task loadImageAsync(string path)
    {
        byte[] bytes = await androidLoadImageAsync(path);

        if(bytes == null)
        {
            Debug.LogError(string.Format("[LocalTest] loadImageAsync | Failed to load image."));
            return;
        }
        
        texture.LoadImage(bytes);
        texture.Apply();
        image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

        Stream stream = new MemoryStream(bytes);
        await testAzureApi(stream);
    }

    async Task<byte[]> androidLoadImageAsync(string path)
    {
        UnityWebRequest request = UnityWebRequest.Get(path);
        Debug.Log(string.Format("[LocalTest] androidLoadImageAsync | path: {0}", path));
        await request.SendWebRequest();
        Debug.Log("[LocalTest] androidLoadImageAsync | request sent.");

        while (!request.isDone && !request.isNetworkError && !request.isHttpError) { }

        if (!request.isNetworkError && !request.isHttpError)
        {
            return request.downloadHandler.data;
        }

        return null;
    }

    async Task testAzureApi(Stream stream)
    {
        ETLab.Emotion emotion = new ETLab.Emotion(9527);
        List<Dictionary<string, double>> emotions = await emotion.extractEmotionWithStream(stream);
        int n_face = emotions.Count;
        Debug.Log(string.Format("[EmotionTest] extractEmotionTest | {0} faces detected.", n_face));
        Dictionary<string, double> dict = emotions[0];

        foreach (string emotion_type in dict.Keys)
        {
            string msg = string.Format("{0}: {1:F4}", emotion_type, dict[emotion_type]);
            Debug.Log(string.Format("[EmotionTest] extractEmotionTest | {0}", msg));
        }
    }
}
