using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ArrayTool
{
    public class Array : MonoBehaviour
    {
        [Header("Main Settings")]
        [SerializeField] private GameObject prefab;
        [SerializeField] [Min(0)] private int arraySize = 0;

        [Header("Position Offset Settings")]
        [SerializeField] private float xOffset = 0;
        [SerializeField] private float yOffset = 0;
        [SerializeField] private float zOffset = 0;

        [Header("Randomize Position Settings")]
        [SerializeField] private bool shouldRandomizePositionFactor = false;
        [SerializeField] [Range(0, 1)] private float xRandomPositionFactor = 0;
        [SerializeField] [Range(0, 1)] private float yRandomPositionFactor = 0;
        [SerializeField] [Range(0, 1)] private float zRandomPositionFactor = 0;

        [Header("Randomize Rotation Settings")]
        [SerializeField] private bool shouldRandomizeRotationFactor = false;
        [SerializeField] [Range(0, 1)] private float xRandomRotationFactor = 0;
        [SerializeField] [Range(0, 1)] private float yRandomRotationFactor = 0;
        [SerializeField] [Range(0, 1)] private float zRandomRotationFactor = 0;

        [Header("Randomize Scale Settings")]
        [SerializeField] private bool shouldRandomizeScaleFactor = false;
        [SerializeField] [Range(0, 1)] private float xRandomScaleFactor = 0;
        [SerializeField] [Range(0, 1)] private float yRandomScaleFactor = 0;
        [SerializeField] [Range(0, 1)] private float zRandomScaleFactor = 0;

        private List<GameObject> createdObjects = new List<GameObject>();
        private FieldChangesTracker changesTracker = new FieldChangesTracker();

        #region Initialization

        private void OnValidate ()
        {
            RefreshCreatedObjectsList();
            StartCoroutine(OnValueChanged());
        }

        private void RefreshCreatedObjectsList()
        {
            createdObjects.Clear();
            if (transform.childCount <= 0) return;
            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject child = transform.GetChild(i).gameObject;
                createdObjects.Add(child);
            }
        }

        private IEnumerator OnValueChanged()
        {
            yield return new WaitForSeconds(0);
            if (changesTracker.TrackFieldChanges(this, x => x.arraySize) || changesTracker.TrackFieldChanges(this, x => x.prefab))
            {
                OnArraySizeChanged();
            }

            if (changesTracker.TrackFieldChanges(this, x => x.xOffset) || changesTracker.TrackFieldChanges(this, x => x.yOffset) || changesTracker.TrackFieldChanges(this, x => x.zOffset))
            {
                OnPositionOffsetChanged();
            }

            if (changesTracker.TrackFieldChanges(this, x => x.shouldRandomizePositionFactor) || changesTracker.TrackFieldChanges(this, x => x.xRandomPositionFactor) || changesTracker.TrackFieldChanges(this, x => x.yRandomPositionFactor) || changesTracker.TrackFieldChanges(this, x => x.zRandomPositionFactor))
            {
                OnRandomizePositionChanged();
            }

            if (changesTracker.TrackFieldChanges(this, x => x.shouldRandomizeRotationFactor) || changesTracker.TrackFieldChanges(this, x => x.xRandomRotationFactor) || changesTracker.TrackFieldChanges(this, x => x.yRandomRotationFactor) || changesTracker.TrackFieldChanges(this, x => x.zRandomRotationFactor))
            {
                OnRandomizeRotationChanged();
            }

            if (changesTracker.TrackFieldChanges(this, x => x.shouldRandomizeScaleFactor) || changesTracker.TrackFieldChanges(this, x => x.xRandomScaleFactor) || changesTracker.TrackFieldChanges(this, x => x.yRandomScaleFactor) || changesTracker.TrackFieldChanges(this, x => x.zRandomScaleFactor))
            {
                OnRandomizeScaleChanged();
            }
        }

        #endregion

        #region Callbacks

        public void OnArraySizeChanged ()
        {
            if (!prefab) return;

            CheckCreatedObjects();

            if (arraySize <= 0) return;

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

        private void CheckCreatedObjects ()
        {
            if (arraySize == 0)
            {
                RemoveAllCreatedObjects();
            } 
            else if (arraySize < createdObjects.Count)
            {
                RemoveSurplusCreatedObjects();
            } 
            else if (arraySize > createdObjects.Count)
            {
                CreateNewObjects();
            }
        }

        private void RemoveAllCreatedObjects ()
        {
            if (createdObjects.Count > 0)
            {
                for (int i = 0; i < createdObjects.Count; i++)
                {
                    DestroyImmediate(createdObjects[i]);
                }
            }
            createdObjects.Clear();
        }

        private void RemoveSurplusCreatedObjects ()
        {
            int surplusAmount = createdObjects.Count - arraySize;
            int targetQuantity = createdObjects.Count - surplusAmount;

            for (int i = createdObjects.Count - 1; i >= targetQuantity; i--)
            {
                GameObject obj = createdObjects[i];
                createdObjects.Remove(obj);
                DestroyImmediate(obj);
            }
        }

        private void CreateNewObjects ()
        {
            int creationAmount = arraySize - createdObjects.Count;

            for (int i = 0; i < creationAmount; i++)
            {
                GameObject newCopy = Instantiate(prefab, this.transform);
                newCopy.name = prefab.name + " (" + createdObjects.Count + ")";
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
            } 
            else
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

    }
}