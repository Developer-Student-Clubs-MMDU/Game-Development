using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VladislavTsurikov.RaycastEditorSystem
{
    public class RaycastObject
    {
        public GameObject GameObject;
        public int Layer;
        public Mesh Mesh;
        public SkinnedMeshRenderer SkinnedMeshRenderer;
        public MeshRenderer MeshRenderer;
        public Renderer Renderer;
        public TerrainCollider TerrainCollider;
        public RectTransform RectTransform;
        public Vector3[] CornerPoints = new Vector3[4];
        public Bounds Bounds;
        public bool removeThisObject = false;

        public bool IsRendererEnabled()
        {
            if(Mesh == null) return false;

            if (MeshRenderer != null) return MeshRenderer.enabled;
            if (SkinnedMeshRenderer != null) return SkinnedMeshRenderer.enabled;

            return false;
        }

        public static RaycastObject MakeRaycastObject(GameObject gameObject)
        {
            if (!gameObject.activeInHierarchy)
                return null;

            Renderer renderer = gameObject.GetComponent<Renderer>();
            MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
            TerrainCollider terrainCollider;
            RectTransform rectTransform;

            if(renderer != null && renderer.enabled && renderer is SkinnedMeshRenderer)
            {
                RaycastObject obj = new RaycastObject();

                obj.Renderer = renderer;
                obj.Bounds = renderer.bounds;
                obj.Layer = 1 << gameObject.layer;

                obj.GameObject = gameObject;

                obj.SkinnedMeshRenderer = gameObject.GetComponent<SkinnedMeshRenderer>();
                obj.Mesh = obj.SkinnedMeshRenderer.sharedMesh;

                if (obj.Mesh == null)
                    return null;

                return obj;
            }
            else
            if (renderer != null && renderer.enabled &&
                meshFilter != null && meshFilter.sharedMesh != null)
            {
                RaycastObject obj = new RaycastObject();

                obj.Renderer = renderer;
                obj.Bounds = renderer.bounds;
                obj.Layer = 1 << gameObject.layer;

                obj.GameObject = gameObject;

                obj.MeshRenderer = gameObject.GetComponent<MeshRenderer>();

                obj.Mesh = meshFilter.sharedMesh;

                return obj;
            }
            else if ((terrainCollider = gameObject.GetComponent<TerrainCollider>()) != null && terrainCollider.enabled)
            {
                RaycastObject obj = new RaycastObject();

                obj.TerrainCollider = terrainCollider;
                obj.Bounds = terrainCollider.bounds;
                obj.Layer = 1 << gameObject.layer;

                obj.GameObject = gameObject;

                return obj;
            }
            else if ((rectTransform = gameObject.GetComponent<RectTransform>()) != null)
            {
                RaycastObject obj = new RaycastObject();

                rectTransform.GetWorldCorners(obj.CornerPoints);

                Bounds bounds = new Bounds(obj.CornerPoints[0], Vector3.zero);
                bounds.Encapsulate(obj.CornerPoints[1]);
                bounds.Encapsulate(obj.CornerPoints[2]);
                bounds.Encapsulate(obj.CornerPoints[3]);

                obj.RectTransform = rectTransform;

                obj.Bounds = bounds;
                obj.Layer = 1 << gameObject.layer;

                obj.GameObject = gameObject;

                return obj;
            }

            return null;
        }
    }
}