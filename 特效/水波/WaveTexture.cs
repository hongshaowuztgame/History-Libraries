using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

//[ExecuteInEditMode]
public class WaveTexture : MonoBehaviour {
    public int waveWidth=128;
    public int waveHeight=128;
    public float lowpower=0.01f;
    public int radius = 20;
    float[,] waveA;
    float[,] waveB;
    Color[] Colorbuffer;
    public int x = 64, y = 64;
    public short height = 1;

    Texture2D tex_uv;

    bool isRun = true;
    int sleeptime;
	// Use this for initialization
	void Start () {
        waveA = new float[waveWidth, waveHeight];
        waveB = new float[waveWidth, waveHeight];

        tex_uv = new Texture2D(waveWidth, waveHeight);
        Colorbuffer = new Color[waveWidth * waveHeight];
        GetComponent<Renderer>().material.SetTexture("_WaveTex", tex_uv);

        Thread th = new Thread(new ThreadStart(ComputeWave));
        th.Start();
	}
	
	// Update is called once per frame
	void Update () {
        sleeptime = (int)(Time.deltaTime * 1000);
        tex_uv.SetPixels(Colorbuffer);
        tex_uv.Apply();
        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray,out hit))
            {
                Vector3 pos=hit.point;
                pos = transform.worldToLocalMatrix.MultiplyPoint(pos);
                int w = (int)((pos.x + 0.5f) * waveWidth);
                int h = (int)((pos.y + 0.5f) * waveHeight);
                Debug.Log(pos+"w:"+w+"h:"+h);
                Putpop(w,h,height);
            }
        }
        //ComputeWave();
	}

    void Putpop(int w,int h,short height)
    {
        int dist;
        int radius_2=radius*radius;
        for(int i=-radius;i<=radius;i++)
        {
            for(int j=-radius;j<=radius;j++)
            {
                if((w+i>=0)&&(w+i<waveWidth-1)&&(h+j>=0)&&(h+j<waveHeight-1))
                {
                    dist = i * i + j * j;
                    if(dist<radius_2)
                    {
                        waveA[w + i, h + j] = (Mathf.Cos(dist * Mathf.PI / radius_2) * height);
                    }
                }
            }
        }
    }

    private void OnDestroy()
    {
        isRun = false;
    }
    void ComputeWave()
    {
        while(isRun)
        {
            for (int w = 1; w < waveWidth - 1; w++)
                for (int h = 1; h < waveHeight - 1; h++)
                {
                    waveB[w, h] = (waveA[w - 1, h] + waveA[w + 1, h] + waveA[w, h - 1] + waveA[w, h + 1] +
                        waveA[w - 1, h - 1] + waveA[w + 1, h - 1] + waveA[w - 1, h + 1] + waveA[w + 1, h + 1]) / 4
                        - waveB[w, h];

                    float value = waveB[w, h];
                    if (value > 1)
                        waveB[w, h] = 1;
                    if (value < -1)
                        waveB[w, h] = -1;

                    float offset_u = (waveB[w - 1, h] - waveB[w + 1, h]) / 2;
                    float offset_v = (waveB[w, h - 1] - waveB[w, h + 1]) / 2;

                    float r = offset_u / 2 + 0.5f;
                    float g = offset_v / 2 + 0.5f;
                    // tex_uv.SetPixel(w, h, new Color(r, g, 0));
                    Colorbuffer[w + waveWidth * h] = new Color(r, g, 0);
                    waveB[w, h] -= waveB[w, h] * lowpower;
                }

            //tex_uv.Apply();
            float[,] temp = waveA;
            waveA = waveB;
            waveB = temp;
            Thread.Sleep(sleeptime);
        }
    }
}
