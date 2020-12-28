using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;


namespace Pathfinding {
	/// <summary>
	/// Sets the destination of an AI to the position of a specified object.
	/// This component should be attached to a GameObject together with a movement script such as AIPath, RichAI or AILerp.
	/// This component will then make the AI move towards the <see cref="target"/> set on this component.
	///
	/// See: <see cref="Pathfinding.IAstarAI.destination"/>
	///
	/// [Open online documentation to see images]
	/// </summary>
	[UniqueComponent(tag = "ai.destination")]
	[HelpURL("http://arongranberg.com/astar/docs/class_pathfinding_1_1_a_i_destination_setter.php")]
	public class AIDestinationSetter : VersionedMonoBehaviour {
		/// <summary>The object that the AI should move to</summary>
		
		
		public Transform target;
		IAstarAI ai;
		GameObject[] player;
		public int numberNPC=1;
		public float remainingToTarget;
		public byte numberTarget;

		//Object untuk menampilkan jarak Player yang diincar NPC, tampilan berada di samping kanan bawah
		Text remNPC;
		int debugMode, target_sekarang=-1;
		private void Awake()
		{
			
			//Membuat NPC menjadi child dari game object environment
			this.transform.SetParent(GameObject.Find("Environment").GetComponent<Transform>(), false);
		}
		private void Start()
		{
			//Initialing Object untuk menampilkan jarak Player yang diincar NPC, tampilan berada di samping kanan bawah
			remNPC = GameObject.Find("RemNPC" + numberNPC).GetComponent<Text>();
			debugMode = PlayerPrefs.GetInt("DebugerMode");
			numberTarget = 0;
		}
		private void FixedUpdate()
		{
			//Memilih player terdekat berdasarkan radius
			player = GameObject.FindGameObjectsWithTag("Player");
			//Debug.Log("Jumlah palyer ditemukan "+player.Length);
			if (player.Length == 1)
			{ // jika player hanya satu
				GantiTarget(0);
			}
			else if (player.Length == 2)
			{ // jika player berjumlah 2
				if (Jarak(gameObject.transform, player[0].transform) < Jarak(gameObject.transform, player[1].transform))
				{
					GantiTarget(0);
				}
				else if (Jarak(gameObject.transform, player[1].transform) < Jarak(gameObject.transform, player[0].transform))
				{
					GantiTarget(1);
				}

			}
			else if (player.Length == 3) // Jika player berjumlah 3
			{
				if (Jarak(gameObject.transform, player[0].transform) < Jarak(gameObject.transform, player[1].transform) && Jarak(gameObject.transform, player[0].transform) < Jarak(gameObject.transform, player[2].transform))
				{
					GantiTarget(0);             
				}
				else if (Jarak(gameObject.transform, player[1].transform) < Jarak(gameObject.transform, player[0].transform) && Jarak(gameObject.transform, player[1].transform) < Jarak(gameObject.transform, player[2].transform))
				{
					GantiTarget(1);             
				}
				else if (Jarak(gameObject.transform, player[2].transform) < Jarak(gameObject.transform, player[0].transform) && Jarak(gameObject.transform, player[2].transform) < Jarak(gameObject.transform, player[1].transform))
				{
					GantiTarget(2);             
				}
			}
			else {
				ai.canMove = false;
			}
			
		}
		void GantiTarget(byte i) {
			if (target_sekarang != i)
			{
				target = player[i].GetComponent<Transform>();
				numberTarget = i;
			}
		}
		//Fungsi untuk menentukan jarak terdekat berdasarkan radius dari NPC ke player
		double Jarak(Transform position1, Transform position2)
		{
			double a1 = position1.position.x;
			double b1 = position1.position.z;
			double a2 = position2.position.x;
			double b2 = position2.position.z;
			double tmp = b1 - b2;
			double dist = (Math.Sin(a1*Mathf.Deg2Rad) * Math.Sin(a2* Mathf.Deg2Rad)) + (Math.Cos(a1 * Mathf.Deg2Rad) * Math.Cos(a2 * Mathf.Deg2Rad) * Math.Cos(tmp * Mathf.Deg2Rad));
			dist = Math.Acos(dist);
			dist = dist*Mathf.Rad2Deg;
			
			return (dist);
		}
		void OnEnable () {
			ai = GetComponent<IAstarAI>();
			
			// Update the destination right before searching for a path as well.
			// This is enough in theory, but this script will also update the destination every
			// frame as the destination is used for debugging and may be used for other things by other
			// scripts as well. So it makes sense that it is up to date every frame.
			if (ai != null) ai.onSearchPath += Update;
			
		}

		void OnDisable () {
			if (ai != null) ai.onSearchPath -= Update;
			
		}

		/// <summary>Updates the AI's destination every frame</summary>
		void Update () {
			if (target != null && ai != null) ai.destination = target.position;

			//Bagian ini untuk memberitahu ke bagian multi player : jarak Player dengan NPC
			remainingToTarget = ai.remainingDistance;
			//Debug.Log(gameObject.name+" : "+ ai.remainingDistance);

			//Hanya menampilkan jarak dalam mode debug
			if (debugMode == 1)
			{
				remNPC.text = remainingToTarget + " : Robot" + numberNPC;
            }
			//target.gameObject.GetComponent<NameCharacter>();
		}
	}
}
