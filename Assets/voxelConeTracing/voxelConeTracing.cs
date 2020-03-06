using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Runtime.InteropServices;

public class voxelConeTracing : MonoBehaviour
{
	public ComputeShader mVoxelizationShader;
	public ComputeShader mVoxelizationResizeShader;
	public ComputeShader mClearVoxelizationShader;///used to clear out the last frame?
	public ComputeShader mClearTriangleShader;

	public struct Voxel{
		public Vector3 Cd;
		public float Alpha;
	}

	private uint mVoxelTexSize = 64;
	private float mLastTime;
	private uint mNumVoxels;
	private uint mNumTris;

	private ComputeBuffer mVoxelBuffer;
	private ComputeBuffer mVoxelResizeBuffer;
	private ComputeBuffer mTriangleBuffer;

	private Texture3D mVoxelTex;

	private Mesh mesh;///this will hold my VBO, the triangles that will draw into the voxel buffer

    void Start()
    {
    	mNumVoxels = mVoxelTexSize*mVoxelTexSize*mVoxelTexSize;

    	//////////////////////////////////////////////////////////////////////////////////
		/// VOXEL DATA
		//////////////////////////////////////////////////////////////////////////////////
        Voxel[] emptyVoxels= new Voxel[mNumVoxels];
        for(uint i = 0; i<mNumVoxels; i++)
        {
        	emptyVoxels[i].Cd = new Vector3(0f,0f,0f);
        	emptyVoxels[i].Alpha = 0f;
        }

        uint vts1 = mVoxelTexSize/2;
    	uint vts2 = vts1/2;
    	uint vts3 = vts2/2;
    	uint numResizeVoxels = (vts1*vts1*vts1)+(vts2*vts2*vts2)+(vts3*vts3*vts3);//32768+4096+512=37376

    	Voxel[] emptyResizeVoxels= new Voxel[numResizeVoxels];
    	for(uint i = 0; i<numResizeVoxels; i++)
        {
        	emptyResizeVoxels[i].Cd = new Vector3(0f,0f,0f);
        	emptyResizeVoxels[i].Alpha = 0f;
        }
        //////////////////////////////////////////////////////////////////////////////////
		/// MAKE THE VOXEL BUFFER
		//////////////////////////////////////////////////////////////////////////////////
		mVoxelBuffer = new ComputeBuffer((int)mNumVoxels, Marshal.SizeOf(typeof(Voxel)) );
		mVoxelBuffer.SetData(emptyVoxels);

		//////////////////////////////////////////////////////////////////////////////////
	    /// MAKE THE VOXEL RESIZE BUFFER
	    //////////////////////////////////////////////////////////////////////////////////
	    mVoxelResizeBuffer = new ComputeBuffer((int)numResizeVoxels, Marshal.SizeOf(typeof(Voxel)) );
	    mVoxelResizeBuffer.SetData(emptyResizeVoxels);
		//////////////////////////////////////////////////////////////////////////////////
		/// MAKE THE VOXEL 3D TEXTURE
		//////////////////////////////////////////////////////////////////////////////////
		Color[] colorArray = new Color[mNumVoxels];
        mVoxelTex = new Texture3D ((int)mVoxelTexSize, (int)mVoxelTexSize, (int)mVoxelTexSize, TextureFormat.RGBA32, true);
        float r = 1.0f / (mVoxelTexSize - 1.0f);
        for (uint x = 0; x < mVoxelTexSize; x++) {
            for (uint y = 0; y < mVoxelTexSize; y++) {
                for (uint z = 0; z < mVoxelTexSize; z++) {
                    Color c = new Color (x * r, y * r, z * r, 1.0f);
                    colorArray[x + (y * mVoxelTexSize) + (z * mVoxelTexSize * mVoxelTexSize)] = c;
                }
            }
        }
        mVoxelTex.wrapMode = TextureWrapMode.Repeat;
        mVoxelTex.SetPixels(colorArray);
        mVoxelTex.Apply ();
        //////////////////////////////////////////////////////////////////////////////////
	    /// MAKE ATOMIC COUNTER BUFFER... THIS WILL MAKE IT SO I CAN ADD SEQUENTIALLY TO MY TRIANGLE BUFFER, MAYBE SKIPPING FOR NOW
	    //////////////////////////////////////////////////////////////////////////////////

	    //////////////////////////////////////////////////////////////////////////////////
        /// MAKE VBO FOR DRAWIN TRIANGLES FROM SSBO
	    //////////////////////////////////////////////////////////////////////////////////
	    uint mNumTris = 65536;//1048576;//2097152

	    var vertices = new List<Vector3>();
		var indices = new List<int>();

		/*float segLength = length/(float)verts;
		///make verts
        for (uint i = 0; i < mNumTris*3; i++) {
        	float x = segLength*(float)i;
        	float nx = (2f*Mathf.PerlinNoise(x,0f))-1f;
        	float ny = (2f*Mathf.PerlinNoise(0f,x))-1f;
        	float nz = (2f*Mathf.PerlinNoise(x,x))-1f;
        	float mag = (x/length)*noiseMagnitude;
            vertices.Add(new Vector3(x, ny*mag, nz*mag));

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
        }*/

        mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = indices.ToArray();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
