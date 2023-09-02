using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceObject : MonoBehaviour
{

//Initialization of the PieceObect pool
#if UNITY_EDITOR
	static List<Stack<PieceObject>> pools;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	static void ClearPools()
	{
		if (pools == null)
		{
			pools = new();
		}
		else
		{
			for (int i = 0; i < pools.Count; i++)
			{
				pools[i].Clear();
			}
		}
	}
#endif

	[System.NonSerialized]
	Stack<PieceObject> pool; //the said pool

	[SerializeField]
	float
		startX, startY; //The translation to put the piece above the grid

	[SerializeField]
	GameObject CenterRotation; //The cube of the piece considered as the center for the rotation


	//Make an instance of the piece object
	public PieceObject GetInstance()
	{
		//Add to the pool or create the pool if no pool
		if (pool == null)
		{
			pool = new();
#if UNITY_EDITOR
			pools.Add(pool);
#endif
		}
		//if already exists just set active
		if (pool.TryPop(out PieceObject instance))
		{
			instance.gameObject.SetActive(true);
		}
		//if not instantiate it
		else
		{
			instance = Instantiate(this);
			instance.pool = pool;
		}
		return instance;
	}

	//Make the piece above the grid to enter in the game
	public void GetInGame()
    {
		this.transform.Translate(startX, startY, 0f);
    }

	//Make the translation of the piece 
	public void Translate(float x)
    {
		transform.position += new Vector3(x, 0f, 0f);
	}

	//Make the rotation of the piece
	public void Rotate(float r)
    {
		transform.RotateAround(CenterRotation.transform.position, new Vector3(0, 0, 1), r*90);
    }

	//Make the piece fall
	public void Fall(float y)
    {
		transform.position+= new Vector3(0f, y, 0f);
    }
}
