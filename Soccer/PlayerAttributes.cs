using UnityEngine;

[System.Serializable]
public class PlayerAttributes
{

    //public Sprite image;
    public string m_LogoAddress;
    public string playerName;
    public float touchPower;
    public float shotPower;
    public float passPower;
    public float clearPower;
    public float shotDistance;
    public float shotAccuracy = 0.5f;

    public float runSpeed;
    public float originalRunSpeed;
    public float dribbleSpeed;
    public float spacingDistance = 20.0f;
    public float defenderDistance = 15.0f;
    public float strikerDistance = 15.0f;

    public float passDetectRange;
    public enum FieldPosition{
        Striker,
        LeftWing,
        RightWing,
        CenterMid,
        Defense,
        Goalie
    }

    public FieldPosition fieldPosition;

    public string occupation;
    public int age;
    public float sizeScale;
    public string greetingMessage;
    public int recruitCost;
    public bool isOnTeam;
}
