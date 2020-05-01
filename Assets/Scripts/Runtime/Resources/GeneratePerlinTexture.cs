using UnityEngine;

public class GeneratePerlinTexture : MonoBehaviour
{
	public Gradient Gradient;
	[Range(1, 30)]
	public float Blending = 100;
	public int pixWidth = 1024;
	public int pixHeight = 1024;
	public float xOrg;
	public float yOrg;
	public float xOrgTex;
	public float yOrgTex;
	public float scale = 20.0F;
	public float scaleTex = 20.0F;
	private Texture2D noiseTex;
	private Color[] pix;
	private Renderer rend;

	void Start()
	{
		rend = GetComponent<Renderer>();
		noiseTex = new Texture2D(pixWidth, pixHeight);
		pix = new Color[noiseTex.width * noiseTex.height];
		rend.material.mainTexture = noiseTex;
	}

	public void FixedUpdate()
	{
		CalcNoise();
	}

	void CalcNoise()
	{
		int y = 0;
		while (y < noiseTex.height)
		{
			int x = 0;
			while (x < noiseTex.width)
			{
				float xCoord = xOrg + (float)x / noiseTex.width * scale;
				float yCoord = yOrg + (float)y / noiseTex.height * scale;
				float xCoordText = xOrgTex + (float)x / noiseTex.width * scaleTex;
				float yCoordText = yOrgTex + (float)y / noiseTex.height * scaleTex;
				float sample = Mathf.PerlinNoise(xCoord, yCoord);
				float sampleTex = Mathf.PerlinNoise(xCoordText, yCoordText);
				sample = ((int)(sample * Blending)) / Blending;
				var color = Gradient.Evaluate(sample);
				var newcolor = color + color * (sampleTex * 2 - 1) * 0.1f;
				newcolor.a = 1;
				pix[y * noiseTex.width + x] = newcolor;
				x++;
			}
			y++;
		}
		noiseTex.SetPixels(pix);
		noiseTex.Apply();
	}
}
