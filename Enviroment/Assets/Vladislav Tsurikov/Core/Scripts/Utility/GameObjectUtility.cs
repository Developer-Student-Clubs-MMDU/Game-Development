using UnityEngine;
using UnityEditor;
using System;

namespace VladislavTsurikov
{
    public class GameObjectUtility
    {
        public static GameObject GetPrefabRoot(GameObject gameObject)
        {
#if UNITY_EDITOR
            if(PrefabUtility.GetPrefabAssetType(gameObject) == PrefabAssetType.NotAPrefab)
            {
                return gameObject;
            }

            return PrefabUtility.GetOutermostPrefabInstanceRoot(gameObject);
#else
            return gameObject;
#endif
        }

        public static bool GetObjectWorldBounds(GameObject gameObject, out Bounds bounds)
        {
            Bounds worldBounds = new Bounds();
            bool found = false;

            ForAllInHierarchy(gameObject, (go) =>
            {
                if (!go.activeInHierarchy)
                    return;

                Renderer renderer = go.GetComponent<Renderer>();
                SkinnedMeshRenderer skinnedMeshRenderer;
                RectTransform rectTransform;

                if (renderer != null)
                {
                    if (!found)
                    {
                        worldBounds = renderer.bounds;
                        found = true;
                    }
                    else
                    {
                        worldBounds.Encapsulate(renderer.bounds);
                    }
                }
                else if ((skinnedMeshRenderer = go.GetComponent<SkinnedMeshRenderer>()) != null)
                {
                    if (!found)
                    {
                        worldBounds = skinnedMeshRenderer.bounds;
                        found = true;
                    }
                    else
                    {
                        worldBounds.Encapsulate(skinnedMeshRenderer.bounds);
                    }
                }
                else if ((rectTransform = go.GetComponent<RectTransform>()) != null)
                {
                    Vector3[] fourCorners = new Vector3[4];
                    rectTransform.GetWorldCorners(fourCorners);
                    Bounds rectBounds = new Bounds();

                    rectBounds.center = fourCorners[0];
                    rectBounds.Encapsulate(fourCorners[1]);
                    rectBounds.Encapsulate(fourCorners[2]);
                    rectBounds.Encapsulate(fourCorners[3]);

                    if (!found)
                    {
                        worldBounds = rectBounds;
                        found = true;
                    }
                    else
                    {
                        worldBounds.Encapsulate(rectBounds);
                    }
                }
             });

            if (!found)
                bounds = new Bounds(gameObject.transform.position, Vector3.one);
            else
                bounds = worldBounds;

            return found;
        }

        public static Bounds GetBoundsFromGameObject(GameObject gameObject)
        {
            Renderer renderer = gameObject.GetComponentInChildren<Renderer>();
            MeshFilter meshFilter = gameObject.GetComponentInChildren<MeshFilter>();
            Collider collider;

            if(renderer != null && renderer.enabled && renderer is SkinnedMeshRenderer)
            {
                return renderer.bounds;
            }
            else if (renderer != null && renderer.enabled &&
                meshFilter != null && meshFilter.sharedMesh != null)
            {
                return renderer.bounds;
            }
            else if ((collider = gameObject.GetComponent<Collider>()) != null && collider.enabled)
            {
                return collider.bounds;
            }

            return new Bounds();
        }

        public static void ForAllInHierarchy(GameObject gameObject, Action<GameObject> action)
        {
            action(gameObject);

            for (int i = 0; i < gameObject.transform.childCount; i++)
                ForAllInHierarchy(gameObject.transform.GetChild(i).gameObject, action);
        }

        public static void ParentGameObjectIfNecessary(GameObject gameObject, GameObject parent)
        {
            if(parent != null)
            {
                if (gameObject != null && parent != null)
                {
                    gameObject.transform.SetParent(parent.transform, true);
                }
            }
        }
    }
}