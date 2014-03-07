using System;

using UnityEngine;

namespace BalloonBasket.Tech {
	public class Utils {
        public static Mesh CreateMesh(Vector2 dimensions) {
            Mesh mesh = new Mesh();
            
            Vector3[] vertices = new Vector3[] {
                new Vector3(dimensions.x, dimensions.y,  0),
                new Vector3(dimensions.x, -dimensions.y, 0),
                new Vector3(-dimensions.x, dimensions.y, 0),
                new Vector3(-dimensions.x, -dimensions.y, 0),
            };
            
            Vector2[] uv = new Vector2[] {
                new Vector2(1, 1),
                new Vector2(1, 0),
                new Vector2(0, 1),
                new Vector2(0, 0),
            };

            int[] triangles = new int[] {
                0, 1, 2,
                2, 1, 3,
            };
            
            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            
            return mesh;
        }

        public static void SetTransform(Transform t, Vector3 position, Vector3 scale, Quaternion rotation) {
            t.localPosition = position;
            t.localScale = scale;
            t.localRotation = rotation;
        }

        public static void SetTransform(Transform t, Vector3 position, Vector3 scale) {
            Utils.SetTransform(t, position, scale, Quaternion.identity);
        }

        public static void SetTransform(Transform t, Vector3 position) {
            Utils.SetTransform(t, position, Vector3.one, Quaternion.identity);
        }

        public static void ResetTransform(Transform t) {
            Utils.SetTransform(t, Vector3.zero, Vector3.one, Quaternion.identity);
        }

        public static UnityEngine.Object LoadResource(string resourceName) {
            UnityEngine.Object obj = Resources.Load(resourceName);
            if (obj == null) {
                throw new ApplicationException(string.Format("Failed to load asset {0} from app", resourceName));
            }
            return obj;
        }

        public static void InitTexture(GameObject go, Transform parent, string textureName, string shaderName) {
            go.AddComponent<MeshRenderer>();
            MeshFilter filter = go.AddComponent<MeshFilter>();
            
            filter.mesh = Utils.CreateMesh(Vector2.one);

            go.transform.parent = parent;
            Utils.SetTransform(go.transform, new Vector3(0f, 0f, 1f));

            go.renderer.material.mainTexture = Utils.LoadResource(textureName) as Texture;

            Shader shader = Shader.Find(shaderName);
            if(shader == null) {
                throw new ApplicationException(string.Format("Failed to find Shader {0}", shaderName));
            }
            go.renderer.material.shader = shader;
        }

        public static string LoadLevel(string levelName) {
            TextAsset level = LoadResource(levelName) as TextAsset;
            if(level == null) {
                return null;
            }
            return level.text;
        }
	}
}

