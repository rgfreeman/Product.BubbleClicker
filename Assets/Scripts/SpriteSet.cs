using UnityEngine;
using System.Collections;

/*
  keep generated textures and sprites for 32*32, 64*64, 128*128, 256*256 sizes
*/

public class SpriteSet{
	public Texture2D[] textures = new Texture2D[]{
		new Texture2D(32,32),
		new Texture2D(64,64),
		new Texture2D(128,128),
		new Texture2D(256,256)
	};
	public Sprite[] sprites = new Sprite[4];
	public int countLinks   = 0; 


	public void DestroySpriteSet(){
		for(int index=0; index<textures.Length; index++){
			Sprite.Destroy(sprites[index]);
			Texture2D.Destroy(textures[index]);
		}
	}
}


