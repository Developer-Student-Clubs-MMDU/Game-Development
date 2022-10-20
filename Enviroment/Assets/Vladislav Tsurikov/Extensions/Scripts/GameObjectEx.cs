#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace VladislavTsurikov.Extensions
{
    public static class GameObjectEx
    {
        public static List<GameObject> GetAllChildren(this GameObject gameObject)
        {
            Transform[] childTransforms = gameObject.GetComponentsInChildren<Transform>();
            var allChildren = new List<GameObject>(childTransforms.Length);

            foreach (var child in childTransforms)
            {
                if (child.gameObject != gameObject) allChildren.Add(child.gameObject);
            }

            return allChildren;
        }

        public static List<GameObject> GetAllChildrenAndSelf(this GameObject gameObject)
        {
            Transform[] childTransforms = gameObject.GetComponentsInChildren<Transform>();
            var allChildren = new List<GameObject>(childTransforms.Length);

            foreach (var child in childTransforms)
            {
                allChildren.Add(child.gameObject);
            }

            return allChildren;
        }

        public static Mesh GetMesh(this GameObject gameObject)
        {
            MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
            if (meshFilter != null && meshFilter.sharedMesh != null) return meshFilter.sharedMesh;

            SkinnedMeshRenderer skinnedMeshRenderer = gameObject.GetComponent<SkinnedMeshRenderer>();
            if (skinnedMeshRenderer != null && skinnedMeshRenderer.sharedMesh != null) return skinnedMeshRenderer.sharedMesh;

            return null;
        }

        public static bool IsRendererEnabled(this GameObject gameObject)
        {
            MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
            if (meshRenderer != null) return meshRenderer.enabled;

            SkinnedMeshRenderer skinnedRenderer = gameObject.GetComponent<SkinnedMeshRenderer>();
            if (skinnedRenderer != null) return skinnedRenderer.enabled;

            return false;
        }

        public static Bounds GetInstantiatedBounds(this GameObject prefab)
		{
			GameObject go = MonoBehaviour.Instantiate(prefab);
			go.transform.position = prefab.transform.position;
			Bounds bounds = new Bounds(go.transform.position, Vector3.zero);
			foreach (Renderer r in go.GetComponentsInChildren<Renderer>())
			{
				bounds.Encapsulate(r.bounds);
			}
			foreach (Collider c in go.GetComponentsInChildren<Collider>())
			{
				bounds.Encapsulate(c.bounds);
			}
			MonoBehaviour.DestroyImmediate(go);
			return bounds;
		}
    }
}
#endif