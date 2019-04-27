using UnityEditor;
using UnityEngine;
using System.Collections;

// special treatment of heightmaps - take max of 2x2 quad instead of average value 
public class UBER_PostProcessMIPHeights : AssetPostprocessor {
	public void OnPostprocessTexture (Texture2D texture) {
		string lowerCaseAssetPath = System.IO.Path.GetFileName(assetPath);
		if (lowerCaseAssetPath.IndexOf("_HeightsQDM") >=0) {
			// QDM - inverted heightmap
			Color32[] c = texture.GetPixels32(0);
			for (int i = 0; i < c.Length; i++) {
				c[i].a = (byte)(255 - c[i].a);
			}
			texture.SetPixels32(c, 0);
			Color32[] cp;
			int tw = texture.width/2;
			int th = texture.height/2;
			for (int m  = 1; m < texture.mipmapCount; m++) {
				cp = c;
				c = texture.GetPixels32(m);
				int i=0;
				int j=0;
				int tw2=tw*2;
				for (int _y = 0; _y < th; _y++) {
					j=(_y*2)*tw2;
					for (int _x = 0; _x < tw; _x++) {
						int j2=j;
						byte min=(byte)cp[j2].a;
						j2++;
						byte min_tmp=(byte)cp[j2].a;
						j2+=tw2;
						if (min_tmp<min) min=min_tmp;
						min_tmp=(byte)cp[j2].a;
						j2--;
						if (min_tmp<min) min=min_tmp;
						min_tmp=(byte)cp[j2].a;
						if (min_tmp<min) min=min_tmp;
						c[i].a = min;
						i++;
						j+=2;
					}
				}
				tw/=2;
				th/=2;
				texture.SetPixels32(c, m);
			}
			texture.Apply(false, true);
		} else if (lowerCaseAssetPath.IndexOf("_Heights") >=0) {
			// POM
			Color32[] c = texture.GetPixels32(0);
			texture.SetPixels32(c, 0);
			Color32[] cp;
			int tw = texture.width/2;
			int th = texture.height/2;
			for (int m  = 1; m < texture.mipmapCount; m++) {
				cp = c;
				c = texture.GetPixels32(m);
				int i=0;
				int j=0;
				int tw2=tw*2;
				for (int _y = 0; _y < th; _y++) {
					j=(_y*2)*tw2;
					for (int _x = 0; _x < tw; _x++) {
						int j2=j;
						byte max=(byte)cp[j2].a;
						j2++;
						byte max_tmp=(byte)cp[j2].a;
						j2+=tw2;
						if (max_tmp>max) max=max_tmp;
						max_tmp=(byte)cp[j2].a;
						j2--;
						if (max_tmp>max) max=max_tmp;
						max_tmp=(byte)cp[j2].a;
						if (max_tmp>max) max=max_tmp;
						c[i].a = max;
						i++;
						j+=2;
					}
				}
				tw/=2;
				th/=2;
				texture.SetPixels32(c, m);
			}
			texture.Apply(false, true);
		}
	}
}