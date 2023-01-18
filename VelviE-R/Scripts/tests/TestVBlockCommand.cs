using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestVBlockCommand : VBlockCore
{
    public override void OnVEnter()
    {
        Debug.Log("ENTERING!");
    }
    public override void OnVExit()
    {
        Debug.Log("EXITING!");
    }
    public override string OnVSummary()
    {
        var summary = "Summary";
        return summary;
    }
}
