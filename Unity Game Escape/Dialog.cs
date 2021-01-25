using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Dialogue : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sprite_DialogueBox;
    [SerializeField] private Text txt_Dialogue;

    public bool isFirst = true;

    private int count = 0;

    [SerializeField] private string[] dialugue;

    private void NextDialogue()
	{
        txt_Dialogue.text = dialugue[count];
        count++;
	}
    public void OnOff(bool _flag)
	{
        sprite_DialogueBox.gameObject.SetActive(_flag);
        txt_Dialogue.gameObject.SetActive(_flag);
        isFirst = _flag;
    }
	private void Start()
	{
        OnOff(isFirst);
    }
	// Update is called once per frame
	void Update()
    {
		if (isFirst)
		{
			if (Input.GetKeyDown(KeyCode.Return))
			{
                if (count < dialugue.Length)
                    NextDialogue();
                else
                    OnOff(false);
			}
		}
    }
}
