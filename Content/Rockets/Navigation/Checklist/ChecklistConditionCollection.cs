﻿using Terraria.UI;
using Terraria.Localization;
using System.Collections.Generic;
using System.Collections;

namespace Macrocosm.Content.Rockets.Navigation.Checklist
{
    public class ChecklistConditionCollection : IEnumerable<ChecklistCondition>
    {
		private List<ChecklistCondition> conditions = new();

        public ChecklistCondition this[int index] => conditions[index];

		public void Add(ChecklistCondition condition) 
            => conditions.Add(condition); 

		public void Remove(ChecklistCondition condition)
			 => conditions.Remove(condition);

        public void Remove(string key)
             => Remove(conditions.Find(x => x.LangKey == key));

        public void Append(ChecklistConditionCollection extraConditions)
            => conditions.AddRange(extraConditions);

        public static ChecklistConditionCollection Merge(ChecklistConditionCollection conditions1, ChecklistConditionCollection conditions2)
        {
            ChecklistConditionCollection output = new();
            output.Append(conditions1);
            output.Append(conditions2);

            return output;
        }

        public bool MetAll()
        {
            foreach (var condition in conditions) 
            {
                if(!condition.Met())
                    return false;
            }

            return true;
        }

        public List<UIElement> ProvideUIElementList()
        {
            var list = new List<UIElement>();

            foreach(var condition in conditions)
            {
                // If true, add only if not hidden while true
                if(!condition.HideIfMet || !condition.Met())
                    list.Add(condition.ProvideUI());
            }

            return list;
        }

        public void AddToUI(UIElement element)
        {
            foreach (var condition in conditions) 
            {
				// If true, add only if not hidden while true
				if (!condition.HideIfMet || !condition.Met())
					element.Append(condition.ProvideUI());
            }
        }

		public IEnumerator<ChecklistCondition> GetEnumerator()
		    => conditions.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator()
		    => conditions.GetEnumerator();
	}
}