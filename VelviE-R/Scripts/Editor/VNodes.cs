using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using VelvieR;

namespace VIEditor
{
    public class VNodes : Node
    {
        public VNodes()
        {
            this.focusable = true;
        }
        public EnableState IsGameStarted = EnableState.None;
        public string nodeTitle { get; set; } = "Untitled";
        public string VNodeId{get;set;}
        public string VText = string.Empty;
        public Port inputPort{get;set;}
        public Port outputPort{get;set;}
        public override void OnSelected()
        {
            PortsUtils.activeVNode = this;
            this.BringToFront();            
            var e = DisplayPopupWindow(nodeTitle);

            PortsUtils.VGraph.InspectorWindow(e);
            PortsUtils.VGraph.AddVBlockLabel("SomeVBlock", VelvieR.VColor.Gray, string.Empty);
        }

        public override void OnUnselected()
        {
            if(!PortsUtils.PlayMode)
            {
                if(PortsUtils.activeVNode == this)
                    PortsUtils.activeVNode = null;

                base.OnUnselected();
            }
        }

        //Use this as a base for all VNodes
        public virtual Box DisplayPopupWindow(string vnodeTitle)
        {
            return new Box();
        }
    }
}