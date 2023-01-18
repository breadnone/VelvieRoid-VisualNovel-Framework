using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UIElements;
using System.Threading;
using System.Threading.Tasks;
using VTasks;

namespace VelvieR
{
    public class VStageComponent : MonoBehaviour
    {
        [SerializeField, HideInInspector] private List<Label> stage = new List<Label>();
        [SerializeField, HideInInspector] public string vstageid;
        [SerializeField, HideInInspector] public string vstageName;

        void OnValidate()
        {
            if (String.IsNullOrEmpty(vstageid))
            {
                vstageid = Guid.NewGuid().ToString() + UnityEngine.Random.Range(0, int.MaxValue);
            }

            if (String.IsNullOrEmpty(vstageName) || vstageName != gameObject.name)
            {
                vstageName = gameObject.name;
            }
        }

        public virtual Vector3 GetStage()
        {
            Vector3 pos = Vector3.zero;

            return pos;
        }

        public virtual void ReValidate()
        {
            OnValidate();
        }

        public virtual void InsertVStage(Vector3 position, Vector3 size, string name)
        {
            if (!stage.Exists(x => x.name == name))
            {
                var lbl = new Label();
                lbl.style.borderBottomColor = Color.black;
                lbl.style.borderTopColor = Color.black;
                lbl.style.borderLeftColor = Color.black;
                lbl.style.borderRightColor = Color.black;

                lbl.style.borderBottomWidth = 3;
                lbl.style.borderTopWidth = 3;
                lbl.style.borderLeftWidth = 3;
                lbl.style.borderRightWidth = 3;
                lbl.name = name;
                lbl.transform.position = position * 2;

                stage.Add(lbl);
            }
        }
        public virtual void RemoveVStage(string name)
        {
            if (stage.Count == 0)
                return;

            for (int i = 0; i < stage.Count; i++)
            {
                if (stage[i].name == name)
                    stage.RemoveAt(i);
            }
        }
        public virtual void MoveCharacter(Transform from, Transform to, float duration, LeanTweenType ltype, bool isLocal = false)
        {
            if (!isLocal)
            {
                Vector3 oriFrom = from.position;
                Vector3 oriTo = to.position;
                LeanTween.move(from.gameObject, oriTo, duration).setEase(ltype);
            }
            else
            {
                Vector3 oriFrom = from.localPosition;
                Vector3 oriTo = to.localPosition;

                LeanTween.moveLocal(from.gameObject, oriTo, duration).setEase(ltype);
            }
        }
        public virtual void SwapCharacter(Transform from, Transform to, float duration, LeanTweenType ltype, bool isLocal = false)
        {
            if (!isLocal)
            {
                Vector3 oriFrom = from.position;
                Vector3 oriTo = to.position;

                LeanTween.move(from.gameObject.GetComponent<RectTransform>(), oriTo, duration).setEase(ltype);
                LeanTween.move(to.gameObject.GetComponent<RectTransform>(), oriFrom, duration).setEase(ltype);
            }
            else
            {
                Vector3 oriFrom = from.localPosition;
                Vector3 oriTo = to.localPosition;

                LeanTween.moveLocal(from.gameObject, oriTo, duration).setEase(ltype);
                LeanTween.moveLocal(to.gameObject, oriFrom, duration).setEase(ltype);
            }
        }
    }
}