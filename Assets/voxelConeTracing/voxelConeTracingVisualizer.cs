using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Runtime.InteropServices;

public class voxelConeTracingVisualizer : MonoBehaviour
{
	public CustomRenderTexture mVoxelTexture;
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

	    	print(_numVoxels);

			voxelRepresentationBuffer = new ComputeBuffer(_numVoxels, Marshal.SizeOf(typeof(voxelPoint)) );///this is to hold positions to render as points
	    	
	    	argPointBuffer = new ComputeBuffer(1, 4 * sizeof(uint), ComputeBufferType.IndirectArguments);
        	argPointBuffer.SetData(new uint[4] { (uint)_numVoxels, 1, 0, 0 });

        	///setup the compute shader that does the transfer from texture to buffer
	    	if(mTextureToBufferTransferShader!=null)
	    	{
	    		transferKernelID = mTextureToBufferTransferShader.FindKernel("CStransfer");
            	transferWarpCount = Mathf.CeilToInt((float)_numVoxels / V_WARP_SIZE);

	    		mTextureToBufferTransferShader.SetTexture(transferKernelID,"VoxelTexture",mVoxelTexture);
	    		mTextureToBufferTransferShader.SetBuffer(transferKernelID,"transferBuffer",voxelRepresentationBuffer);
	    		mTextureToBufferTransferShader.SetVector("voxelDimensions",new Vector3(_w,_h,_d));
	    	}
	    	////give the debug material the buffer to draw
	    	if(mVisualizeMaterial!=null)
    		{
    			mVisualizeMaterial.SetBuffer("renderPointBuffer", voxelRepresentationBuffer);//maybe only set this once?
    		}
	    }
    }

    // Update is called once per frame
    void Update()
    {
    	///transfer from texture to buffer for drawing of points
        mTextureToBufferTransferShader.Dispatch(transferKernelID, transferWarpCount, 1, 1);
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
