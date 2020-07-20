using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityAsync;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;


public class Main : MonoBehaviour
{
	// Used for the Identify, Snapshot, and Delete examples.
	// The same person group is used for both Person Group Operations and Snapshot Operations.
	// 在類別根目錄上宣告字串變數，以代表您將建立的 PersonGroup 識別碼。
	static string sourcePersonGroup = null;

    static string targetPersonGroup = null;

	public Texture2D texture;

	/* 宣告 helper 欄位
	 * 您稍後將新增的幾個臉部作業需要下列欄位。 
	 * 在類別的根目錄中，定義下列 URL 字串。 
	 * 此 URL 會指向範例影像的資料夾。
	 */
	const string URL = "https://csdx.blob.core.windows.net/resources/Face/Images/";

	/* 定義要指向不同辨識模型類型的字串。
	 * Recognition01: 單純偵測人臉位置。
	 * Recognition02: 除了偵測人臉位置，還會取出其特徵，用於辨別情緒、頭髮顏色、性別、表情、、、等資訊。
	 */
	// Used in the Detect Faces and Verify examples.
	// Recognition model 2 is used for feature extraction, use 1 to simply recognize/detect a face. 
	// However, the API calls to Detection that are used with Verify, Find Similar, or Identify must share the same recognition model.
	const string RECOGNITION_MODEL1 = RecognitionModel.Recognition01;
	const string RECOGNITION_MODEL2 = RecognitionModel.Recognition02;

	private void Awake()
    {
        Utils.initConfigData();
    }

    // Start is called before the first frame update
    void Start()
    {
		// Authenticate.
		IFaceClient client = Utils.authenticate(ConfigData.FACE_ENDPOINT, ConfigData.FACE_SUBSCRIPTION_KEY1);

		//_ = Utils.printAllPersonGroupId(client);
		//_ = testPersonGroup(client, "5ce45c41-d387-4afc-8241-279da096e27e");

		#region DetectFaceExtract
		// 偵測影像中的人臉 Detect - get features from faces.
		// 最終的偵測作業會採用 FaceClient 物件、影像 URL 和辨識模型。
		//_ = DetectFaceExtract(client, IMAGE_BASE_URL, RECOGNITION_MODEL2);
		//_ = extractFaceWithUrlDemo(client);
		//_ = extractFaceWithStreamDemo(client);
		#endregion

		#region FindSimilar
		// Find Similar - find a similar face from a list of faces.
		//_ = FindSimilar(client, URL, RECOGNITION_MODEL1);
		#endregion

		#region PersonGroup
		// Identify - recognize a face(s) in a person group (a person group is created in this example).
		//_ = IdentifyInPersonGroup(client, URL, RECOGNITION_MODEL1);
		//identifyInPersonGroupWithTrainingDemo(client);
		//_ = identifyInPersonGroup(client, group_id: "9fecd399-d395-488d-be66-7123165fcd9e", image_name: "identification1.jpg");
		//trainPersonGroupWithStreamAsyncDemo(client, group_id: "9fecd399-d395-488d-be66-7123165fcd9e");
		//_ = customizedPersonGroupDemo(client);
		//appendPersonGroupWithStreamDemo(client, group_id: "c5c2dc65-8bb4-4da8-9da3-707e5184b8a7");
		#endregion

		#region IdentifyInPersonGroup
		// Identify - recognize a face(s) in a person group (a person group is created in this example).
		//_ = IdentifyInPersonGroup(client, URL, RECOGNITION_MODEL1);
		//identifyInPersonGroupWithTrainingDemo(client);
		//_ = identifyInPersonGroup(client, group_id: "9fecd399-d395-488d-be66-7123165fcd9e", image_name: "identification1.jpg");
		#endregion

		#region DeletePersonGroup
		//// 建立了 PersonGroup，但想要將其刪除，請在程式中執行下列程式碼
		//// sourcePersonGroup: PersonGroup 建立時產生的唯一識別碼
		//_ = deletePersonGroup(client, group_id: "b384fcd7-2300-4171-996d-19fbab9b2cf7");

		// 刪除 PersonGroup 當中個別的 Person
		//_ = deletePersonFromGroupAsyncDemo(client, group_id: "9fecd399-d395-488d-be66-7123165fcd9e");
		#endregion

		#region 尚未檢視
		#region 快照集: 用於移轉資料
		/* 設定目標訂用帳戶
		 * 首先，您必須擁有第二個臉部資源 Azure 訂用帳戶；若要這麼做，請遵循設定一節中的步驟。
		 * 然後，在程式的 Main 方法中定義下列變數。 
		 * 您需要為 Azure 帳戶的訂用帳戶識別碼建立新的環境變數，以及為新的 (目標) 帳戶建立金鑰、端點和訂用帳戶識別碼。
		 * 在此範例中，為目標 PersonGroup 的識別碼宣告一個變數—此物件屬於新的訂用帳戶，而您會將資料複製到其中。
		 */

		// 取得第二個臉部資源 Azure 訂用帳戶
		//The Snapshot example needs its own 2nd client, since it uses two different regions.
		//string TARGET_SUBSCRIPTION_KEY = Environment.GetEnvironmentVariable("FACE_SUBSCRIPTION_KEY2");
		//string TARGET_ENDPOINT = Environment.GetEnvironmentVariable("FACE_ENDPOINT2");

		// 取得第一個臉部資源 Azure 訂用帳戶
		//// Grab your subscription ID, from any resource in Azure, 
		//// from the Overview page(all resources have the same subscription ID).
		//Guid AZURE_SUBSCRIPTION_ID = new Guid(Environment.GetEnvironmentVariable("AZURE_SUBSCRIPTION_ID"));

		//// Target subscription ID.It will be the same as the source ID if created Face resources from the same
		//// subscription(but moving from region to region).If they are different subscriptions, add the other
		//// target ID here.
		//Guid TARGET_AZURE_SUBSCRIPTION_ID = new Guid(Environment.GetEnvironmentVariable("AZURE_SUBSCRIPTION_ID")); 

		//// 驗證目標用戶端: 來驗證您的第二個臉部訂用帳戶。
		//// Authenticate for another region or subscription(used in Snapshot only).
		//IFaceClient clientTarget = Utils.Authenticate(TARGET_ENDPOINT, TARGET_SUBSCRIPTION_KEY);

		/* 使用快照集
		 * 其餘快照集作業必須在非同步方法中進行。
		 * 第一個步驟是取得快照集，這會將原始訂用帳戶的臉部資料儲存至暫存的雲端位置。 此方法會傳回識別碼供您查詢作業的狀態。
		 */
		//_ = Snapshot(client, clientTarget, sourcePersonGroup, AZURE_SUBSCRIPTION_ID, TARGET_AZURE_SUBSCRIPTION_ID);
		#endregion

		//// Large FaceList variables
		//const string LargeFaceListId = "mylargefacelistid_001"; // must be lowercase, 0-9, "_" or "-" characters
		//const string LargeFaceListName = "MyLargeFaceListName";

		//// Verify - compare two images if the same person or not.
		//Verify(client, IMAGE_BASE_URL, RECOGNITION_MODEL2).Wait();

		//// LargePersonGroup - create, then get data.
		//LargePersonGroup(client, IMAGE_BASE_URL, RECOGNITION_MODEL1).Wait();
		//// Group faces - automatically group similar faces.
		//Group(client, IMAGE_BASE_URL, RECOGNITION_MODEL1).Wait();
		//// FaceList - create a face list, then get data
		//FaceListOperations(client, IMAGE_BASE_URL).Wait();
		//// Large FaceList - create a large face list, then get data
		//LargeFaceListOperations(client, IMAGE_BASE_URL).Wait();


		//// <snippet_persongroup_delete>
		//// At end, delete person groups in both regions (since testing only)
		//Debug.Log("========DELETE PERSON GROUP========"); 
		#endregion
	}

	// Update is called once per frame
	void Update()
    {
        
    }

	#region Origin demo code
	/* DETECT FACES
	 * Detects features from faces and IDs them.
	 * 偵測指定 URL 上三個影像中的臉部，並在程式記憶體中建立 DetectedFace 物件的清單。 
	 * FaceAttributeType 值的清單會指定要擷取的特徵。
	 * 
	 * 偵測影像中的人臉 Detect - get features from faces.
	 * 最終的偵測作業會採用 FaceClient 物件、影像 URL 和辨識模型。
	 */
	public static async Task DetectFaceExtract(IFaceClient client, string url, string recognition_model)
	{
		Debug.Log(string.Format("[Main] DetectFaceExtract"));
		Debug.Log("========DETECT FACES========");

		// Create a list of images
		List<string> imageFileNames = new List<string>
		{
			"detection1.jpg",    // single female with glasses
			// "detection2.jpg", // (optional: single man)
			// "detection3.jpg", // (optional: single male construction worker)
			// "detection4.jpg", // (optional: 3 people at cafe, 1 is blurred)
			"detection5.jpg",    // family, woman child man
			"detection6.jpg"     // elderly couple, male female
		};

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

		// 儲存偵測到的臉部列表
		IList<DetectedFace> detectedFaces;

		float start_time;
		foreach (var imageFileName in imageFileNames)
		{
			start_time = Utils.getCurrentTimestamp();

			// Detect faces with all attributes from image url.			
			detectedFaces = await client.Face.DetectWithUrlAsync(
					string.Format("{0}{1}", url, imageFileName),
					returnFaceAttributes: return_face_attributes,
					recognitionModel: recognition_model);

			Debug.Log(string.Format("[Main] DetectFaceExtract | {0} face(s) detected from image {1}.",
				detectedFaces.Count, imageFileName));

			Debug.Log(string.Format("[Main] DetectFaceExtract | imageFileName: {0}", imageFileName));
			await new UnityEngine.WaitForSecondsRealtime(Time.deltaTime);

			// 剖析每個偵測到的臉部，並列印屬性資料。
			// Parse and print all attributes of each detected face.
			foreach (var face in detectedFaces)
			{

				Debug.Log(string.Format("[Main] DetectFaceExtract | Face attributes for {0}:", imageFileName));

				// Get bounding box of the faces
				Debug.Log(string.Format("[Main] DetectFaceExtract | Rectangle(Left/Top/Width/Height): {0}, {1}, {2}, {3}",
					face.FaceRectangle.Left, face.FaceRectangle.Top, face.FaceRectangle.Width, face.FaceRectangle.Height));

				// Get accessories of the faces
				List<Accessory> accessoriesList = (List<Accessory>)face.FaceAttributes.Accessories;
				int count = face.FaceAttributes.Accessories.Count;
				string accessory;
				string[] accessoryArray = new string[count];
				if (count == 0)
				{
					accessory = "NoAccessories";
				}
				else
				{
					for (int i = 0; i < count; ++i)
					{
						accessoryArray[i] = accessoriesList[i].Type.ToString();
					}

					accessory = string.Join(",", accessoryArray);
				}

				Debug.Log(string.Format("[Main] DetectFaceExtract | Accessories: {0}", accessory));

				// Get face other attributes
				Debug.Log(string.Format("[Main] DetectFaceExtract | Age: {0}", face.FaceAttributes.Age));
				Debug.Log(string.Format("[Main] DetectFaceExtract | Blur: {0}", face.FaceAttributes.Blur.BlurLevel));

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

				Debug.Log(string.Format("[Main] DetectFaceExtract | Emotion: {0}", emotionType));

				// Get more face attributes
				Debug.Log(string.Format("[Main] DetectFaceExtract | Exposure: {0}", face.FaceAttributes.Exposure.ExposureLevel));
				Debug.Log(string.Format("[Main] DetectFaceExtract | FacialHair: {0}", face.FaceAttributes.FacialHair.Moustache + face.FaceAttributes.FacialHair.Beard + face.FaceAttributes.FacialHair.Sideburns > 0 ? "Yes" : "No"));
				Debug.Log(string.Format("[Main] DetectFaceExtract | Gender: {0}", face.FaceAttributes.Gender));
				Debug.Log(string.Format("[Main] DetectFaceExtract | Glasses: {0}", face.FaceAttributes.Glasses));

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

				HairColorType returnColor = HairColorType.Unknown;
				double maxConfidence = 0.0f;
				foreach (HairColor hairColor in hair.HairColor)
				{
					if (hairColor.Confidence <= maxConfidence)
					{
						continue;
					}

					maxConfidence = hairColor.Confidence;
					returnColor = hairColor.Color;
					color = returnColor.ToString();
				}

				Debug.Log(string.Format("[Main] DetectFaceExtract | Hair: {0}", color));

				// Get more attributes
				Debug.Log(string.Format("[Main] DetectFaceExtract | HeadPose: Pitch: {0}, Roll: {1}, Yaw: {2}",
					Math.Round(face.FaceAttributes.HeadPose.Pitch, 2),
					Math.Round(face.FaceAttributes.HeadPose.Roll, 2),
					Math.Round(face.FaceAttributes.HeadPose.Yaw, 2)));
				Debug.Log(string.Format("[Main] DetectFaceExtract | Makeup: {0}", (face.FaceAttributes.Makeup.EyeMakeup || face.FaceAttributes.Makeup.LipMakeup) ? "Yes" : "No"));
				Debug.Log(string.Format("[Main] DetectFaceExtract | Noise: {0}", face.FaceAttributes.Noise.NoiseLevel));
				Debug.Log(string.Format("[Main] DetectFaceExtract | Occlusion: EyeOccluded: {0}, ForeheadOccluded: {1}, MouthOccluded: {2}",
					(face.FaceAttributes.Occlusion.EyeOccluded ? "Yes" : "No"),
					(face.FaceAttributes.Occlusion.ForeheadOccluded ? "Yes" : "No"),
					(face.FaceAttributes.Occlusion.MouthOccluded ? "Yes" : "No")));
				Debug.Log(string.Format("[Main] DetectFaceExtract | Smile: {0}", face.FaceAttributes.Smile));


			}

			Debug.Log(string.Format("[Main] DetectFaceExtract | timestemp: {0:F8}", Utils.getCurrentTimestamp() - start_time));
		}



		Debug.Log("==================================================");
		Debug.Log("==================================================");
	}

	#region 尋找類似臉部
	/* 下列程式碼會取得一個偵測到的臉部 (來源)，並搜尋一組其他臉部 (目標) 來尋找相符臉部。
	 * 若找到相符的臉部，便會將相符臉部的識別碼列印到主控台。
	 */

	/* 尋找相符臉部
	 * 下列方法會在一組目標影像和單一來源影像中偵測臉部。 
	 * 然後，該方法會比較這些影像，並尋找與來源影像類似的所有目標影像。
	 */
	public static async Task FindSimilar(IFaceClient client, string url, string RECOGNITION_MODEL1)
	{
		Debug.Log("========FIND SIMILAR========");
		string full_url;

		List<string> targetImageFileNames = new List<string>
		{
			"Family1-Dad1.jpg",
			"Family1-Daughter1.jpg",
			"Family1-Mom1.jpg",
			"Family1-Son1.jpg",
			"Family2-Lady1.jpg",
			"Family2-Man1.jpg",
			"Family3-Lady1.jpg",
			"Family3-Man1.jpg"
		};

		string sourceImageFileName = "findsimilar.jpg";
		IList<Guid?> targetFaceIds = new List<Guid?>();
		float start_time;
		foreach (var targetImageFileName in targetImageFileNames)
		{
			start_time = Utils.getCurrentTimestamp();

			// Detect faces from target image url.
			full_url = string.Format("{0}{1}", url, targetImageFileName);
			var faces = await DetectFaceRecognize(client, full_url, RECOGNITION_MODEL1);
			// Add detected faceId to list of GUIDs.
			Guid? item = faces[0].FaceId.Value;
			Debug.Log(string.Format("[Main] FindSimilar | Guid item: {0}, file_name: {1}", item, targetImageFileName));
			targetFaceIds.Add(item);

			Debug.Log(string.Format("[Main] FindSimilar | timestemp: {0:F8}", Utils.getCurrentTimestamp() - start_time));
		}

		// Detect faces from source image url.
		full_url = string.Format("{0}{1}", url, sourceImageFileName);
		IList<DetectedFace> detectedFaces = await DetectFaceRecognize(client, full_url, RECOGNITION_MODEL1);

		// Find a similar face(s) in the list of IDs. Comapring only the first in list for testing purposes.
		IList<SimilarFace> similarResults = await client.Face.FindSimilarAsync(
			detectedFaces[0].FaceId.Value, null, null, targetFaceIds);

		/* 列印相符臉部
		 * 下列程式碼會將相符臉部的詳細資料列印到主控台：
		 */
		foreach (var similarResult in similarResults)
		{
			//Debug.Log($"Faces from {sourceImageFileName} & ID:{similarResult.FaceId} are similar with confidence: {similarResult.Confidence}.");
			Debug.Log(string.Format("[Main] FindSimilar | Faces from {0} & ID: {1} are similar with confidence: {2}",
				sourceImageFileName, similarResult.FaceId, similarResult.Confidence));
		}

	}

	/* DetectFaceRecognize: 建立在影像中偵測到的所有臉部清單
	 * 首先，定義第二個臉部偵測方法。 您必須先偵測影像中的臉部，才能進行比較，而此偵測方法已針對比較作業進行最佳化。 
	 * 該方法不會擷取詳細的臉部特性 (如上節中所述)，而是會使用不同的辨識模型。
	 */
	private static async Task<List<DetectedFace>> DetectFaceRecognize(IFaceClient faceClient, string url, string RECOGNITION_MODEL1)
	{
		// Detect faces from image URL. Since only recognizing, use the recognition model 1.
		IList<DetectedFace> detectedFaces = await faceClient.Face.DetectWithUrlAsync(url, recognitionModel: RECOGNITION_MODEL1);
		//Debug.Log($"{detectedFaces.Count} face(s) detected from image `{Path.GetFileName(url)}`");
		Debug.Log(string.Format("[Main] DetectFaceRecognize | {0} face(s) detected from image {1}",
			detectedFaces.Count, Path.GetFileName(url)));
		return detectedFaces.ToList();
	}
	#endregion

	/*
	 * VERIFY
	 * The Verify operation takes a face ID from DetectedFace or PersistedFace and either another face ID 
	 * or a Person object and determines whether they belong to the same person. If you pass in a Person object, 
	 * you can optionally pass in a PersonGroup to which that Person belongs to improve performance.
	 */
	public static async Task Verify(IFaceClient client, string url, string recognitionModel02)
	{
		Debug.Log("========VERIFY========");


		List<string> targetImageFileNames = new List<string> { "Family1-Dad1.jpg", "Family1-Dad2.jpg" };
		string sourceImageFileName1 = "Family1-Dad3.jpg";
		string sourceImageFileName2 = "Family1-Son1.jpg";


		List<Guid> targetFaceIds = new List<Guid>();
		foreach (var imageFileName in targetImageFileNames)
		{
			// Detect faces from target image url.
			List<DetectedFace> detectedFaces = await DetectFaceRecognize(client, $"{url}{imageFileName} ", recognitionModel02);
			targetFaceIds.Add(detectedFaces[0].FaceId.Value);
			Debug.Log($"{detectedFaces.Count} faces detected from image `{imageFileName}`.");
		}

		// Detect faces from source image file 1.
		List<DetectedFace> detectedFaces1 = await DetectFaceRecognize(client, $"{url}{sourceImageFileName1} ", recognitionModel02);
		Debug.Log($"{detectedFaces1.Count} faces detected from image `{sourceImageFileName1}`.");
		Guid sourceFaceId1 = detectedFaces1[0].FaceId.Value;

		// Detect faces from source image file 2.
		List<DetectedFace> detectedFaces2 = await DetectFaceRecognize(client, $"{url}{sourceImageFileName2} ", recognitionModel02);
		Debug.Log($"{detectedFaces2.Count} faces detected from image `{sourceImageFileName2}`.");
		Guid sourceFaceId2 = detectedFaces2[0].FaceId.Value;

		// Verification example for faces of the same person.
		VerifyResult verifyResult1 = await client.Face.VerifyFaceToFaceAsync(sourceFaceId1, targetFaceIds[0]);
		Debug.Log(
			verifyResult1.IsIdentical
				? $"Faces from {sourceImageFileName1} & {targetImageFileNames[0]} are of the same (Positive) person, similarity confidence: {verifyResult1.Confidence}."
				: $"Faces from {sourceImageFileName1} & {targetImageFileNames[0]} are of different (Negative) persons, similarity confidence: {verifyResult1.Confidence}.");

		// Verification example for faces of different persons.
		VerifyResult verifyResult2 = await client.Face.VerifyFaceToFaceAsync(sourceFaceId2, targetFaceIds[0]);
		Debug.Log(
			verifyResult2.IsIdentical
				? $"Faces from {sourceImageFileName2} & {targetImageFileNames[0]} are of the same (Negative) person, similarity confidence: {verifyResult2.Confidence}."
				: $"Faces from {sourceImageFileName2} & {targetImageFileNames[0]} are of different (Positive) persons, similarity confidence: {verifyResult2.Confidence}.");


	}

	/*
	 * IDENTIFY FACES  此方法會執行識別作業。
	 * To identify faces, you need to create and define a person group.
	 * The Identify operation takes one or several face IDs from DetectedFace or PersistedFace and a PersonGroup and returns 
	 * a list of Person objects that each face might belong to. Returned Person objects are wrapped as Candidate objects, 
	 * which have a prediction confidence value.
	 */
	public static async Task IdentifyInPersonGroup(IFaceClient client, string url, string recognitionModel)
	{
		Debug.Log(string.Format("[Main] IdentifyInPersonGroup"));

		#region Create PersonGroup
		// Create a dictionary for all your images, grouping similar ones under the same key.
		// 將人員的名稱與範例影像產生關聯。
		Dictionary<string, string[]> personDictionary = new Dictionary<string, string[]>
		{
			{ "Family1-Dad", new[] { "Family1-Dad1.jpg", "Family1-Dad2.jpg" } },
			{ "Family1-Mom", new[] { "Family1-Mom1.jpg", "Family1-Mom2.jpg" } },
			{ "Family1-Son", new[] { "Family1-Son1.jpg", "Family1-Son2.jpg" } },
			{ "Family1-Daughter", new[] { "Family1-Daughter1.jpg", "Family1-Daughter2.jpg" } },
			{ "Family2-Lady", new[] { "Family2-Lady1.jpg", "Family2-Lady2.jpg" } },
			{ "Family2-Man", new[] { "Family2-Man1.jpg", "Family2-Man2.jpg" } }
		};

		/* 在字典中為每個人建立 Person 物件，並從適當的影像加入臉部資料。 
		 * 每個 Person 物件都會透過其唯一的識別碼字串(personGroupId)，與相同的 PersonGroup 產生關聯。
		 */
		string personGroupId = Guid.NewGuid().ToString();
		Debug.Log(string.Format("[Main] IdentifyInPersonGroup | Create a person group ({0})", personGroupId));

		// This is solely for the snapshot operations example
		sourcePersonGroup = personGroupId;

		// A person group is the container of the uploaded person data, including face images and face recognition features.
		await client.PersonGroup.CreateAsync(personGroupId, personGroupId, recognitionModel: recognitionModel);

		// The similar faces will be grouped into a single person group person.
		foreach (var groupedFace in personDictionary.Keys)
		{
			// Limit TPS
			await Task.Delay(250);

			// 取得 PersonGroup 裡面的 Person
			Person person = await client.PersonGroupPerson.CreateAsync(personGroupId: personGroupId, name: groupedFace);
			Debug.Log(string.Format("[Main] IdentifyInPersonGroup | Create a person group person '{0}'.", groupedFace));

			// Add face to the person group person.
			// 將同一 person 的臉部資料，加入 person 當中
			foreach (var similarImage in personDictionary[groupedFace])
			{
				Debug.Log(string.Format("[Main] IdentifyInPersonGroup | Add face to the person group person({0}) " +
					"from image {1}", groupedFace, similarImage));

				// AddFaceFromStreamAsync
				PersistedFace face = await client.PersonGroupPerson.AddFaceFromUrlAsync(personGroupId, person.PersonId,
					string.Format("{0}{1}", url, similarImage), similarImage);
			}
		}
		#endregion

		#region PersonGroup training
		/* 訓練 PersonGroup
		 * 必須訓練 PersonGroup，以識別與其每一個 Person 物件相關聯的視覺特徵。
		 * 下列程式碼會呼叫非同步訓練方法並輪詢結果。
		 */
		Debug.Log(string.Format("[Main] IdentifyInPersonGroup | Train person group {0}", personGroupId));
		await client.PersonGroup.TrainAsync(personGroupId);

		// Wait until the training is completed.
		while (true)
		{
			await Task.Delay(1000);

			// 取得訓練狀態
			TrainingStatus trainingStatus = await client.PersonGroup.GetTrainingStatusAsync(personGroupId);
			Debug.Log(string.Format("[Main] IdentifyInPersonGroup | Training status: {0}.", trainingStatus.Status));

			if (trainingStatus.Status == TrainingStatusType.Succeeded)
			{
				break;
			}
		}
		#endregion

		#region identify_sources
		// 取得測試影像
		// A group photo that includes some of the persons you seek to identify from your dictionary.
		string sourceImageFileName = "identification1.jpg";

		// DetectFaceRecognize: 建立在影像中偵測到的所有臉部清單
		List<DetectedFace> detectedFaces = await DetectFaceRecognize(
			client, string.Format("{0}{1}", url, sourceImageFileName), RECOGNITION_MODEL1: recognitionModel);

		// Add detected faceId to sourceFaceIds.
		List<Guid> sourceFaceIds = new List<Guid>();
		foreach (var detectedFace in detectedFaces)
		{
			sourceFaceIds.Add(detectedFace.FaceId.Value);
		}
		#endregion

		#region identify
		// Identify the faces in a person group. 
		// 輸入臉部 ID 列表(sourceFaceIds)，IdentifyAsync 當中會藉由此 ID 去取用臉部資訊，進而和 PersonGroup 當中的人臉做比對
		// 輸出較為相似的
		IList<IdentifyResult> identifyResults = await client.Face.IdentifyAsync(sourceFaceIds, personGroupId);

		foreach (IdentifyResult identifyResult in identifyResults)
		{
			// 取出相似可能候選第一位的 ID
			Person person = await client.PersonGroupPerson.GetAsync(personGroupId, identifyResult.Candidates[0].PersonId);

			Debug.Log(string.Format("[Main] IdentifyInPersonGroup | Person '{0}' is identified for face " +
				"in: {1} - {2},  confidence: {3}.",
				person.Name, sourceImageFileName, identifyResult.FaceId, identifyResult.Candidates[0].Confidence));
		}
		#endregion
	}


	/*
	 * LARGE PERSON GROUP
	 * The example will create a large person group, retrieve information from it, 
	 * list the Person IDs it contains, and finally delete a large person group.
	 * For simplicity, the same images are used for the regular-sized person group in IDENTIFY FACES of this quickstart.
	 * A large person group is made up of person group persons. 
	 * One person group person is made up of many similar images of that person, which are each PersistedFace objects.
	 */
	public static async Task LargePersonGroup(IFaceClient client, string url, string recognitionModel)
	{
		Debug.Log("========LARGE PERSON GROUP========");


		// Create a dictionary for all your images, grouping similar ones under the same key.
		Dictionary<string, string[]> personDictionary =
		new Dictionary<string, string[]>
		{ 
			{ "Family1-Dad", new[] { "Family1-Dad1.jpg", "Family1-Dad2.jpg" } },
			{ "Family1-Mom", new[] { "Family1-Mom1.jpg", "Family1-Mom2.jpg" } },
			{ "Family1-Son", new[] { "Family1-Son1.jpg", "Family1-Son2.jpg" } },
			{ "Family1-Daughter", new[] { "Family1-Daughter1.jpg", "Family1-Daughter2.jpg" } },
			{ "Family2-Lady", new[] { "Family2-Lady1.jpg", "Family2-Lady2.jpg" } },
			{ "Family2-Man", new[] { "Family2-Man1.jpg", "Family2-Man2.jpg" } }
		};

		// Create a large person group ID. 
		string largePersonGroupId = Guid.NewGuid().ToString();
		Debug.Log($"Create a large person group ({largePersonGroupId}).");

		// Create the large person group
		await client.LargePersonGroup.CreateAsync(largePersonGroupId: largePersonGroupId, name: largePersonGroupId, recognitionModel);

		// Create Person objects from images in our dictionary
		// We'll store their IDs in the process
		List<Guid> personIds = new List<Guid>();
		foreach (var groupedFace in personDictionary.Keys)
		{
			// Limit TPS
			await Task.Delay(250);

			Person personLarge = await client.LargePersonGroupPerson.CreateAsync(largePersonGroupId, groupedFace);

			Debug.Log($"Create a large person group person '{groupedFace}' ({personLarge.PersonId}).");

			// Store these IDs for later retrieval
			personIds.Add(personLarge.PersonId);

			// Add face to the large person group person.
			foreach (var image in personDictionary[groupedFace])
			{
				Debug.Log($"Add face to the person group person '{groupedFace}' from image `{image}`");
				PersistedFace face = await client.LargePersonGroupPerson.AddFaceFromUrlAsync(largePersonGroupId, personLarge.PersonId,
					$"{url}{image}", image);
			}
		}

		// Start to train the large person group.

		Debug.Log($"Train large person group {largePersonGroupId}.");
		await client.LargePersonGroup.TrainAsync(largePersonGroupId);

		// Wait until the training is completed.
		while (true)
		{
			await Task.Delay(1000);
			var trainingStatus = await client.LargePersonGroup.GetTrainingStatusAsync(largePersonGroupId);
			Debug.Log($"Training status: {trainingStatus.Status}.");
			if (trainingStatus.Status == TrainingStatusType.Succeeded) { break; }
		}


		// Now that we have created and trained a large person group, we can retrieve data from it.
		// Get list of persons and retrieve data, starting at the first Person ID in previously saved list.
		IList<Person> persons = await client.LargePersonGroupPerson.ListAsync(largePersonGroupId, start: "");

		Debug.Log($"Persisted Face IDs (from {persons.Count} large person group persons): ");
		foreach (Person person in persons)
		{
			foreach (Guid pFaceId in person.PersistedFaceIds)
			{
				Debug.Log($"The person '{person.Name}' has an image with ID: {pFaceId}");
			}
		}


		// After testing, delete the large person group, PersonGroupPersons also get deleted.
		await client.LargePersonGroup.DeleteAsync(largePersonGroupId);
		Debug.Log($"Deleted the large person group {largePersonGroupId}.");

	}
	/*
	 * END - LARGE PERSON GROUP
	 */

	/*
	 * GROUP FACES
	 * This method of grouping is useful if you don't need to create a person group. It will automatically group similar
	 * images, whereas the person group method allows you to define the grouping.
	 * A single "messyGroup" array contains face IDs for which no similarities were found.
	 */
	public static async Task Group(IFaceClient client, string url, string RECOGNITION_MODEL1)
	{
		Debug.Log("========GROUP FACES========");


		// Create list of image names
		List<string> imageFileNames = new List<string>
		{
			"Family1-Dad1.jpg",
			"Family1-Dad2.jpg",
			"Family3-Lady1.jpg",
			"Family1-Daughter1.jpg",
			"Family1-Daughter2.jpg",
			"Family1-Daughter3.jpg"
		};

		// Create empty dictionary to store the groups
		Dictionary<string, string> faces = new Dictionary<string, string>();
		List<Guid> faceIds = new List<Guid>();

		// First, detect the faces in your images
		foreach (var imageFileName in imageFileNames)
		{
			// Detect faces from image url.
			IList<DetectedFace> detectedFaces = await DetectFaceRecognize(client, $"{url}{imageFileName}", RECOGNITION_MODEL1);
			// Add detected faceId to faceIds and faces.
			faceIds.Add(detectedFaces[0].FaceId.Value);
			faces.Add(detectedFaces[0].FaceId.ToString(), imageFileName);
		}

		// Group the faces. Grouping result is a group collection, each group contains similar faces.
		var groupResult = await client.Face.GroupAsync(faceIds);

		// Face groups contain faces that are similar to all members of its group.
		for (int i = 0; i < groupResult.Groups.Count; i++)
		{
			Console.Write($"Found face group {i + 1}: ");
			foreach (var faceId in groupResult.Groups[i]) { Console.Write($"{faces[faceId.ToString()]} "); }
			Debug.Log(".");
		}

		// MessyGroup contains all faces which are not similar to any other faces. The faces that cannot be grouped.
		if (groupResult.MessyGroup.Count > 0)
		{
			Console.Write("Found messy face group: ");
			foreach (var faceId in groupResult.MessyGroup) { Console.Write($"{faces[faceId.ToString()]} "); }
			Debug.Log(".");
		}

	}
	/*
	 * END - GROUP FACES
	 */

	/*
	 * FACELIST OPERATIONS
	 * Create a face list and add single-faced images to it, then retrieve data from the faces.
	 * Images are used from URLs.
	 */
	public static async Task FaceListOperations(IFaceClient client, string baseUrl)
	{
		Debug.Log("========FACELIST OPERATIONS========");


		const string FaceListId = "myfacelistid_001";
		const string FaceListName = "MyFaceListName";

		// Create an empty FaceList with user-defined specifications, it gets stored in the client.
		await client.FaceList.CreateAsync(faceListId: FaceListId, name: FaceListName);

		// Create a list of single-faced images to append to base URL. Images with mulitple faces are not accepted.
		List<string> imageFileNames = new List<string>
		{
			"detection1.jpg",    // single female with glasses
			"detection2.jpg",    // single male
			"detection3.jpg",    // single male construction worker
		};

		// Add Faces to the FaceList.
		foreach (string image in imageFileNames)
		{
			string urlFull = baseUrl + image;
			// Returns a Task<PersistedFace> which contains a GUID, and is stored in the client.
			await client.FaceList.AddFaceFromUrlAsync(faceListId: FaceListId, url: urlFull);
		}

		// Print the face list
		Debug.Log("Face IDs from the face list: ");


		// List the IDs of each stored image
		FaceList faceList = await client.FaceList.GetAsync(FaceListId);

		foreach (PersistedFace face in faceList.PersistedFaces)
		{
			Debug.Log(face.PersistedFaceId);
		}

		// Delete the face list, for repetitive testing purposes (cannot recreate list with same name).
		await client.FaceList.DeleteAsync(FaceListId);

		Debug.Log("Deleted the face list.");

	}
	/*
	 * END - FACELIST OPERATIONS
	 */

	/*
	* LARGE FACELIST OPERATIONS
	* Create a large face list and adds single-faced images to it, then retrieve data from the faces.
	* Images are used from URLs. Large face lists are preferred for scale, up to 1 million images.
	*/
	public static async Task LargeFaceListOperations(IFaceClient client, string baseUrl)
	{
		Debug.Log("======== LARGE FACELIST OPERATIONS========");


		const string LargeFaceListId = "mylargefacelistid_001"; // must be lowercase, 0-9, or "_"
		const string LargeFaceListName = "MyLargeFaceListName";
		const int timeIntervalInMilliseconds = 1000; // waiting time in training

		List<string> singleImages = new List<string>
		{
			"Family1-Dad1.jpg",
			"Family1-Daughter1.jpg",
			"Family1-Mom1.jpg",
			"Family1-Son1.jpg",
			"Family2-Lady1.jpg",
			"Family2-Man1.jpg",
			"Family3-Lady1.jpg",
			"Family3-Man1.jpg"
		};

		// Create a large face list
		Debug.Log("Creating a large face list...");
		await client.LargeFaceList.CreateAsync(largeFaceListId: LargeFaceListId, name: LargeFaceListName);

		// Add Faces to the LargeFaceList.
		Debug.Log("Adding faces to a large face list...");
		foreach (string image in singleImages)
		{
			// Returns a PersistedFace which contains a GUID.
			await client.LargeFaceList.AddFaceFromUrlAsync(largeFaceListId: LargeFaceListId, url: $"{baseUrl}{image}");
		}

		// Training a LargeFaceList is what sets it apart from a regular FaceList.
		// You must train before using the large face list, for example to use the Find Similar operations.
		Debug.Log("Training a large face list...");
		await client.LargeFaceList.TrainAsync(LargeFaceListId);

		// Wait for training finish.
		while (true)
		{
			Task.Delay(timeIntervalInMilliseconds).Wait();
			var status = await client.LargeFaceList.GetTrainingStatusAsync(LargeFaceListId);

			if (status.Status == TrainingStatusType.Running)
			{
				Debug.Log($"Training status: {status.Status}");
				continue;
			}
			else if (status.Status == TrainingStatusType.Succeeded)
			{
				Debug.Log($"Training status: {status.Status}");
				break;
			}
			else
			{
				throw new Exception("The train operation has failed!");
			}
		}

		// Print the large face list

		Debug.Log("Face IDs from the large face list: ");

		Parallel.ForEach(
				await client.LargeFaceList.ListFacesAsync(LargeFaceListId),
				faceId =>
				{
					Debug.Log(faceId.PersistedFaceId);
				}
			);

		// Delete the large face list, for repetitive testing purposes (cannot recreate list with same name).
		await client.LargeFaceList.DeleteAsync(LargeFaceListId);

		Debug.Log("Deleted the large face list.");

	}
	/*
	* END - LARGE FACELIST OPERATIONS
	*/


	#region 快照集: 用於移轉資料
	/* 快照集功能可讓您將已儲存的臉部資料 (例如已完成訓練的 PersonGroup) 移至不同的 Azure 認知服務臉部訂用帳戶。 
	 * 舉例來說，如果您已使用免費試用版訂用帳戶建立 PersonGroup 物件，而且想要將該物件遷移至付費訂用帳戶，就可以使用此功能。 
	 * 如需快照集功能的概觀，請參閱遷移臉部資料。
	 * 在此範例中，您會遷移在建立並訓練人員群組中所建立的 PersonGroup。 您可以先完成該區段，或建立要遷移的自有臉部資料建構。
	 */
	public static async Task Snapshot(IFaceClient clientSource, IFaceClient clientTarget, string personGroupId, Guid azureId, Guid targetAzureId)
	{
		Debug.Log("========SNAPSHOT OPERATIONS========");

		/* 第一個步驟是取得快照集，這會將原始訂用帳戶的臉部資料儲存至暫存的雲端位置。 此方法會傳回識別碼供您查詢作業的狀態。 */
		// Take a snapshot for the person group that was previously created in your source region.
		// add targetAzureId to this array if your target ID is different from your source ID.
		var takeSnapshotResult = await clientSource.Snapshot.TakeAsync(
			SnapshotObjectType.PersonGroup, personGroupId, new[] { azureId });

		// Get operation id from response for tracking the progress of snapshot taking.
		var operationId = Guid.Parse(takeSnapshotResult.OperationLocation.Split('/')[2]);
		Debug.Log($"Taking snapshot(operation ID: {operationId})... Started");

		/* 查詢識別碼直到作業完成 */
		OperationStatus operationStatus = null;
		do
		{
			Thread.Sleep(TimeSpan.FromMilliseconds(1000));
			// Get the status of the operation.
			operationStatus = await clientSource.Snapshot.GetOperationStatusAsync(operationId);
			Debug.Log($"Operation Status: {operationStatus.Status}");
		}
		while (operationStatus.Status != OperationStatusType.Succeeded && operationStatus.Status != OperationStatusType.Failed);
		// Confirm the location of the resource where the snapshot is taken and its snapshot ID
		var snapshotId = Guid.Parse(operationStatus.ResourceLocation.Split('/')[2]);
		Debug.Log($"Source region snapshot ID: {snapshotId}");
		Debug.Log($"Taking snapshot of person group: {personGroupId}... Done\n");


		/* 使用套用作業將臉部資料寫入至目標訂用帳戶。 此方法也會傳回識別碼值。 */
		var newPersonGroupId = Guid.NewGuid().ToString();
		targetPersonGroup = newPersonGroupId;

		try
		{
			var applySnapshotResult = await clientTarget.Snapshot.ApplyAsync(snapshotId, newPersonGroupId);

			// Get operation id from response for tracking the progress of snapshot applying.
			var applyOperationId = Guid.Parse(applySnapshotResult.OperationLocation.Split('/')[2]);
			Debug.Log($"Applying snapshot(operation ID: {applyOperationId})... Started");

			/* 同樣地，請查詢新識別碼直到作業完成。 */
			do
			{
				Thread.Sleep(TimeSpan.FromMilliseconds(1000));
				// Get the status of the operation.
				operationStatus = await clientSource.Snapshot.GetOperationStatusAsync(applyOperationId);
				Debug.Log($"Operation Status: {operationStatus.Status}");
			}
			while (operationStatus.Status != OperationStatusType.Succeeded &&
			operationStatus.Status != OperationStatusType.Failed);

			// Confirm location of the target resource location, with its ID.
			Debug.Log(string.Format("Person group in new region: {0}", newPersonGroupId));
			Debug.Log("Applying snapshot... Done");
		}
		catch (Exception e)
		{
			throw new ApplicationException("Do you have a second Face resource in Azure? " +
				"It's needed to transfer the person group to it for the Snapshot example.", e);
		}

		/* 此時，新 PersonGroup 物件應具有與原始物件相同的資料，而且應該可從新的 (目標) Azure 臉部訂用帳戶存取。 */
	}
	#endregion

	/* 清除資源
	 * 如果您想要清除和移除認知服務訂用帳戶，則可以刪除資源或資源群組。 刪除資源群組也會刪除其關聯的任何其他資源。*/
	public static async Task deletePersonGroup(IFaceClient client, string group_id)
	{
		await client.PersonGroup.DeleteAsync(group_id);
		Debug.Log(string.Format("Deleted the person group {0}.", group_id));
	}
	#endregion

	#region My demo code
	#region Extract face
	public async Task extractFaceWithUrlDemo(IFaceClient client)
	{
		// Create a list of images
		List<string> image_names = new List<string>
		{
			"detection1.jpg",    // single female with glasses
			// "detection2.jpg", // (optional: single man)
			// "detection3.jpg", // (optional: single male construction worker)
			// "detection4.jpg", // (optional: 3 people at cafe, 1 is blurred)
			"detection5.jpg",    // family, woman child man
			"detection6.jpg"     // elderly couple, male female
		};

		foreach (string image_name in image_names)
		{
			Debug.Log(string.Format("[Main] extractFaceWithUrlDemo | {0}", image_name));
			await Utils.extractFaceWithUrl(client, string.Format("{0}{1}", URL, image_name));
			Debug.Log("[Main] extractFaceWithUrlDemo | ==========");
		}
	}

	public async Task extractFaceWithStreamDemo(IFaceClient client)
	{
		List<string> image_names = new List<string>
		{
			"detection1.jpg",    // single female with glasses
			// "detection2.jpg", // (optional: single man)
			// "detection3.jpg", // (optional: single male construction worker)
			// "detection4.jpg", // (optional: 3 people at cafe, 1 is blurred)
			"detection5.jpg",    // family, woman child man
			"detection6.jpg"     // elderly couple, male female
		};

		foreach (string image_name in image_names)
		{
			Stream image = File.OpenRead(Path.Combine(Application.streamingAssetsPath, "image", image_name));
			Debug.Log(string.Format("[Main] extractFaceWithStreamDemo | {0}", image_name));
			await Utils.extractFaceWithStream(client, image);
			Debug.Log(string.Format("[Main] extractFaceWithStreamDemo | ==============="));
		}
	}
	#endregion

	#region PersonGroup
	public async Task customizedPersonGroupDemo(IFaceClient client)
	{
		//Dictionary<string, string[]> person_dict = new Dictionary<string, string[]> {
		//	{ "B1", new[] { "B1-1.PNG", "B1-2.PNG", "B1-3.PNG", "B1-4.PNG" } },
		//	{ "B2", new[] { "B2-1.PNG", "B2-2.PNG", "B2-3.PNG", "B2-4.PNG" } },
		//	{ "B3", new[] { "B3-1.PNG", "B3-2.PNG", "B3-3.PNG", "B3-4.PNG" } },
		//	{ "B4", new[] { "B4-1.PNG", "B4-2.PNG", "B4-3.PNG", "B4-4.PNG" } },
		//	{ "B5", new[] { "B5-1.PNG", "B5-2.PNG", "B5-3.PNG", "B5-4.PNG" } },
		//	{ "G1", new[] { "G1-1.PNG", "G1-2.PNG", "G1-3.PNG", "G1-4.PNG" } },
		//	{ "G2", new[] { "G2-1.PNG", "G2-2.PNG", "G2-3.PNG", "G2-4.PNG" } },
		//	{ "G3", new[] { "G3-1.PNG", "G3-2.PNG", "G3-3.PNG", "G3-4.PNG" } },
		//	{ "G4", new[] { "G4-1.PNG", "G4-2.PNG", "G4-3.PNG", "G4-4.PNG" } },
		//	{ "G5", new[] { "G5-1.PNG", "G5-2.PNG", "G5-3.PNG", "G5-4.PNG" } },
		//	//{ "MB1", new[] { "MB1-1.PNG", "MB1-2.PNG", "MB1-3.PNG", "MB1-4.PNG" } },
		//	//{ "MG1", new[] { "MG1-1.PNG", "MG1-2.PNG", "MG1-3.PNG", "MG1-4.PNG" } }
		//};

		//string group_id = await createPersonGroupWithStreamAsync(client);
		//Debug.Log(string.Format("[Main] customizedPersonGroupDemo | group_id: {0}", group_id));

		//await appendPersonGroupWithStreamAsync(client, group_id, person_dict);

		string group_id = "c5c2dc65-8bb4-4da8-9da3-707e5184b8a7";
		//await trainPersonGroupAsync(client, group_id);

		//string[] images = {
		//	"B2-5.PNG", "B3-5.PNG", "B4-5.PNG", "B5-5.PNG",
		//	"G1-5.PNG", "G2-5.PNG", "G3-5.PNG", "G4-5.PNG", "G5-5.PNG",
		//	"B1-5.PNG",
		//};

		//foreach (string image_name in images)
		//{
		//	Debug.Log(string.Format("[Main] identifyInPersonGroupWithTrainingDemo | image_name: {0}", image_name));
		//	await identifyInPersonGroup(client, group_id, image_name);
		//	Debug.Log("==============================");
		//	// Limit TPS (避免請求頻率過高) 3000
		//	Debug.Log(string.Format("Limit TPS"));
		//	await Task.Delay(3000);
		//}

		string image_name = "B345-5.PNG";
		//string image_name = "G12345-5.PNG";
		Debug.Log(string.Format("[Main] identifyInPersonGroupWithTrainingDemo | image_name: {0}", image_name));
		await identifyInPersonGroup(client, group_id, image_name);

	}

	/// <summary>
	/// 建立 PersonGroup，並返回 group_id
	/// </summary>
	/// <param name="client"></param>
	/// <param name="person_dict">將人物名稱(key)與相對應的圖片名稱(value)做映射</param>
	/// <returns></returns>
	public static async Task<string> createPersonGroupWithStreamAsync(IFaceClient client)
	{
		// 當前創建的 PersonGroup 的唯一識別碼
		string group_id = Guid.NewGuid().ToString();
		Debug.Log(string.Format("[Main] createPersonGroupWithStreamAsync | Create a person group ({0})", group_id));

		// 實際創建的 PersonGroup
		// PersonGroup 為裝載 Person 數據的容器(包含臉部圖片及用於識別的特徵)
		await client.PersonGroup.CreateAsync(group_id, group_id, recognitionModel: RecognitionModel.Recognition01);

		// TODO: 寫出 group_id
		return group_id;
	}

	public void appendPersonGroupWithStreamDemo(IFaceClient client, string group_id)
	{
		Dictionary<string, string[]> person_dict = new Dictionary<string, string[]> {
			{ "B1", new[] { "B1-1.PNG", "B1-2.PNG", "B1-3.PNG", "B1-4.PNG" } },
			{ "B2", new[] { "B2-1.PNG", "B2-2.PNG", "B2-3.PNG", "B2-4.PNG" } },
			{ "B3", new[] { "B3-1.PNG", "B3-2.PNG", "B3-3.PNG", "B3-4.PNG" } },
			{ "B4", new[] { "B4-1.PNG", "B4-2.PNG", "B4-3.PNG", "B4-4.PNG" } },
			{ "B5", new[] { "B5-1.PNG", "B5-2.PNG", "B5-3.PNG", "B5-4.PNG" } },
			{ "G1", new[] { "G1-1.PNG", "G1-2.PNG", "G1-3.PNG", "G1-4.PNG" } },
			{ "G2", new[] { "G2-1.PNG", "G2-2.PNG", "G2-3.PNG", "G2-4.PNG" } },
			{ "G3", new[] { "G3-1.PNG", "G3-2.PNG", "G3-3.PNG", "G3-4.PNG" } },
			{ "G4", new[] { "G4-1.PNG", "G4-2.PNG", "G4-3.PNG", "G4-4.PNG" } },
			{ "G5", new[] { "G5-1.PNG", "G5-2.PNG", "G5-3.PNG", "G5-4.PNG" } },
			//{ "MB1", new[] { "MB1-1.PNG", "MB1-2.PNG", "MB1-3.PNG", "MB1-4.PNG" } },
			//{ "MG1", new[] { "MG1-1.PNG", "MG1-2.PNG", "MG1-3.PNG", "MG1-4.PNG" } }
		};

		appendPersonGroupWithStream(client, group_id, person_dict);
	}

	public static void appendPersonGroupWithStream(IFaceClient client, string group_id, Dictionary<string, string[]> person_dict)
	{
		_ = appendPersonGroupWithStreamAsync(client, group_id, person_dict);
	}

	/// <summary>
	/// 事後添加 Person 到 PersonGroup
	/// </summary>
	/// <param name="client"></param>
	/// <param name="group_id">PersonGroup 的唯一對應碼</param>
	/// <param name="person_dict">要添加進 PersonGroup 的 Person 名稱及其圖片對應名稱</param>
	/// <returns></returns>
	public static async Task appendPersonGroupWithStreamAsync(IFaceClient client, string group_id, Dictionary<string, string[]> person_dict)
	{
		// The similar faces will be grouped into a single person group person.
		string[] image_paths;
		Stream image;
		PersistedFace face;

		// TODO: 請求頻率似乎太高了
		foreach (var person_name in person_dict.Keys)
		{
			// 每個 Person 物件都會透過其唯一的識別碼字串(group_id)，與同一個 PersonGroup 產生關聯。
			// 於 PersonGroup 裡面建立 Person 並取得建立好的物件
			Person person = await client.PersonGroupPerson.CreateAsync(personGroupId: group_id, name: person_name);
			Debug.Log(string.Format("[Main] createPersonGroupWithStreamAsync | Create a person group person '{0}'.", person_name));

			// 將同一 Person 的臉部資料，加入 Person 當中
			image_paths = person_dict[person_name];
			foreach (string image_path in image_paths)
			{
				Debug.Log(string.Format("[Main] IdentifyInPersonGroup | Add face to the person group person({0}) " +
					"from image {1}", person_name, image_path));

				// AddFaceFromStreamAsync
				image = File.OpenRead(Path.Combine(Application.streamingAssetsPath, "image", image_path));

				// TODO: 能否中斷這項任務？
				face = await client.PersonGroupPerson.AddFaceFromStreamAsync(
					group_id, person.PersonId, image, image_path);

				// Limit TPS (避免請求頻率過高) 3000
				Debug.Log(string.Format("Limit TPS"));
				await Task.Delay(3000);
			}
		}
	}

	/// <summary>
	/// 根據 group_id 訓練對應的 PersonGroup
	/// </summary>
	/// <param name="client"></param>
	/// <param name="group_id">PersonGroup 的唯一對應碼</param>
	/// <returns></returns>
	public static async Task trainPersonGroupAsync(IFaceClient client, string group_id)
	{
		/* 訓練 PersonGroup
		 * 必須訓練 PersonGroup，以識別與其每一個 Person 物件相關聯的視覺特徵。
		 * 下列程式碼會呼叫非同步訓練方法並輪詢結果。 */
		Debug.Log(string.Format("[Main] trainPersonGroupAsync | Train person group {0}", group_id));
		await client.PersonGroup.TrainAsync(group_id);
		// Limit TPS (避免請求頻率過高) 3000
		Debug.Log(string.Format("Limit TPS"));
		await Task.Delay(3000);

		// Wait until the training is completed.
		TrainingStatus training_status;
		while (true)
		{
			await Task.Delay(1000);

			// 取得訓練狀態
			training_status = await client.PersonGroup.GetTrainingStatusAsync(group_id);
			Debug.Log(string.Format("[Main] trainPersonGroupAsync | Training status: {0}.", training_status.Status));

			if (training_status.Status == TrainingStatusType.Succeeded)
			{
				break;
			}
		}
	}

	/// <summary>
	/// 於 group_id 對應的 PersonGroup 當中，辨識圖片中的人，並將結果印出
	/// </summary>
	/// <param name="client"></param>
	/// <param name="group_id">PersonGroup 的唯一對應碼</param>
	/// <param name="image_name">要辨識的圖片名稱</param>
	/// <returns></returns>
	public async Task identifyInPersonGroup(IFaceClient client, string group_id, string image_name)
	{
		string path = Path.Combine(Application.streamingAssetsPath, "image", image_name);
		Debug.Log(string.Format("[Main] identifyInPersonGroup | path: {0}", path));
		Stream image = File.OpenRead(path);
		List<Tuple<string, double>> identify_results = await identifyPersonWithStreamAsync(client, group_id, image);
		Debug.Log(string.Format("[Main] identifyInPersonGroup | #identify_results: {0}", identify_results.Count));
		string name;
		double confidence;

		foreach (var result in identify_results)
		{
			name = result.Item1;
			confidence = result.Item2;
			Debug.Log(string.Format("[Main] identifyInPersonGroup | Person {0} in {1}, confidence: {2:F4}",
				name, image_name, confidence));
		}

		Debug.Log(string.Format("[Main] identifyInPersonGroup | Ending"));
	}

	public static async Task deletePersonFromGroupAsync(IFaceClient client, string group_id, string person_name)
	{
		// 取得原有 Person 清單
		List<Person> origin_person = await Utils.getPersons(client, group_id);

		foreach (Person person in origin_person)
		{
			if (person.Name.Equals(person_name))
			{
				Debug.Log(string.Format("[Main] deletePersonFromGroupAsync | Delete {0}, id: {1}", person.Name, person.PersonId));
				await client.PersonGroupPerson.DeleteAsync(group_id, person.PersonId);
				break;
			}
		}
	}

	public async Task deletePersonFromGroupAsyncDemo(IFaceClient client, string group_id)
	{
		string[] delete_target = { /*"Family1-Dad", "Family1-Mom", "Family1-Son", "Family1-Daughter", "Family2-Lady",*/ "Family2-Man" };

		foreach(string target in delete_target)
		{
			await deletePersonFromGroupAsync(client, group_id, target);
		}
	}

	// ==================================================
	// ==================================================
	public void trainPersonGroupWithStreamAsyncDemo(IFaceClient client, string group_id) {
		Dictionary<string, string[]> person_dict = new Dictionary<string, string[]> {
			//{ "Family1-Dad", new[] { "Family1-Dad1.jpg", "Family1-Dad2.jpg" } },
			//{ "Family1-Mom", new[] { "Family1-Mom1.jpg", "Family1-Mom2.jpg" } },
			//{ "Family1-Son", new[] { "Family1-Son1.jpg", "Family1-Son2.jpg" } },
			//{ "Family1-Daughter", new[] { "Family1-Daughter1.jpg", "Family1-Daughter2.jpg" } },
			//{ "Family2-Lady", new[] { "Family2-Lady1.jpg", "Family2-Lady2.jpg" } },
			{ "Family2-Man", new[] { "Family2-Man1.jpg", "Family2-Man2.jpg" } }
		};

		_ = trainPersonGroupWithStreamAsync(client, group_id, person_dict);
	}

	public static async Task trainPersonGroupWithStreamAsync(IFaceClient client, string group_id, Dictionary<string, string[]> person_dict)
	{
		// 當前的 PersonGroup 的唯一識別碼
		Debug.Log(string.Format("[Main] addPersonGroupWithStreamAsync | Get a person group ({0})", group_id));

		// 取得原有 Person 清單
		List<Person> origin_person = await Utils.getPersons(client, group_id);
		bool in_person_list = false;

		// The similar faces will be grouped into a single person group person.
		string[] image_paths;
		Stream image;
		PersistedFace face;

		foreach (var person_name in person_dict.Keys)
		{
			// 每個 Person 物件都會透過其唯一的識別碼字串(group_id)，與同一個 PersonGroup 產生關聯。
			Person person = new Person();
			foreach (Person p in origin_person)
			{
				if (p.Name.Equals(person_name))
				{
					person = p;
					in_person_list = true;
					break;
				}
			}

			// Limit TPS (避免請求頻率過高)
			await Task.Delay(250);

			// 若原本就存在 PersonGroup 當中
			if (in_person_list)
			{
				// 根據 PersonId 取得 Person 物件
				person = await client.PersonGroupPerson.GetAsync(personGroupId: group_id, personId: person.PersonId);
				Debug.Log(string.Format("[Main] createPersonGroupWithStreamAsync | Get a person in group person '{0}'.", person_name));
			}
			// 原本不存在 PersonGroup 當中
			else
			{
				// 於 PersonGroup 裡面建立 Person 並取得建立好的物件
				person = await client.PersonGroupPerson.CreateAsync(personGroupId: group_id, name: person_name);
				Debug.Log(string.Format("[Main] createPersonGroupWithStreamAsync | Create a person into group person '{0}'.", person_name));
			}

			// 將同一 Person 的臉部資料，加入 Person 當中
			image_paths = person_dict[person_name];
			foreach (string image_path in image_paths)
			{
				Debug.Log(string.Format("[Main] IdentifyInPersonGroup | Add face to the person group person({0}) " +
					"from image {1}", person_name, image_path));

				// AddFaceFromStreamAsync
				image = File.OpenRead(Path.Combine(Application.streamingAssetsPath, "image", image_path));
				face = await client.PersonGroupPerson.AddFaceFromStreamAsync(
					group_id, person.PersonId, image, image_path);
			}

			in_person_list = false;
		}

		await trainPersonGroupAsync(client, group_id);
	}

	void identifyInPersonGroupWithTrainingDemo(IFaceClient client)
	{
		Dictionary<string, string[]> person_dict = new Dictionary<string, string[]> {
			{ "Family1-Dad", new[] { "Family1-Dad1.jpg", "Family1-Dad2.jpg" } },
			{ "Family1-Mom", new[] { "Family1-Mom1.jpg", "Family1-Mom2.jpg" } },
			{ "Family1-Son", new[] { "Family1-Son1.jpg", "Family1-Son2.jpg" } },
			{ "Family1-Daughter", new[] { "Family1-Daughter1.jpg", "Family1-Daughter2.jpg" } },
			{ "Family2-Lady", new[] { "Family2-Lady1.jpg", "Family2-Lady2.jpg" } },
			{ "Family2-Man", new[] { "Family2-Man1.jpg", "Family2-Man2.jpg" } }
		};

		string image_name = "identification1.jpg";

		_ = identifyInPersonGroupWithTraining(client, person_dict, image_name);
	}

	public async Task identifyInPersonGroupWithTraining(IFaceClient client, Dictionary<string, string[]> person_dict, string image_name)
	{
		string group_id = await createPersonGroupWithStreamAsync(client);
		Debug.Log(string.Format("[Main] identifyInPersonGroupWithTraining | group_id: {0}", group_id));

		await appendPersonGroupWithStreamAsync(client, group_id, person_dict);

		await trainPersonGroupAsync(client, group_id);

		string path = Path.Combine(Application.streamingAssetsPath, "image", image_name);
		Debug.Log(string.Format("[Main] identifyInPersonGroupWithTraining | path: {0}", path));
		Stream image = File.OpenRead(path);
		List<Tuple<string, double>> identify_results = await identifyPersonWithStreamAsync(client, group_id, image);
		Debug.Log(string.Format("[Main] identifyInPersonGroupWithTraining | #identify_results: {0}", identify_results.Count));
		string name;
		double confidence;

		foreach (var result in identify_results)
		{
			name = result.Item1;
			confidence = result.Item2;
			Debug.Log(string.Format("[Main] identifyInPersonGroupWithTraining | Person {0} in {1}, confidence: {2:F4}",
				name, image_name, confidence));
		}

		Debug.Log(string.Format("[Main] identifyInPersonGroupWithTraining | Ending"));
	}

	/// <summary>
	/// 偵測臉部位置
	/// </summary>
	/// <param name="client"></param>
	/// <param name="image">以 Stream 型態將圖片讀入</param>
	/// <returns></returns>
	public static async Task<List<DetectedFace>> detectWithStreamAsync(IFaceClient client, Stream image)
	{
		IList<DetectedFace> detected_faces = 
			await client.Face.DetectWithStreamAsync(image, recognitionModel: RecognitionModel.Recognition01);
		// Limit TPS (避免請求頻率過高) 3000
		Debug.Log(string.Format("Limit TPS"));
		await Task.Delay(3000);

		return detected_faces.ToList();
	}

	/// <summary>
	/// 辨識圖片中，事先註冊在 PersonGroup 中的人臉
	/// </summary>
	/// <param name="client"></param>
	/// <param name="group_id">PersonGroup 的唯一對應碼</param>
	/// <param name="image">以 Stream 型態將圖片讀入</param>
	/// <returns></returns>
	public static async Task<List<Tuple<string, double>>> identifyPersonWithStreamAsync(IFaceClient client, string group_id, Stream image)
	{
		List<DetectedFace> detected_faces = await detectWithStreamAsync(client, image);
		Debug.Log(string.Format("[Main] identifyPersonWithStreamAsync | {0} face detected.", detected_faces.Count));

		// 將偵測到的臉部的 ID 加入 face_ids 陣列當中
		List<Guid> face_ids = new List<Guid>();
		foreach (DetectedFace face in detected_faces)
		{
			face_ids.Add(face.FaceId.Value);
		}

		// 輸入臉部 ID 列表(face_ids)，IdentifyAsync 當中會藉由此 ID 去取用臉部資訊，
		// 進而和 PersonGroup 當中的人臉做比對
		IList<IdentifyResult> results = await client.Face.IdentifyAsync(face_ids, group_id);
		// Limit TPS (避免請求頻率過高) 3000
		Debug.Log(string.Format("Limit TPS"));
		await Task.Delay(3000);

		List<Tuple<string, double>> identify_result = new List<Tuple<string, double>>();
		foreach (IdentifyResult result in results)
		{
			// result.Candidates 為相似度高的候選人，取出相似可能候選第一位的 ID
			Person person = await client.PersonGroupPerson.GetAsync(group_id, result.Candidates[0].PersonId);
			identify_result.Add(Tuple.Create(person.Name, result.Candidates[0].Confidence));

			// Limit TPS (避免請求頻率過高) 3000
			Debug.Log(string.Format("Limit TPS"));
			await Task.Delay(3000);
		}

		return identify_result;
	}

	public async Task isPersonGroupExistAsync(IFaceClient client, string group_id)
	{
		bool is_group_exist = await Utils.isPersonGroupExist(client, group_id);

		Debug.Log(string.Format("[Main] testPersonGroup {0} {1} exist.",
			group_id, is_group_exist ? "is" : "is not"));
	} 
	#endregion
	#endregion


}
