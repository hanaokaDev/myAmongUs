using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUIManager : MonoBehaviour
{
    public static InGameUIManager Instance;
    
    [SerializeField]
    private InGameIntroUI inGameIntroUI;
    public InGameIntroUI InGameIntroUI{ get { return inGameIntroUI; } }

    [SerializeField]
    private KillButtonUI killButtonUI;
    public KillButtonUI KillButtonUI{ get { return killButtonUI; } }

    [SerializeField]
    private KillUI killUI;
    public KillUI KillUI{ get { return killUI; } }

    private void Awake()
    {
        Instance = this;
    }
    
}
