using UnityEngine;
using System.Collections;
using System.IO;
using System.Xml;





public class jsonTest : MonoBehaviour {





    public Chracter Player = new Chracter(201,"adepter",50.1f,true, new int[]{0,1,2,3,4,5});



    //JsonData playerData;

    string jasonData;
    


    void Start () {


        

        jasonData = JsonUtility.ToJson(Player);

        


        //XmlDocument doc = (XmlDocument)JsonConvert.DeserializeXmlNode(jasonData);
      
        //File.WriteAllText(Application.dataPath + "/Player.json", jasonData);
        
        //playerData = JsonMapper.ToJson(Player);
        //Debug.Log(playerData);

    }


}


public class Chracter
{
    public int id;
    public string name;
    public float health;
    public bool agg;
    public int[] stats;

    public Chracter(int id, string name, float health, bool agg, int[] stats)
    {

        this.id = id;
        this.name = name;
        this.health = health;
        this.agg = agg;
        this.stats = stats;
    }
}