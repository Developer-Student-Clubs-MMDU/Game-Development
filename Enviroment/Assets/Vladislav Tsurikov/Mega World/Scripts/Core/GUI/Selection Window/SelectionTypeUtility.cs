#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

namespace VladislavTsurikov.MegaWorldSystem
{
    public static class SelectionTypeUtility 
    {
        public static void DisableAllPrototype(List<Group> groupList)
        {
            foreach (Group group in groupList)
            {
                foreach (PrototypeGameObject prototype in group.ProtoGameObjectList)
                {
                    prototype.Selected = false;
                }
                foreach (PrototypeTerrainDetail prototype in group.ProtoTerrainDetailList)
                {
                    prototype.Selected = false;
                }
                foreach (PrototypeInstantItem prototype in group.ProtoInstantItemList)
                {
                    prototype.Selected = false;
                }
            }
        }

        public static void AddGroup(List<Group> groupList, ResourceType resourceType)
        {
            Group newAsset = ProfileFactory.CreateGroup("New");
            newAsset.ResourceType = resourceType;
            
            groupList.Add(newAsset);       
        }

        public static void DeleteSelectedGroups(List<Group> groupList)
        {
            groupList.RemoveAll((group) => group.Selected);
        }

        public static void SelectAllGroups(List<Group> groupList)
        {
            DisableAllPrototype(groupList);

            foreach (Group group in groupList)
            {
                SelectionPrototypeUtility.SetSelectedAllPrototypes(group, true);
                group.Selected = true;
            }
        }

        public static void DisableAllGroup(List<Group> groupList)
        {
            foreach (Group group in groupList)
            {
                group.Selected = false;
            }
        }

        public static void SelectType(List<Group> groupList, int groupIndex)
        {
            if(groupIndex < 0 && groupIndex >= groupList.Count)
            {
                return;
            }
            
            foreach (Group group in groupList)
            {
                group.Selected = false;
            }

            DisableAllPrototype(groupList);
            SelectionPrototypeUtility.SetSelectedAllPrototypes(groupList[groupIndex], true);
            groupList[groupIndex].Selected = true;
        }

        public static void SelectTypeAdditive(List<Group> groupList, int groupIndex)
        {
            if(groupIndex < 0 && groupIndex >= groupList.Count)
            {
                return;
            }

            groupList[groupIndex].Selected = !groupList[groupIndex].Selected;
            if(groupList[groupIndex].Selected)
            {
                SelectionPrototypeUtility.SetSelectedAllPrototypes(groupList[groupIndex], true);
            }
            else
            {
                SelectionPrototypeUtility.SetSelectedAllPrototypes(groupList[groupIndex], false);
            }
        }
        
        public static void SelectGroupRange(List<Group> groupList, int groupIndex)
        {
            if(groupIndex < 0 && groupIndex >= groupList.Count)
            {
                return;
            }

            int rangeMin = groupIndex;
            int rangeMax = groupIndex;

            for (int i = 0; i < groupList.Count; i++)
            {
                if (groupList[i].Selected)
                {
                    rangeMin = Mathf.Min(rangeMin, i);
                    rangeMax = Mathf.Max(rangeMax, i);
                }
            }

            for (int i = rangeMin; i <= rangeMax; i++) 
            {
                if (groupList[i].Selected != true)
                {
                    break;
                }
            }

            for (int i = rangeMin; i <= rangeMax; i++) 
            {
                SelectionPrototypeUtility.SetSelectedAllPrototypes(groupList[groupIndex], true);
                groupList[i].Selected = true;
            }
        }

        public static void InsertSelectedGroup(List<Group> groupList, int index, bool after)
        {
            List<Group> selectedBrushes = new List<Group>();
            groupList.ForEach ((group) => { if(group.Selected) selectedBrushes.Add(group); });

            if(selectedBrushes.Count > 0)
            {
                index += after ? 1 : 0;
                index = Mathf.Clamp(index, 0, groupList.Count);

                groupList.Insert(index, null);    // insert null marker
                groupList.RemoveAll (b => b != null && b.Selected); // remove all selected
                groupList.InsertRange(groupList.IndexOf(null), selectedBrushes); // insert selected brushes after null marker
                groupList.RemoveAll ((b) => b == null); // remove null marter
            }
        }
    }
}
#endif