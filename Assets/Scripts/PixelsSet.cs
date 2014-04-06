using UnityEngine;
using System.Collections;

/*
  keep generated textures and sprites for 32*32, 64*64, 128*128, 256*256 sizes
*/

public class PixelsSet{
	////jugged array
	public Color[][] pixels = new Color[][]{
		new Color[32*32],
		new Color[64*64],
		new Color[128*128],
		new Color[256*256],
	};
	public int[] sizes = new[]{
		32,
		64,
		128,
		256
	};
	public Color color;
}
