using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EPlayerColor
{
    Red, Blue, Green, 
    Pink, Orange, Yellow, 
    Gray, White, Purple,
    Brown, Cyan, Lime

}

public class PlayerColor : MonoBehaviour
{
    private static List<Color> colors = new List<Color>(){
        Color.red, Color.blue, Color.green,
        new Color(1, 0.4f, 0.7f), new Color(1, 0.5f, 0), Color.yellow,
        Color.gray, Color.white, new Color(0.5f, 0, 0.5f),
        new Color(0.5f, 0.3f, 0), Color.cyan, new Color(0, 1, 0)
    };
    public static Color GetColor(EPlayerColor playerColor){
        return colors[(int)playerColor];
    }

    public static Color Red{ get { return colors[(int)EPlayerColor.Red]; } }
    public static Color Blue{ get { return colors[(int)EPlayerColor.Blue]; } }
    public static Color Green{ get { return colors[(int)EPlayerColor.Green]; } }
    public static Color Pink{ get { return colors[(int)EPlayerColor.Pink]; } }
    public static Color Orange{ get { return colors[(int)EPlayerColor.Orange]; } }
    public static Color Yellow{ get { return colors[(int)EPlayerColor.Yellow]; } }
    public static Color Gray{ get { return colors[(int)EPlayerColor.Gray]; } }
    public static Color White{ get { return colors[(int)EPlayerColor.White]; } }
    public static Color Purple{ get { return colors[(int)EPlayerColor.Purple]; } }
    public static Color Brown{ get { return colors[(int)EPlayerColor.Brown]; } }
    public static Color Cyan{ get { return colors[(int)EPlayerColor.Cyan]; } }
    public static Color Lime{ get { return colors[(int)EPlayerColor.Lime]; } }

}
