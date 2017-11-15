using GameFW;
using GameFW.Entity;
using GameFW.ID;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseTowerTester : MonoBehaviour {

    DefenseTowerAgent fightAgent;
    int id;
	// Use this for initialization
	void Start () {
        EntityRegister register = GetComponent<EntityRegister>();
        if (register == null)
            register = gameObject.AddComponent<EntityRegister>();

        id = IDCaculater.TransformIdInSceneHierachy(transform);
        //register to aoi
        register.Regist(id);
        register.RegistAOI(id, gameObject);

        fightAgent = gameObject.GetComponent<DefenseTowerAgent>();
        if (fightAgent != null)
        {
            //Data Model setting
            fightAgent.Id = id;
            fightAgent.AtkRange = 20f;
            fightAgent.AtkSpeed = 2f;
            //start ai
            fightAgent.Initial();
            fightAgent.StartAI();
        }

        //Invoke("MakeDeath", 15f);
    }

    void MakeDeath() {
        fightAgent._set_isDead(true);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
