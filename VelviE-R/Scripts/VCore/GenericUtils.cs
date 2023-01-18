using UnityEngine;
using System;
using System.Collections.Generic;
using TMPro;
using System.Collections;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Events;
using System.Reflection;
using System.Linq;

namespace VelvieR
{
    [System.Serializable]
    public class TriggererCalls
    {
        public string trigId;
        public string thisNodeId;
        public string thatNodeId;
        public string nodeName;
    }
    [System.Serializable]
    public enum VBlockCoreEnterState
    {
        Enter,
        Exit,
        None
    }
    [System.Serializable]
    public class VBindKey
    {
        public GameObject obj;
        public bool isKeyboard = false;
        public bool isGamepad = false;
        public bool isMouse = false;
        public string methodName;
        public string executeVBlock;
        public UnityEvent action = new UnityEvent();
        public string guid;
        public UnityEngine.InputSystem.LowLevel.MouseButton mouseBindKeyCode;
        public KeyCode keyboardBindKeyCode;
        public GamepadButton gamepadBindKeyCode;
    }
    [System.Serializable]
    public class VAudioClass
    {
        public AudioSource audioSource;
        public string name;
        public bool enable = true;
        public float volume;
        public string assetClipPath;
        public string assetClipName;
        public bool loop = false;
        public float pitchLevel;
        public float stereoPanLevel;
    }
    [System.Serializable]
    public class VStageClass
    {
        public string name;
        public GameObject stageObject;
        public Transform stageTransform;
        public string stageHashId;
        public Vector3 stageDefaultPos;
        public Vector3 stageDefaultScale;
        public bool stageIs2d;
        public RectTransform rect;
    }
    [System.Serializable]
    public class CharacterAudio
    {
        public AudioClip audioClip;
        public string audioName;
        public string audioHashId;
        public string charaId;
    }
    [System.Serializable]
    public class CharacterPortrait
    {
        public string name;
        public Image portrait;
        public Image thumbnail;
        public string charaId;
        public Vector3 defaultSize;
        public Vector3 defaultScale;
        public Vector3 defaultPosition;
    }
    [System.Serializable]
    public class VCharacterV
    {
        public string name;
        public string fullName;
        public string description;
        public string profile;
        public bool isClickable = false;
        public List<AudioClip> charaSound = new List<AudioClip>();
        public List<GameObject> charaObject3D = new List<GameObject>();
        public List<PortraitProps> charaPortrait = new List<PortraitProps>();
        public int activePortraitIndex = -1;
        public int activeAudioClipIndex;
        public int activeObject3dIndex;
        public string charaId;
        public bool isVisible = false;
        public bool is2D = true;
        public string lastStagePosition;
        public GameObject root;
        public RectTransform rootRect;
        public int editorIndex;
        public string customTag;
        public float lastStageDuration = 0.2f;
        public bool dimmed = false;
        public VCharacterUtil vcharacterUtil;
    }
    [System.Serializable]
    public class AnimThumbnailProps
    {
        public string name;
        public string description;
        public float delay = 0.1f;
        public bool loopClamp = false;
        public List<Sprite> sprites = new List<Sprite>();
    }
    [System.Serializable]
    public class VCustomFields
    {
        public string fieldName;
        public string content;
        public string customId;
    }
    [System.Serializable]
    public class PortraitProps
    {
        public UnityEngine.UIElements.Image portraitContainer;
        public Sprite portraitSprite;
        public GameObject portraitObj;
        public RectTransform portraitRect;
    }
    [System.Serializable]
    public enum VTextSpeed
    {
        Twenty = 20,
        Forty = 40,
        Sixty = 60,
        Eighty = 80,
        Hundred = 100,
        HundredTwenty = 120,
        HundredForty = 140,
        HundredSixty = 160,
        HundredEighty = 180,
        TwoHundred = 200,
        TwoHundredTwenty = 220,
        TwoHundredForty = 240,
        TwoHundredSixty = 260
    }
    [System.Serializable]
    public enum WaitForClick
    {
        Enable,
        Disable
    }
    [System.Serializable]
    public enum ClickOnDialogue
    {
        ClickOnVDialogPanel,
        ClickAnywhere
    }
    [System.Serializable]
    public enum ShowHideEffect
    {
        FadeInOut,
        ZoomInOut,
        SlideLeftRight,
        SlideRightLeft,
        SlideUp,
        None
    }
    [System.Serializable]
    public enum VDialogueState
    {
        Writing,
        StopWriting,
        Paused,
        WaitForClick
    }
    [System.Serializable]
    public enum WritingIndicator
    {
        Icon,
        Text
    }
    [System.Serializable]
    public enum ContinueAnim
    {
        ThreeDots,
        Icon,
        None
    }

    [System.Serializable]
    public enum DialogType
    {
        TypeWriter,
        GradientFade,
        None
    }
    [System.Serializable]
    public class VBlockComponent
    {
        public string vnodeId;
        public List<VelvieBlockComponent> vblocks = new List<VelvieBlockComponent>();
    }
    [System.Serializable]
    public enum VBlockState
    {
        Entering,
        Exiting,
        Paused,
        Waiting,
        None
    }

    [System.Serializable]
    public enum VAnimSceneState
    {
        Animating,
        Paused,
        Stopped
    }
    [System.Serializable]
    public class AnimationState
    {
        public int animEntityInstanceId;
        public VAnimSceneState sceneAnimState = VAnimSceneState.Stopped;
    }
    [System.Serializable]
    public class AttachedComponent
    {
        public string componentId;
        public Component component;
        public int leftMargin = 0;
        public float oppacity = 0f;
        public bool isIf = false;
        public bool isEndIf = false;
        public bool isWhile = false;
    }
    [System.Serializable]
    public enum EnableState
    {
        None,
        GameStarted,
        Scheduler
    }
    [System.Serializable]
    public class VelvieBlockComponent
    {
        public string guid;
        public string name;
        public Type monoComponent;
        public string componentId;
        public AttachedComponent attachedComponent;
        public string headerValue;
        public string summaryValue;
        public VColor vcolor;
        public string vnodeId;
        public bool isInGameStartedBlock = false;
        public float onEnterDelay = 0;
        public float onExitDelay = 0;
        public bool enable = true;
        public string vnodeName;
    }

    [System.Serializable]
    public class VBoolean : IVar, IGetValue<bool>
    {
        public bool value;
        public string name;
        public int guid;
        public int index;
        public bool ispublic;
        public bool IsPublic{get{return ispublic;} set{ispublic = value;}}
        public string sceneGuid;
        public string SceneGuid{get{return sceneGuid;} set{sceneGuid = value;}}
        public int Vindex { get { return index; } set { index = value; } }
        public string Name { get { return this.name; } set { this.name = value; } }
        public int VarId { get { return this.guid; } set { this.guid = value; } }
        public VTypes GetVtype() { return VTypes.Boolean; }
        public bool GetValue()
        {
            return ((VBoolean)this.GetIVar()).value;
        }
        public void SetValue(bool val)
        {
            this.GetIVar().SetValue(val);
        }
    }
    [System.Serializable]
    public class VInteger : IVar, IGetValue<int>
    {
        public int value = 0;
        public string name;
        public int guid;
        public int index;
        public bool ispublic;
        public bool IsPublic{get{return ispublic;} set{ispublic = value;}}
        public string sceneGuid;
        public string SceneGuid{get{return sceneGuid;} set{sceneGuid = value;}}
        public int Vindex { get { return index; } set { index = value; } }
        public string Name { get { return this.name; } set { this.name = value; } }
        public int VarId { get { return this.guid; } set { this.guid = value; } }
        public VTypes GetVtype() { return VTypes.Integer; }
        public int GetValue()
        {
            return ((VInteger)this.GetIVar()).value;
        }
        public void SetValue(int val)
        {
            this.GetIVar().SetValue(val);
        }

    }
    [System.Serializable]
    public class VFloat : IVar, IGetValue<float>
    {
        public float value = 0f;
        public string name;
        public int guid;
        public int index;
        public bool ispublic;
        public bool IsPublic{get{return ispublic;} set{ispublic = value;}}
        public string sceneGuid;
        public string SceneGuid{get{return sceneGuid;} set{sceneGuid = value;}}
        public int Vindex { get { return index; } set { index = value; } }
        public string Name { get { return this.name; } set { this.name = value; } }
        public int VarId { get { return this.guid; } set { this.guid = value; } }
        public VTypes GetVtype() { return VTypes.Float; }
        public float GetValue()
        {
            return ((VFloat)this.GetIVar()).value;
        }
        public void SetValue(float val)
        {
            this.GetIVar().SetValue(val);
        }
    }
    [System.Serializable]
    public class VDouble : IVar, IGetValue<double>
    {
        public double value = 0;
        public string name;
        public int guid;
        public int index;
        public bool ispublic;
        public bool IsPublic{get{return ispublic;} set{ispublic = value;}}
        public string sceneGuid;
        public string SceneGuid{get{return sceneGuid;} set{sceneGuid = value;}}
        public int Vindex { get { return index; } set { index = value; } }
        public string Name { get { return this.name; } set { this.name = value; } }
        public int VarId { get { return this.guid; } set { this.guid = value; } }
        public VTypes GetVtype() { return VTypes.Double; }
        public double GetValue()
        {
            return ((VDouble)this.GetIVar()).value;
        }
        public void SetValue(double val)
        {
            this.GetIVar().SetValue(val);
        }
    }
    [System.Serializable]
    public class VString : IVar, IGetValue<string>
    {
        public string value;
        public string name;
        public int guid;
        public int index;
        public bool ispublic;
        public bool IsPublic{get{return ispublic;} set{ispublic = value;}}
        public string sceneGuid;
        public string SceneGuid{get{return sceneGuid;} set{sceneGuid = value;}}
        public int Vindex { get { return index; } set { index = value; } }
        public string Name { get { return this.name; } set { this.name = value; } }
        public int VarId { get { return this.guid; } set { this.guid = value; } }
        public VTypes GetVtype() { return VTypes.String; }
        public string GetValue()
        {
            return ((VString)this.GetIVar()).value;
        }
        public void SetValue(string val)
        {
            this.GetIVar().SetValue(val);
        }
    }
    [System.Serializable]
    public class VVector3 : IVar, IGetValue<Vector3>
    {
        public Vector3 value = Vector3.zero;
        public string name;
        public int guid;
        public int index;
        public bool ispublic;
        public bool IsPublic{get{return ispublic;} set{ispublic = value;}}
        public string sceneGuid;
        public string SceneGuid{get{return sceneGuid;} set{sceneGuid = value;}}
        public int Vindex { get { return index; } set { index = value; } }
        public string Name { get { return this.name; } set { this.name = value; } }
        public int VarId { get { return this.guid; } set { this.guid = value; } }
        public VTypes GetVtype() { return VTypes.Vector3; }
        public Vector3 GetValue()
        {
            return ((VVector3)this.GetIVar()).value;
        }
        public void SetValue(Vector3 val)
        {
            this.GetIVar().SetValue(val);
        }
    }
    [System.Serializable]
    public class VVector2 : IVar, IGetValue<Vector2>
    {
        public Vector2 value = Vector2.zero;
        public string name;
        public int guid;
        public int index;
        public bool ispublic;
        public bool IsPublic{get{return ispublic;} set{ispublic = value;}}
        public string sceneGuid;
        public string SceneGuid{get{return sceneGuid;} set{sceneGuid = value;}}
        public int Vindex { get { return index; } set { index = value; } }
        public string Name { get { return this.name; } set { this.name = value; } }
        public int VarId { get { return this.guid; } set { this.guid = value; } }
        public VTypes GetVtype() { return VTypes.Vector2; }
        public Vector2 GetValue()
        {
            return ((VVector2)this.GetIVar()).value;
        }
        public void SetValue(Vector2 val)
        {
            this.GetIVar().SetValue(val);
        }
    }
    [System.Serializable]
    public class VVector4 : IVar, IGetValue<Vector4>
    {
        public Vector4 value = Vector4.zero;
        public string name;
        public int guid;
        public int index;
        public bool ispublic;
        public bool IsPublic{get{return ispublic;} set{ispublic = value;}}
        public string sceneGuid;
        public string SceneGuid{get{return sceneGuid;} set{sceneGuid = value;}}
        public int Vindex { get { return index; } set { index = value; } }
        public string Name { get { return this.name; } set { this.name = value; } }
        public int VarId { get { return this.guid; } set { this.guid = value; } }
        public VTypes GetVtype() { return VTypes.Vector4; }
        public Vector4 GetValue()
        {
            return ((VVector4)this.GetIVar()).value;
        }
        public void SetValue(Vector4 val)
        {
            this.GetIVar().SetValue(val);
        }
    }

    [System.Serializable]
    public class VTransform : IVar, IGetValue<Transform>
    {
        public Transform value = null;
        public string name;
        public int guid;
        public int index;
        public bool ispublic;
        public bool IsPublic{get{return ispublic;} set{ispublic = value;}}
        public string sceneGuid;
        public string SceneGuid{get{return sceneGuid;} set{sceneGuid = value;}}
        public int Vindex { get { return index; } set { index = value; } }
        public string Name { get { return this.name; } set { this.name = value; } }
        public int VarId { get { return this.guid; } set { this.guid = value; } }
        public VTypes GetVtype() { return VTypes.Transform; }
        public Transform GetValue()
        {
            return ((VTransform)this.GetIVar()).value;
        }
        public void SetValue(Transform val)
        {
            this.GetIVar().SetValue(val);
        }
    }
    [System.Serializable]
    public class VGameObject : IVar, IGetValue<GameObject>
    {
        public GameObject value = null;
        public string name;
        public int guid;
        public int index;
        public bool ispublic;
        public bool IsPublic{get{return ispublic;} set{ispublic = value;}}
        public string sceneGuid;
        public string SceneGuid{get{return sceneGuid;} set{sceneGuid = value;}}
        public int Vindex { get { return index; } set { index = value; } }
        public string Name { get { return this.name; } set { this.name = value; } }
        public int VarId { get { return this.guid; } set { this.guid = value; } }
        public VTypes GetVtype() { return VTypes.GameObject; }
        public GameObject GetValue()
        {
            return ((VGameObject)this.GetIVar()).value;
        }
        public void SetValue(GameObject val)
        {
            this.GetIVar().SetValue(val);
        }
    }
    [System.Serializable]
    public class VTmp : IVar, IGetValue<TMP_Text>
    {
        public TMP_Text value = null;
        public string name;
        public int guid;
        public int index;
        public bool ispublic;
        public bool IsPublic{get{return ispublic;} set{ispublic = value;}}
        public string sceneGuid;
        public string SceneGuid{get{return sceneGuid;} set{sceneGuid = value;}}
        public int Vindex { get { return index; } set { index = value; } }
        public string Name { get { return this.name; } set { this.name = value; } }
        public int VarId { get { return this.guid; } set { this.guid = value; } }
        public VTypes GetVtype() { return VTypes.Transform; }
        public TMP_Text GetValue()
        {
            return ((VTmp)this.GetIVar()).value;
        }
        public void SetValue(TMP_Text val)
        {
            this.GetIVar().SetValue(val);
        }
    }
    [System.Serializable]
    public class VList : IVar
    {
        public IList value;
        public string name;
        public VTypes dataType;
        public int guid;
        public int index;
        public bool ispublic;
        public bool IsPublic{get{return ispublic;} set{ispublic = value;}}
        public string sceneGuid;
        public string SceneGuid{get{return sceneGuid;} set{sceneGuid = value;}}
        public VTypes GetVtype() { return VTypes.VList; }
        public string Name { get { return name; } set { name = value; } }
        public int VarId { get { return guid; } set { guid = value; } }
        public int Vindex { get { return index; } set { index = value; } }
    }

    [System.Serializable]
    public enum VTypes
    {
        None,
        String,
        Float,
        Integer,
        Double,
        Boolean,
        Vector2,
        Vector3,
        Vector4,
        Transform,
        GameObject,
        VList
    }

    [System.Serializable]
    public class VVariablePools
    {
        public HashSet<VString> vstring = new HashSet<VString>();
        public HashSet<VInteger> vint = new HashSet<VInteger>();
        public HashSet<VBoolean> vbool = new HashSet<VBoolean>();
        public HashSet<VFloat> vfloat = new HashSet<VFloat>();
        public HashSet<VDouble> vdouble = new HashSet<VDouble>();
        public HashSet<VTransform> vtransform = new HashSet<VTransform>();
        public HashSet<VVector2> vvec2 = new HashSet<VVector2>();
        public HashSet<VVector3> vvec3 = new HashSet<VVector3>();
        public HashSet<VVector4> vvec4 = new HashSet<VVector4>();
        //TODO : Save block and animation/tween states
        public List<VBlockComponent> lastBlockExecuted = new List<VBlockComponent>();
        public List<AnimationState> sceneAnimationState = new List<AnimationState>();
        public List<Transform> trackAllDefaultGOPosition = new List<Transform>();
    }
    [System.Serializable]
    public class GetSetValue<T>
    {
        private T _value;

        public T Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        public static implicit operator T(GetSetValue<T> value)
        {
            return value.Value;
        }

        public static implicit operator GetSetValue<T>(T value)
        {
            return new GetSetValue<T> { Value = value };
        }
    }

    [System.Serializable]
    public class AnyTypes : AnyAbstract
    {
    }

    [System.Serializable]
    public class BackgroundScheduler
    {
        public string name;
        public string guid;
        public double nvalue;
        public double stopValue;
        public string vcore;
        public string vnode;
        public bool unscaledTime = false;
        public bool pause = false;
        public bool isMinutes = false;
        public bool RunOnThreadPool = true;
    }
    [System.Serializable]
    public class VInventoryCategory
    {
        public string name;
        public int guid;
        public string description;
        public GameObject categoryObject;
        public TMP_Text tmp;
        public Sprite sprite;
        public void SetGuid()
        {
            guid = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        }
    }
    [System.Serializable]
    public class VInventoryClass : AVStat<int>
    {
        public VInventoryCategory category;
        public Sprite sprite;
        public GameObject inventoryObject;
        public TMP_Text tmp;
        public void SetGuid()
        {
            guid = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        }
        public VInventoryClass GetVInventory()
        {
            return VPropsManager.vinventory.Find(x => x.name == this.name && x.guid == this.guid);
        }
    }

    [System.Serializable]
    public class StringIntPair
    {
        public string name;
        public int value;
        public string guid;
    }
    [System.Serializable]
    public class VBlendShapes
    {
        public string blendName;
        public float blendValue;
        public float prevValue;
        public bool zeroedPrevValue = false;
        public float duration = 0.5f;
        public int blendShapeindex;
        public float min = 0f;
        public float max = 100f;
    }
}
