using UnityEngine;
using Elympics;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using AIMStudio.Scripts;

public class LeaderboardsDisplayer : MonoBehaviour
{
    private static readonly int RecordsToFetch = 10;

    [SerializeField] private LeaderboardEntryUI[] leaderboardVisualEntries;
    [SerializeField] private int fetchDelayMs = 500;

    [SerializeField] private TextMeshProUGUI timeLeftText;
    [SerializeField] private TextMeshProUGUI playersText;

    [SerializeField] private RectTransform rectTransform;

    private LeaderboardClient leaderboardClient;
    private LeaderboardClient leaderboardClientAllTime;

    [SerializeField] private TextMeshProUGUI bestScoreText;

    private LeaderboardEntry[] storedEntries;

    public event Action<LeaderboardEntry> OnCurrentPlayerEntrySet;
    public bool FetchAlreadyStarted { get; private set; } = false;

    private int playersCount;
    private float allTimeBestScore;

    private void Start()
    {
        ElympicsAuthenticationHandler.instance.OnStatusChanged += ElympicsOnStatusChanged;
        ElympicsAuthenticationHandler.instance.OnStatusCheckEverySecond += FindCurrentPlayerAndHighlight;
        ElympicsOnStatusChanged();
    }

    private void OnDestroy()
    {
        if (ElympicsAuthenticationHandler.instance)
        {
            ElympicsAuthenticationHandler.instance.OnStatusChanged -= ElympicsOnStatusChanged;
            ElympicsAuthenticationHandler.instance.OnStatusCheckEverySecond -= FindCurrentPlayerAndHighlight;
        }
    }

    private void ElympicsOnStatusChanged()
    {
        if (ElympicsAuthenticationHandler.instance.IsWebSocketConnected())
        {
            InitializeAndRun();
        }
    }

    /// <summary>
    /// Init leaderboards
    /// </summary>
    public void InitializeAndRun()
    {
        if (ElympicsLobbyClient.Instance == null)
        {
            Debug.LogWarning("Leaderboards won't work unless you start from the menu scene.");
            return;
        }

        FetchAlreadyStarted = true;
        LoadLeaderboard();
    }

    private void Update()
    {
        if (DateTime.UtcNow > DateTime.UtcNow.Date.AddHours(1)) SetTimeLeft();
        else SetTimeLeftToReset();
    }

    private void SetTimeLeft()
    {
        var timeLeft = DateTime.UtcNow.Date.AddDays(1) - DateTime.UtcNow;
        SetTimeText(timeLeft);
    }

    private void SetTimeLeftToReset()
    {
        var timeLeft = DateTime.UtcNow.Date.AddHours(1) - DateTime.UtcNow;
        SetTimeText(timeLeft);
    }

    private void SetTimeText(TimeSpan timeLeft)
    {
        string FormatTime(int value, string unit) => $"<b>{value:D2}{unit}</b>";

        string hoursText = FormatTime(timeLeft.Hours, "H");
        string minutesText = FormatTime(timeLeft.Minutes, "M");
        string secondsText = FormatTime(timeLeft.Seconds, "S");

        timeLeftText.text =
            timeLeft.Hours > 0 ? $"{hoursText} {minutesText}" :
            timeLeft.Minutes > 0 ? minutesText : secondsText;
    }

    private void SetPlayersText()
    {
        playersText.text = "Players: " + "<b>" + playersCount.ToString() + "</b>";
    }

    /// <summary>
    /// Create leaderboardClient for current day
    /// </summary>
    private void CreateLeaderboardClient()
    {
        Debug.Log("###Elympics### Create Lobby with Queue " +
                  ElympicsAuthenticationHandler.instance.GetLeaderboardQueue());

        var gameVersion =
            LeaderboardGameVersion.All; // Worth changing to Current if new version contains important balance changes
        var leaderboardType = LeaderboardType.BestResult; // Adjust to the type of game

        var customTimeScopeFrom = DateTime.UtcNow.Date.AddHours(1); // 1:00 am of the current day
        var customTimeScopeTo = DateTime.UtcNow; // current time

        // Convert DateTime to DateTimeOffset correctly
        var customTimeScopeFromOffset = new DateTimeOffset(customTimeScopeFrom, TimeSpan.Zero);
        var customTimeScopeToOffset = new DateTimeOffset(customTimeScopeTo, TimeSpan.Zero);

        var timeScopeObject = new LeaderboardTimeScope(customTimeScopeFromOffset, customTimeScopeToOffset);

        leaderboardClient = new LeaderboardClient(RecordsToFetch, timeScopeObject,
            ElympicsAuthenticationHandler.instance.GetLeaderboardQueue(), gameVersion,
            leaderboardType); //Create leaderboardClient for current day
    }

    /// <summary>
    /// Create leaderboardClient for previous day
    /// </summary>
    private void CreateLeaderboardClientForPreviousTimeScope()
    {
        Debug.Log("###Elympics### Create Lobby with Queue " +
                  ElympicsAuthenticationHandler.instance.GetLeaderboardQueue());

        var gameVersion =
            LeaderboardGameVersion.All; // Worth changing to Current if new version contains important balance changes
        var leaderboardType = LeaderboardType.BestResult; // Adjust to the type of game

        var customTimeScopeFrom = DateTime.UtcNow.Date.AddDays(-1).AddHours(1); // 1:00 am of the previous day
        var customTimeScopeTo = DateTime.UtcNow.Date; // current day midnight time

        // Convert DateTime to DateTimeOffset correctly
        var customTimeScopeFromOffset = new DateTimeOffset(customTimeScopeFrom, TimeSpan.Zero);
        var customTimeScopeToOffset = new DateTimeOffset(customTimeScopeTo, TimeSpan.Zero);

        var timeScopeObject = new LeaderboardTimeScope(customTimeScopeFromOffset, customTimeScopeToOffset);

        leaderboardClient = new LeaderboardClient(RecordsToFetch, timeScopeObject,
            ElympicsAuthenticationHandler.instance.GetLeaderboardQueue(), gameVersion,
            leaderboardType); //Create leaderboardClient for previous day
    }

    /// <summary>
    /// Create leaderboardClient for All time 
    /// </summary>
    private void CreateAllTimeBestLeaderboardClient()
    {
        var gameVersion =
            LeaderboardGameVersion.All; // Worth changing to Current if new version contains important balance changes
        var leaderboardType = LeaderboardType.BestResult; // Adjust to the type of game
        leaderboardClientAllTime = new LeaderboardClient(10, new LeaderboardTimeScope(LeaderboardTimeScopeType.AllTime),
            ElympicsAuthenticationHandler.instance.GetLeaderboardQueue(), gameVersion,
            leaderboardType); //Create leaderboardClient for All time 
    }

    /// <summary>
    /// Create and Fetch Leaderboards
    /// </summary>
    private void LoadLeaderboard()
    {
        storedEntries = new LeaderboardEntry[leaderboardVisualEntries.Length];
        HideAll();

        if (DateTime.UtcNow > DateTime.UtcNow.Date.AddHours(1))
            CreateLeaderboardClient(); //Create leaderboardClient for current day if current time is past 1:00 am of the current day
        else CreateLeaderboardClientForPreviousTimeScope(); //Else create leaderboardClient for previous day

        CreateAllTimeBestLeaderboardClient(); //Create leaderboardClient for All Time BestResults of the current user

        if (ElympicsLobbyClient.Instance.IsAuthenticated)
            FetchTopEntries(); //Fetch Leaderboard 
        else
            ElympicsLobbyClient.Instance.AuthenticationSucceeded += FetchTopEntries;
    }

    private void FetchTopEntries(Elympics.Models.Authentication.AuthData _ = null) =>
        leaderboardClient.FetchFirstPage(HandleFirstPageFetch, OnTopEntriesFailure);

    private void FetchUserEntryAllTime(Elympics.Models.Authentication.AuthData _ = null) =>
        leaderboardClientAllTime.FetchPageWithUser(HandleFirstPageFetchAllTime, OnFailure);

    /// <summary>
    /// Adjust Best Score Text to the fetched leaderboard
    /// </summary>
    private void HandleFirstPageFetchAllTime(LeaderboardFetchResult fetchResult)
    {
        int playerIndexInResult = FindCurrentPlayerInEntries(fetchResult.Entries);

        if (playerIndexInResult != -1)
        {
            allTimeBestScore = fetchResult.Entries[playerIndexInResult].Score;
            ShowBestScore((int)allTimeBestScore);
        }
        else HideBestScore();

        Debug.Log("###Elympics### allTimeBestScore = " + allTimeBestScore);

        FetchAlreadyStarted = false; //Reset flag after succesfully fetched all the leaderboards 
    }

    /// <summary>
    /// Adjust UI to the first page of fetched top 10 leaderboard 
    /// </summary>
    private void HandleFirstPageFetch(LeaderboardFetchResult fetchResult)
    {
        playersCount = fetchResult.TotalRecords;
        SetPlayersText();

        StoreEntries(fetchResult.Entries);

        DisplayEntries();

        FindCurrentPlayerAndHighlight();

        FetchUserEntryAllTime(); //After succesfully fetched First Page Leaderboard now Fetch All Time Best Results of the current user
    }

    private void OnTopEntriesFailure(LeaderboardFetchError fetchError)
    {
        FetchUserEntryAllTime();
        Debug.Log("###Elympics### TopEntries LeaderboardFetchError = " + fetchError.ToString());
    }

    private void OnFailure(LeaderboardFetchError fetchError)
    {
        FetchAlreadyStarted = false;
        Debug.Log("###Elympics### BestScore LeaderboardFetchError = " + fetchError.ToString());
    }

    private void StoreEntries(List<LeaderboardEntry> entries)
    {
        entries.CopyTo(storedEntries);
    }

    private void ShowBestScore(int score)
    {
        if (ElympicsAuthenticationHandler.instance.GetPlayQueue() == "training")
            bestScoreText.text = "Best guest Score: " + "<b>" + score.ToString() + "</b>";
        else
            bestScoreText.text = "Best Score: " + "<b>" + score.ToString() + "</b>";
        

        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
    }

    private void HideBestScore()
    {
        bestScoreText.text = "0";
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
    }

    private int FindCurrentPlayerInEntries(List<LeaderboardEntry> entries)
    {
        return entries.FindIndex(entry => ContainsCurrentUser(entry));
    }

    private void FindCurrentPlayerAndHighlight()
    {
        if (storedEntries.Count() == 0 || storedEntries == null)
        {
            return;
        }

        int playerIndexInResult = FindCurrentPlayerInEntries(storedEntries.ToList());

        if (playerIndexInResult != -1)
        {
            leaderboardVisualEntries[playerIndexInResult].HighlightEntry(); // Hilight current user
        }
    }

    private static bool ContainsCurrentUser(LeaderboardEntry entry)
    {
        var playerId = ElympicsLobbyClient.Instance.UserGuid.ToString();

        return entry != null && entry.UserId.Equals(playerId);
    }

    private void DisplayEntries()
    {
        for (int i = 0; i < leaderboardVisualEntries.Length; i++)
        {
            if (storedEntries[i] == null) leaderboardVisualEntries[i].gameObject.SetActive(false);
            else leaderboardVisualEntries[i].SetValues(storedEntries[i]);
        }
    }

    private void HideAll()
    {
        for (int i = 0; i < leaderboardVisualEntries.Length; i++)
        {
            leaderboardVisualEntries[i].gameObject.SetActive(false);
        }
    }
}