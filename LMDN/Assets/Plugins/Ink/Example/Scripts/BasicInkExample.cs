using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;

// This is a super bare bones example of how to play and display a ink story in Unity.
public class BasicInkExample : MonoBehaviour
{

	public GameObject chatPanel, textObject, msgObject;

    public Text header, headerStatus, amyChatStatus, kaylaChatStatus;

    public Button kaylaB, amyB;

	public int currState = 1;

    public List<Message> currentList;

    public bool k1 = false;
    public bool k2 = false;
    public bool a1 = false;

    public TextAsset inkJSONAsset1;
    public TextAsset inkJSONAsset2;
    public TextAsset currAsset;

    bool inBetween;

    [SerializeField]
    List<Message> msgListKayla = new List<Message>();
    List<Message> msgListAmy = new List<Message>();



	void Awake () {
		// Remove the default message
		RemoveChildren();

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
			string text = story.Continue ();
			// This removes any white space from the text.
			text = text.Trim();
			// Display the text on screen!
			//CreateContentView(text);

            receiveMessage(text);
		}

		// Display all the choices, if there are any!
		if(story.currentChoices.Count > 0) {
			for (int i = 0; i < story.currentChoices.Count; i++) {
				Choice choice = story.currentChoices [i];
				Button button = CreateChoiceView (choice.text.Trim ());
				// Tell the button what to do when we press it
				button.onClick.AddListener (delegate {
					OnClickChoiceButton (choice);
                    receiveMessage("You: " + button.GetComponentInChildren<Text>().text);
				});
			}
		}
		// If we've read all the content and there's no choices, the story is finished!
		else {
            if(currAsset == inkJSONAsset)
            {
                k1 = true;
                amyChatStatus.text = "Active";
            }
            else if(currAsset == inkJSONAsset1)
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
		RefreshView();
	}


    void OnClickChangeChat(string person)
    {
        if(person.Equals("Kayla"))
        {
            currentList = msgListKayla;
            if (inBetween && !k2)
            {
                inBetween = false;
                currAsset = inkJSONAsset1;
                StartStory();
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
                GameObject newMsgO = Instantiate(msgObject, chatPanel.transform);

                newMsgO.GetComponentInChildren<Text>().text = currentList[i].text;

                //newMsg.textObject = newTxt.GetComponent<Text>();

                //newMsg.textObject.text = newMsg.text;

                currentList[i].mo = newMsgO;
            }

            if(currAsset == inkJSONAsset2)
            {
                RemoveChildren();
            }
            else
            {
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
                                receiveMessage("You: " + button.GetComponentInChildren<Text>().text);
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
                GameObject newMsgO = Instantiate(msgObject, chatPanel.transform);

                newMsgO.GetComponentInChildren<Text>().text = currentList[i].text;

                //newMsg.textObject = newTxt.GetComponent<Text>();

                //newMsg.textObject.text = newMsg.text;

                currentList[i].mo = newMsgO;
            }

            if (currAsset == inkJSONAsset || currAsset == inkJSONAsset1)
            {
                RemoveChildren();
            }
            else
            {
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
                                receiveMessage("You: " + button.GetComponentInChildren<Text>().text);
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

    public void receiveMessage(string text)
    {
        Message newMsg = new Message();
	
        newMsg.text = text;

        //GameObject newTxt = Instantiate(textObject, chatPanel.transform);

	    GameObject newMsgO = Instantiate(msgObject, chatPanel.transform);

	    newMsgO.GetComponentInChildren<Text>().text = text;

        //newMsg.textObject = newTxt.GetComponent<Text>();

        //newMsg.textObject.text = newMsg.text;

	    newMsg.mo = newMsgO;
	    
	    currentList.Add(newMsg);
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
}