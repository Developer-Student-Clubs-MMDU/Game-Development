using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace VladislavTsurikov.MegaWorldSystem.Stamper
{
    [ExecuteInEditMode]
    public partial class StamperTool : MonoBehaviour
    {
        public Area Area = new Area();
        public BasicData Data = new BasicData();
        public StamperToolControllerSettings StamperToolControllerSettings = new StamperToolControllerSettings();
        
        public IEnumerator UpdateCoroutine;
        public float SpawnProgress = 0f;
        public bool CancelSpawn = false;
		public bool SpawnComplete = true;

#if UNITY_EDITOR
        public SettingsTypesStack SettingsTypesStack;
        public AutoRespawnController AutoRespawnController = new AutoRespawnController();
        public StamperVisualisation StamperVisualisation = new StamperVisualisation();

        public void AddPrototypeSettingsTypes()
        {
            foreach (ResourceType resourceType in typeof(ResourceType).GetEnumValues())
            {
                List<System.Type> settingsTypes = new List<System.Type>();

                switch (resourceType)
                {
                    case ResourceType.InstantItem:
                    {
                        settingsTypes.Add(typeof(SuccessSettings));
                        settingsTypes.Add(typeof(OverlapCheckSettings));
                        settingsTypes.Add(typeof(TransformComponentSettings));

                        SettingsTypesStack.AddPrototypeSettingsTypes(ResourceType.InstantItem, settingsTypes);

                        break;
                    }
                    case ResourceType.GameObject:
                    {
                        settingsTypes.Add(typeof(SuccessSettings));
                        settingsTypes.Add(typeof(OverlapCheckSettings));
                        settingsTypes.Add(typeof(TransformComponentSettings));

                        SettingsTypesStack.AddPrototypeSettingsTypes(ResourceType.GameObject, settingsTypes);

                        break;
                    }
                    case ResourceType.TerrainDetail:
                    {
                        settingsTypes.Add(typeof(MaskFilterSettings));
                        settingsTypes.Add(typeof(SpawnDetailSettings));

                        SettingsTypesStack.AddPrototypeSettingsTypes(ResourceType.TerrainDetail, settingsTypes);

                        break;
                    }
                    case ResourceType.TerrainTexture:
                    {
                        settingsTypes.Add(typeof(MaskFilterSettings));

                        SettingsTypesStack.AddPrototypeSettingsTypes(ResourceType.TerrainTexture, settingsTypes);

                        break;
                    }
                }
            }
        }

        public void AddGroupSettingsTypes()
        {
            List<System.Type> settingsTypes = new List<System.Type>();
            settingsTypes.Add(typeof(ScatterSettings));
            settingsTypes.Add(typeof(SimpleFilterSettings));
            settingsTypes.Add(typeof(MaskFilterSettings));

            SettingsTypesStack.AddGroupSettingsTypes(settingsTypes);
        }

        public void CreateSettingsTypesStack()
        {
            SettingsTypesStack = new SettingsTypesStack(this.GetType());
            AllSettingsTypes.SettingsTypesList.Add(SettingsTypesStack);
        }

        public void AddSettingsTypes()
        {
            AddPrototypeSettingsTypes();
            AddGroupSettingsTypes();
        }
#endif

        void OnEnable()
        {
            Area.SetAreaBoundsIfNecessary(this, true);

#if UNITY_EDITOR
            CreateSettingsTypesStack();
            AddSettingsTypes();
#endif
        }

        void Update()
        {
            Data.SelectedVariables.DeleteNullValueIfNecessary(Data.GroupList);
            Data.SelectedVariables.SetAllSelectedParameters(Data.GroupList);

            Area.SetAreaBoundsIfNecessary(this);
        }

        public void StartEditorUpdates()
        {
#if UNITY_EDITOR
            EditorApplication.update += EditorUpdate;
#endif
        }

        public void StopEditorUpdates()
        {
  #if UNITY_EDITOR
            EditorApplication.update -= EditorUpdate;
#endif
        }

        private void EditorUpdate()
        {
            if (UpdateCoroutine == null)
            {
                StopEditorUpdates();
                return;
            }
            else
            {
                UpdateCoroutine.MoveNext();
            }
        }
        
        public void Spawn()
        {
            SpawnComplete = false;
            UpdateCoroutine = RunSpawnCoroutine();
            StartEditorUpdates();
        }

        public void SpawnWithCells(List<Bounds> cellList)
        {
            SpawnComplete = false;
            UpdateCoroutine = RunSpawnCoroutineWithSpawnCells(cellList);
            StartEditorUpdates();
        }

        public void GenerateNewRandomSeed(Group group)
        {
            if (group.GenerateRandomSeed)
            {
                group.RandomSeed = UnityEngine.Random.Range(0, int.MaxValue);
            }
        }

        public void Spawn(Group group, AreaVariables areaVariables)
        {            
            GenerateNewRandomSeed(group);

            UnityEngine.Random.InitState(group.RandomSeed);

            switch (group.ResourceType)
            {
                case ResourceType.GameObject:
                {
                    SpawnType.SpawnGameObject(group, areaVariables, false);

                    break;
                }
                case ResourceType.TerrainDetail:
                {
                    SpawnType.SpawnTerrainDetails(group, group.ProtoTerrainDetailList, areaVariables);
                    
                    break;
                }
                case ResourceType.TerrainTexture:
                {
                    SpawnType.SpawnTerrainTexture(group, group.ProtoTerrainTextureList, areaVariables, 1);
            
                    break;
                }
                case ResourceType.InstantItem:
                {
                    SpawnType.SpawnInstantItem(group, areaVariables);
            
                    break;
                }
            }
        }

        public void RunSpawn()
        {
            for (int groupIndex = 0; groupIndex < Data.GroupList.Count; groupIndex++)
            {
                LayerSettings layerSettings = MegaWorldPath.CommonDataPackage.layerSettings;

                RayHit rayHit = RaycastUtility.Raycast(RayUtility.GetRayDown(transform.position), layerSettings.GetCurrentPaintLayers(Data.GroupList[groupIndex].ResourceType));

                if(rayHit == null)
                {
                    continue;
                }

                AreaVariables areaVariables = Area.GetAreaVariables(rayHit);

                Spawn(Data.GroupList[groupIndex], areaVariables);
            }
        }

        public IEnumerator RunSpawnCoroutine()
        {
            CancelSpawn = false;

            int maxTypes = Data.GroupList.Count;
            int completedTypes = 0;
            
            for (int groupIndex = 0; groupIndex < Data.GroupList.Count; groupIndex++)
            {
                if (CancelSpawn)
                {
                    break;
                }
#if UNITY_EDITOR
                EditorUtility.DisplayProgressBar("Running", "Running " + Data.GroupList[groupIndex].Name, (float)completedTypes / (float)maxTypes);
#endif

                LayerSettings layerSettings = MegaWorldPath.CommonDataPackage.layerSettings;

                RayHit rayHit = RaycastUtility.Raycast(RayUtility.GetRayDown(transform.position), layerSettings.GetCurrentPaintLayers(Data.GroupList[groupIndex].ResourceType));

                if(rayHit == null)
                {
                    continue;
                }

                AreaVariables areaVariables = Area.GetAreaVariables(rayHit);
                
                Spawn(Data.GroupList[groupIndex], areaVariables);

                completedTypes++;
                SpawnProgress = (float)completedTypes / (float)maxTypes;
                yield return null;
            }

            SpawnProgress = (float)completedTypes / (float)maxTypes;
            yield return null;

            SpawnProgress = 0;

#if UNITY_EDITOR
            EditorUtility.ClearProgressBar();
#endif
            SpawnComplete = true;
            UpdateCoroutine = null;
        }

        public IEnumerator RunSpawnCoroutineWithSpawnCells(List<Bounds> spawnCellList)
        {
            CancelSpawn = false;

            int maxTypes = Data.GroupList.Count;
            int completedTypes = 0;

            float oneStep = (float)1 / (float)spawnCellList.Count;

            for (int cellIndex = 0; cellIndex < spawnCellList.Count; cellIndex++)
            {
                float cellProgress = ((float)cellIndex / (float)spawnCellList.Count) * 100;

                for (int groupIndex = 0; groupIndex < Data.GroupList.Count; groupIndex++)
                {
                    if (CancelSpawn)
                    {
                        break;
                    }

                    SpawnProgress = cellProgress / 100;

                    if(maxTypes != 1)
                    {
                        SpawnProgress = (cellProgress / 100) + Mathf.Lerp(0, oneStep, (float)completedTypes / (float)maxTypes);
                    }

                    float typesProgress = (float)completedTypes / (float)maxTypes;
#if UNITY_EDITOR
                    EditorUtility.DisplayProgressBar("Cell: " + cellProgress + "%" + " (" + cellIndex + "/" + spawnCellList.Count + ")", "Running " + Data.GroupList[groupIndex].Name, SpawnProgress);
#endif

                    Bounds bounds = spawnCellList[cellIndex];

                    LayerSettings layerSettings = MegaWorldPath.CommonDataPackage.layerSettings;

                    RayHit rayHit = RaycastUtility.Raycast(RayUtility.GetRayDown(bounds.center), layerSettings.GetCurrentPaintLayers(Data.GroupList[groupIndex].ResourceType));

                    if(rayHit == null)
                    {
                        continue;
                    }

                    AreaVariables areaVariables = new AreaVariables(bounds, rayHit);
                
                    Spawn(Data.GroupList[groupIndex], areaVariables);

                    completedTypes++;
                    yield return null;
                }
            }

            SpawnProgress = 1;
            yield return null;

            SpawnProgress = 0;

#if UNITY_EDITOR
            EditorUtility.ClearProgressBar();
#endif
            SpawnComplete = true;
            UpdateCoroutine = null;
        }
    }
}