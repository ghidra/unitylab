using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;//for getting but size or a struct automaticall to send to compute shaders

public class proc_geo_test : MonoBehaviour{
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

/*
public class proc_geo_test : verlet_base {

	public ComputeShader _tonandiAMeshShader;
	public Material meshMaterial;
	private ComputeBuffer meshBuffer;//this will be filled by the above shader, after being fed, the velet points and ids
	private ComputeBuffer meshVertIdBuffer;///a buffer of ints to grab from the verlet points
	private ComputeBuffer argsMeshBuffer;//used to render the mesh buffer
	private int meshKernelID;
	private int meshWarpCount;
	private const int M_WARP_SIZE = 16;

	private struct MeshVert
	{
		public Vector4 P;///i might sneak uv in w of pos and norm
		public Vector4 N;
		public Vector4 distance;
	}
	private struct MeshVert
	{
		public Vector4 P;///i might sneak uv in w of pos and norm
		public Vector4 v;
		public Vector4 preP;
	}

	protected override void IntializeBuffers () {
		//lets make a triangle... that just neighbors to each other
		verletPoints = 3;

		VerletPoint[] verletArray = new VerletPoint[3];
    	uint[] neighborArray = new uint[3*2];
    	float[] lengthArray = new float[3*2];

    	Vector4 p0 = new Vector4(-0.4f,0.0f,0.0f,-1.0f);
    	Vector4 p1 = new Vector4(0.0f,0.4f,0.0f,-1.0f);
    	Vector4 p2 = new Vector4(0.4f,0.0f,0.0f,-1.0f);

    	verletArray[0].P = p0;
    	verletArray[0].preP = p0;
    	verletArray[0].preP.w = 2.0f;
    	neighborArray[0] = 1;
    	neighborArray[1] = 2;
    	lengthArray[0] = 0.5656f;
    	lengthArray[1] = 0.8f;

    	verletArray[1].P = p1;
    	verletArray[1].preP = p1;
    	verletArray[1].preP.w = 2.0f;
    	neighborArray[2] = 0;
    	neighborArray[3] = 2;
    	lengthArray[2] = 0.5656f;
    	lengthArray[3] = 0.5656f;

    	verletArray[2].P = p2;
    	verletArray[2].preP = p2;
    	verletArray[2].preP.w = 2.0f;
    	neighborArray[4] = 0;
    	neighborArray[5] = 1;
    	lengthArray[4] = 0.8f;
    	lengthArray[5] = 0.5656f;

    	base.IntializeBuffers();
    	FillVerletBuffers(verletArray,neighborArray,lengthArray);

    	////////////////////////

    	meshBuffer = new ComputeBuffer( (int)3, Marshal.SizeOf(typeof(MeshVert)) );
        meshVertIdBuffer = new ComputeBuffer( (int)3, sizeof(uint) );
		argsMeshBuffer = new ComputeBuffer(1, 4 * sizeof(uint), ComputeBufferType.IndirectArguments);
		argsMeshBuffer.SetData(new int[4] { (int)3, 1, 0, 0 });
		meshKernelID = _tonandiAMeshShader.FindKernel("CSVerletMeshTonandiA");
    	meshWarpCount = Mathf.CeilToInt((float)3 / M_WARP_SIZE);

    	uint[] idArray = new uint[3];
    	idArray[0]=0;
    	idArray[1]=1;
    	idArray[2]=2;
    	meshVertIdBuffer.SetData(idArray);

    	SetBufferConstants();
		UpdateBuffers();

	}
	protected override void SetBufferConstants()
	{
		base.SetBufferConstants();
		//////MESHING SHADER
		_tonandiAMeshShader.SetBuffer( meshKernelID, "meshBuffer", meshBuffer );///for the normals
		_tonandiAMeshShader.SetBuffer( meshKernelID, "meshVertIdBuffer", meshVertIdBuffer );
		_tonandiAMeshShader.SetBuffer( meshKernelID, "verletPointBuffer", verletPointBuffer );

		meshMaterial.SetBuffer( "MeshBuffer", meshBuffer );

	}

	protected override void UpdateBuffers()
	{
		base.UpdateBuffers();//update verlet

    	//meshMaterial.SetFloat( "handDistance", verlet.handPushDistance );

		verlet._verletShader.SetVector("noise", verlet.noise);
        verlet._verletShader.SetVector("pulse", new Vector3(0.0f,0.0f,0.0f));
        verlet._verletShader.SetFloat("pulseStrength", verlet.pulseStrength);

	}

	protected override void Dispatch()
    {
        base.Dispatch();
		_tonandiAMeshShader.Dispatch( meshKernelID, meshWarpCount, 1, 1);///mesh

    }

	protected override void ReleaseBuffers()
	{
		base.ReleaseBuffers();

		if (meshBuffer != null) meshBuffer.Release();
		if (meshVertIdBuffer != null) meshVertIdBuffer.Release();
		if (argsMeshBuffer != null) argsMeshBuffer.Release();
	}
	///////////

	void Awake()
    {
        IntializeBuffers();
    }
	
	void Update () 
	{
		UpdateBuffers();
		Dispatch();
	}

	protected override void OnRenderObject()
	{
		base.OnRenderObject();
		meshMaterial.SetPass(0);
		Graphics.DrawProceduralIndirect(MeshTopology.Triangles,argsMeshBuffer);
	}

	void OnDisable () 
	{
		ReleaseBuffers();
	}

}
*/