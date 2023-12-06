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

        [Header("Position Offset Settings")]
        [SerializeField] [OnChangedCall(nameof(OnPositionOffsetChanged))] private float xOffset = 0;
        [SerializeField] [OnChangedCall(nameof(OnPositionOffsetChanged))] private float yOffset = 0;
        [SerializeField] [OnChangedCall(nameof(OnPositionOffsetChanged))] private float zOffset = 0;

        [Header("Randomize Position Settings")]
        [SerializeField] [OnChangedCall(nameof(OnRandomizePositionChanged))] private bool shouldRandomizePositionFactor = false;
        [SerializeField] [OnChangedCall(nameof(OnRandomizePositionChanged))] [Range(0, 1)] private float xRandomPositionFactor = 0;
        [SerializeField] [OnChangedCall(nameof(OnRandomizePositionChanged))] [Range(0, 1)] private float yRandomPositionFactor = 0;
        [SerializeField] [OnChangedCall(nameof(OnRandomizePositionChanged))] [Range(0, 1)] private float zRandomPositionFactor = 0;

        [Header("Randomize Rotation Settings")]
        [SerializeField] [OnChangedCall(nameof(OnRandomizeRotationChanged))] private bool shouldRandomizeRotationFactor = false;
        [SerializeField] [OnChangedCall(nameof(OnRandomizeRotationChanged))] [Range(0, 1)] private float xRandomRotationFactor = 0;
        [SerializeField] [OnChangedCall(nameof(OnRandomizeRotationChanged))] [Range(0, 1)] private float yRandomRotationFactor = 0;
        [SerializeField] [OnChangedCall(nameof(OnRandomizeRotationChanged))] [Range(0, 1)] private float zRandomRotationFactor = 0;

        [Header("Randomize Scale Settings")]
        [SerializeField] [OnChangedCall(nameof(OnRandomizeScaleChanged))] private bool shouldRandomizeScaleFactor = false;
        [SerializeField] [OnChangedCall(nameof(OnRandomizeScaleChanged))] [Range(0, 1)] private float xRandomScaleFactor = 0;
        [SerializeField] [OnChangedCall(nameof(OnRandomizeScaleChanged))] [Range(0, 1)] private float yRandomScaleFactor = 0;
        [SerializeField] [OnChangedCall(nameof(OnRandomizeScaleChanged))] [Range(0, 1)] private float zRandomScaleFactor = 0;

        private List<GameObject> createdObjects = new List<GameObject>();

        #region Callbacks

        public void OnArraySizeChanged ()
        {
            if (!prefab || arraySize <= 0) return;

            ClearChildObjects();
            GenerateCopyObjects();
            CheckForPositionChange();
            CheckForRotationChange();
            CheckForScaleChange();
        }

        public void OnPositionOffsetChanged ()
        {
            if (!prefab || arraySize <= 0 || createdObjects.Count <= 0) return;
            CheckForPositionChange();
        }

        public void OnRandomizePositionChanged ()
        {
            if (!prefab || arraySize <= 0 || createdObjects.Count <= 0) return;
            CheckForPositionChange();
        }

        public void OnRandomizeRotationChanged ()
        {
            if (!prefab || arraySize <= 0 || createdObjects.Count <= 0) return;
            CheckForRotationChange();
        }

        public void OnRandomizeScaleChanged ()
        {
            if (!prefab || arraySize <= 0 || createdObjects.Count <= 0) return;
            CheckForScaleChange();
        }

        #endregion

        #region Objects

        private void ClearChildObjects ()
        {
            for (int i = 0; i < createdObjects.Count; i++)
            {
                StartCoroutine(DestroyGameObjectNow(createdObjects[i]));
            }

            createdObjects.Clear();
        }

        private void GenerateCopyObjects ()
        {
            for (int i = 0; i < arraySize; i++)
            {
                GameObject newCopy = Instantiate(prefab, this.transform);
                newCopy.name = prefab.name + "(" + i + ")";
                createdObjects.Add(newCopy);
            }
        }

        private void CheckForPositionChange()
        {
            if (shouldRandomizePositionFactor)
            {
                ApplyRandomPositionFactor();
            }
            else
            {
                ApplyPositionOffset();
            }
        }

        private void CheckForRotationChange ()
        {
            if (shouldRandomizeRotationFactor)
            {
                ApplyRandomRotationFactor();
            }
            else
            {
                ResetRotation();
            }
        }

        private void CheckForScaleChange ()
        {
            if (shouldRandomizeScaleFactor)
            {
                ApplyRandomScaleFactor();
            } else
            {
                ResetScale();
            }
        }

        #endregion

        #region Tranform

        private void ApplyPositionOffset ()
        {
            for (int i = 0; i < createdObjects.Count; i++)
            {
                createdObjects[i].transform.localPosition = Vector3.zero;
                float newX = createdObjects[i].transform.localPosition.x + (i * xOffset);
                float newY = createdObjects[i].transform.localPosition.y + (i * yOffset);
                float newZ = createdObjects[i].transform.localPosition.z + (i * zOffset);
                createdObjects[i].transform.localPosition = new Vector3(newX, newY, newZ);
            }
        }

        private void ApplyRandomPositionFactor()
        {
            for (int i = 0; i < createdObjects.Count; i++)
            {
                createdObjects[i].transform.localPosition = Vector3.zero;
                float newX = createdObjects[i].transform.localPosition.x + (i * xOffset) + (Random.Range(-xRandomPositionFactor, xRandomPositionFactor) * 2);
                float newY = createdObjects[i].transform.localPosition.y + (i * yOffset) + (Random.Range(-yRandomPositionFactor, yRandomPositionFactor) * 2);
                float newZ = createdObjects[i].transform.localPosition.z + (i * zOffset) + (Random.Range(-zRandomPositionFactor, zRandomPositionFactor) * 2);
                createdObjects[i].transform.localPosition = new Vector3(newX, newY, newZ);
            }
        }

        private void ResetRotation ()
        {
            for (int i = 0; i < createdObjects.Count; i++)
            {
                createdObjects[i].transform.localEulerAngles = Vector3.zero;
            }
        }

        private void ApplyRandomRotationFactor ()
        {
            if (!shouldRandomizeRotationFactor) return;
            for (int i = 0; i < createdObjects.Count; i++)
            {
                createdObjects[i].transform.localEulerAngles = Vector3.zero;
                float newX = createdObjects[i].transform.localEulerAngles.x + (Random.Range(-xRandomRotationFactor, xRandomRotationFactor) * 5);
                float newY = createdObjects[i].transform.localEulerAngles.y + (Random.Range(-yRandomRotationFactor, yRandomRotationFactor) * 5);
                float newZ = createdObjects[i].transform.localEulerAngles.z + (Random.Range(-zRandomRotationFactor, zRandomRotationFactor) * 5);
                createdObjects[i].transform.localEulerAngles = new Vector3(newX, newY, newZ);
            }
        }

        private void ResetScale ()
        {
            for (int i = 0; i < createdObjects.Count; i++)
            {
                createdObjects[i].transform.localScale = Vector3.one;
            }
        }

        private void ApplyRandomScaleFactor ()
        {
            if (!shouldRandomizeScaleFactor) return;
            for (int i = 0; i < createdObjects.Count; i++)
            {
                createdObjects[i].transform.localScale = Vector3.one;
                float newX = createdObjects[i].transform.localScale.x + (Random.Range(0, xRandomScaleFactor) * 2);
                float newY = createdObjects[i].transform.localScale.y + (Random.Range(0, yRandomScaleFactor) * 2);
                float newZ = createdObjects[i].transform.localScale.z + (Random.Range(0, zRandomScaleFactor) * 2);
                createdObjects[i].transform.localScale = new Vector3(newX, newY, newZ);
            }
        }

        #endregion

        #region Helpers

        private IEnumerator DestroyGameObjectNow (GameObject obj)
        {
            yield return new WaitForSeconds(0);
            DestroyImmediate(obj);
        }

        #endregion

    }
}