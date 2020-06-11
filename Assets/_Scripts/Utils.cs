using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

using Microsoft.Azure.CognitiveServices.Vision.Face;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;

public static class Utils
{
    // 使用端點和金鑰來具現化用戶端。使用金鑰建立 ApiKeyServiceClientCredentials 物件，並使用該物件與您的端點建立 FaceClient 物件。
    public static IFaceClient Authenticate(string endpoint, string key)
    {
        return new FaceClient(new ApiKeyServiceClientCredentials(key)) { Endpoint = endpoint };
    }

    public static async Task<IList<DetectedFace>> extractWithUrlAsync(IFaceClient client, string url, IList<FaceAttributeType> attributes = null)
    {
        IList<DetectedFace> detected_faces = await client.Face.DetectWithUrlAsync(
            url,
            returnFaceAttributes: attributes, 
            recognitionModel: RecognitionModel.Recognition02);

        return detected_faces;
    }

    public static async Task<IList<DetectedFace>> extractWithStreamAsync(IFaceClient client, Stream image, IList<FaceAttributeType> attributes = null)
    {
        IList<DetectedFace> detected_faces = await client.Face.DetectWithStreamAsync(
            image,
            returnFaceAttributes: attributes,
            recognitionModel: RecognitionModel.Recognition02);

        return detected_faces;
    }

    // TODO: 考慮不同裝置的讀檔方式
    public static void initConfigData()
    {
        using (StreamReader reader = new StreamReader(ConfigData.config_path))
        {
            string line;
            string[] content;

            while ((line = reader.ReadLine()) != null)
            {
                content = line.Split(' ');

                switch (content[0])
                {
                    case "FACE_SUBSCRIPTION_KEY1":
                        ConfigData.FACE_SUBSCRIPTION_KEY1 = content[1];
                        break;
                    case "FACE_SUBSCRIPTION_KEY2":
                        ConfigData.FACE_SUBSCRIPTION_KEY2 = content[1];
                        break;
                    case "FACE_ENDPOINT":
                        ConfigData.FACE_ENDPOINT = content[1];
                        break;
                    default:
                        Debug.LogError(string.Format("[Utils] loadConfigData | key: {0}, value: {1}", content[0], content[1]));
                        break;
                }
            }
        }
    }

    public static float getCurrentTimestamp()
    {
        return Time.time;
    }

    #region 無法藉由點擊 Editor log 跳到實際腳本位置
    /* 以下屬性只能置於參數位置，不能於函式內呼叫
     * CallerLineNumber: 實際呼叫的行數位置
     * CallerMemberName: 實際呼叫的函數名稱
     * CallerFilePath: 實際呼叫的腳本路徑
     * 參考網站: https://stackoverflow.com/questions/12556767/how-do-i-get-the-current-line-number
     */
    public static void log(string message, [CallerLineNumber] int line_num = 0, [CallerMemberName] string member = "", [CallerFilePath] string file_path = "")
    {
        message = string.Format("[{0}] ({1}) {2}\n{3}", member, line_num, message, file_path);
        Debug.Log(message);
    }

    public static void error(string message, [CallerLineNumber] int line_num = 0, [CallerMemberName] string member = "", [CallerFilePath] string file_path = "")
    {
        message = string.Format("[{0}] ({1}) {2}\n{3}", member, line_num, message, file_path);
        Debug.LogError(message);
    }
    #endregion


}
