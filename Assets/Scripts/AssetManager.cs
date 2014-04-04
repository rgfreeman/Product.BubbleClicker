using UnityEngine;
using System.Collections;

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

		////computed
		public Texture2D[] cmp_textures = new Texture2D[4];
		public Sprite[]    cmp_sprites  = new Sprite[4];

		////link to external 
		public GameObject ext_prefubCircle;


	void Start () {

		////load textures from resources
		for(int index=0; index<opt_images.Length; index++){
			cmp_textures[index] = Resources.Load<Texture2D>(opt_images[index]);
			cmp_sprites[index] = new Sprite();
			cmp_sprites[index] = Sprite.Create(
				cmp_textures[index],
				new Rect(0, 0, cmp_textures[index].width, cmp_textures[index].height),
				new Vector2(0.5f, 0.5f)  //pivot always on center
			);
		}
	}
	
	// Update is called once per frame
	void Update () {
	}
}
