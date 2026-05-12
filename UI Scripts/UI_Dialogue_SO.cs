using UnityEngine;

[CreateAssetMenu(fileName = "UI_Dialogue_SO", menuName = "Scriptable Objects/UI_Dialogue_SO")]
public class UI_Dialogue_SO : ScriptableObject
{
    public Sprite Portrait;
    public string SpeakerName;

    [TextArea(3, 10), Tooltip("168 recommended character limit.")]
    public string DialogueHere;

    public UI_Dialogue_SO NextDialogue;

}
