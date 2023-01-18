using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace VelvieR
{
    [System.Serializable]
    public enum VStageEffects
    {
        Jump,
        Shake,
        Punch,
        None
    }
    [VTag("VProps/PortraitEffects", "Vstage effects.", VColor.White, "Mu")]
    public class PortraitEffects : VBlockCore
    {
        [SerializeField] public VStageUtil vstage;
        [SerializeField] public VStageEffects effects;
        [SerializeField] public float power = 3f;
        [SerializeField] public float duration = 1f;
        [SerializeField] public int loopCount = 0;
        [SerializeField] public bool waitUntilFinished;
        public override void OnVEnter()
        {
            if (vstage != null)
            {
                var obj2d = vstage.gameObject.transform.Find("2DStage").gameObject;
                var obj3d = vstage.gameObject.transform.Find("3DStage").gameObject;

                var vecDef2d = obj2d.transform.position;
                var vecDef3D = obj3d.transform.position;

                if (effects == VStageEffects.Shake)
                {
                    Lean(obj2d, obj3d, obj2d.transform.position, obj3d.transform.position);
                }
                else if (effects == VStageEffects.Jump)
                {
                    LeanY(obj2d, obj3d, obj2d.transform.position, obj3d.transform.position);
                }
                else if (effects == VStageEffects.Punch)
                {
                    LeanTween.scale(obj2d, Vector3.one + new Vector3(power, power, power), duration).setLoopPingPong().setEase(LeanTweenType.punch).setLoopCount(loopCount);
                    LeanTween.scale(obj3d, Vector3.one + new Vector3(power, power, power), duration).setLoopPingPong().setEase(LeanTweenType.punch).setLoopCount(loopCount).setOnComplete(()=> 
                    {
                        obj2d.transform.localScale = Vector3.one;
                        obj3d.transform.localScale = Vector3.one;

                        if(waitUntilFinished)
                        {
                            OnVContinue();
                        }
                    });
                }
            }
        }
        private void Lean(GameObject obj2d, GameObject obj3d, Vector3 defPos2d, Vector3 defPos3d)
        {
            var ltseq = LeanTween.sequence();

            ltseq.append(() =>
            {
                LeanTween.moveX(obj2d, obj2d.transform.position.x + power / 2f, duration);
                LeanTween.moveX(obj3d, obj3d.transform.position.x + power / 2f, duration);
            });

            ltseq.append(() =>
            {
                LeanTween.moveX(obj2d, obj2d.transform.position.x - power, duration);
                LeanTween.moveX(obj3d, obj3d.transform.position.x - power, duration);
            });

            ltseq.append(() =>
            {
                LeanTween.moveX(obj2d, obj2d.transform.position.x, duration);
                LeanTween.moveX(obj3d, obj3d.transform.position.x, duration).setOnComplete(()=> 
                {
                    if(counter != loopCount)
                    {
                        counter++;
                        Lean(obj2d, obj3d, defPos2d, defPos3d);
                    }
                    else
                    {
                        if(waitUntilFinished)
                        {
                            counter = 0;
                            OnVContinue();
                        }
                    }
                });
            });

            ltseq.append(()=>
            {
                obj2d.transform.position = defPos2d;
                obj3d.transform.position = defPos3d;
            });
        }
        private int counter;
        private void LeanY(GameObject obj2d, GameObject obj3d, Vector3 defPos2d, Vector3 defPos3d)
        {
            var ltseq = LeanTween.sequence();

            ltseq.append(() =>
            {
                LeanTween.moveY(obj2d, obj2d.transform.position.y + power / 2f, duration);
                LeanTween.moveY(obj3d, obj3d.transform.position.y + power / 2f, duration);
            });

            ltseq.append(() =>
            {
                LeanTween.moveY(obj2d, obj2d.transform.position.y - power, duration);
                LeanTween.moveY(obj3d, obj3d.transform.position.y - power, duration);
            });

            ltseq.append(() =>
            {
                LeanTween.moveY(obj2d, obj2d.transform.position.y, duration);
                LeanTween.moveY(obj3d, obj3d.transform.position.y, duration).setOnComplete(()=> 
                {
                    if(counter != loopCount)
                    {
                        counter++;
                        LeanY(obj2d, obj3d, defPos2d, defPos3d);
                    }
                    else
                    {
                        if(waitUntilFinished)
                        {
                            counter = 0;
                            OnVContinue();
                        }
                    }
                });
            });

            ltseq.append(()=>
            {
                obj2d.transform.position = defPos2d;
                obj3d.transform.position = defPos3d;
            });
        }
        public override void OnVExit()
        {
            if (!waitUntilFinished)
                OnVContinue();
        }
        public override string OnVSummary()
        {
            var summary = string.Empty;

            if (vstage == null)
            {
                summary += "VStage can't be empty!";
            }

            return summary;
        }
    }
}