using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System.Text;

public class GameRuleStore : NetworkBehaviour
{
    /****************************************************/
    [SyncVar(hook = nameof(SetIsRecommendRule_Hook))]
    private bool isRecommendRule;
    [SerializeField]
    private Toggle isRecommendRuleToggle;
    public void SetIsRecommendRule_Hook(bool _, bool value)
    { 
        UpdateGameRuleOverview();
    }
    public void OnRecommendToggle(bool value) // 콜백함수
    {
        isRecommendRule = value;
        if(isRecommendRule)
        {
            SetRecommendGameRule();
        }
    }

    /****************************************************/
    [SyncVar(hook = nameof(SetConfirmEjects_Hook))]
    private bool confirmEjects;
    [SerializeField]
    private Toggle confirmEjectsToggle;
    public void SetConfirmEjects_Hook(bool _, bool value)
    {
        UpdateGameRuleOverview();
    }
    public void OnConfirmEjectsToggle(bool value) // 콜백함수
    {
        isRecommendRule = false;
        isRecommendRuleToggle.isOn = false;
        confirmEjects = value;
    }

    /****************************************************/
    [SyncVar(hook = nameof(SetEmergencyMeetings_Hook))]
    private int emergencyMeetings;
    [SerializeField]
    private Text emergencyMeetingsText;
    public void SetEmergencyMeetings_Hook(int _, int value)
    {
        emergencyMeetingsText.text = value.ToString();
        UpdateGameRuleOverview();
    }
    public void OnChangeEmergencyMeetings(bool isPlus)
    {
        emergencyMeetings = Mathf.Clamp(emergencyMeetings + (isPlus ? 1 : -1), 0, 9); // 1 단위로 0에서 9까지.
        isRecommendRule = false;
        isRecommendRuleToggle.isOn = false;
    }

    /****************************************************/
    [SyncVar(hook = nameof(SetEmergencyMeetingsCooldown_Hook))]
    private int emergencyMeetingsCooldown;
    [SerializeField]
    private Text emergencyMeetingsCooldownText;
    public void SetEmergencyMeetingsCooldown_Hook(int _, int value)
    {
        emergencyMeetingsCooldownText.text = string.Format("{0}s", value);
        UpdateGameRuleOverview();
    }
    public void OnChangeEmergencyMeetingsCooldown(bool isPlus)
    {
        emergencyMeetingsCooldown = Mathf.Clamp(emergencyMeetingsCooldown + (isPlus ? 5 : -5), 0, 60); // 5 단위로 0에서 60까지.
        isRecommendRule = false;
        isRecommendRuleToggle.isOn = false;
    }

    /****************************************************/
    [SyncVar(hook = nameof(SetMeetingsTime_Hook))]
    private int meetingsTime;
    [SerializeField]
    private Text meetingsTimeText;
    public void SetMeetingsTime_Hook(int _, int value)
    {
        meetingsTimeText.text = string.Format("{0}s", value);
        UpdateGameRuleOverview();
    }
    public void OnChangeMeetingsTime(bool isPlus)
    {
        meetingsTime = Mathf.Clamp(meetingsTime + (isPlus ? 5 : -5), 0, 120); // 5 단위로 0에서 120까지.
        isRecommendRule = false;
        isRecommendRuleToggle.isOn = false;
    }

    /****************************************************/
    [SyncVar(hook = nameof(SetVoteTime_Hook))]
    private int voteTime;
    [SerializeField]
    private Text voteTimeText;
    public void SetVoteTime_Hook(int _, int value)
    {
        voteTimeText.text = string.Format("{0}s", value);
        UpdateGameRuleOverview();
    }
    public void OnChangeVoteTime(bool isPlus)
    {
        voteTime = Mathf.Clamp(voteTime + (isPlus ? 5 : -5), 0, 300); // 5 단위로 0에서 300까지.
        isRecommendRule = false;
        isRecommendRuleToggle.isOn = false;
    }

    /****************************************************/
    [SyncVar(hook = nameof(SetAnonymousVotes_Hook))]
    private bool anonymousVotes;
    [SerializeField]
    private Toggle anonymousVotesToggle;
    public void SetAnonymousVotes_Hook(bool _, bool value)
    {
        UpdateGameRuleOverview();
    }
    public void OnAnonymousVotesToggle(bool value) // 콜백함수
    {
        isRecommendRule = false;
        isRecommendRuleToggle.isOn = false;
        anonymousVotes = value;
    }

    /****************************************************/
    [SyncVar(hook = nameof(SetMoveSpeed_Hook))]
    private float moveSpeed;
    [SerializeField]
    private Text moveSpeedText;
    public void SetMoveSpeed_Hook(float _, float value)
    {
        moveSpeedText.text = string.Format("{0:0.0}x", value);
        UpdateGameRuleOverview();
    }
    public void OnChangeMoveSpeed(bool isPlus)
    {
        moveSpeed = Mathf.Clamp(moveSpeed + (isPlus ? 0.25f : -0.25f), 0.5f, 3f); // 0.25 단위로 0.5에서 3까지.
        isRecommendRule = false;
        isRecommendRuleToggle.isOn = false;
    }

    /****************************************************/
    [SyncVar(hook = nameof(SetCrewSight_Hook))]
    private float crewSight;
    [SerializeField]
    private Text crewSightText;
    public void SetCrewSight_Hook(float _, float value)
    {
        crewSightText.text = string.Format("{0:0.0}x", value);
        UpdateGameRuleOverview();
    }
    public void OnChangeCrewSight(bool isPlus)
    {
        crewSight = Mathf.Clamp(crewSight + (isPlus ? 0.25f : -0.25f), 0.25f, 5f); // 0.25 단위로 0.25에서 5까지.
        isRecommendRule = false;
        isRecommendRuleToggle.isOn = false;
    }

    /****************************************************/
    [SyncVar(hook = nameof(SetImposterSight_Hook))]
    private float imposterSight;
    [SerializeField]
    private Text imposterSightText;
    public void SetImposterSight_Hook(float _, float value)
    {
        imposterSightText.text = string.Format("{0:0.0}x", value);
        UpdateGameRuleOverview();
    }
    public void OnChangeImposterSight(bool isPlus)
    {
        imposterSight = Mathf.Clamp(imposterSight + (isPlus ? 0.25f : -0.25f), 0.25f, 5f); // 0.25 단위로 0.25에서 5까지.
        isRecommendRule = false;
        isRecommendRuleToggle.isOn = false;
    }

    /****************************************************/
    [SyncVar(hook = nameof(SetKillCoolDown_Hook))]
    private float killCoolDown;
    [SerializeField]
    private Text killCoolDownText;
    public void SetKillCoolDown_Hook(float _, float value)
    {
        killCoolDownText.text = value.ToString();
        UpdateGameRuleOverview();
    }
    public void OnChangeKillCoolDown(bool isPlus)
    {
        killCoolDown = Mathf.Clamp(killCoolDown + (isPlus ? 2.5f : -2.5f), 10f, 60f); // 2.5 단위로 10에서 60까지.
        isRecommendRule = false;
        isRecommendRuleToggle.isOn = false;
    }

    /****************************************************/
    [SyncVar(hook = nameof(SetKillRange_Hook))]
    private EKillRange killRange;
    [SerializeField]
    private Text killRangeDropText;
    public void SetKillRange_Hook(EKillRange _, EKillRange value)
    {
        killRangeDropText.text = value.ToString();
        UpdateGameRuleOverview();
    }
    public void OnChangeKillRange(bool isPlus)
    {
        killRange = (EKillRange)Mathf.Clamp((int)killRange + (isPlus ? 1 : -1), 0, 2); // 0에서 2까지.
        isRecommendRule = false;
        isRecommendRuleToggle.isOn = false;
    }

    /****************************************************/
    [SyncVar(hook = nameof(SetVisualTasks_Hook))]
    private bool visualTasks;
    [SerializeField]
    private Toggle visualTasksToggle;
    public void SetVisualTasks_Hook(bool _, bool value)
    {
        UpdateGameRuleOverview();
    }
    public void OnVisualTasksToggle(bool value) // 콜백함수
    {
        isRecommendRule = false;
        isRecommendRuleToggle.isOn = false;
        visualTasks = value;
    }

    /****************************************************/
    [SyncVar(hook = nameof(SetTaskBarUpdates_Hook))]
    private ETaskBarUpdates taskBarUpdates;
    [SerializeField]
    private Text taskBarUpdatesDropText;
    public void SetTaskBarUpdates_Hook(ETaskBarUpdates _, ETaskBarUpdates value)
    {
        taskBarUpdatesDropText.text = value.ToString();
        UpdateGameRuleOverview();
    }
    public void OnChangeTaskBarUpdates(bool isPlus)
    {
        taskBarUpdates = (ETaskBarUpdates)Mathf.Clamp((int)taskBarUpdates + (isPlus ? 1 : -1), 0, 2); // 0에서 2까지.
        isRecommendRule = false;
        isRecommendRuleToggle.isOn = false;
    }

    /****************************************************/
    [SyncVar(hook = nameof(SetCommonTasks_Hook))]
    private int commonTasks;
    [SerializeField]
    private Text commonTasksText;
    public void SetCommonTasks_Hook(int _, int value)
    {
        commonTasksText.text = value.ToString();
        UpdateGameRuleOverview();
    }
    public void OnChangeCommonTasks(bool isPlus)
    {
        commonTasks = Mathf.Clamp(commonTasks + (isPlus ? 1 : -1), 0, 2); // 1 단위로 0에서 2까지.
        isRecommendRule = false;
        isRecommendRuleToggle.isOn = false;
    }

    /****************************************************/
    [SyncVar(hook = nameof(SetComplexTasks_Hook))]
    private int complexTasks;
    [SerializeField]
    private Text complexTasksText;
    public void SetComplexTasks_Hook(int _, int value)
    {
        complexTasksText.text = value.ToString();
        UpdateGameRuleOverview();
    }
    public void OnChangeComplexTasks(bool isPlus)
    {
        complexTasks = Mathf.Clamp(complexTasks + (isPlus ? 1 : -1), 0, 3); // 1 단위로 0에서 2까지.
        isRecommendRule = false;
        isRecommendRuleToggle.isOn = false;
    }

    /****************************************************/
    [SyncVar(hook = nameof(SetSimpleTasks_Hook))]
    private int simpleTasks;
    [SerializeField]
    private Text simpleTasksText;
    public void SetSimpleTasks_Hook(int _, int value)
    {
        simpleTasksText.text = value.ToString();
        UpdateGameRuleOverview();
    }
    public void OnChangeSimpleTasks(bool isPlus)
    {
        simpleTasks = Mathf.Clamp(simpleTasks + (isPlus ? 1 : -1), 0, 5); // 1 단위로 0에서 5까지.
        isRecommendRule = false;
        isRecommendRuleToggle.isOn = false;
    }

    /****************************************************/
    [SyncVar(hook = nameof(SetImposterCount_Hook))]
    private int imposterCount;
    public void SetImposterCount_Hook(int _, int value)
    {
        UpdateGameRuleOverview();
    }


    /****************************************************/
    [SerializeField]
    private Text gameRuleOverview;
    private void UpdateGameRuleOverview()
    {
        var manager = NetworkManager.singleton as AmongUsRoomManager;
        StringBuilder sb = new StringBuilder(isRecommendRule ? "추천 규칙" : "사용자 규칙");
        sb.Append("맵: The Skeld\n");
        sb.Append($"#임포스터: {imposterCount}\n");
        sb.Append(string.Format("Confirm Ejects: {0}\n", confirmEjects ? "켜짐" : "꺼짐"));
        sb.Append(string.Format($"긴급 회의: {emergencyMeetings}\n"));
        sb.Append(string.Format("Anonymous Votes: {0}\n", anonymousVotes ? "켜짐" : "꺼짐"));
        sb.Append($"긴급 회의 쿨타임: {emergencyMeetingsCooldown}\n");
        sb.Append($"회의 시간: {meetingsTime}\n");
        sb.Append($"투표 시간: {voteTime}\n");
        sb.Append($"이동 속도: {moveSpeed}\n");
        sb.Append($"크루원 시야: {crewSight}\n");
        sb.Append($"임포스터 시야: {imposterSight}\n");
        sb.Append($"킬 쿨타임: {killCoolDown}\n");
        sb.Append($"킬 범위: {killRange}\n");
        sb.Append($"Task Bar Updates: {taskBarUpdates}\n");
        sb.Append(string.Format("Visual Tasks: {0}\n", visualTasks ? "켜짐" : "꺼짐"));
        sb.Append($"공통 임무: {commonTasks}\n");
        sb.Append($"복잡한 임무: {complexTasks}\n");
        sb.Append($"간단한 임무: {simpleTasks}\n");
        gameRuleOverview.text = sb.ToString();
    }

    private void SetRecommendGameRule()
    {
        isRecommendRule = true;
        confirmEjects = true;
        emergencyMeetings = 1;
        emergencyMeetingsCooldown = 15;
        meetingsTime = 15;
        voteTime = 120;
        moveSpeed = 1f;
        crewSight = 1f;
        imposterSight = 1.5f;
        killCoolDown = 45f;
        killRange = EKillRange.Normal;
        visualTasks = true;
        commonTasks = 1;
        complexTasks = 1;
        simpleTasks = 2;
    }

    void Start()
    {
        if(isServer)
        {
            var manager = NetworkManager.singleton as AmongUsRoomManager;
            imposterCount = manager.imposterCount;
            anonymousVotes = false;
            taskBarUpdates = ETaskBarUpdates.Always;

            SetRecommendGameRule();
        }
    }

    public GameRuleData GetGameRuleData()
    {
        return new GameRuleData()
        {
            confirmEjects = confirmEjects,
            emergencyMeetings = emergencyMeetings,
            emergencyMeetingsCooldown = emergencyMeetingsCooldown,
            meetingsTime = meetingsTime,
            voteTime = voteTime,
            anonymousVotes = anonymousVotes,
            moveSpeed = moveSpeed,
            crewSight = crewSight,
            imposterSight = imposterSight,
            killCoolDown = killCoolDown,
            killRange = killRange,
            visualTasks = visualTasks,
            taskBarUpdates = taskBarUpdates,
            commonTasks = commonTasks,
            complexTasks = complexTasks,
            simpleTasks = simpleTasks
        };
    }
}


public enum EKillRange
{
    Short,
    Normal,
    Long
}

public enum ETaskBarUpdates
{
    Always,
    Meetings,
    Never
}

public struct GameRuleData
{
    public bool confirmEjects;
    public int emergencyMeetings;
    public int emergencyMeetingsCooldown;
    public float meetingsTime;
    public int voteTime;
    public bool anonymousVotes;
    public float moveSpeed;
    public float crewSight;
    public float imposterSight;
    public float killCoolDown;
    public EKillRange killRange;
    public bool visualTasks;
    public ETaskBarUpdates taskBarUpdates;
    public int commonTasks;
    public int complexTasks;
    public int simpleTasks;
}