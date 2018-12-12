using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Ink.Runtime;
using UnityEngine.Networking;

// This is a super bare bones example of how to play and display a ink story in Unity.
public class BasicInkExample : MonoBehaviour
{

    public GameObject chatPanel, msgObject, chatMsgObject;

    public Text header, headerStatus, amyChatStatus, kaylaChatStatus;

    public Sprite pikachu, eevee, togepi;

    public Button kaylaB, amyB;

    public int currState = 1;

    public List<Message> currentList;

    public bool k1 = false;
    public bool k2 = false;
    public bool a1 = false;
    public bool trans = false;

    public TextAsset inkJSONAsset1;
    public TextAsset inkJSONAsset2;
    public TextAsset currAsset;

    public AudioSource musicPlayer;
    public AudioSource speechPlayer;

    public AudioClip secrets, howl, moon;

    public Button play, nextT, prevT;

    bool inBetween;
    bool paused = false;

    public AudioSource ping;
    public AudioSource typing;

    public Text time;

    [SerializeField]
    List<Message> msgListKayla = new List<Message>();
    List<Message> msgListAmy = new List<Message>();

    private void Update()
    {
        time.text = System.DateTime.Now.ToString();
        if(!paused && !musicPlayer.isPlaying)
        {
            nextTrack();
        }
    }

    public IEnumerator TTS(string text)
    {
        Debug.Log("http://api.voicerss.org/?key=e0556d631477478aa4f1d2f6f77fc62c&hl=en-us&src=" + text);
        using(UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("http://api.voicerss.org/?key=e0556d631477478aa4f1d2f6f77fc62c&c=OGG&hl=en-us&src=" + text, AudioType.OGGVORBIS))
        {
            yield return www.SendWebRequest();

            if(www.isHttpError || www.isNetworkError)
            {
                Debug.Log(www.error);
            }
            else
            {
                speechPlayer.Stop();
                //Debug.Log("succeeded");
                AudioClip speech = DownloadHandlerAudioClip.GetContent(www);
                speechPlayer.clip = speech;
                speechPlayer.Play();
            }
        }
    }

    void Awake () {
		// Remove the default message
		RemoveChildren();

        play.onClick.AddListener(delegate
        {
            pausePlay();
        });
        nextT.onClick.AddListener(delegate
        {
            nextTrack();
        });
        prevT.onClick.AddListener(delegate
        {
            prevTrack();
        });

        kaylaB.onClick.AddListener(delegate {
            OnClickChangeChat("Kayla");
        });

        amyB.onClick.AddListener(delegate {
            OnClickChangeChat("Amy");
        });

        currAsset = inkJSONAsset;
        inBetween = false;

        currentList = msgListKayla;

        StartStory();
	}


    // Creates a new Story object with the compiled story which we can then play!
	void StartStory () {
		story = new Story (currAsset.text);
        RefreshView();
	}
	
	// This is the main function called every time the story changes. It does a few things:
	// Destroys all the old content and choices.
	// Continues over all the lines of text, then displays all the choices. If there are no choices, the story is finished!
	void RefreshView () {
		// Remove all the UI on screen
		RemoveChildren ();
		
		// Read all the content until we can't continue any more
		while (story.canContinue) {
            // Continue gets the next line of the story
            string text = story.Continue();
            // This removes any white space from the text.
            text = text.Trim();
            // Display the text on screen!
            //CreateContentView(text);
            StartCoroutine("transition", text);
		}

        StartCoroutine("transitionChoice");
        //logicChoiceCreation();
		//// Display all the choices, if there are any!
		//if(story.currentChoices.Count > 0) {
		//	for (int i = 0; i < story.currentChoices.Count; i++) {
		//		Choice choice = story.currentChoices [i];
		//		Button button = CreateChoiceView (choice.text.Trim ());
		//		// Tell the button what to do when we press it
		//		button.onClick.AddListener (delegate {
		//			OnClickChoiceButton (choice);
  //                  //receiveMessage("You: " + button.GetComponentInChildren<Text>().text);
		//		});
		//	}
		//}
		//// If we've read all the content and there's no choices, the story is finished!
		//else {
  //          if(currAsset == inkJSONAsset)
  //          {
  //              k1 = true;
  //              amyChatStatus.text = "Active";
  //          }
  //          else if(currAsset == inkJSONAsset1)
  //          {
  //              k2 = true;
  //              kaylaChatStatus.text = "Offline";
  //          }
  //          else
  //          {
  //              amyChatStatus.text = "Offline";
  //              a1 = true;
  //          }
  //          inBetween = true;
		//}
	}


    void logicChoiceCreation()
    {
        // Display all the choices, if there are any!
        if (story.currentChoices.Count > 0)
        {
            for (int i = 0; i < story.currentChoices.Count; i++)
            {
                Choice choice = story.currentChoices[i];
                Button button = CreateChoiceView(choice.text.Trim());
                // Tell the button what to do when we press it
                button.onClick.AddListener(delegate {
                    OnClickChoiceButton(choice);
                    //receiveMessage("You: " + button.GetComponentInChildren<Text>().text);
                });
            }
        }
        
        // If we've read all the content and there's no choices, the story is finished!
        else
        {
            if (currAsset == inkJSONAsset)
            {
                k1 = true;
                amyChatStatus.text = "Active";
            }
            else if (currAsset == inkJSONAsset1)
            {
                k2 = true;
                kaylaChatStatus.text = "Offline";
            }
            else
            {
                amyChatStatus.text = "Offline";
                a1 = true;
            }
            inBetween = true;
        }
    }

	// When we click the choice button, tell the story to choose that choice!
	void OnClickChoiceButton (Choice choice) {
		story.ChooseChoiceIndex (choice.index);
        receiveMessage(choice.text.Trim(), 0);
        RefreshView();
	}


    void OnClickChangeChat(string person)
    {
        bool startedStory = false;
        if(person.Equals("Kayla"))
        {
            currentList = msgListKayla;
            if (inBetween && !k2)
            {
                inBetween = false;
                currAsset = inkJSONAsset1;
                StartStory();
                startedStory = true;
            }
            header.text = person;
            if(k2)
            {
                headerStatus.text = "Offline";
                //RemoveChildren();
            }
            else
            {
                headerStatus.text = "Active";
            }

            destroyCurrentChat();

            for(int i = 0; i < currentList.Count; i++)
            {

                GameObject newMsgO = Instantiate(chatMsgObject, chatPanel.transform);

                newMsgO.GetComponentInChildren<TextMeshProUGUI>().SetText(currentList[i].text);

                if (currentList[i].k == 0)
                {
                    newMsgO.GetComponent<HorizontalLayoutGroup>().childAlignment = TextAnchor.LowerRight;
                    Destroy(newMsgO.transform.GetChild(0).gameObject);
                    newMsgO.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().sprite = eevee;
                    newMsgO.transform.GetChild(1).GetComponent<Image>().color = new Color32(188, 255, 243, 200);
                }
                else if (currentList[i].k == 1)
                {
                    newMsgO.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().sprite = togepi;
                    newMsgO.transform.GetChild(1).GetComponent<Image>().color = new Color32(230, 255, 150, 200);
                }
                else
                {
                    newMsgO.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().sprite = pikachu;
                    newMsgO.transform.GetChild(1).GetComponent<Image>().color = new Color32(230, 255, 150, 200);
                }

                newMsgO.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate
                {
                    StartCoroutine("TTS", currentList[i].text);
                });

                currentList[i].mo = newMsgO;
            }

            if(currAsset == inkJSONAsset2)
            {
                RemoveChildren();
            }
            else
            {
                if(startedStory)
                {
                    return;
                }
                if(canvas.transform.childCount == 0) {
                    if (story.currentChoices.Count > 0)
                    {
                        for (int i = 0; i < story.currentChoices.Count; i++)
                        {
                            Choice choice = story.currentChoices[i];
                            Button button = CreateChoiceView(choice.text.Trim());
                            // Tell the button what to do when we press it
                            button.onClick.AddListener(delegate {
                                OnClickChoiceButton(choice);
                                //receiveMessage("You: " + button.GetComponentInChildren<Text>().text);
                            });
                        }
                    }
                }
            }

        }
        if (person.Equals("Amy"))
        {
            currentList = msgListAmy;
            if (inBetween && !a1)
            {
                inBetween = false;
                currAsset = inkJSONAsset2;
                StartStory();
                startedStory = true;
            }
            header.text = person;
            if (!k1 || a1)
            {
                headerStatus.text = "Offline";
                //RemoveChildren();
            }
            else
            {
                headerStatus.text = "Active";
            }

            destroyCurrentChat();

            for (int i = 0; i < currentList.Count; i++)
            {
                GameObject newMsgO = Instantiate(chatMsgObject, chatPanel.transform);

                newMsgO.GetComponentInChildren<TextMeshProUGUI>().SetText(currentList[i].text);

                if (currentList[i].k == 0)
                {
                    newMsgO.GetComponent<HorizontalLayoutGroup>().childAlignment = TextAnchor.LowerRight;
                    Destroy(newMsgO.transform.GetChild(0).gameObject);
                    newMsgO.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().sprite = eevee;
                    newMsgO.transform.GetChild(1).GetComponent<Image>().color = new Color32(188, 255, 243, 200);
                }
                else if (currentList[i].k == 1)
                {
                    newMsgO.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().sprite = togepi;
                    newMsgO.transform.GetChild(1).GetComponent<Image>().color = new Color32(230, 255, 150, 200);
                }
                else
                {
                    newMsgO.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().sprite = pikachu;
                    newMsgO.transform.GetChild(1).GetComponent<Image>().color = new Color32(230, 255, 150, 200);
                }

                newMsgO.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate
                {
                    StartCoroutine("TTS", currentList[i].text);
                });

                currentList[i].mo = newMsgO;
            }

            if (currAsset == inkJSONAsset || currAsset == inkJSONAsset1)
            {
                RemoveChildren();
            }
            else
            {
                if(startedStory)
                {
                    return;
                }
                if (canvas.transform.childCount == 0) {
                    if (story.currentChoices.Count > 0)
                    {
                        for (int i = 0; i < story.currentChoices.Count; i++)
                        {
                            Choice choice = story.currentChoices[i];
                            Button button = CreateChoiceView(choice.text.Trim());
                            // Tell the button what to do when we press it
                            button.onClick.AddListener(delegate {
                                OnClickChoiceButton(choice);
                                //receiveMessage("You: " + button.GetComponentInChildren<Text>().text);
                            });
                        }
                    }
                }
            }
        }
    }

	// Creates a button showing the choice text
	void CreateContentView (string text) {
		Text storyText = Instantiate (textPrefab) as Text;
		storyText.text = text;
		storyText.transform.SetParent (canvas.transform, false);
	}

	// Creates a button showing the choice text
	Button CreateChoiceView (string text) {
		// Creates the button from a prefab
		Button choice = Instantiate (buttonPrefab) as Button;
		choice.transform.SetParent (canvas.transform, false);
		
		// Gets the text from the button prefab
		Text choiceText = choice.GetComponentInChildren<Text> ();
		choiceText.text = text;

		// Make the button expand to fit the text
		HorizontalLayoutGroup layoutGroup = choice.GetComponent <HorizontalLayoutGroup> ();
		layoutGroup.childForceExpandHeight = false;

		return choice;
	}

	// Destroys all the children of this gameobject (all the UI)
	void RemoveChildren () {
		int childCount = canvas.transform.childCount;
		for (int i = childCount - 1; i >= 0; --i) {
			GameObject.Destroy (canvas.transform.GetChild (i).gameObject);
		}
	}

    void destroyCurrentChat()
    {
        int childCount = chatPanel.transform.childCount;
        for (int i = childCount - 1; i >= 0; --i)
        {
            GameObject.Destroy(chatPanel.transform.GetChild(i).gameObject);
        }
    }

    public void receiveMessage(string text, int c)
    {

        Message newMsg = new Message();

        newMsg.text = text;


        //GameObject newMsgO = Instantiate(msgObject, chatPanel.transform);

        GameObject newMsgO = Instantiate(chatMsgObject, chatPanel.transform);

        newMsgO.GetComponentInChildren<TextMeshProUGUI>().SetText(text);

        if (c == 0)
        {
            newMsgO.GetComponent<HorizontalLayoutGroup>().childAlignment = TextAnchor.LowerRight;
            Destroy(newMsgO.transform.GetChild(0).gameObject);
            newMsgO.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().sprite = eevee;
            newMsgO.transform.GetChild(1).GetComponent<Image>().color = new Color32(188, 255, 243, 200);
        }
        else if (c == 1)
        {
            newMsgO.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().sprite = togepi;
            newMsgO.transform.GetChild(1).GetComponent<Image>().color = new Color32(230, 255, 150, 200);
        }
        else
        {
            newMsgO.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().sprite = pikachu;
            newMsgO.transform.GetChild(1).GetComponent<Image>().color = new Color32(230, 255, 150, 200);
        }

        newMsgO.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate
        {
            StartCoroutine("TTS", text);
        });

        newMsg.mo = newMsgO;
        newMsg.k = c;

        currentList.Add(newMsg);

    }

    IEnumerator transition(string text)
    {

        if(text.Length == 0)
        {
            yield return null;
        }

        if (text.Contains("You: "))
        {
            receiveMessage(text.Substring(4), 0);
            yield break;
        }

        while (trans)
        {
            yield return new WaitForSeconds(0.1f);
        }

        trans = true;

        typing.Play();

        float timer = Random.Range(0.75f, 3.0f);

        Message transitionMsg = new Message();

        transitionMsg.text = "...";

        GameObject newMsg0 = Instantiate(chatMsgObject, chatPanel.transform);

        newMsg0.GetComponentInChildren<TextMeshProUGUI>().SetText("...");

        if (currAsset != inkJSONAsset2)
        {
            newMsg0.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().sprite = togepi;
        }
        else
        {
            newMsg0.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().sprite = pikachu;
        }


        transitionMsg.mo = newMsg0;

        currentList.Add(transitionMsg);

        while(timer > 0)
        {
            yield return new WaitForSeconds(0.1f);
            timer -= 0.1f;
        }

        currentList.Remove(transitionMsg);

        Destroy(newMsg0);

        trans = false;

        typing.Stop();

        //receiveMessage(text);


        if (currAsset != inkJSONAsset2)
        {
            receiveMessage(text, 1);
        }
        else
        {
            receiveMessage(text, 2);
        }


        ping.Play();

        yield return null;
    }

    IEnumerator transitionChoice()
    {
        while(trans)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Display all the choices, if there are any!
        if (story.currentChoices.Count > 0)
        {
            for (int i = 0; i < story.currentChoices.Count; i++)
            {
                Choice choice = story.currentChoices[i];
                Button button = CreateChoiceView(choice.text.Trim());
                // Tell the button what to do when we press it
                button.onClick.AddListener(delegate {
                    OnClickChoiceButton(choice);
                    //receiveMessage("You: " + button.GetComponentInChildren<Text>().text);
                });
            }
        }
        // If we've read all the content and there's no choices, the story is finished!
        else
        {
            if (currAsset == inkJSONAsset)
            {
                k1 = true;
                amyChatStatus.text = "Active";
                GameObject.Find("Amy").transform.SetSiblingIndex(2);
            }
            else if (currAsset == inkJSONAsset1)
            {
                k2 = true;
                kaylaChatStatus.text = "Offline";
                GameObject.Find("Kayla").transform.SetSiblingIndex(GameObject.Find("Offline").transform.GetSiblingIndex() + 1);
            }
            else
            {
                amyChatStatus.text = "Offline";
                a1 = true;
                GameObject.Find("Amy").transform.SetSiblingIndex(GameObject.Find("Offline").transform.GetSiblingIndex() + 1);
            }
            inBetween = true;
        }

        yield return null;

    }

    public void nextTrack()
    {
        musicPlayer.Stop();
        if(musicPlayer.clip == secrets)
        {
            musicPlayer.clip = howl;
        }
        else if(musicPlayer.clip == howl)
        {
            musicPlayer.clip = moon;
        }
        else
        {
            musicPlayer.clip = secrets;
        }
        musicPlayer.Play();
    }

    public void pausePlay()
    {
        if(musicPlayer.isPlaying)
        {
            musicPlayer.Pause();
            paused = true;
        }
        else
        {
            paused = false;
            musicPlayer.Play();
        }
    }

    public void prevTrack()
    {
        musicPlayer.Stop();
        if (musicPlayer.clip == secrets)
        {
            musicPlayer.clip = moon;
        }
        else if (musicPlayer.clip == howl)
        {
            musicPlayer.clip = secrets;
        }
        else
        {
            musicPlayer.clip = howl;
        }
        musicPlayer.Play();
    }


	[SerializeField]
	private TextAsset inkJSONAsset;
    private Story story;

	[SerializeField]
	private Canvas canvas;

	// UI Prefabs
	[SerializeField]
	private Text textPrefab;
	[SerializeField]
	private Button buttonPrefab;
}

[System.Serializable]
public class Message
{
    public string text;
    //public Text textObject;
	public GameObject mo;
    public int k;
}