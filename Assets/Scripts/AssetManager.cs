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
		private int  st_currentLevel = 0; //set in updateTextures method

		////local 
		//// PixelSets are created only once
		//// PixelSets possible to using into separate thread
	    private PixelsSet loc_zeroLevelPixelsSet = null;  ////need for keep alpha chanel
		private PixelsSet loc_genericPixelsSet   = null;  ////fill into separate thread

		////dictionary of sets of sprites
		private Dictionary<int, SpriteSet> loc_sets = new Dictionary<int, SpriteSet>();

		////external
		public GameObject ext_game;
		public GameObject ext_guiSets;


	void Start () {
		////this is default SpriteSet for textures that can't create
		loc_sets.Add(0, new SpriteSet());
		loc_zeroLevelPixelsSet = new PixelsSet();
		loc_genericPixelsSet   = new PixelsSet();

		////load textures from resources
		for(int index=0; index<opt_images.Length; index++){
			loc_sets[0].textures[index] = Resources.Load<Texture2D>(opt_images[index]);

			loc_zeroLevelPixelsSet.pixels[index] = loc_sets[0].textures[index].GetPixels();

			loc_sets[0].sprites[index] = Sprite.Create(
				loc_sets[0].textures[index],
				new Rect(0, 0, loc_sets[0].textures[index].width, loc_sets[0].textures[index].height),
				new Vector2(0.5f, 0.5f)  //pivot always on center
			);
		}
		////start gen textures, 0 - will be increment
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
			Debug.Log("need clear");
			loc_sets[level].DestroySpriteSet();
			loc_sets.Remove(level);
		}
		////get set by level
		////decrease count of sprites
	}


	////this method call on start and then level up
	public void updateTextures(int currentLevel){
		////if current level 0 - need to create texture level 1
	
		////save current level 
		st_currentLevel = currentLevel;

		loc_sets.Add(st_currentLevel+1, new SpriteSet());

		st_generating = "generating";
		////this new thread fill loc_genericColorSet
		Thread thread = new System.Threading.Thread(genColorsInThread);
		thread.Start();
	}


	public void onFinishUpdate(){
		int currentLevel = st_currentLevel;
		currentLevel++;

		////get textures using loc_genericPixelSet
		for (int index=0; index<loc_sets[currentLevel].textures.Length; index++){
			loc_sets[currentLevel].textures[index].SetPixels(loc_genericPixelsSet.pixels[index]);
			loc_sets[currentLevel].textures[index].Apply();

			loc_sets[currentLevel].sprites[index] = Sprite.Create(
				loc_sets[currentLevel].textures[index],
				new Rect(
					0,
					0,
					loc_sets[currentLevel].textures[index].width,
					loc_sets[currentLevel].textures[index].height
				),
				new Vector2(0.5f, 0.5f) //pivot point always on center
			);
		}
	}


	////this method called in separate thread
	//// method return arrays of colors
	public void genColorsInThread(){

		Color color = new Color();
		for(int aindex=0; aindex<loc_genericPixelsSet.pixels.Length; aindex++){
			for (int cindex=0; cindex<loc_genericPixelsSet.pixels[aindex].Length; cindex++){
				color = loc_zeroLevelPixelsSet.pixels[aindex][cindex];
				color.r = (aindex+0.3f)/4.0f;
				color.g = (aindex+0.5f)/2.0f;
				color.b = (aindex+0.9f)/4.0f;
				loc_genericPixelsSet.pixels[aindex][cindex] = color;
			}
		}
		st_generating="ready";
		Debug.Log("ready");

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
