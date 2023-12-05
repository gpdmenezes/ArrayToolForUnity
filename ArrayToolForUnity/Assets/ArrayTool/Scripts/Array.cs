using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArrayTool
{
    public class Array : MonoBehaviour
    {
        [Header("Main Settings")]
        [SerializeField] [OnChangedCall(nameof(OnArraySizeChanged))] private GameObject prefab;
        [SerializeField] [OnChangedCall(nameof(OnArraySizeChanged))] [Min(0)] private int arraySize = 0;

        [Header("Offset Settings")]
        [SerializeField] [OnChangedCall(nameof(OnOffsetChanged))] private float xOffset = 0;
        [SerializeField] [OnChangedCall(nameof(OnOffsetChanged))] private float yOffset = 0;
        [SerializeField] [OnChangedCall(nameof(OnOffsetChanged))] private float zOffset = 0;

        private List<GameObject> createdObjects = new List<GameObject>();

        public void OnArraySizeChanged ()
        {
            if (!prefab) return;

            ClearChildObjects();
            GenerateCopyObjects();
            ApplyOffset();
        }

        public void OnOffsetChanged ()
        {
            ApplyOffset();
        }

        private void ClearChildObjects ()
        {
            if (createdObjects.Count <= 0) return;

            for (int i = 0; i < createdObjects.Count; i++)
            {
                StartCoroutine(DestroyGameObjectNow(createdObjects[i]));
            }

            createdObjects.Clear();
        }

        private void GenerateCopyObjects ()
        {
            if (arraySize <= 0) return;
            for (int i = 0; i < arraySize; i++)
            {
                GameObject newCopy = Instantiate(prefab, this.transform);
                newCopy.name = prefab.name + "(" + i + ")";
                createdObjects.Add(newCopy);
            }
        }

        private void ApplyOffset ()
        {
            if (createdObjects.Count <= 0) return;
            for (int i = 0; i < createdObjects.Count; i++)
            {
                createdObjects[i].transform.localPosition = Vector3.zero;
                float newX = createdObjects[i].transform.localPosition.x + (i * xOffset);
                float newY = createdObjects[i].transform.localPosition.y + (i * yOffset);
                float newZ = createdObjects[i].transform.localPosition.z + (i * zOffset);
                createdObjects[i].transform.localPosition = new Vector3(newX, newY, newZ);
            }
        }

        private IEnumerator DestroyGameObjectNow (GameObject obj)
        {
            yield return new WaitForSeconds(0);
            DestroyImmediate(obj);
        }

    }
}