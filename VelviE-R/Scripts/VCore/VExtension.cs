using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using VTasks;
using System.Threading;
using UnityEngine.UI;
using System.ComponentModel;

#if UNITY_EDITOR
using System.Diagnostics;
#endif

namespace VelvieR
{
    public static class VExtension
    {
        public static void SetLeanInit(int defaultVal) { LeanTween.init(defaultVal); }
        public static void CancelAllTweens() => LeanTween.cancelAll();
        public static void SetImageAlpha(float setTo, GameObject img = null, Image imgCom = null)
        {
            Image image = null;

            if (img != null)
            {
                image = img.GetComponent<Image>();
            }
            else if (imgCom != null)
            {
                image = imgCom;
            }

            var tempColor = image.color;
            tempColor.a = setTo;
            image.color = tempColor;
        }
        public static void SetRect(GameObject rect, Vector2 sizeDelta, Vector2 anchorPos, bool setAnchorPosition = false)
        {
            var getRect = rect.GetComponent<RectTransform>();
            getRect.sizeDelta = sizeDelta;

            if (setAnchorPosition)
                getRect.anchoredPosition = anchorPos;
        }
        public static void VTweenUpAndDown(RectTransform rect = null, float to = 0f, float duration = 0.5f, int loopCount = 0, bool setOnComplete = false, Action act = null, LeanTweenType loopType = LeanTweenType.notUsed, LeanTweenType ease = LeanTweenType.notUsed, bool isLocal = false)
        {
            if (rect == null)
                return;

            if (!isLocal)
            {
                var defPos = rect.anchoredPosition;

                LeanTween.moveY(rect, to, duration).setLoopType(loopType).setLoopCount(loopCount).setEase(ease).setOnComplete(() =>
                {
                    rect.anchoredPosition = defPos;

                    if (setOnComplete && act != null)
                    {
                        act.Invoke();
                    }
                });

            }
            else
            {

                VExtension.CheckTweensToCancel(rect.gameObject);
                var dePos = rect.gameObject.transform.localPosition;

                LeanTween.moveLocalY(rect.gameObject, to, duration).setLoopType(loopType).setLoopCount(loopCount).setEase(ease).setOnComplete(() =>
                {
                    rect.gameObject.transform.localPosition = dePos;

                    if (setOnComplete && act != null)
                    {
                        act.Invoke();
                    }
                });
            }
        }
        public static void VTweenLeftAndRight(RectTransform rect = null, float to = 0f, float duration = 0.5f, int loopCount = 0, bool setOnComplete = false, Action act = null, LeanTweenType loopType = LeanTweenType.pingPong, LeanTweenType ease = LeanTweenType.notUsed, bool isLocal = false)
        {
            if (rect == null)
                return;

            if (rect != null)
            {
                VExtension.CheckTweensToCancel(rect.gameObject);

                if (!isLocal)
                {
                    var defPos = rect.anchoredPosition;

                    LeanTween.moveX(rect, to, duration).setLoopType(loopType).setLoopCount(loopCount).setEase(ease).setOnComplete(() =>
                    {
                        rect.anchoredPosition = defPos;

                        if (setOnComplete && act != null)
                        {
                            act.Invoke();
                        }
                    });
                }
                else
                {
                    var defPos = rect.gameObject.transform.localPosition;

                    LeanTween.moveLocalX(rect.gameObject, to, duration).setLoopType(loopType).setLoopCount(loopCount).setEase(ease).setOnComplete(() =>
                    {
                        rect.gameObject.transform.localPosition = defPos;

                        if (setOnComplete && act != null)
                        {
                            act.Invoke();
                        }
                    });
                }
            }
        }
        public static void VTweenShake(GameObject obj = null, float to = 0f, float duration = 0.5f, int loopCount = 0, bool setOnComplete = false, Action act = null, LeanTweenType loopType = LeanTweenType.pingPong, LeanTweenType ease = LeanTweenType.notUsed)
        {
            if (obj == null)
                return;
            VExtension.CheckTweensToCancel(obj);

            LeanTween.moveX(obj, obj.transform.position.x + to, duration).setLoopType(loopType).setLoopCount(loopCount).setEase(ease).setOnComplete(() =>
            {
                if (setOnComplete && act != null)
                {
                    act.Invoke();
                }
            });
        }
        public static void VTweenScale(GameObject obj, Vector3 to, float duration = 0.5f, int loopCount = 0, bool setOnComplete = false, Action act = null, LeanTweenType loopType = LeanTweenType.pingPong, LeanTweenType ease = LeanTweenType.notUsed)
        {
            if (obj == null)
                return;

            if (LeanTween.isTweening(obj))
            {
                LeanTween.cancel(obj, true);
                obj.transform.localScale = Vector3.one;
            }

            LeanTween.scale(obj, to, duration).setLoopType(loopType).setLoopCount(loopCount).setEase(ease).setOnComplete(() =>
            {
                obj.transform.localScale = Vector3.one;

                if (setOnComplete && act != null)
                {
                    act.Invoke();
                }
            });

        }
        public static void VTweenRotateAround(GameObject obj, Vector3 to, float addAngle, float duration = 0.5f, int loopCount = 0, bool isLocal = false, bool setOnComplete = false, Action act = null, LeanTweenType loopType = LeanTweenType.notUsed, LeanTweenType ease = LeanTweenType.notUsed)
        {
            if (obj == null)
                return;

            //Reset the rotation angle. so custom thumbnail rotation in the hierarchy will mostly ignored. Notes to those reading this
            obj.transform.rotation = Quaternion.identity;

            if(!isLocal)
            {
                LeanTween.rotateAround(obj, to, addAngle, duration).setLoopType(loopType).setLoopCount(loopCount).setEase(LeanTweenType.punch).setOnComplete(() =>
                {
                    if (setOnComplete && act != null)
                    {
                        act.Invoke();
                    }
                });
            }
            else
            {
                LeanTween.rotateAroundLocal(obj, to, addAngle, duration).setLoopType(loopType).setLoopCount(loopCount).setEase(LeanTweenType.punch).setOnComplete(() =>
                {
                    if (setOnComplete && act != null)
                    {
                        act.Invoke();
                    }
                });
            }

        }
        public static void VTweenValueFloat(GameObject obj, float from, float to, Action<float> paramAct, float duration = 1f, int loopCount = 0, bool setOnComplete = false, Action act = null, LeanTweenType loopType = LeanTweenType.notUsed)
        {
            if (obj == null)
                return;

            VExtension.CheckTweensToCancel(obj);
            LeanTween.value(obj, (val) =>
            {
                paramAct.Invoke(val);

            }, from, to, duration).setLoopType(loopType).setLoopCount(loopCount).setOnComplete(() =>
            {
                if (setOnComplete && act != null)
                {
                    act.Invoke();
                }
            });
        }
        public static void VTweenAlpha(RectTransform rect, float from, float to, float duration = 1f, int loopCount = 0, bool setOnComplete = false, Action act = null, LeanTweenType loopType = LeanTweenType.notUsed)
        {
            if (rect == null)
                return;

            if (LeanTween.isTweening(rect))
            {
                LeanTween.cancel(rect.gameObject, true);
                SetImageAlpha(from, rect.gameObject);
            }

            LeanTween.alpha(rect, to, duration).setLoopType(loopType).setLoopCount(loopCount).setOnComplete(() =>
            {
                SetImageAlpha(from, rect.gameObject);

                if (setOnComplete && act != null)
                {
                    act.Invoke();
                }
            });
        }
        public static void VAnimateSprites(RectTransform rect, Sprite[] sprites, float frameRate, float lowerBoundsMultiplier = 2f, float upperBoundsMultiplier = 3f, int loopCount = -1, LeanTweenType loopType = LeanTweenType.notUsed, bool setonrepeat = true, bool randomizeFramerate = true)
        {
            if (rect == null || sprites == null || sprites.Length == 0)
                return;

            VExtension.CheckTweensToCancel(rect.gameObject);
            int dscr = 0;

            dscr = LeanTween.play(rect, sprites).setFrameRate(frameRate).setLoopType(loopType).setLoopCount(loopCount).setOnCompleteOnRepeat(setonrepeat).setOnComplete(() =>
            {
                if (randomizeFramerate)
                    RandomSpeed(dscr, frameRate, true);
            }).id;
        }
        private static void RandomSpeed(int dscr, float minRandom, bool randomizedLoopType)
        {
            var descr = LeanTween.descr(dscr);
            int randoms = (int)UnityEngine.Random.Range(3, 5);
            float ranMultiplier = UnityEngine.Random.Range(minRandom + randoms, minRandom * randoms);
            descr.setFrameRate(ranMultiplier);

            if (randoms % 2 == 0)
            {
                descr.setLoopType(LeanTweenType.clamp);
            }
            else
            {
                descr.setLoopType(LeanTweenType.pingPong);
            }
        }

        public static void VTweenMove(GameObject obj, Vector3 to, float duration, int loopCount = 0, bool setOnComplete = false, Action act = null, LeanTweenType loopType = LeanTweenType.notUsed, LeanTweenType ease = LeanTweenType.notUsed, bool isLocal = false)
        {
            if (obj == null)
                return;

            VExtension.CheckTweensToCancel(obj);

            if (!isLocal)
            {
                var defPos = obj.transform.position;

                LeanTween.move(obj, to, duration).setLoopType(loopType).setLoopCount(loopCount).setEase(ease).setOnComplete(() =>
                {
                    obj.transform.position = defPos;

                    if (setOnComplete && act != null)
                    {
                        act.Invoke();
                    }
                });
            }
            else
            {
                var defPos = obj.transform.localPosition;

                LeanTween.moveLocal(obj, to, duration).setLoopType(loopType).setLoopCount(loopCount).setEase(ease).setOnComplete(() =>
                {
                    obj.transform.localPosition = defPos;

                    if (setOnComplete && act != null)
                    {
                        act.Invoke();
                    }
                });
            }
        }
        public static void VTweenAlphaCanvas(CanvasGroup canvy, float from, float to, float duration, bool setOnComplete = false, Action act = null, LeanTweenType ease = LeanTweenType.notUsed)
        {
            if (canvy == null)
                return;

            VExtension.CheckTweensToCancel(canvy.gameObject);
            canvy.alpha = from;

            LeanTween.alphaCanvas(canvy, to, duration).setEase(ease).setOnComplete(() =>
            {
                canvy.alpha = to;

                if (setOnComplete && act != null)
                {
                    act.Invoke();
                }
            });
        }
        //Mostly for editor tooling
        public static void SetVar(IVar setThisVar, string val)
        {
            if (setThisVar is VInteger vvint)
            {
                int t = 0;
                int.TryParse(val, out t);
                vvint.value = t;
            }
            else if (setThisVar is VDouble vvdob)
            {
                double t = 0;
                double.TryParse(val, out t);
                vvdob.value = t;
            }
            else if (setThisVar is VString vvstr)
            {
                vvstr.value = val;
            }
            else if (setThisVar is VFloat vvflo)
            {
                float t = 0;
                float.TryParse(val, out t);
                vvflo.value = t;
            }
            else if (setThisVar is VBoolean vvbool)
            {
                bool t = false;
                Boolean.TryParse(val, out t);
                vvbool.value = t;
            }
        }
        public static void SetVarVectors(IVar setThisVar, Vector4 vec)
        {
            if (setThisVar is VVector2 vvec2)
            {
                vvec2.value = new Vector2(vec.x, vec.y);
            }
            else if (setThisVar is VVector3 vvec3)
            {
                vvec3.value = new Vector3(vec.x, vec.y, vec.z);
            }
            else if (setThisVar is VVector4 vvec4)
            {
                vvec4.value = new Vector4(vec.x, vec.y, vec.z, vec.w);
            }
        }
        public static void CheckTweensToCancel(GameObject obj, bool executeOnComplete = true)
        {
            if(obj == null)
                return;

            if (LeanTween.isTweening(obj))
            {
                LeanTween.cancel(obj, executeOnComplete);
            }
        }
        public static void PauseAllTweens(bool state = true)
        {
            if(state)
            LeanTween.pauseAll();
            else
            LeanTween.resumeAll();
        }
    }
}
