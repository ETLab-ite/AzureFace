using Microsoft.Azure.CognitiveServices.Vision.Face;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityAsync;
using System.IO;
using AzureModels = Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System;

namespace ETLab
{
    public class Emotion
    {
        IFaceClient client;

        public Emotion(int config_code = 0)
        {
            Utils.initConfigData(config_code);
            client = Utils.authenticate(ConfigData.FACE_ENDPOINT, ConfigData.FACE_SUBSCRIPTION_KEY1);
        }

        public async Task<List<Dictionary<string, double>>> extractEmotionWithStream(Stream image)
        {
			List<Dictionary<string, double>> emotions = new List<Dictionary<string, double>>();
			Dictionary<string, double> emotion_dict;


			IList<AzureModels.FaceAttributeType> return_face_attributes = new List<AzureModels.FaceAttributeType> {
				AzureModels.FaceAttributeType.Emotion,
				AzureModels.FaceAttributeType.Smile
            };

            // 儲存偵測到的臉部列表
            List<AzureModels.DetectedFace> detected_faces =
                await Utils.extractWithStreamAsync(client, image, return_face_attributes);

			// 剖析每個偵測到的臉部，並列印屬性資料。
			// Parse and print all attributes of each detected face.
			foreach (var face in detected_faces)
			{
				emotion_dict = new Dictionary<string, double>();
				AzureModels.Emotion emotion = face.FaceAttributes.Emotion;

				// emotion 下的各個情緒，變數類型為 double
				emotion_dict.Add("Anger", emotion.Anger);
				emotion_dict.Add("Contempt", emotion.Contempt);
				emotion_dict.Add("Disgust", emotion.Disgust);
				emotion_dict.Add("Fear", emotion.Fear);
				emotion_dict.Add("Happiness", emotion.Happiness);
				emotion_dict.Add("Neutral", emotion.Neutral);
				emotion_dict.Add("Sadness", emotion.Sadness);
				emotion_dict.Add("Surprise", emotion.Surprise);
				emotion_dict.Add("Smile", (double)face.FaceAttributes.Smile);

				emotions.Add(emotion_dict);
			}

			return emotions;
		}
	}
}
