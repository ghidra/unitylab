using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tmp_strands : MonoBehaviour
{
	private Mesh mesh;

	[Range(0.1f, 64.0f)]
	public float length=1.0f;
	[Range(1, 1024)]
	public uint verts=32;
	[Range(0.0f, 64.0f)]
	public float noiseMagnitude=1.0f;

	//////DEBUG
	//public bool drawDebug;
    public bool drawPoints;
    //public bool drawLines;
    public Material debugMaterial;
    //////DEBUG RENDER BUFFERS
 	private ComputeBuffer renderPointBuffer;///this is just a float4 buffer
    private ComputeBuffer argPointBuffer;
    //private ComputeBuffer renderLineBuffer;///this is just a float4 buffer
    //private ComputeBuffer argLineBuffer;

    // Start is called before the first frame update
    void Start()
    {
    	Debug.Log("ANYTHING");
        var vertices = new List<Vector3>();
        var vertices4 = new List<Vector4>();
		var indices = new List<int>();

		float segLength = length/(float)verts;
		///make verts
        for (int i = 0; i < (int)verts; i++) {
        	float x = segLength*(float)i;
        	float nx = (2f*Mathf.PerlinNoise(x,0f))-1f;
        	float ny = (2f*Mathf.PerlinNoise(0f,x))-1f;
        	float nz = (2f*Mathf.PerlinNoise(x,x))-1f;
        	float mag = (x/length)*noiseMagnitude;
            vertices.Add(new Vector3(x, ny*mag, nz*mag));
            vertices4.Add(new Vector4(x, ny*mag, nz*mag,1.0f));
        }

        mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        //foreach( var human in vertices4.ToArray() )
 		//{
   		//	Debug.Log( human );
 		//}
        //////
        //////DEBUG
        ///point render
    	renderPointBuffer = new ComputeBuffer((int)verts, 4*sizeof(float) );///this is to hold positions to render as points
    	renderPointBuffer.SetData(vertices4.ToArray());
    	
    	argPointBuffer = new ComputeBuffer(1, 4 * sizeof(uint), ComputeBufferType.IndirectArguments);
        argPointBuffer.SetData(new uint[4] { verts, 1, 0, 0 });
        ////////////////////
        //int numLines = (int)verts-1;//
        //renderLineBuffer = new ComputeBuffer(numLines, 3*sizeof(float) );///this is to hold positions to render as points
    	//argLineBuffer = new ComputeBuffer(1, 4 * sizeof(uint), ComputeBufferType.IndirectArguments);
        //argLineBuffer.SetData(new uint[4] { (uint)numLines, 1, 0, 0 });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnDisable () 
	{
		ReleaseBuffers();
	}

    protected virtual void ReleaseBuffers(){//protected override 
        if(renderPointBuffer!=null)renderPointBuffer.Release();
        //renderLineBuffer.Release();

        if(argPointBuffer!=null)argPointBuffer.Release();
        //argLineBuffer.Release();
    }

    protected virtual void OnRenderObject()
    {
        //if(drawDebug)
        //{
           	//_computeShader.SetVector("playerPosition", Camera.main.transform.position);
        	if(drawPoints)
        	{
               	////draw the lines
               	debugMaterial.SetBuffer("renderPointBuffer", renderPointBuffer);//maybe only set this once?
        	    debugMaterial.SetPass(0);///this has to be set
        	    Graphics.DrawProceduralIndirectNow(MeshTopology.Points,argPointBuffer);
            }
            //if(drawLines)
            //{
        	//    debugMaterial.SetBuffer("renderPointBuffer", renderLineBuffer);
            //    debugMaterial.SetPass(0);///this has to be set
        	//    //Graphics.DrawProceduralIndirect(MeshTopology.Lines,argVerletPointBuffer);
        	//    Graphics.DrawProceduralIndirectNow(MeshTopology.Lines,argLineBuffer);
    		//}
        //}
    }

}
