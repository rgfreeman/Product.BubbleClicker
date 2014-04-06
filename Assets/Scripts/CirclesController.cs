using UnityEngine;
using System.Collections;

public class CirclesController : MonoBehaviour {

	////fields
		////options
		public bool  opt_active = false;     
		public float opt_genInterval    = 0.5f; //in seconds
		public float opt_minGenInterval = 0.2f; //in seconds
		public float opt_speedFactor    = 4.5f;

		////link to external objects
		public GameObject ext_circlePrefub;
		public GameObject ext_game;          //object with game controller bahaviour
		public GameObject ext_assetManager;

		////state
		private float st_timer;

		////computed
		private Rect cmp_screenRect;

	////methods
		void Start () {
			st_timer = opt_genInterval;
		    
			////get screen coordinates in units
			Vector3 screenToUnits = Camera.main.ScreenToWorldPoint( new Vector3(0,0,0) );
			cmp_screenRect.xMin =  screenToUnits.x;
			cmp_screenRect.xMax = -screenToUnits.x;
			cmp_screenRect.yMin =  screenToUnits.y;
			cmp_screenRect.yMax = -screenToUnits.y;
		}
		
		void Update () {

			if(opt_active==false){return;}

			////every [opt_genInterval] seconds create a circle
			if((st_timer -= Time.deltaTime) < 0){
				
				////create circle
				GameObject circle = (Instantiate(ext_circlePrefub) as GameObject);
				circle.transform.parent = transform;

				////set parameters to created circle 
				circle.GetComponent<CircleController>().opt_screenRect   = cmp_screenRect;
				circle.GetComponent<CircleController>().opt_speedFactor  = opt_speedFactor;
				circle.GetComponent<CircleController>().ext_game         = ext_game;		 //need for inc score
				circle.GetComponent<CircleController>().ext_assetManager = ext_assetManager; //need for choosing textures

				////reset timer - check and stop decrease of gen interval 
				opt_genInterval = (opt_genInterval<=opt_minGenInterval) ? opt_minGenInterval : opt_genInterval;
				st_timer = opt_genInterval;
			}
		}
}
