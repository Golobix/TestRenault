using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{


    [SerializeField]
    private int NbrKnights = 3;
    [SerializeField]
    private int NbrSamurais = 7;

    [SerializeField]
    private GameObject Knight;
    [SerializeField]
    private GameObject Samurai;

    [SerializeField]
    private Transform KnightSpawn;
    [SerializeField]
    private Transform SamuraiSpawn;

    [SerializeField]
    private int ScoreKnight = 0;
    [SerializeField]
    private int ScoreSamurai = 0;

    private bool IsRunning;

    public List<FighterGeneric> TeamKnight = new List<FighterGeneric>();
    public List<FighterGeneric> TeamSamurai = new List<FighterGeneric>();

    public FlagBehaviour KnightFlag;
    public FlagBehaviour SamuraiFlag;

    public RectTransform ConfigPnl;

    public InputField SamuraiNumber;
    public InputField SamuraiSpeed;
    public InputField SamuraiDetectionRadius;
    public InputField KnightNumber;
    public InputField KnightSpeed;
    public InputField KnightDetectionRadius;

    public Button StartButton;
    public Text Score;

    // Use this for initialization
    void Start ()
    {
        SamuraiNumber.placeholder.GetComponent<Text>().text = NbrSamurais.ToString();
        KnightNumber.placeholder.GetComponent<Text>().text = NbrKnights.ToString();
        SamuraiSpeed.placeholder.GetComponent<Text>().text = Samurai.GetComponent<FighterGeneric>().Speed.ToString();
        KnightSpeed.placeholder.GetComponent<Text>().text = Knight.GetComponent<FighterGeneric>().Speed.ToString();
        SamuraiDetectionRadius.placeholder.GetComponent<Text>().text = Samurai.GetComponent<FighterGeneric>().DetectionRadius.ToString();
        KnightDetectionRadius.placeholder.GetComponent<Text>().text = Knight.GetComponent<FighterGeneric>().DetectionRadius.ToString();

        StartButton.onClick.AddListener(InitGame);     
    }
	
	// Update is called once per frame
	void Update ()
    {

	}

    private void InitGame()
    {
        InitVariables();

        KnightFlag.SetTeam("Knight");
        SamuraiFlag.SetTeam("Samurai");

        for (int i = 0; i < NbrKnights; i++)
        {
            GameObject knight = GameObject.Instantiate(Knight, KnightSpawn.position + new Vector3(0, Random.Range(12, -12), 0), KnightSpawn.rotation);
            knight.gameObject.SetActive(true);
            knight.GetComponent<FighterGeneric>().InitFighter(this, KnightSpawn, KnightFlag, SamuraiFlag);
            TeamKnight.Add(knight.GetComponent<FighterGeneric>());
        }

        for (int i = 0; i < NbrSamurais; i++)
        {
            GameObject samurai = GameObject.Instantiate(Samurai, SamuraiSpawn.position + new Vector3(0, Random.Range(12, -12), 0), SamuraiSpawn.rotation);
            samurai.gameObject.SetActive(true);
            samurai.GetComponent<FighterGeneric>().InitFighter(this, SamuraiSpawn, SamuraiFlag, KnightFlag);
            TeamSamurai.Add(samurai.GetComponent<FighterGeneric>());
        }

        IsRunning = true;
        StartButton.gameObject.SetActive(false);
        ConfigPnl.gameObject.SetActive(false);
    }

    private void InitVariables()
    {
        if(!SamuraiNumber.text.Equals(""))
        {
            NbrSamurais = int.Parse(SamuraiNumber.text);
        }

        if (!KnightNumber.text.Equals(""))
        {
            NbrKnights = int.Parse(KnightNumber.text);
        }

        if (!SamuraiSpeed.text.Equals(""))
        {
            Samurai.GetComponent<FighterGeneric>().Speed = int.Parse(SamuraiSpeed.text);
        }

        if (!KnightSpeed.text.Equals(""))
        {
            Knight.GetComponent<FighterGeneric>().Speed = int.Parse(KnightSpeed.text);
        }

        if (!SamuraiDetectionRadius.text.Equals(""))
        {
            Samurai.GetComponent<FighterGeneric>().DetectionRadius = int.Parse(SamuraiDetectionRadius.text);
        }

        if (!KnightDetectionRadius.text.Equals(""))
        {
            Knight.GetComponent<FighterGeneric>().DetectionRadius = int.Parse(KnightDetectionRadius.text);
        }
    }

    private void ResetGame()
    {
        
        KnightFlag.InitFlag();
        SamuraiFlag.InitFlag();
        
        foreach(FighterGeneric fighter in TeamKnight)
        {
            fighter.transform.position = new Vector3(0, Random.Range(12, -12), 0) + KnightSpawn.position;
            fighter.gameObject.SetActive(true);
            fighter.Idle();
            fighter.GetComponent<FighterGeneric>().InitFighter(this, KnightSpawn, KnightFlag, SamuraiFlag);
            fighter.Go();
        }
        
        foreach (FighterGeneric fighter in TeamSamurai)
        {
            fighter.transform.position = new Vector3(0, Random.Range(12, -12), 0) + SamuraiSpawn.position;
            fighter.gameObject.SetActive(true);
            fighter.Idle();
            fighter.GetComponent<FighterGeneric>().InitFighter(this, SamuraiSpawn, SamuraiFlag, KnightFlag);
            fighter.Go();
        }

        IsRunning = true;

        StartButton.gameObject.SetActive(false);
    }

    public void GameDone(FlagBehaviour CapturedFlag)
    {
        IsRunning = false;

        List<FighterGeneric> AllFighters = new List<FighterGeneric>();
        AllFighters.AddRange(TeamKnight);
        AllFighters.AddRange(TeamSamurai);

        foreach(FighterGeneric fighter in AllFighters)
        {
            fighter.Idle();
        }

        if(CapturedFlag.isKnightFlag())
        {
            ScoreSamurai++;
        }
        else
        {
            ScoreKnight++;
        }
        Score.text = "[Score] Chevalier: "+ ScoreKnight + " Samurai: "+ ScoreSamurai;

        StartButton.GetComponentInChildren<Text>().text = "Reset Game";
        StartButton.onClick.RemoveListener(InitGame);
        StartButton.onClick.AddListener(ResetGame);

        if(ScoreSamurai == 3 || ScoreKnight == 3)
        {
            Debug.Log("Partie terminée");
            if(ScoreSamurai > ScoreKnight)
            {
                Score.text = "Samurai Team Wins";
            }
            else
            {
                Score.text = "Knight Team Wins";
            }
        }
        else
        {
            StartButton.gameObject.SetActive(true);
        }

    }

    public void Resurect(string team, int sec)
    {
        if(team == "Knight")
        {
            StartCoroutine(RespawFighter(TeamKnight, sec));
        }

        if(team == "Samurai")
        {
            StartCoroutine(RespawFighter(TeamSamurai, sec));
        }
    }

    IEnumerator RespawFighter(List<FighterGeneric> team, int sec)
    {
        bool res = false;
        int i = 0;
        yield return new WaitForSeconds(sec);

        while(res == false)
        {
            if (!team[i].isAlive)
            {
                team[i].gameObject.SetActive(true);

                if (IsRunning)
                {
                    team[i].Go();
                }
                else
                {
                    team[i].Idle();
                }
                team[i].ResetFighter();
                team[i].transform.position = new Vector3(0, Random.Range(12, -12), 0) + team[i].SpawnZone.position;       
                 
                res = true;
            }
            else
            {
                if(i == team.Count-1)
                {
                    res = true;
                }

                i++;
            }
        }

    }
}
