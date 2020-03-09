using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Runtime.InteropServices;
using UnityEngine.Rendering;

public class voxelConeTracingVisualizer : MonoBehaviour
{
	public CustomRenderTexture mVoxelTexture;

	private RenderTexture mVoxelTex;
	public uint mVoxelTexSize=64;
	public ComputeShader mWriteToTextureShader;
	private int writeKernelID;
	private int writeWarpCount;

	public ComputeShader mTextureToBufferTransferShader;
	public Material mVisualizeMaterial;
	public bool mDrawPoints = true;

	private int transferKernelID;
    private int transferWarpCount;
    private const int V_WARP_SIZE = 256;

	private int _w;
	private int _h;
	private int _d;
	private int _numVoxels;
    
    //lets make a buffer that is going to make a point per pixel
    private ComputeBuffer voxelRepresentationBuffer;
    private ComputeBuffer argPointBuffer;

    public struct voxelPoint{
        public Vector3 P;
        public Vector3 Cd;
        public Vector2 dummy;
    }

    void Start()
    {
    	if(mVoxelTexture!=null)
    	{
	        //get the size of the texture so that we know how to parse it
	        _w = mVoxelTexture.width;
	        _h = mVoxelTexture.height;
	    	_d = mVoxelTexture.volumeDepth;
	    	_numVoxels = _w*_h*_d;
	    	//print(_numVoxels);

        	///setup the compute shader that does the transfer from texture to buffer



	    }else{
	    	//////////////////////////////////////////////////////////////////////////////////
			/// MAKE THE VOXEL 3D TEXTURE
			//////////////////////////////////////////////////////////////////////////////////
			_w = (int)mVoxelTexSize;
	        _h = (int)mVoxelTexSize;
	    	_d = (int)mVoxelTexSize;
	    	_numVoxels = _w*_h*_d;
			/*Color[] colorArray = new Color[_numVoxels];
	        mVoxelTex = new Texture3D (_w, _h, _d, TextureFormat.RGBA32, true);
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
*/
	        mVoxelTex = new RenderTexture(_w,_h,16, RenderTextureFormat.ARGB32);
	        mVoxelTex.volumeDepth = _d;
	        mVoxelTex.dimension = TextureDimension.Tex3D;
	        mVoxelTex.enableRandomWrite=true;

	        
	        if(mWriteToTextureShader!=null){

	        	print("----------------------------");
	        	print(mVoxelTex.dimension);
		        ///set up and write the first pass of color into this texture from a compute shader
		        writeKernelID = mWriteToTextureShader.FindKernel("CSWriteTexture");
	        	writeWarpCount = Mathf.CeilToInt((float)_numVoxels / V_WARP_SIZE);
	        	mWriteToTextureShader.SetTexture(writeKernelID,"VoxelTexture",mVoxelTex);
	        	mWriteToTextureShader.SetVector("voxelDimensions",new Vector3(_w,_h,_d));
	        	//dispatch this guy
	        	mWriteToTextureShader.Dispatch(writeKernelID, writeWarpCount, 1, 1);
	        }
	    }

	    /////NOW SET UP THE COMPUTE JUNK

	    voxelRepresentationBuffer = new ComputeBuffer(_numVoxels, Marshal.SizeOf(typeof(voxelPoint)) );///this is to hold positions to render as points
	    	
    	argPointBuffer = new ComputeBuffer(1, 4 * sizeof(uint), ComputeBufferType.IndirectArguments);
    	argPointBuffer.SetData(new uint[4] { (uint)_numVoxels, 1, 0, 0 });

    	if(mTextureToBufferTransferShader!=null)
    	{
    		transferKernelID = mTextureToBufferTransferShader.FindKernel("CStransfer");
        	transferWarpCount = Mathf.CeilToInt((float)_numVoxels / V_WARP_SIZE);

        	if(mVoxelTexture!=null)
    		{
    			mTextureToBufferTransferShader.SetTexture(transferKernelID,"VoxelTexture",mVoxelTexture);
			}else{
				mTextureToBufferTransferShader.SetTexture(transferKernelID,"VoxelTexture",mVoxelTex);
			}
    		mTextureToBufferTransferShader.SetBuffer(transferKernelID,"transferBuffer",voxelRepresentationBuffer);
    		mTextureToBufferTransferShader.SetVector("voxelDimensions",new Vector3(_w,_h,_d));
    	}
    	////give the debug material the buffer to draw
    	if(mVisualizeMaterial!=null)
		{
			mVisualizeMaterial.SetBuffer("renderPointBuffer", voxelRepresentationBuffer);//maybe only set this once?
		}

    }

    // Update is called once per frame
    void Update()
    {
    	if(mTextureToBufferTransferShader!=null)
    	{
	    	///transfer from texture to buffer for drawing of points
	        mTextureToBufferTransferShader.Dispatch(transferKernelID, transferWarpCount, 1, 1);
	    }
    }
    void OnDisable () 
	{
		if(voxelRepresentationBuffer!=null)voxelRepresentationBuffer.Release();
        if(argPointBuffer!=null)argPointBuffer.Release();
	}

    protected virtual void OnRenderObject()
    {
    	if(mDrawPoints)
    	{
    		//mVisualizeMaterial.SetBuffer("renderPointBuffer", voxelRepresentationBuffer);
    	    mVisualizeMaterial.SetPass(0);///this has to be set
    	    Graphics.DrawProceduralIndirectNow(MeshTopology.Points,argPointBuffer);
        }
    }
}
