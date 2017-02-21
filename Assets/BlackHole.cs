using UnityEngine;
using System.Collections;

public class BlackHole : MonoBehaviour {
    struct data
    {
        public Vector4 pos;
        public Vector4 vel;
    };
    
    private Vector3 bh1;
    private Vector3 bh2;
    

    public Shader shader;
   // public Shader bhShader;
    data[] value;
    Vector3 att1;
    Vector3 att2;
    ComputeBuffer cBuff;
    public ComputeShader cShader;
    private Material mat;
    private Material mat1;
    private int kernelHandle;
    private int width, height, depth;
    private int particleSize;
    private float angle = 0.0f;
    private float speed = 35.0f;
    //values for count frame
    private int masterCount = 0;
    private float fpsSum = 0;


    // Use this for initialization
    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 9999;
        init();
    }
    private void OnDisable()
    {
        cBuff.Release();
    }
    void init()
    {
        //create material with input shader
        mat = new Material(shader);
        //mat1 = new Material(bhShader);
        //set Size
        width = 100;
        height = 100;
        depth = 100;
        //calculate particle size
        particleSize = width * height * depth;
        //create Particle data
        value = new data[particleSize];
        //initialize particle data
        for (int i = 0; i < particleSize; i++)
        {
            value[i].pos = new Vector4(0, 0, 0,0);
            value[i].vel = new Vector4(0, 0, 0,0);
        }
        float dx = 1.0f / (2.0f * width), dy = 1.0f / (1.0f * height), dz = 1.0f / depth;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                for (int k = 0; k < depth; k++)
                {
                    value[i * height + j].pos.x = (dx * i) - 0.25f;
                    value[i * height + j].pos.y = (dy * j)-0.5f;
                    value[i * height + j].pos.z = (dz * k) - 0.5f;
                    value[i * height + j].pos.w = 1.0f;
                }
            }
        }
        //setting blackhole
        bh1 = new Vector3(5, 0, 0);
        bh2 = new Vector3(-5, 0, 0);
        
        //create sharing buffer
        cBuff = new ComputeBuffer(particleSize, 32);
        //get Kernel handle
        kernelHandle = cShader.FindKernel("CSMain");
        //set Particle data to sharing buffer
        cBuff.SetData(value);
        //set Particle data 
        mat.SetBuffer("particles", cBuff);

        cShader.SetVector("bh1", bh1);
        cShader.SetVector("bh2", bh2);
        cShader.SetBuffer(kernelHandle, "particles", cBuff);
        //cShader.SetBuffer(kernelHandle, "bhp", cBuff);
    }
    private void OnPostRender()
    {
        mat.SetPass(0);
        //mat.SetBuffer("value", cBuff);
        Graphics.DrawProcedural(MeshTopology.Points, particleSize, 1);
        
    }
    private void OnRenderObject()
    {
        
        //GL.PushMatrix();
        //GL.LoadOrtho();
        //GL.Begin(GL.QUADS);
        //{
        //    GL.Vertex3(att1.x + 0.005f, att1.y - 0.005f, 0);
        //    GL.Vertex3(att1.x + 0.005f, att1.y + 0.005f, 0);
        //    GL.Vertex3(att1.x - 0.005f, att1.y + 0.005f, 0);
        //    GL.Vertex3(att1.x - 0.005f, att1.y - 0.005f, 0);


        //}
        //GL.End();
        //GL.Begin(GL.QUADS);
        //GL.Vertex3(att2.x + 0.005f, att2.y - 0.005f, 0);
        //GL.Vertex3(att2.x + 0.005f, att2.y + 0.005f, 0);
        //GL.Vertex3(att2.x - 0.005f, att2.y + 0.005f, 0);
        //GL.Vertex3(att2.x - 0.005f, att2.y - 0.005f, 0);
        //GL.End();
    }
    void Update()
    {
        angle += speed * Time.deltaTime;
        att1 = bh1;
        att2 = bh2;
        att1 = Quaternion.AngleAxis(angle, Vector3.forward) * att1;
        att2 = Quaternion.AngleAxis(angle, Vector3.forward) * att2;


        cShader.SetVector("bh1", att1);
        cShader.SetVector("bh2", att2);

        cShader.Dispatch(kernelHandle, particleSize / 1000, 1, 1);
        
        //cBuff.GetData(value);
        //Debug.Log(value[350].pos.y);
    }
}
