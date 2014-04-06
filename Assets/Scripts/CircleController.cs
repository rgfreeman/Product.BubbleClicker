using UnityEngine;
using System.Collections;

public class CircleController : MonoBehaviour {

	////fields
		////options
		public float opt_minScale = 0.125f; //for 32 pixels sprite
		public float opt_maxScale = 1.0f;	//for 256 pixels sprite
		public int   opt_minScore = 1;
		public int   opt_maxScore = 3;

		public int   opt_baseSize = 256; //opt_maxScale equal opt_baseSize

		public float opt_speedFactor;	 //set by "circles" object
		public Rect  opt_screenRect;	 //set by "circles" object
	
		////computed
		private float cmp_speed;
		private float cmp_radius;
		private int   cmp_score;
		private int   cmp_level;

		////external elements
		public GameObject ext_game;          //object with game controller bahaviour
		public GameObject ext_assetManager;

	////methods
		void Start () {
			float scale = Random.Range(opt_minScale, opt_maxScale);

			////get target size by scale and base size 
			float cmp_target_pixelSize;	 //size of sprite in pixels after scaling
			cmp_target_pixelSize = scale*opt_baseSize;

			////compute the required size of the texture
			int   cmp_sprite_pixelSize;
		    int   cmp_textureIndex;      //index texture in array of textures
			float spriteCollisionRadius;

			////analog of mipmap
			if(cmp_target_pixelSize>128){cmp_textureIndex=3; cmp_sprite_pixelSize=256; spriteCollisionRadius=1.28f;}else
			if(cmp_target_pixelSize>64) {cmp_textureIndex=2; cmp_sprite_pixelSize=128; spriteCollisionRadius=0.64f;}else
			if(cmp_target_pixelSize>32) {cmp_textureIndex=1; cmp_sprite_pixelSize=64;  spriteCollisionRadius=0.32f;}else
										{cmp_textureIndex=0; cmp_sprite_pixelSize=32;  spriteCollisionRadius=0.16f;}

			GetComponent<SpriteRenderer>().sprite = 
			ext_assetManager.GetComponent<AssetManager>().getNewSprite(cmp_textureIndex);

			////scale to score - using formula of Line by two points
			cmp_score = Mathf.RoundToInt(((scale - opt_minScale)/(opt_maxScale - opt_minScale)*(opt_minScore - opt_maxScore))) + opt_maxScore;

			////scale to speed 
			cmp_speed = (opt_maxScale-scale)*opt_speedFactor;

			////correction scale
			scale = cmp_target_pixelSize/cmp_sprite_pixelSize;
	        ////apply corrected scale
			transform.localScale = new Vector2(scale, scale);

			////apply collision radius
			transform.GetComponent<CircleCollider2D>().radius=spriteCollisionRadius;

			cmp_radius = renderer.bounds.size.x/2;

			transform.position = new Vector2(
				Random.Range(opt_screenRect.xMin + cmp_radius, opt_screenRect.xMax - cmp_radius),
				opt_screenRect.yMax - cmp_radius
			);

			cmp_level = ext_game.GetComponent<GameController>().getLevel();
		}
		
	
		void OnMouseDown () {
			this.ext_game.GetComponent<GameController>().incScore(this.cmp_score);
			DestroyCircle();
		}


		void Update () {
			transform.Translate( 0, -cmp_speed * Time.deltaTime, 0 );
			
			////destroy condition
			if (transform.position.y - cmp_radius < opt_screenRect.yMin){
				DestroyCircle();
			}
		}

		void DestroyCircle(){
			ext_assetManager.GetComponent<AssetManager>().releaseSprite(cmp_level);
			Destroy(this.gameObject);
		}
}
