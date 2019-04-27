using UnityEditor;
using UnityEngine;
using System.Collections;

// we're processing B&W extrusion map into RGBA distance map
public class UBER_PostProcessDistanceMap : AssetPostprocessor {

	void OnPreprocessTexture () {
		string lowerCaseAssetPath = System.IO.Path.GetFileName(assetPath);
		if (lowerCaseAssetPath.IndexOf("_UBER_DistanceMap") >=0) {
			TextureImporter textureImporter  = (TextureImporter) assetImporter;
			//textureImporter.isReadable = true;
			textureImporter.textureType = TextureImporterType.Default;
			textureImporter.mipmapEnabled=false;
			textureImporter.filterMode = FilterMode.Point;
			textureImporter.textureCompression=TextureImporterCompression.Uncompressed;
			textureImporter.sRGBTexture=false;
            textureImporter.alphaSource=TextureImporterAlphaSource.FromGrayScale;
            textureImporter.wrapMode=TextureWrapMode.Repeat;
			textureImporter.maxTextureSize=256;
		}
	}

	public void OnPostprocessTexture (Texture2D texture) {
		string lowerCaseAssetPath = System.IO.Path.GetFileName(assetPath);
		if (lowerCaseAssetPath.IndexOf("_UBER_DistanceMap") >=0) {
			//
			Color32[] pixels = texture.GetPixels32(0);
			int channel=-1;
			int val;
			val=pixels[0].a;
			for(int j=1; j<pixels.Length; j++) {
				if (pixels[j].a!=val) {
					channel=3; // we've got some data here on this color channel - use it
					break;
				}
			}
			if (channel==-1) {
				val=pixels[0].r;
				for(int j=1; j<pixels.Length; j++) {
					if (pixels[j].r!=val) {
						channel=0; // we've got some data here on this color channel - use it
						break;
					}
				}
			}
			if (channel==-1) {
				val=pixels[0].g;
				for(int j=1; j<pixels.Length; j++) {
					if (pixels[j].g!=val) {
						channel=1; // we've got some data here on this color channel - use it
						break;
					}
				}
			}
			if (channel==-1) {
				val=pixels[0].b;
				for(int j=1; j<pixels.Length; j++) {
					if (pixels[j].b!=val) {
						channel=2; // we've got some data here on this color channel - use it
						break;
					}
				}
			}
			if (channel==-1) {
				Debug.LogError ("I can't find any data in source distance map texture - "+texture.name);
				return;
			}

			bool[] height_flag=new bool[pixels.Length];
			int idxA=0;
			for(int j=texture.height-1; j>=0; j--) {
				int idxB=j*texture.width;
				for(int i=0; i<texture.width; i++) {
					if (channel==3) {
						height_flag[idxA++]=pixels[idxB].a>127;
					} else if (channel==0) {
						height_flag[idxA++]=pixels[idxB].r>127;
					} else if (channel==1) {
						height_flag[idxA++]=pixels[idxB].g>127;
					} else { // channel==2
						height_flag[idxA++]=pixels[idxB].b>127;
					}
					idxB++;
				}
			}
			int lx=0;
			int rx=1;

			Rect rect;
			int _left;
			int _top;
			int _right;
			int _bottom;
			int _sx=256/texture.width;
			int _sy=256/texture.height;
			int idx;

			while(lx<texture.width) {
				for(int i=lx; (i<rx) && (i<texture.width); i++) {
					idx=i+texture.width*(texture.height-1);
					for(int j=0; j<texture.height; j++) {
						rect=getDistRect(i,j, texture.width, texture.height, height_flag);
						_left=Mathf.RoundToInt(i-rect.xMin)*_sx;
						_right=Mathf.RoundToInt(rect.xMax-i)*_sx;
						_top=Mathf.RoundToInt(j-rect.yMin)*_sy;
						_bottom=Mathf.RoundToInt(rect.yMax-j)*_sy;
						if (_top>0 || _bottom>0) { _top+=_sy; _bottom-=_sy; };
						if (_left>255) _left=255;
						if (_right>255) _right=255;
						if (_top>255) _top=255;
						if (_bottom>255) _bottom=255;
						pixels[idx]=new Color32((byte)_left, (byte)_right, (byte)_top, (byte)_bottom);
						idx-=texture.width;
					}
				}
				lx++;
				rx++;
			}
			texture.SetPixels32(pixels);
			texture.Apply(false, true);
		} 
	}

	Rect getDistRect(int x, int y, int width, int height, bool[] height_flag) {
		const int texel_penetration_count=1;
		int up_stop_count = texel_penetration_count;
		int down_stop_count = texel_penetration_count;
		int left_stop_count = texel_penetration_count;
		int right_stop_count = texel_penetration_count;
		Rect rect=new Rect(x,y,0,0);
		if (height_flag[x+y*width]) return rect;
		int lx=x;
		int rx=x;
		int ty=y;
		int _by=y;
		do {
			if (up_stop_count>0) ty--;
			if (ty<-height) {
				ty=-2048;
				up_stop_count=0;
			}
			if (up_stop_count>0) {
				if (scanLineH(lx, rx, ty, width, height, height_flag)) {
					up_stop_count--;
					if (up_stop_count==0) ty++;
				}
			}
			if (down_stop_count>0) _by++;
			if (_by>=height*2) {
				_by=height+2048;
				down_stop_count=0;
			}
			if (down_stop_count>0) {
				if (scanLineH(lx, rx, _by, width, height, height_flag)) {
					down_stop_count--;
					if (down_stop_count==0) _by--;
				}
			}
			if (left_stop_count>0) lx--;
			if (lx<-width) {
				lx=-2048;
				left_stop_count=0;
			}
			if (left_stop_count>0) {
				if (scanLineV(lx, ty, _by, width, height, height_flag)) {
					left_stop_count--;
					if (left_stop_count==0) lx++;
				}
			}
			if (right_stop_count>0) rx++;
			if (rx>=width*2) {
				rx=width+2048;
				right_stop_count=0;
			}
			if (right_stop_count>0) {
				if (scanLineV(rx, ty, _by, width, height, height_flag)) {
					right_stop_count--;
					if (right_stop_count==0) rx--;
				}
			}
		} while (up_stop_count>0 || down_stop_count>0 || right_stop_count>0 || left_stop_count>0);
		rect.x=lx;
		rect.y=ty;
		rect.width=rx-lx+1;
		rect.height=_by-ty+1;
		return rect;
	}
	
	bool scanLineH(int lx, int rx, int y, int width, int height, bool[] height_flag) {
		if (lx>rx) return true;
		int n=rx-lx;
		int _offset=mod(y, height)*width;
		while(n>=0) {
			if ( height_flag[mod(lx++,width) + _offset] ) return true;
			n--;
		}
		return false;
	}
	
	bool scanLineV(int x, int ty, int _by, int width, int height, bool[] height_flag) {
		if (ty>_by) return true;
		int n=_by-ty;
		int _offset=mod(x,width);
		while(n>=0) {
			if ( height_flag[_offset + mod(ty++, height)*width] ) return true;
			n--;		
		}
		return false;
	}
	
	int mod(int n, int m) {
		n=n%m;
		if (n<0) n+=m;
		return n;
	}

}