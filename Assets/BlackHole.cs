using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BlackHole : MonoBehaviour {
    //입자의 정보를 저장할 자료형
    struct data
    {
        public Vector4 pos;
        public Vector4 vel;
    };
    public Text text;
    //블랙홀의 중심 좌표
    private Vector3 bh1;
    private Vector3 bh2;

    public Shader shader;
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
    private float dts;
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
        text.text = "";
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
        //화면에 그릴 Material 설정
        mat.SetPass(0);
        //vert shader 와 frag shader를 이용하여 화면에 particleSize만큼 픽셀을 그림
        Graphics.DrawProcedural(MeshTopology.Points, particleSize, 1);
        
    }
    void Update()
    {
        //블랙홀의 중심을 회전
        angle += speed * Time.deltaTime;
        att1 = bh1;
        att2 = bh2;
        att1 = Quaternion.AngleAxis(angle, Vector3.forward) * att1;
        att2 = Quaternion.AngleAxis(angle, Vector3.forward) * att2;

        cShader.SetVector("bh1", att1);
        cShader.SetVector("bh2", att2);
        //컴퓨트 셰이더 실행
        cShader.Dispatch(kernelHandle, particleSize / 1000, 1, 1);
        //프레임 계산 
        masterCount++;
        dts += (1 / Time.deltaTime);
        if (masterCount <= 2001)
            if (masterCount % 200 == 0)
            {
                text.text += dts / 200.0 + "\n";
                dts = 0;
            }
    }
}
