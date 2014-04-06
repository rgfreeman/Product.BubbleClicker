using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	////fields
		public float opt_timeInTick           = 1.0f;  //1 tick = 1 second
		public int   opt_ticksInLevel         = 10;    //1 level = 10 ticks
		public float opt_speedFactorIncSteep  = 0.9f;
		public float opt_genIntervalDecSteep  = 0.002f;

	////state
		private int   st_score             = 0;
		private int   st_ticks             = 0;
		private int   st_level             = 0;
		private int   st_ticksToNextLevel;
		private float st_timer;
		
		////link to external
		public GameObject ext_circles;
		public GameObject ext_guiScore;
		public GameObject ext_guiTime;
		public GameObject ext_assetManager;


	////methods
		void Start () {
			st_timer = opt_timeInTick; //reset timer
			st_ticksToNextLevel = opt_ticksInLevel;	//reset level ticks
			ext_guiScore.GetComponent<GUIText>().text = "Score:0";
			ext_guiTime.GetComponent<GUIText>().text  = "Time:0";

			////enable spawn circles
			ext_circles.GetComponent<CirclesController>().opt_active = true;
		}

		public int getLevel(){
			return st_level;
		}

		public void incScore(int score){
			st_score+=score;
			ext_guiScore.GetComponent<GUIText>().text = "Score:" + st_score.ToString();
		}

		public void incTicks(int tickCount){
			st_ticks+=tickCount;
			ext_guiTime.GetComponent<GUIText>().text = "Time:" + st_ticks.ToString();

			////it's time to level up?
			if((st_ticksToNextLevel-=tickCount)<=0){
				st_level++;
				////update textures
				ext_assetManager.GetComponent<AssetManager>().updateTextures(st_level);
				st_ticksToNextLevel = opt_ticksInLevel;
			}

			////increase speed factor for circles
			ext_circles.GetComponent<CirclesController>().opt_speedFactor += opt_speedFactorIncSteep;
			
			////decrease spawn interval for circles
			ext_circles.GetComponent<CirclesController>().opt_genInterval -= opt_genIntervalDecSteep;
		}

		// Update is called once per frame
		void Update () {
			if((st_timer -= Time.deltaTime) < 0){
				incTicks(1);
				st_timer = opt_timeInTick;
			}
		}
}
