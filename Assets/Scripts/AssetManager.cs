using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;


public class AssetManager : MonoBehaviour {
	////need Unity Pro to load bundle - using local resources

	////fields
		////options
		public string[] opt_images = new string[]{
			"img_circle32",
			"img_circle64",
			"img_circle128",
			"img_circle256"
		};

		////state
		private string st_generating = "indefined";
		private int    st_currentLevel = 0; //set in updateTextures method
		private int    st_nextLevel    = 1; //set in updateTextures method

		////local 
		//// PixelSets are created only once
		//// PixelSets can be used in the separate thread
	    private PixelsSet loc_zeroLevelPixelsSet = null;  ////need for keep alpha chanel
		private PixelsSet loc_genericPixelsSet   = null;  ////fill into separate thread

		////dictionary sets of sprites <level, spriteSet>
		private Dictionary<int, SpriteSet> loc_sets = new Dictionary<int, SpriteSet>();

		////external
		public GameObject ext_game;
		public GameObject ext_guiSets;


	void Start () {
		loc_zeroLevelPixelsSet = new PixelsSet();
		loc_genericPixelsSet   = new PixelsSet();

		////load textures from resources
		for(int index=0; index<opt_images.Length; index++){
			////temp_texture can not be destroyed because it is resource
			Texture2D temp_texture = Resources.Load<Texture2D>(opt_images[index]);
				////this need to keep alpha channel and use it in separate thread
				loc_zeroLevelPixelsSet.pixels[index] = temp_texture.GetPixels();
		}

		////force update texture for 0 level - this not using thread
		forceUpdateTextures();

		////update textures for 1 level then 0 level started - this using thread
		updateTextures(0);
	}


	////this method called by circle then it create
	public Sprite getNewSprite(int textureIndex){
		////return set of actual game level, for 0 level -set already create in Start()
		loc_sets[ st_currentLevel ].countLinks++;
		return loc_sets[ st_currentLevel ].sprites[textureIndex];
	}

	////this method called by circle then it destroy
	public void releaseSprite(int level){
		loc_sets[level].countLinks--;
		if((level!=st_currentLevel) && (loc_sets[level].countLinks==0)){
			Debug.Log("Set will be clear");
			loc_sets[level].DestroySpriteSet();
			loc_sets.Remove(level);
		}
	}

	public void randomColor(){
		////set random color
		loc_genericPixelsSet.color=new Color(Random.Range(0.5f,1.0f), Random.Range(0.5f,1.0f), Random.Range(0.5f,1.0f));
	}

	////this method is called then level up
	public void updateTextures(int currentLevel){
		////if current level 0 - need to create texture level 1
		////save current level 
		st_currentLevel = currentLevel;
		st_nextLevel    = currentLevel+1;

		////remove ramaining empty set
		if(loc_sets.ContainsKey(currentLevel-1)){
			if (loc_sets[currentLevel-1].countLinks==0){
				loc_sets.Remove(currentLevel-1);
			}
		}

		randomColor();
		st_generating = "generating";
		////this new thread fill loc_genericColorSet
		Thread thread = new System.Threading.Thread(genColorsInThread);
		thread.Start();
	}

	////this method is called only at start
	public void forceUpdateTextures(){
		st_nextLevel = st_currentLevel;

		randomColor();
		genColor();
		onFinishUpdate();
	}


	public void onFinishUpdate(){
		////creat new set
		Debug.Log(st_nextLevel);
		loc_sets.Add(st_nextLevel, new SpriteSet());

		////get textures using loc_genericPixelSet
		for (int index=0; index<loc_sets[st_nextLevel].textures.Length; index++){
			loc_sets[st_nextLevel].textures[index].SetPixels(loc_genericPixelsSet.pixels[index]);
			loc_sets[st_nextLevel].textures[index].Apply();

			loc_sets[st_nextLevel].sprites[index] = Sprite.Create(
				loc_sets[st_nextLevel].textures[index],
				new Rect(
					0,
					0,
					loc_sets[st_nextLevel].textures[index].width,
					loc_sets[st_nextLevel].textures[index].height
				),
				new Vector2(0.5f, 0.5f) //pivot point always on center
			);
		}
	}


	////this method called in separate thread
	//// method modify arrays of colors - loc_genericPixelsSet
	public void genColorsInThread(){
		genColor();
		st_generating="ready";
		Debug.Log("Set ready");
	}

	public void genColor(){
		Color color = new Color();
		for(int aindex=0; aindex<loc_genericPixelsSet.pixels.Length; aindex++){// 0..3
			int height = loc_zeroLevelPixelsSet.sizes[aindex];
			int pixel_counter = 0;
			int line = 0;
			for (int cindex=0; cindex<loc_genericPixelsSet.pixels[aindex].Length; cindex++){
				if(pixel_counter++==height){pixel_counter=0; line++;}
				color = loc_zeroLevelPixelsSet.pixels[aindex][cindex]; //get color with alpha
				color.r = loc_genericPixelsSet.color.r * ((float)line)/height;
				color.g = loc_genericPixelsSet.color.g * ((float)line)/height;
				color.b = loc_genericPixelsSet.color.b * ((float)line)/height;
				loc_genericPixelsSet.pixels[aindex][cindex] = color;
			}
		}
	}


	void Update () {
		if(st_generating=="ready"){
			st_generating="undefined";
			onFinishUpdate();
		}
		////update GUI
		ext_guiSets.GetComponent<GUIText>().text = "Sets in memory:"+loc_sets.Count.ToString(); 
	}
}
