using UnityEngine;
using System.Collections;

public class temp : MonoBehaviour {

    public ComputeShader cs;
    private ComputeBuffer cBuff;
    private int kernelHandle;
    private Material mat;
    struct Particle
    {
        public Vector3 pos;
        public Vector3 vel;
    };

    Particle[] particles;
    public Shader shader;
    public int n, m;
	// Use this for initialization
	void Start () {
        mat = new Material(shader);
        particles = new Particle[n * m];
        float dx=(1.0f/n), dy=(1.0f/m);
        for(int i = 0; i < n; i++)
        {
            for (int j = 0; j < m; j++) {
                particles[i * m + j].pos = new Vector3(dx * i, dy * j, 0);
                particles[i * m + j].vel = new Vector3(0, 0, 0);
            }
        }
        kernelHandle = cs.FindKernel("CSMain");
        cBuff = new ComputeBuffer(n * m, 24);
        cBuff.SetData(particles);
        cs.SetBuffer(kernelHandle, "values", cBuff);
        mat.SetBuffer("particle", cBuff);
	}
	
    private void OnPostRender()
    {
        mat.SetPass(0);
        Graphics.DrawProcedural(MeshTopology.Points, n * m);
    }

	// Update is called once per frame
	void Update () {
        cs.Dispatch(kernelHandle, (n * m) / 32,1,1);
        cBuff.GetData(particles);
        Debug.Log(particles[0].pos.y);
	}
}
