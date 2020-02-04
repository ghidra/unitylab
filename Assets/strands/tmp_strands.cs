using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class tmp_strands : MonoBehaviour
{
	private Mesh mesh;
    public Material strandMaterial;
    private ComputeBuffer renderPointBuffer;//

    public struct renderPoint{
        public Vector3 P;
        public Vector3 tangent;
        public Vector2 uv;
    }

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
 	private ComputeBuffer debugRenderPointBuffer;///this is just a float4 buffer
    private ComputeBuffer argPointBuffer;
    //private ComputeBuffer renderLineBuffer;///this is just a float4 buffer
    //private ComputeBuffer argLineBuffer;

    // Start is called before the first frame update
    void Start()
    {
    	//Debug.Log("ANYTHING");
        var vertices = new List<Vector3>();
        var vertices4 = new List<Vector4>();
        //var uvs = new List<Vector2>();
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

            ///uv
            //float uvx = (float)i/((float)verts-1);
            //uvs.Add(new Vector2(uvx,0f));

            ///make a crappy triangle to see if that matters.
            if(i>0 && i<(int)verts-1)
            {
                indices.Add(i-1);
                indices.Add(i);
                indices.Add(i+1);
            }
        }

        mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        //mesh.uv = uvs.ToArray();
        mesh.triangles = indices.ToArray();

       
        //foreach( var human in vertices4.ToArray() )
 		//{
   		//	Debug.Log( human );
 		//}
        //////
        //////DEBUG
        ///point render
    	debugRenderPointBuffer = new ComputeBuffer((int)verts, 4*sizeof(float) );///this is to hold positions to render as points
    	debugRenderPointBuffer.SetData(vertices4.ToArray());
    	
    	argPointBuffer = new ComputeBuffer(1, 4 * sizeof(uint), ComputeBufferType.IndirectArguments);
        argPointBuffer.SetData(new uint[4] { verts, 1, 0, 0 });
        ////////////////////
        //int numLines = (int)verts-1;//
        //renderLineBuffer = new ComputeBuffer(numLines, 3*sizeof(float) );///this is to hold positions to render as points
    	//argLineBuffer = new ComputeBuffer(1, 4 * sizeof(uint), ComputeBufferType.IndirectArguments);
        //argLineBuffer.SetData(new uint[4] { (uint)numLines, 1, 0, 0 });

        ///give that point buffer to the surface material
         ////give the mesh to the thing;
        GetComponent<MeshFilter>().mesh = mesh;
        if(strandMaterial!=null)
        {
            strandMaterial.SetBuffer("renderPointBuffer", debugRenderPointBuffer);
            GetComponent<MeshRenderer>().material = strandMaterial;
        }
        
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
        if(debugRenderPointBuffer!=null)debugRenderPointBuffer.Release();
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
               	debugMaterial.SetBuffer("renderPointBuffer", debugRenderPointBuffer);//maybe only set this once?
        	    debugMaterial.SetPass(0);///this has to be set
        	    Graphics.DrawProceduralIndirectNow(MeshTopology.Points,argPointBuffer);
            }
            //if(drawLines)
            //{
        	//    debugMaterial.SetBuffer("debugRenderPointBuffer", renderLineBuffer);
            //    debugMaterial.SetPass(0);///this has to be set
        	//    //Graphics.DrawProceduralIndirect(MeshTopology.Lines,argVerletPointBuffer);
        	//    Graphics.DrawProceduralIndirectNow(MeshTopology.Lines,argLineBuffer);
    		//}
        //}
    }

}
