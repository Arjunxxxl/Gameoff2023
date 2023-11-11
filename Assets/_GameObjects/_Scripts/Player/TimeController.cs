using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    [Header("Time Scale")]
    [SerializeField] private float currentTimeScale;
    [SerializeField] private float normalTimeScale;
    [SerializeField] private float stopingTimeScale;
    [SerializeField] private float fixedDeltaTimeDefaultValue;
    [SerializeField] private float timeScaleChangeSpeed;

    private Player player;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        ChangeTimeScale();
    }

    public void SetUp(Player player)
    {
        this.player = player;

        currentTimeScale = normalTimeScale;
        Time.timeScale = currentTimeScale;
        Time.fixedDeltaTime = Time.timeScale * fixedDeltaTimeDefaultValue;
    }

    private void ChangeTimeScale()
    {
        currentTimeScale = Mathf.Lerp(currentTimeScale, player.userInput.TimeScaleInput ? stopingTimeScale : normalTimeScale, 1f - Mathf.Pow(0.5f, Time.unscaledDeltaTime * timeScaleChangeSpeed));
        
        if(player.userInput.TimeScaleInput)
        {
            if(Mathf.Abs(currentTimeScale - stopingTimeScale) < 0.01f)
            {
                currentTimeScale = stopingTimeScale;
            }
        }
        else
        {
            if (Mathf.Abs(currentTimeScale - normalTimeScale) < 0.01f)
            {
                currentTimeScale = normalTimeScale;
            }
        }
        
        Time.timeScale = currentTimeScale;
        Time.fixedDeltaTime = Time.timeScale * fixedDeltaTimeDefaultValue;
    }
}
