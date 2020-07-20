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
    public Text log;
    
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
            Debug.Log(string.Format("[EmotionTest] Start | {0}", device.name));
        }

        web_cam_texture = new WebCamTexture();
        raw.material.mainTexture = web_cam_texture;

        start_button.onClick.AddListener(() => {
            close_camara = false;
            web_cam_texture.Play();
            _ = openCamera();

            //int config_code;
            //if (config.text.Equals(""))
            //{
            //    config_code = 0;
            //}
            //else
            //{
            //    config_code = int.Parse(config.text);
            //}

            //Utils.initConfigData(config_code: config_code);
            //log.text = string.Format("KEY: {0}", ConfigData.FACE_SUBSCRIPTION_KEY1);
        });

        take_photo_button.onClick.AddListener(()=> {            
            _ = takePhoto();            
        });

        config.onValueChanged.AddListener((string config_text)=> {
            log.text = config_text;
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

            if (!close_camara)
            {
                photo = new Texture2D(web_cam_texture.width, web_cam_texture.height);
                photo.SetPixels(web_cam_texture.GetPixels());
                photo.Apply();
                raw.material.mainTexture = photo;
            }
        }

        //web_cam_texture.Stop();
    }

    async Task takePhoto()
    {
        Texture2D texture = new Texture2D(web_cam_texture.width, web_cam_texture.height);
        texture.SetPixels(web_cam_texture.GetPixels());
        texture.Apply();
        close_camara = true;
        web_cam_texture.Stop();
        // Encode to a PNG 
        byte[] bytes = texture.EncodeToPNG();
        Debug.Log(string.Format("[EmotionTest] extractEmotionTest | #bytes: {0}", bytes.Length));

        int config_code;
        if (config.text.Equals(""))
        {
            config_code = 0;
        }
        else
        {
            config_code = int.Parse(config.text);
        }
        Debug.Log(string.Format("[EmotionTest] extractEmotionTest | config_code: {0}", config_code));
        Debug.Log(string.Format("[EmotionTest] extractEmotionTest | before Emotion: Application.HasUserAuthorization(UserAuthorization.WebCam): {0}",
            Application.HasUserAuthorization(UserAuthorization.WebCam)));

        ETLab.Emotion emotion = new ETLab.Emotion(config_code);
        log.text = string.Format("KEY: {0}", ConfigData.FACE_SUBSCRIPTION_KEY1);
        Debug.Log(string.Format("[EmotionTest] extractEmotionTest | log.text: {0}", log.text));

        Debug.Log(string.Format("[EmotionTest] extractEmotionTest | before Application.RequestUserAuthorization(UserAuthorization.WebCam)"));
        //await Application.RequestUserAuthorization(UserAuthorization.WebCam);
        Debug.Log(string.Format("[EmotionTest] extractEmotionTest | after Application.RequestUserAuthorization(UserAuthorization.WebCam)"));
        Debug.Log(string.Format("[EmotionTest] extractEmotionTest | Application.HasUserAuthorization(UserAuthorization.WebCam): {0}",
            Application.HasUserAuthorization(UserAuthorization.WebCam)));

        using(MemoryStream memory_stream = new MemoryStream(bytes))
        {
            Debug.Log(string.Format("[EmotionTest] extractEmotionTest | new MemoryStream"));

            if (Application.HasUserAuthorization(UserAuthorization.WebCam))
            {
                List<Dictionary<string, double>> emotions = await emotion.extractEmotionWithStream(memory_stream);
                int index = 0, n_face = emotions.Count;
                Debug.Log(string.Format("[EmotionTest] extractEmotionTest | {0} faces detected.", n_face));
                Dictionary<string, double> dict = emotions[0];

                foreach (string emotion_type in dict.Keys)
                {
                    string msg = string.Format("{0}: {1:F4}", emotion_type, dict[emotion_type]);
                    Debug.Log(string.Format("[EmotionTest] extractEmotionTest | {0}", msg));
                    texts[index].text = msg;
                    index++;
                }
            }
        }        
    }

}
