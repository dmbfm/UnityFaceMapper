using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ImageVideoContactPicker;

public class Example : MonoBehaviour {

	string log = "";

	void OnEnable()
	{
		PickerEventListener.onImageSelect += OnImageSelect;
		PickerEventListener.onImageLoad += OnImageLoad;
		PickerEventListener.onVideoSelect += OnVideoSelect;
		PickerEventListener.onContactSelect += OnContactSelect;
		PickerEventListener.onError += OnError;
		PickerEventListener.onCancel += OnCancel;
	}
	
	void OnDisable()
	{
		PickerEventListener.onImageSelect -= OnImageSelect;
		PickerEventListener.onImageLoad -= OnImageLoad;
		PickerEventListener.onVideoSelect -= OnVideoSelect;
		PickerEventListener.onContactSelect -= OnContactSelect;
		PickerEventListener.onError -= OnError;
		PickerEventListener.onCancel -= OnCancel;
	}

	
	void OnImageSelect(string imgPath)
	{
		Debug.Log ("Image Location : "+imgPath);
		log += "\nImage Path : " + imgPath;
	}

	void OnImageLoad(string imgPath, Texture2D tex)
	{
		Debug.Log ("Image Location : "+imgPath);
		GameObject.Find("Cube").GetComponent<Renderer>().material.mainTexture = tex;
	}
	void OnVideoSelect(string vidPath)
	{
		Debug.Log ("Video Location : "+vidPath);
		log += "\nVideo Path : " + vidPath;
	}
	void OnContactSelect(string name, List<string> numbers, List<string> emails)
	{
		Debug.Log("Name : "+name);
		log += "\nName : "+name;
		for(int i=0;i<numbers.Count;i++){
			log += "\nContact "+(1+i)+" : " + numbers[i];
			Debug.Log(numbers[i]);
		}
		for(int i=0;i<emails.Count;i++){
			log += "\nEmail "+(1+i)+" : " + emails[i];
			Debug.Log(emails[i]);
		}
	}

	void OnError(string errorMsg)
	{
		Debug.Log ("Error : "+errorMsg);
	}

	void OnCancel()
	{
		Debug.Log ("Cancel by user");
	}

	void OnGUI()
	{
		GUILayout.Label (log);

		if(GUI.Button(new Rect(Screen.width/2 - 75,Screen.height/2 - 100,150,35),"Browse Image"))
		 {
			#if UNITY_ANDROID
			AndroidPicker.BrowseImage();
			#elif UNITY_IPHONE
			IOSPicker.BrowseImage();
			#endif
		}
		if(GUI.Button(new Rect(Screen.width/2 - 75,Screen.height/2 - 20,150,35),"Browse Video"))
		{
			#if UNITY_ANDROID
			AndroidPicker.BrowseVideo();
			#elif UNITY_IPHONE
			IOSPicker.BrowseVideo();
			#endif
		}
		if(GUI.Button(new Rect(Screen.width/2 - 75,Screen.height/2 + 60 ,150,35),"Browse Contact"))
		{
			#if UNITY_ANDROID
			AndroidPicker.BrowseContact();
			#elif UNITY_IPHONE
			IOSPicker.BrowseContact();
			#endif
		}
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

}
