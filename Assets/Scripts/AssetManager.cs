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

		////local 
		private SpriteSet loc_zeroLevelSet;
		private PixelsSet loc_zeroLevelPixelsSet; ////need for keep alpha chanel

		private SpriteSet loc_genericLevelSet  = null;
		private PixelsSet loc_genericPixelsSet = null; ////with pixels it's possible work in thread

		////external
		public GameObject ext_game;


	void Start () {
		////this is default SpriteSet for textures that can't create
		loc_zeroLevelSet = new SpriteSet();
		loc_zeroLevelPixelsSet = new PixelsSet();

		////load textures from resources
		for(int index=0; index<opt_images.Length; index++){
			loc_zeroLevelSet.textures[index] = Resources.Load<Texture2D>(opt_images[index]);
			loc_zeroLevelPixelsSet.pixels[index] = loc_zeroLevelSet.textures[index].GetPixels();

			loc_zeroLevelSet.sprites[index] = new Sprite();
			loc_zeroLevelSet.sprites[index] = Sprite.Create(
				loc_zeroLevelSet.textures[index],
				new Rect(0, 0, loc_zeroLevelSet.textures[index].width, loc_zeroLevelSet.textures[index].height),
				new Vector2(0.5f, 0.5f)  //pivot always on center
			);
		}

		////from the start of program - start generate new textures
		this.updateTextures();
	}

	////this method called by circle then it create
	public Sprite getNewSprite(int textureIndex){
		if (loc_genericLevelSet == null){
			return loc_zeroLevelSet.sprites[textureIndex];
		}else{
			return loc_genericLevelSet.sprites[textureIndex];
		}
	}

	////this method call on start and then level up
	public void updateTextures(){
		st_generating = "generating";
		loc_genericLevelSet  = new SpriteSet();
		loc_genericPixelsSet  = new PixelsSet();

		////this new thread fill loc_genericColorSet
		Thread thread = new System.Threading.Thread(genColorsInThread);
		thread.Start();
	}


	public void onFinishUpdate(){
		////loc_genericColorSet generated
		for (int index=0; index<loc_genericLevelSet.textures.Length; index++){
			loc_genericLevelSet.textures[index].SetPixels(loc_genericPixelsSet.pixels[index]);
			loc_genericLevelSet.textures[index].Apply();
			loc_genericLevelSet.sprites[index] = Sprite.Create(
				loc_genericLevelSet.textures[index],
				new Rect(
					0,
					0,
					loc_genericLevelSet.textures[index].width,
					loc_genericLevelSet.textures[index].height
				),
				new Vector2(0.5f, 0.5f) //pivot point always on center
			);
		}

	}


	////this method called in separate thread
	//// method return arrays of colors
	public void genColorsInThread(){

		Color color;
		for(int aindex=0; aindex<loc_genericPixelsSet.pixels.Length; aindex++){
			for (int cindex=0; cindex<loc_genericPixelsSet.pixels[aindex].Length; cindex++){
				color = loc_zeroLevelPixelsSet.pixels[aindex][cindex];
				color.r = 1.0f;
				color.g = 0.0f;
				color.b = 0.0f;
				loc_genericPixelsSet.pixels[aindex][cindex] = color;
			}
		}
		st_generating="ready";

	}


	// Update is called once per frame
	void Update () {
		if(st_generating=="ready"){
			st_generating="undefined";
			onFinishUpdate();
		}
	}
}
