using System.Collections;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {

	//#if UNITY_EDITOR
	//[Help("To enable KEYBOARD Control for button navigation, activate 'Send Navigation Events' on the game object 'EventSystem'.", UnityEditor.MessageType.Info)]
	//#endif
	[Header("Panels")]
	[Tooltip("The UI Panel holding the Home Screen elements")]
	public GameObject homeScreen;
	[Tooltip("The UI Panel holding the Loading Screen elements")]
	public GameObject loadScreen;
	[Tooltip("The UI Panel holding the Game Settings Screen elements")]
	public GameObject settingsScreen;
	[Tooltip("The UI Panel holding New and Load Buttons")]
	public GameObject newLoadScreen;
	[Tooltip("The UI Pop Up confirming quit action ('Are You Sure?')")]
	public GameObject homeQuestionScreen;
	[Tooltip("The UI Pop Up confirming New Game action ('Are You Sure?')")]
	public GameObject newGameQuestionScreen;
	[Tooltip("The UI Pop Up confirming Load Game action ('Load save data')")]
	public GameObject loadGameQuestionScreen;

	[Header("Panel Animators")]
	[Tooltip("The Animator Component attached to the Home Panel")]
	public Animator homeScreenAnim;
	[Tooltip("The Animator Component attached to the Loading Panel")]
	public Animator loadScreenAnim;
	[Tooltip("The Animator Component attached to the Settings Panel")]
	public Animator settingsScreenAnim;
	[Tooltip("The Animator Component attached to the New Load Panel")]
	public Animator newLoadScreenAnim;

	[Header("UI Elements & User Data")]
	[Tooltip("The dropdown menu containing all the resolutions that your game can adapt to")]
	public TMP_Dropdown ResolutionDropDown;
	private Resolution[] resolutions;
	[Tooltip("The text object in the Settings Panel displaying the current quality setting enabled")]
	public TMP_Text qualityText; // text displaying current selected quality
	[Tooltip("The icon showing the current quality selected in the Settings Panels")]
	public Animator qualityDisplay;
	private string[] qualityNames;
	private int tempQualityLevel;// store it for start up text update
	[Tooltip("The volume slider UI element in the Settings Screen")]
	public Slider audioSlider;

	[Header("Loading Screen Elements")]
	[Tooltip("The name of the scene loaded when a 'NEW GAME' is started")]
	public string newSceneName;
	[Tooltip("The loading bar Slider UI element in the Loading Screen")]
	public Slider loadingBar;
	private string loadSceneName; // scene name is defined when the load game data is retrieved

	[Header("Debug")]
	[Tooltip("If this is true, pressing 'R' will reload the scene.")]
	public bool reloadSceneButton = true;
	
	// Just for reloading the scene! You can delete this function entirely if you want to
	void Update(){
		if(reloadSceneButton){
			if(Input.GetKeyDown(KeyCode.R)){
				SceneManager.LoadScene("Menu");
			}
		}
	}

	void Start(){
		// By default, starts on the home screen, disables others
		homeScreen.SetActive(true);
		loadScreen.SetActive(false);
		settingsScreen.SetActive(false);
		newLoadScreen.SetActive(false);
		homeQuestionScreen.SetActive(false);
		newGameQuestionScreen.SetActive(false);
		loadGameQuestionScreen.SetActive(false);

		// Get quality settings names
		qualityNames = QualitySettings.names;

		// Get screens possible resolutions
		resolutions = Screen.resolutions;

		// Set Drop Down resolution options according to possible screen resolutions of your monitor
		for (int i = 0; i < resolutions.Length; i++)
         {
             ResolutionDropDown.options.Add (new TMP_Dropdown.OptionData (ResToString (resolutions [i])));
 
             ResolutionDropDown.value = i;
 
             ResolutionDropDown.onValueChanged.AddListener(delegate { Screen.SetResolution(resolutions
			 [ResolutionDropDown.value].width, resolutions[ResolutionDropDown.value].height, true);});
         
         }
		 
		 // Check if first time so the volume can be set to MAX
		 if(PlayerPrefs.GetInt("firsttime")==0){
			 // it's the player's first time. Set to false now...
			 PlayerPrefs.SetInt("firsttime",1);
			 PlayerPrefs.SetFloat("volume",1);
		 }

		 // Check volume that was saved from last play
		 audioSlider.value = PlayerPrefs.GetFloat("volume");

	}

	// Make sure all the settings panel text are displaying current quality settings properly and updating UI
	public void CheckSettings(){
		tempQualityLevel = QualitySettings.GetQualityLevel(); 
		if(tempQualityLevel == 0){
			qualityText.text = qualityNames[0];
			qualityDisplay.SetTrigger("Low");
		}else if(tempQualityLevel == 1){
			qualityText.text = qualityNames[1];
			qualityDisplay.SetTrigger("Medium");
		}else if(tempQualityLevel == 2){
			qualityText.text = qualityNames[2];
			qualityDisplay.SetTrigger("High");
		}else if(tempQualityLevel == 3){
			qualityText.text = qualityNames[3];
			qualityDisplay.SetTrigger("Ultra");
		}
	}

	// Converts the resolution into a string form that is then used in the dropdown list as the options
	string ResToString(Resolution res)
	{
		return res.width + " x " + res.height;
	}

	// Whenever a value on the audio slider in the settings panel is changed, this 
	// function is called and updates the overall game volume
	public void AudioSlider(){
		AudioListener.volume = audioSlider.value;
		PlayerPrefs.SetFloat("volume",audioSlider.value);
	}

	// Called when starting a new game or load game so the animation is smooth
	public void NewGameDisabled(){
		StartCoroutine(NewOut());
		newLoadScreenAnim.SetBool("Animate",false); // disable home screen animation
	}

	// The timer before the homescreen panel disables
	IEnumerator NewOut(){
		yield return new WaitForSeconds(1);
		newLoadScreen.gameObject.SetActive(false);
	}

	// When accepting the QUIT question, the application will close 
	// (Only works in Executable. Disabled in Editor)
	public void Quit(){
		Application.Quit();
	} 

	// Changes the current quality settings by taking the number passed in from the UI 
	// element and matching it to the index of the Quality Settings
	public void QualityChange(int x){
		if(x == 0){
			QualitySettings.SetQualityLevel(x, true);	
			qualityText.text = qualityNames[0];
		}else if(x == 1){
			QualitySettings.SetQualityLevel(x, true);
			qualityText.text = qualityNames[1];
		}else if(x == 2){
			QualitySettings.SetQualityLevel(x, true);
			qualityText.text = qualityNames[2];
		}if(x == 3){
			QualitySettings.SetQualityLevel(x, true);
			qualityText.text = qualityNames[3];
		}
	}

	// Called when loading new game scene
	public void LoadNewLevel (){
		if(newSceneName != ""){
			StartCoroutine(LoadAsynchronously(newSceneName));
		}
	}

	// Called when loading saved scene
	public void LoadSavedLevel (){
		if(loadSceneName != ""){
			StartCoroutine(LoadAsynchronously(loadSceneName));
		}
	}

	// Load Bar synching animation
	IEnumerator LoadAsynchronously (string sceneName){ // scene name is just the name of the current scene being loaded
		AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

		while (!operation.isDone){
			float progress = Mathf.Clamp01(operation.progress / .95f);

			loadingBar.value = progress;

			yield return null;
		}
	}
}
