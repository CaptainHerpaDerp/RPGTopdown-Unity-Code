using System;
using System.Collections.Generic;
using System.Linq;

namespace QuestSystem
{
    public class OngoingQuest
    {
        public Quest quest;
        public Dictionary<string, bool> completedQuestConditions = new();

        public OngoingQuest(Quest quest)
        {
            this.quest = quest;
        }

        public List<string> QuestConditionNames
        {
            get
            {
                return completedQuestConditions.Keys.ToList();
            }
        } 

        public bool IsQuestConditionComplete()
        {
            foreach (var condition in completedQuestConditions)
            {
                // Return false if any of the values are false
                if (!condition.Value == true)
                {
                    return false;
                }
            }

            return true;
        }
    }
}