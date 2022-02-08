using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameFlow : MonoBehaviour
{
    public static GameFlow instance { set; get; }
    public bool isDead {set;get;}

    public Transform sea;
    public Transform playerTransform;
    public List<GameObject> Areas;
    public GameObject StartArea;

    private Vector3 nexSeaSpawn;
    private Vector3 nexAreaSpawn;
    private bool isGameStarted=false;

    public Text scoreText, coinText, modifierText;
    private float score, coinScore, modifierScore;

    private const int coinAmount = 5;
    private int lastScore;

    private PlayerController controller;

    public float zDistance = 30;

    //death menu
    public Animator deathMenuAnim;
    public Text deathScoreText, deathCoinText;

    private void Awake()
    {
        instance = this;
        controller = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        modifierScore = 1;
        modifierText.text = "x" + modifierScore.ToString("0.0");
        coinText.text = coinScore.ToString("0");
        scoreText.text = score.ToString("0");
        nexSeaSpawn.y = -1f;
        Instantiate(StartArea, new Vector3(0, 0, 0), StartArea.transform.rotation);
        Instantiate(sea, nexSeaSpawn, sea.rotation);
        nexSeaSpawn.z = 285.6312f;
        GetAreaZPos(StartArea);
    }

    

    void Start()
    {

        StartCoroutine(SpawnSea());

        StartCoroutine(SpawnAreas());
    }

    // Update is called once per frame
    void Update()
    {
        if (MobileInput.Instance.Tap && !isGameStarted)
        {
            isGameStarted = true;
            controller.StartRunning();

        }
        if (isGameStarted && !isDead)
        {
            lastScore = (int)score;
            score += Time.deltaTime * modifierScore;
           if(lastScore!=(int)score) {
                lastScore = (int)score;
                scoreText.text = score.ToString("0");
            }
        }
    }

    IEnumerator SpawnSea()
    {
        for (; ; )
        {
            Instantiate(sea, nexSeaSpawn, sea.rotation);
            nexSeaSpawn.z += 285.6312f;
            GameObject SeaObj = GameObject.FindGameObjectsWithTag("Sea")[1];
            if (SeaObj.transform.position.z < playerTransform.position.z)
                Destroy(SeaObj, 2f);
            
            yield return new WaitUntil(generateNextSea) ;
        }
    }

    private bool generateNextSea()
    {
        return playerTransform.position.z > nexSeaSpawn.z - 300;
    }

    public void GetCoin()
    {
        coinScore++;
        score += coinAmount;
        coinText.text = coinScore.ToString("0");
        scoreText.text = score.ToString("0");
    }

    public void UpdateModifier(float modifierAmount)
    {
        modifierScore = 1.0f + modifierAmount;
        modifierText.text = "x" + modifierScore.ToString("0.0");
    }

    IEnumerator SpawnAreas()
    {
        for(; ; ){

            for(int i = 0; i < 10; i++)
            {
                int j = Random.Range(0, 5);
                Instantiate(Areas[j], nexAreaSpawn, Areas[j].transform.rotation);
                GetAreaZPos(Areas[j]);
            };

            yield return new WaitUntil(generateNextSea);
    }
    }

    Bounds GetChildRendererBounds(GameObject go)
    {
        Renderer[] renderers = go.GetComponentsInChildren<Renderer>();

        if (renderers.Length > 0)
        {
            Bounds bounds = renderers[0].bounds;
            for (int i = 1, ni = renderers.Length; i < ni; i++)
            {
                bounds.Encapsulate(renderers[i].bounds);
            }
            return bounds;
        }
        else
        {
            return new Bounds();
        }
    }

    void GetAreaZPos(GameObject obj)
    {
        Bounds bounds = GetChildRendererBounds(obj);
        nexAreaSpawn.z += bounds.size.z-0.3f;
    }

    public void OnPlayClick()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("RunnerScene");
    }

    public void OnDeath()
    {
        isDead = true;
        deathScoreText.text = score.ToString("0");
        deathCoinText.text = coinScore.ToString("0");
        deathMenuAnim.SetTrigger("Dead");
    }
}
