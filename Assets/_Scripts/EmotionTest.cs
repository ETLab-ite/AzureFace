using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityAsync;

public class EmotionTest : MonoBehaviour
{
    public Text[] texts;
    public RawImage raw;
    public Button start_button;
    public Button take_photo_button;
    public InputField config;
    
    WebCamTexture web_cam_texture;
    Texture2D photo;
    bool close_camara = false;

    // Start is called before the first frame update
    void Start()
    {
        // TODO: 測試 Android 讀檔
        // TODO: 安裝到平板上的實際大小

        WebCamDevice[] devices = WebCamTexture.devices;
        foreach (WebCamDevice device in devices)
        {
            Debug.Log(string.Format(device.name));
        }

        web_cam_texture = new WebCamTexture();
        raw.material.mainTexture = web_cam_texture;

        start_button.onClick.AddListener(() => {
            close_camara = false;
            web_cam_texture.Play();
            _ = openCamera();
        });

        take_photo_button.onClick.AddListener(()=> {
            close_camara = true;
            _ = takePhoto();
        });
    }

    async Task extractEmotionTest()
    {
        ETLab.Emotion emotion = new ETLab.Emotion();
        string image_name = "detection1.jpg";
        Stream image = File.OpenRead(Path.Combine(Application.streamingAssetsPath, "image", image_name));

        List<Dictionary<string, double>> emotions = await emotion.extractEmotionWithStream(image);
        int index = 0, n_face = emotions.Count;
        Debug.Log(string.Format("[EmotionTest] extractEmotionTest | {0} faces detected.", n_face));
        Dictionary<string, double> dict = emotions[0];

        foreach (string emotion_type in dict.Keys)
        {
            string msg = string.Format("{0}: {1:F4}", emotion_type, dict[emotion_type]);
            Debug.Log(msg);
            texts[index].text = msg;
            index++;
        }
    }

    async Task openCamera()
    {
        while (!close_camara)
        {
            await new UnityEngine.WaitForSecondsRealtime(Time.deltaTime);

            photo = new Texture2D(web_cam_texture.width, web_cam_texture.height);
            photo.SetPixels(web_cam_texture.GetPixels());
            photo.Apply();
            raw.material.mainTexture = photo;
        }

        web_cam_texture.Stop();
    }

    async Task takePhoto()
    {
        // Encode to a PNG 
        byte[] bytes = photo.EncodeToPNG();
        MemoryStream memory_stream = new MemoryStream(bytes);

        int config_code = int.Parse(config.text);
        ETLab.Emotion emotion = new ETLab.Emotion(config_code);
        List<Dictionary<string, double>> emotions = await emotion.extractEmotionWithStream(memory_stream);
        int index = 0, n_face = emotions.Count;
        Debug.Log(string.Format("[EmotionTest] extractEmotionTest | {0} faces detected.", n_face));
        Dictionary<string, double> dict = emotions[0];

        foreach (string emotion_type in dict.Keys)
        {
            string msg = string.Format("{0}: {1:F4}", emotion_type, dict[emotion_type]);
            Debug.Log(msg);
            texts[index].text = msg;
            index++;
        }
    }
}
