using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class AndroidCamera : MonoBehaviour
{
    public RawImage raw;
    WebCamTexture web_cam_texture;

    // Start is called before the first frame update
    void Start()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        foreach(WebCamDevice device in devices)
        {
            Debug.Log(string.Format(device.name));
        }

        web_cam_texture = new WebCamTexture();
        raw.material.mainTexture = web_cam_texture;
        web_cam_texture.Play();

        StartCoroutine(takePhoto());
    }

    IEnumerator takePhoto()
    {
        // NOTE - you almost certainly have to do this here:
        yield return new WaitForEndOfFrame();

        // it's a rare case where the Unity doco is pretty clear, 
        // http://docs.unity3d.com/ScriptReference/WaitForEndOfFrame.html 
        // be sure to scroll down to the SECOND long example on that doco page 

        Texture2D photo = new Texture2D(web_cam_texture.width, web_cam_texture.height);
        photo.SetPixels(web_cam_texture.GetPixels());
        photo.Apply();

        raw.material.mainTexture = photo;

        // Encode to a PNG 
        byte[] bytes = photo.EncodeToPNG();
        MemoryStream memory_stream = new MemoryStream(bytes);
        Stream stream = memory_stream;

        //
        ////Write out the PNG. Of course you have to substitute your_path for something sensible 
        //File.WriteAllBytes(your_path + "photo.png", bytes);
    }

}
