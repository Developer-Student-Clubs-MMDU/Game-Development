#if UNITY_EDITOR
using System.Collections.Generic;

namespace VladislavTsurikov.MegaWorldSystem.Stamper
{
    public class AutoRespawn 
    {
        public Group _modifiedType = null;
        private PrototypeTerrainDetail _modifiedTerrainDetailProto = null;
        private StamperTool _stamperTool;

        public AutoRespawn(Group group, StamperTool stamperTool)
        {
            _stamperTool = stamperTool;
            _modifiedType = group;
        }

        public AutoRespawn(PrototypeTerrainDetail proto, StamperTool stamperTool)
        {
            _stamperTool = stamperTool;
            _modifiedTerrainDetailProto = proto;
        }

        public void TypeHasChanged()
        {
            if(_stamperTool.Area.UseSpawnCells == false)
			{
                if(_modifiedType != null)
			    {
			    	UnspawnTypesForAutoRespawn(_modifiedType, _stamperTool.Data);
                    _stamperTool.RunSpawn();
			    }
			    else if(_modifiedTerrainDetailProto != null)
			    {
			    	if(_stamperTool.Data.SelectedVariables.HasOneSelectedGroup())
			    	{
			    		List<PrototypeTerrainDetail> protoTerrainDetailList = new List<PrototypeTerrainDetail>();
			    		protoTerrainDetailList.Add(_modifiedTerrainDetailProto);
			    	    Unspawn.UnspawnTerrainDetail(protoTerrainDetailList, false);

			    		Group group = _stamperTool.Data.SelectedVariables.SelectedGroup;

                        LayerSettings layerSettings = MegaWorldPath.CommonDataPackage.layerSettings;

			    		RayHit rayHit = RaycastUtility.Raycast(RayUtility.GetRayDown(_stamperTool.transform.position), 
                            layerSettings.GetCurrentPaintLayers(group.ResourceType));
                    	
                		if(rayHit != null)
                		{
                		    AreaVariables areaVariables = _stamperTool.Area.GetAreaVariables(rayHit);

                            SpawnType.SpawnTerrainDetails(group, protoTerrainDetailList, areaVariables);
                		}
			    	}
			    }
            }
        }

        public void UnspawnTypesForAutoRespawn(Group modifiedType, BasicData data)
        {            
            foreach (Group group in data.SelectedVariables.SelectedGroupList)
            {
                if(modifiedType.ResourceType == ResourceType.TerrainDetail)
                {
                    Unspawn.UnspawnTerrainDetail(group.ProtoTerrainDetailList, false);
                }
                else if(modifiedType.ResourceType == ResourceType.InstantItem)
                {
                    Unspawn.UnspawnInstantItem(group, false);
                }
                else if(modifiedType.ResourceType == ResourceType.GameObject)
                {
                    Unspawn.UnspawnGameObject(group, false);
                }
            }
        }
    }
}
#endif