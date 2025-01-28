using System.Collections.Generic;
using UnityEngine;
using DialogueSystem.Data;
using DialogueSystem.Enumerations;
using GraphSystem.Base.ScriptableObjects;

namespace DialogueSystem.ScriptableObjects
{
    public class DialogueSystemDialogueSO : BaseNodeSO
    {
        [field: SerializeField] public string DialogueID { get; set; }
        [field: SerializeField][field: TextArea()] public string Text { get; set; }
        [field: SerializeField] public List<DialogueSystemDialogueChoiceData> Choices { get; set; }
        [field: SerializeField] public DialogueSystemDiagType DialogueType { get; set; }
        [field: SerializeField] public DialogueSystemDiagEvent EventType { get; set; }
        [field: SerializeField] public bool IsStartingDialogue { get; set; }
        [field: SerializeField] public ScriptableObject AssociatedObject { get; set; }
        [field: SerializeField] public string NextGroupName { get; set; }
        [field: SerializeField] public string ItemId { get; set; }


        public void Initialize(string ID, string text, List<DialogueSystemDialogueChoiceData> choices, 
            DialogueSystemDiagType dialogueType, bool isStartingDialogue, DialogueSystemDiagEvent eventDropdown, 
            ScriptableObject associatedObject, string nextGroupName, string itemIDField)
        {
            DialogueID = ID;
            Text = text;
            Choices = choices;
            DialogueType = dialogueType;
            IsStartingDialogue = isStartingDialogue;
            EventType = eventDropdown;
            AssociatedObject = associatedObject;
            NextGroupName = nextGroupName;
            ItemId = itemIDField;
        }


        public bool HasChoices()
        {
            return Choices != null && Choices.Count > 1;
        }

        public bool HasConnection()
        {
            return Choices != null && Choices.Count != 0;
        }
    }
}
