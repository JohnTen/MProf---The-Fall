using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutScript : MonoBehaviour {

    [SerializeField]
    GameObject TutPlant1;
    [SerializeField]
    GameObject TutPlant2;
    [SerializeField]
    GameObject TutPlantMinigame1;
    [SerializeField]
    GameObject TutPlantMinigame2;
    [SerializeField]
    GameObject TutHarvest;
    [SerializeField]
    GameObject TutHarvestMinigame;


    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void OnHowToPlay()
    {
    TutPlant1.SetActive(true);
    TutPlant2.SetActive(false);
    TutPlantMinigame1.SetActive(false);
    TutPlantMinigame2.SetActive(false);
    TutHarvest.SetActive(false);
    TutHarvestMinigame.SetActive(false);
    }

    public void OnNext1()
    {
        TutPlant1.SetActive(false);
        TutPlant2.SetActive(true);
        TutPlantMinigame1.SetActive(false);
        TutPlantMinigame2.SetActive(false);
        TutHarvest.SetActive(false);
        TutHarvestMinigame.SetActive(false);
    }
    public void OnNext2()
    {
        TutPlant1.SetActive(false);
        TutPlant2.SetActive(false);
        TutPlantMinigame1.SetActive(true);
        TutPlantMinigame2.SetActive(false);
        TutHarvest.SetActive(false);
        TutHarvestMinigame.SetActive(false);
    }
    public void OnNext3()
    {
        TutPlant1.SetActive(false);
        TutPlant2.SetActive(false);
        TutPlantMinigame1.SetActive(false);
        TutPlantMinigame2.SetActive(true);
        TutHarvest.SetActive(false);
        TutHarvestMinigame.SetActive(false);
    }
    public void OnNext4()
    {
        TutPlant1.SetActive(false);
        TutPlant2.SetActive(false);
        TutPlantMinigame1.SetActive(false);
        TutPlantMinigame2.SetActive(false);
        TutHarvest.SetActive(true);
        TutHarvestMinigame.SetActive(false);
    }
    public void OnNext5()
    {
        TutPlant1.SetActive(false);
        TutPlant2.SetActive(false);
        TutPlantMinigame1.SetActive(false);
        TutPlantMinigame2.SetActive(false);
        TutHarvest.SetActive(false);
        TutHarvestMinigame.SetActive(true);
    }
    public void OnMenu()
    {
        TutPlant1.SetActive(false);
        TutPlant2.SetActive(false);
        TutPlantMinigame1.SetActive(false);
        TutPlantMinigame2.SetActive(false);
        TutHarvest.SetActive(false);
        TutHarvestMinigame.SetActive(false);
    }
}
