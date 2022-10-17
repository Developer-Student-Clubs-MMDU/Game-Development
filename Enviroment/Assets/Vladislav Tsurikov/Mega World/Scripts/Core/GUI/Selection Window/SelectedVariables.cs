using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VladislavTsurikov.MegaWorldSystem
{
	public class SelectedVariables
	{
		public List<Group> SelectedGroupList = new List<Group>();

		public List<PrototypeGameObject> SelectedProtoGameObjectList = new List<PrototypeGameObject>();
		public List<PrototypeTerrainDetail> SelectedProtoTerrainDetailList = new List<PrototypeTerrainDetail>();
		public List<PrototypeTerrainTexture> SelectedProtoTerrainTextureList = new List<PrototypeTerrainTexture>();
        public List<PrototypeInstantItem> SelectedProtoInstantItemList = new List<PrototypeInstantItem>();

		public Group SelectedGroup;
		public PrototypeGameObject SelectedProtoGameObject;
		public PrototypeTerrainDetail SelectedProtoTerrainDetail;
		public PrototypeTerrainTexture SelectedProtoTerrainTexture;
		public PrototypeInstantItem SelectedProtoInstantItem;

		public void SetAllSelectedParameters(List<Group> groupList)
		{
			ClearSelectedList();
			SetSelectedList(groupList);
			SetSelected();
		}

		public void ClearSelectedList()
		{
			SelectedGroupList.Clear();
			SelectedProtoGameObjectList.Clear();
			SelectedProtoTerrainDetailList.Clear();
			SelectedProtoTerrainTextureList.Clear();
			SelectedProtoInstantItemList.Clear();
		}

		public void SetSelected()
		{
			SetSelectedGroup();
			SetSelectedProtoGameObject();
			SetSelectedProtoTerrainDetail();
			SetSelectedProtoTerrainTexture();

			SetSelectedProtoInstantItem();
		}

		public void SetSelectedList(List<Group> groupList)
		{
		    for (int index = 0; index < groupList.Count; index++)
		    {
		    	if(groupList[index].Selected)
		    	{
					Group selectedGroup = groupList[index];
					SelectedGroupList.Add(selectedGroup);

					switch (selectedGroup.ResourceType)
					{
						case ResourceType.GameObject:
            			{
							SelectedVariables.SetSelectedPrototypeListFromAssets(selectedGroup.ProtoGameObjectList, SelectedProtoGameObjectList, typeof(PrototypeGameObject));
            			    break;
            			}
            			case ResourceType.TerrainDetail:
            			{
							SelectedVariables.SetSelectedPrototypeListFromAssets(selectedGroup.ProtoTerrainDetailList, SelectedProtoTerrainDetailList, typeof(PrototypeTerrainDetail));
            			    break;
            			}
						case ResourceType.TerrainTexture:
            			{
							SelectedVariables.SetSelectedPrototypeListFromAssets(selectedGroup.ProtoTerrainTextureList, SelectedProtoTerrainTextureList, typeof(PrototypeTerrainTexture));
            			    break;
            			}
						case ResourceType.InstantItem:
						{
							SelectedVariables.SetSelectedPrototypeListFromAssets(selectedGroup.ProtoInstantItemList, SelectedProtoInstantItemList, typeof(PrototypeInstantItem));
							break;
						}
					}
		    	}
		    }
		}

		public void SetSelectedGroup()
		{
			if(SelectedGroupList.Count == 1)
			{
				SelectedGroup = SelectedGroupList[SelectedGroupList.Count - 1];
			}
			else
			{
				SelectedGroup = null;
			}
		}

		public void SetSelectedProtoGameObject()
		{
			if(SelectedProtoGameObjectList.Count == 1)
			{
				SelectedProtoGameObject = SelectedProtoGameObjectList[SelectedProtoGameObjectList.Count - 1];
			}
			else
			{
				SelectedProtoGameObject = null;
			}
		}

		public void SetSelectedProtoInstantItem()
		{
			if(SelectedProtoInstantItemList.Count == 1)
			{
				SelectedProtoInstantItem = SelectedProtoInstantItemList[SelectedProtoInstantItemList.Count - 1];
			}
			else
			{
				SelectedProtoInstantItem = null;
			}
		}

		public void SetSelectedProtoTerrainDetail()
		{
			if(SelectedProtoTerrainDetailList.Count == 1)
			{
				SelectedProtoTerrainDetail = SelectedProtoTerrainDetailList[SelectedProtoTerrainDetailList.Count - 1];
			}
			else
			{
				SelectedProtoTerrainDetail = null;
			}
		}

		public void SetSelectedProtoTerrainTexture()
		{
			if(SelectedProtoTerrainTextureList.Count == 1)
			{
				SelectedProtoTerrainTexture = SelectedProtoTerrainTextureList[SelectedProtoTerrainTextureList.Count - 1];
			}
			else
			{
				SelectedProtoTerrainTexture = null;
			}
		}

		public bool OnlyInstantItemSelected()
        {
            if(SelectedProtoGameObjectList.Count != 0) 
			{
				return false;
			}
			if(SelectedProtoTerrainDetailList.Count != 0)
			{
				return false;
			}
			if(SelectedProtoTerrainTextureList.Count != 0)
			{
				return false;
			}

			return true;
        }

		public bool HasOneSelectedGroup()
		{
			if(SelectedGroup == null)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		public bool HasOneSelectedProtoGameObject()
		{
			if(SelectedProtoGameObject == null)
			{
				return false;
			}
			else
			{
				return true;
			}
		}
		
		public bool HasOneSelectedProtoTerrainDetail()
		{
			if(SelectedProtoTerrainDetail == null)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		public bool HasOneSelectedProtoTerrainTexture()
		{
			if(SelectedProtoTerrainTexture == null)
			{
				return false;
			}
			else
			{
				return true;
			}
		}
		
		public bool HasOneSelectedResourceProto()
		{
			if(SelectedProtoGameObject != null || SelectedProtoTerrainDetail != null || SelectedProtoTerrainTexture != null || SelectedProtoInstantItem != null)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool HasOneSelectedProtoInstantItem()
		{
			if(SelectedProtoInstantItem == null)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		public bool HasSelectedResourceGameObject()
		{
			foreach (Group group in SelectedGroupList)
			{
				if(group.ResourceType == ResourceType.GameObject)
				{
					return true;
				}
			}

			return false;
		}

		public bool HasSelectedResourceInstantItem()
		{
			foreach (Group group in SelectedGroupList)
			{
				if(group.ResourceType == ResourceType.InstantItem)
				{
					return true;
				}
			}

			return false;
		}

		public void DeleteNullValueIfNecessary(List<Group> groupList)
		{
			foreach (Group group in groupList)
			{
				if(group == null)
				{
					groupList.Remove(group);
					return;
				}
			}
		}

		public List<Prototype> GetAllSelectedPrototypes()
		{
			List<Prototype> protoList = new List<Prototype>();

			protoList.AddRange(SelectedProtoGameObjectList);
			protoList.AddRange(SelectedProtoTerrainDetailList);
			protoList.AddRange(SelectedProtoTerrainTextureList);
			protoList.AddRange(SelectedProtoInstantItemList);

			return protoList;
		}

		public static void SetSelectedPrototypeListFromAssets<T>(List<T> baseList, List<T> setPrototypeList, System.Type prototypeType) where T : Prototype
        {
            foreach (T asset in baseList)
            {
                if (asset.GetType() == prototypeType)
                {
                    if(asset.Selected)
                    {
                        setPrototypeList.Add((T)asset);
                    }
                }
            }
        }
	}
}