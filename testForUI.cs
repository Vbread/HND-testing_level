using UnityEngine;

public class testForUI : MonoBehaviour
{

    public UI_Dialogue_SO dialogue;

    public UI_In_Game PlayersUI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {

        PlayersUI = FindFirstObjectByType<UI_In_Game>();

        PlayersUI.OpenDialogue(dialogue);


    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
