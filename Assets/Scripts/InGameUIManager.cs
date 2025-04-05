using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUIManager : MonoBehaviour
{
    public static InGameUIManager Instance;
    
    // 버튼들을 캐싱하여 InGameUIManager가 쉽게 접근토록해주었음.
    [SerializeField]
    private InGameIntroUI inGameIntroUI;
    public InGameIntroUI InGameIntroUI{ get { return inGameIntroUI; } }

    [SerializeField]
    private KillButtonUI killButtonUI;
    public KillButtonUI KillButtonUI{ get { return killButtonUI; } }

    [SerializeField]
    private KillUI killUI;
    public KillUI KillUI{ get { return killUI; } }

    [SerializeField]
    private ReportButtonUI reportButtonUI;
    public ReportButtonUI ReportButtonUI{ get { return reportButtonUI; } }

    [SerializeField]
    private ReportUI reportUI;
    public ReportUI ReportUI{ get { return reportUI; } }

    [SerializeField]
    private MeetingUI meetingUI;
    public MeetingUI MeetingUI{ get { return meetingUI; } }

    private void Awake()
    {
        Instance = this;
    }
    
}
