using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System.Linq;

public static class Utils
{
    // 使用端點和金鑰來具現化用戶端。使用金鑰建立 ApiKeyServiceClientCredentials 物件，並使用該物件與您的端點建立 FaceClient 物件。
    public static IFaceClient authenticate(string endpoint, string key)
    {
        return new FaceClient(new ApiKeyServiceClientCredentials(key)) { Endpoint = endpoint };
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

	#region Extract face
	public static async Task<List<DetectedFace>> extractWithUrlAsync(IFaceClient client, string url, IList<FaceAttributeType> attributes = null)
	{
		IList<DetectedFace> detected_faces = await client.Face.DetectWithUrlAsync(
			url,
			returnFaceAttributes: attributes,
			recognitionModel: RecognitionModel.Recognition02);

		return detected_faces.ToList();
	}

	public static async Task<List<DetectedFace>> extractWithStreamAsync(IFaceClient client, Stream image, IList<FaceAttributeType> attributes = null)
	{
		IList<DetectedFace> detected_faces = await client.Face.DetectWithStreamAsync(
			image,
			returnFaceAttributes: attributes,
			recognitionModel: RecognitionModel.Recognition02);

		return detected_faces.ToList();
	}

	public static async Task extractFaceWithUrl(IFaceClient client, string url)
	{
		Debug.Log(string.Format("[Main] extractFaceWithUrl | url: {0}", url));

		IList<FaceAttributeType> return_face_attributes = new List<FaceAttributeType> {
				FaceAttributeType.Accessories,
				FaceAttributeType.Age,
				FaceAttributeType.Blur,
				FaceAttributeType.Emotion,
				FaceAttributeType.Exposure,
				FaceAttributeType.FacialHair,
				FaceAttributeType.Gender,
				FaceAttributeType.Glasses,
				FaceAttributeType.Hair,
				FaceAttributeType.HeadPose,
				FaceAttributeType.Makeup,
				FaceAttributeType.Noise,
				FaceAttributeType.Occlusion,
				FaceAttributeType.Smile
		};

		float start_time = getCurrentTimestamp();
		// 儲存偵測到的臉部列表
		List<DetectedFace> detected_faces =
			await extractWithUrlAsync(client, url, return_face_attributes);

		Debug.Log(string.Format("[Main] extractFace | {0} face(s) detected from image.",
			detected_faces.Count));

		// 剖析每個偵測到的臉部，並列印屬性資料。
		// Parse and print all attributes of each detected face.
		foreach (var face in detected_faces)
		{
			// Get bounding box of the faces
			Debug.Log(string.Format("[Main] extractFace | Rectangle(Left/Top/Width/Height): {0}, {1}, {2}, {3}",
				face.FaceRectangle.Left, face.FaceRectangle.Top, face.FaceRectangle.Width, face.FaceRectangle.Height));

			// Get accessories(配件) of the faces
			List<Accessory> accessoriesList = (List<Accessory>)face.FaceAttributes.Accessories;
			int count = face.FaceAttributes.Accessories.Count;
			string accessory;

			if (count == 0)
			{
				accessory = "NoAccessories";
			}
			else
			{
				string[] accessoryArray = new string[count];

				for (int i = 0; i < count; ++i)
				{
					accessoryArray[i] = accessoriesList[i].Type.ToString();
				}

				accessory = string.Join(",", accessoryArray);
			}

			Debug.Log(string.Format("[Main] extractFace | Accessories: {0}", accessory));

			// Get face other attributes
			Debug.Log(string.Format("[Main] extractFace | Age: {0}", face.FaceAttributes.Age));
			Debug.Log(string.Format("[Main] extractFace | Blur: {0}", face.FaceAttributes.Blur.BlurLevel));

			// Get emotion on the face
			string emotionType = string.Empty;
			double emotionValue = 0.0;
			Emotion emotion = face.FaceAttributes.Emotion;

			// emotion 下的各個情緒，變數類型為 double
			if (emotion.Anger > emotionValue)
			{
				emotionValue = emotion.Anger;
				emotionType = "Anger";
			}

			if (emotion.Contempt > emotionValue)
			{
				emotionValue = emotion.Contempt;
				emotionType = "Contempt";
			}

			if (emotion.Disgust > emotionValue)
			{
				emotionValue = emotion.Disgust;
				emotionType = "Disgust";
			}

			if (emotion.Fear > emotionValue)
			{
				emotionValue = emotion.Fear;
				emotionType = "Fear";
			}

			if (emotion.Happiness > emotionValue)
			{
				emotionValue = emotion.Happiness;
				emotionType = "Happiness";
			}

			if (emotion.Neutral > emotionValue)
			{
				emotionValue = emotion.Neutral;
				emotionType = "Neutral";
			}

			if (emotion.Sadness > emotionValue)
			{
				emotionValue = emotion.Sadness;
				emotionType = "Sadness";
			}

			if (emotion.Surprise > emotionValue)
			{
				emotionType = "Surprise";
			}

			Debug.Log(string.Format("[Main] extractFace | Emotion: {0}", emotionType));

			// Get more face attributes
			Debug.Log(string.Format("[Main] extractFace | Exposure: {0}", face.FaceAttributes.Exposure.ExposureLevel));
			Debug.Log(string.Format("[Main] extractFace | FacialHair: {0}", face.FaceAttributes.FacialHair.Moustache + face.FaceAttributes.FacialHair.Beard + face.FaceAttributes.FacialHair.Sideburns > 0 ? "Yes" : "No"));
			Debug.Log(string.Format("[Main] extractFace | Gender: {0}", face.FaceAttributes.Gender));
			Debug.Log(string.Format("[Main] extractFace | Glasses: {0}", face.FaceAttributes.Glasses));

			// Get hair color
			Hair hair = face.FaceAttributes.Hair;
			string color = null;
			if (hair.HairColor.Count == 0)
			{
				if (hair.Invisible)
				{
					color = "Invisible";
				}
				else
				{
					color = "Bald";
				}
			}

			double maxConfidence = 0.0f;
			foreach (HairColor hairColor in hair.HairColor)
			{
				if (hairColor.Confidence <= maxConfidence)
				{
					continue;
				}

				maxConfidence = hairColor.Confidence;
				color = hairColor.Color.ToString();
			}

			Debug.Log(string.Format("[Main] extractFace | Hair: {0}", color));

			// Get more attributes
			Debug.Log(string.Format("[Main] extractFace | HeadPose: Pitch: {0}, Roll: {1}, Yaw: {2}",
				Math.Round(face.FaceAttributes.HeadPose.Pitch, 2),
				Math.Round(face.FaceAttributes.HeadPose.Roll, 2),
				Math.Round(face.FaceAttributes.HeadPose.Yaw, 2)));
			Debug.Log(string.Format("[Main] extractFace | Makeup: {0}", (face.FaceAttributes.Makeup.EyeMakeup || face.FaceAttributes.Makeup.LipMakeup) ? "Yes" : "No"));
			Debug.Log(string.Format("[Main] extractFace | Noise: {0}", face.FaceAttributes.Noise.NoiseLevel));
			Debug.Log(string.Format("[Main] extractFace | Occlusion: EyeOccluded: {0}, ForeheadOccluded: {1}, MouthOccluded: {2}",
				(face.FaceAttributes.Occlusion.EyeOccluded ? "Yes" : "No"),
				(face.FaceAttributes.Occlusion.ForeheadOccluded ? "Yes" : "No"),
				(face.FaceAttributes.Occlusion.MouthOccluded ? "Yes" : "No")));
			Debug.Log(string.Format("[Main] extractFace | Smile: {0}", face.FaceAttributes.Smile));


		}

		Debug.Log(string.Format("[Main] extractFace | timestemp: {0:F8}", Utils.getCurrentTimestamp() - start_time));
	}

	public static async Task extractFaceWithStream(IFaceClient client, Stream image)
	{
		Debug.Log(string.Format("[Main] extractFaceWithStream"));

		IList<FaceAttributeType> return_face_attributes = new List<FaceAttributeType> {
				FaceAttributeType.Accessories,
				FaceAttributeType.Age,
				FaceAttributeType.Blur,
				FaceAttributeType.Emotion,
				FaceAttributeType.Exposure,
				FaceAttributeType.FacialHair,
				FaceAttributeType.Gender,
				FaceAttributeType.Glasses,
				FaceAttributeType.Hair,
				FaceAttributeType.HeadPose,
				FaceAttributeType.Makeup,
				FaceAttributeType.Noise,
				FaceAttributeType.Occlusion,
				FaceAttributeType.Smile
		};

		float start_time;
		start_time = getCurrentTimestamp();

		// 儲存偵測到的臉部列表
		List<DetectedFace> detected_faces =
			await extractWithStreamAsync(client, image, return_face_attributes);

		Debug.Log(string.Format("[Main] extractFaceWithStream | {0} face(s) detected from image.",
			detected_faces.Count));

		// 剖析每個偵測到的臉部，並列印屬性資料。
		// Parse and print all attributes of each detected face.
		foreach (var face in detected_faces)
		{
			// Get bounding box of the faces
			Debug.Log(string.Format("[Main] extractFaceWithStream | Rectangle(Left/Top/Width/Height): {0}, {1}, {2}, {3}",
				face.FaceRectangle.Left, face.FaceRectangle.Top, face.FaceRectangle.Width, face.FaceRectangle.Height));

			// Get accessories of the faces
			List<Accessory> accessories_list = (List<Accessory>)face.FaceAttributes.Accessories;
			int count = face.FaceAttributes.Accessories.Count;

			string accessory;

			if (count == 0)
			{
				accessory = "NoAccessories";
			}
			else
			{
				string[] accessory_array = new string[count];

				for (int i = 0; i < count; ++i)
				{
					accessory_array[i] = accessories_list[i].Type.ToString();
				}

				accessory = string.Join(",", accessory_array);
			}

			Debug.Log(string.Format("[Main] extractFaceWithStream | Accessories: {0}", accessory));

			// Get face other attributes
			Debug.Log(string.Format("[Main] extractFaceWithStream | Age: {0}", face.FaceAttributes.Age));
			Debug.Log(string.Format("[Main] extractFaceWithStream | Blur: {0}", face.FaceAttributes.Blur.BlurLevel));

			// Get emotion on the face
			string emotionType = string.Empty;
			double emotionValue = 0.0;
			Emotion emotion = face.FaceAttributes.Emotion;

			// emotion 下的各個情緒，變數類型為 double
			if (emotion.Anger > emotionValue)
			{
				emotionValue = emotion.Anger;
				emotionType = "Anger";
			}

			if (emotion.Contempt > emotionValue)
			{
				emotionValue = emotion.Contempt;
				emotionType = "Contempt";
			}

			if (emotion.Disgust > emotionValue)
			{
				emotionValue = emotion.Disgust;
				emotionType = "Disgust";
			}

			if (emotion.Fear > emotionValue)
			{
				emotionValue = emotion.Fear;
				emotionType = "Fear";
			}

			if (emotion.Happiness > emotionValue)
			{
				emotionValue = emotion.Happiness;
				emotionType = "Happiness";
			}

			if (emotion.Neutral > emotionValue)
			{
				emotionValue = emotion.Neutral;
				emotionType = "Neutral";
			}

			if (emotion.Sadness > emotionValue)
			{
				emotionValue = emotion.Sadness;
				emotionType = "Sadness";
			}

			if (emotion.Surprise > emotionValue)
			{
				emotionType = "Surprise";
			}

			Debug.Log(string.Format("[Main] extractFaceWithStream | Emotion: {0}", emotionType));

			// Get more face attributes
			Debug.Log(string.Format("[Main] extractFaceWithStream | Exposure: {0}", face.FaceAttributes.Exposure.ExposureLevel));
			Debug.Log(string.Format("[Main] extractFaceWithStream | FacialHair: {0}", face.FaceAttributes.FacialHair.Moustache + face.FaceAttributes.FacialHair.Beard + face.FaceAttributes.FacialHair.Sideburns > 0 ? "Yes" : "No"));
			Debug.Log(string.Format("[Main] extractFaceWithStream | Gender: {0}", face.FaceAttributes.Gender));
			Debug.Log(string.Format("[Main] extractFaceWithStream | Glasses: {0}", face.FaceAttributes.Glasses));

			// Get hair color
			Hair hair = face.FaceAttributes.Hair;
			string color = null;
			if (hair.HairColor.Count == 0)
			{
				if (hair.Invisible)
				{
					color = "Invisible";
				}
				else
				{
					color = "Bald";
				}
			}

			double maxConfidence = 0.0f;
			foreach (HairColor hairColor in hair.HairColor)
			{
				if (hairColor.Confidence <= maxConfidence)
				{
					continue;
				}

				maxConfidence = hairColor.Confidence;
				color = hairColor.Color.ToString();
			}

			Debug.Log(string.Format("[Main] extractFaceWithStream | Hair: {0}", color));

			// Get more attributes
			Debug.Log(string.Format("[Main] extractFaceWithStream | HeadPose: Pitch: {0}, Roll: {1}, Yaw: {2}",
				Math.Round(face.FaceAttributes.HeadPose.Pitch, 2),
				Math.Round(face.FaceAttributes.HeadPose.Roll, 2),
				Math.Round(face.FaceAttributes.HeadPose.Yaw, 2)));
			Debug.Log(string.Format("[Main] extractFaceWithStream | Makeup: {0}", (face.FaceAttributes.Makeup.EyeMakeup || face.FaceAttributes.Makeup.LipMakeup) ? "Yes" : "No"));
			Debug.Log(string.Format("[Main] extractFaceWithStream | Noise: {0}", face.FaceAttributes.Noise.NoiseLevel));
			Debug.Log(string.Format("[Main] extractFaceWithStream | Occlusion: EyeOccluded: {0}, ForeheadOccluded: {1}, MouthOccluded: {2}",
				(face.FaceAttributes.Occlusion.EyeOccluded ? "Yes" : "No"),
				(face.FaceAttributes.Occlusion.ForeheadOccluded ? "Yes" : "No"),
				(face.FaceAttributes.Occlusion.MouthOccluded ? "Yes" : "No")));
			Debug.Log(string.Format("[Main] extractFaceWithStream | Smile: {0}", face.FaceAttributes.Smile));
		}

		Debug.Log(string.Format("[Main] extractFaceWithStream | timestemp: {0:F8}", Utils.getCurrentTimestamp() - start_time));
	} 
	#endregion

	#region PersonGroup
	public static async Task<bool> isPersonGroupExist(IFaceClient client, string group_id)
	{
		IList<PersonGroup> groups = await client.PersonGroup.ListAsync();
		foreach (PersonGroup group in groups)
		{
			// 5ce45c41-d387-4afc-8241-279da096e27e
			if (group.PersonGroupId == group_id)
			{
				return true;
			}
		}

		return false;
	}

	public static async Task printAllPersonGroupId(IFaceClient client)
	{
		IList<PersonGroup> groups = await client.PersonGroup.ListAsync();
		foreach (PersonGroup group in groups)
		{
			Debug.Log(string.Format("ID: {0}", group.PersonGroupId));
		}
	}

	public static async Task<List<Person>> getPersons(IFaceClient client, string group_id)
	{
		IList<Person> person_list = await client.PersonGroupPerson.ListAsync(group_id);
		return person_list.ToList();
	}
	#endregion

	// ==================================================
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
