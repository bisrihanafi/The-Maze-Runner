using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

using Photon.Pun;
using Photon.Realtime;


namespace Hanafi
{
	[System.Serializable]
	public class Sel
	{
		public bool dikunjungi;
		public GameObject atas;
		public GameObject kanan;
		public GameObject kiri;
		public GameObject bawah;
	}

	public class MazeGenerator : MonoBehaviourPunCallbacks
	{
		private int baris;
		private int kolom;
		[SerializeField] GameObject tembok;
		[SerializeField] GameObject endpoint;
		[SerializeField] GameObject finder;
		[SerializeField] NavMeshSurface surface;
		public Sel[] sel;
		List<int> selList;

		private Vector3 titikAwal;
		private int selSkrg = 0;
		private int totalSel;
		private int tetanggaSkrg = 0;
		private int cadangan = 0;
		private float panjangTembok = 4.0f;
		private GameObject tembokHolder, tempTembok, endPointObject;

		// Start is called before the first frame update
		private void Start()
		{
			
		}
		// Update is called once per frame
		void Update()
		{
			//surface.BuildNavMesh();
		}

		public void GenerateLabirinBaru()
		{
			GameObject labirinSebelumnya = GameObject.FindGameObjectWithTag("Tembok");
			GameObject finishSebelumnya = GameObject.FindGameObjectWithTag("Finish");
			GameObject finderSebelumnya = GameObject.FindGameObjectWithTag("Finder");
			try
			{
				totalSel = baris * kolom;

				if (labirinSebelumnya != null)
					PhotonNetwork.Destroy(labirinSebelumnya);
				if (finishSebelumnya != null)
					PhotonNetwork.Destroy(finishSebelumnya);
				if (finderSebelumnya != null)
					PhotonNetwork.Destroy(finderSebelumnya);


				BuatTembok();
				GenerateEndPoint();
				//SpawnFinder();
			}
			catch (System.FormatException e)
			{
				Debug.Log(e);
			}
		}

		//membuat gameobject tembok berdasarkan baris dan kolom
		public void BuatTembok()
		{
			
			tembokHolder = new GameObject();
			tembokHolder.name = "Tembok";
			tembokHolder.tag = "Tembok";
			tembokHolder.transform.SetParent(GameObject.Find("Environment").GetComponent<Transform>(), false);
			
			titikAwal = new Vector3((-kolom / 1) + panjangTembok / 2, 0.5f, (-baris / 1) + panjangTembok / 1);
			Vector3 myPos = titikAwal;

			//membuat kolom
			for (int a = 0; a < baris; a++)
			{
				for (int b = 0; b <= kolom; b++)
				{
					myPos = new Vector3(titikAwal.x + (b * panjangTembok) - panjangTembok / 2, 0.5f, titikAwal.z + (a * panjangTembok) - panjangTembok / 2);
					tempTembok = PhotonNetwork.InstantiateSceneObject(
						this.tembok.name, 
						myPos, 
						Quaternion.Euler(0, 90, 0)
						) as GameObject;
					tempTembok.name = "kolom " + a + ", " + b;
					tempTembok.transform.parent = tembokHolder.transform;
				}
			}

			//membuat baris
			for (int a = 0; a <= baris; a++)
			{
				for (int b = 0; b < kolom; b++)
				{
					myPos = new Vector3(titikAwal.x + (b * panjangTembok), 0.5f, titikAwal.z + (a * panjangTembok) - panjangTembok);
					tempTembok = PhotonNetwork.InstantiateSceneObject(
						this.tembok.name, 
						myPos, 
						Quaternion.identity
						) as GameObject;
					tempTembok.name = "baris " + a + ", " + b;
					tempTembok.transform.parent = tembokHolder.transform;
				}
			}
			
			BuatSel();
		}

		public void BuatSel()
		{
			selList = new List<int>();
			int children = tembokHolder.transform.childCount;
			GameObject[] semuaTembok = new GameObject[children];
			sel = new Sel[totalSel];

			int prosesKananKiri = 0;
			int prosesChild = 0;
			int hitungTerm = 0;
			int prosesSel = 0;

			//memasukkan semua tembok ke array
			for (int i = 0; i < children; i++)
			{
				semuaTembok[i] = tembokHolder.transform.GetChild(i).gameObject;
			}

			//membentuk tembok dalam sel
			for (int j = 0; j < kolom; j++)
			{
				sel[prosesSel] = new Sel();

				sel[prosesSel].kiri = semuaTembok[prosesKananKiri];
				sel[prosesSel].bawah = semuaTembok[prosesChild + (kolom + 1) * baris];
				hitungTerm++;
				prosesChild++;
				sel[prosesSel].atas = semuaTembok[(prosesChild + (kolom + 1) * baris) + kolom - 1];
				prosesKananKiri++;
				sel[prosesSel].kanan = semuaTembok[prosesKananKiri];

				prosesSel++;
				if (hitungTerm == kolom && prosesSel < sel.Length)
				{
					prosesKananKiri++;
					hitungTerm = 0;
					j = -1;
				}
			}
			BuatLabirin();
		}

		void BuatLabirin()
		{
			bool mulaiBuat = false;
			int selDikunjungi = 0;
			while (selDikunjungi < totalSel)
			{
				if (mulaiBuat)
				{
					AmbilTetangga();
					if (!sel[tetanggaSkrg].dikunjungi && sel[selSkrg].dikunjungi)
					{
						int tetanggaAcak = Random.Range(0, 5);
						sel[tetanggaSkrg].dikunjungi = true;
						selDikunjungi++;
						selList.Add(selSkrg);
						selSkrg = tetanggaSkrg;

						if (selList.Count > 0)
							cadangan = selList.Count - 1;
					}
				}
				else
				{
					selSkrg = Random.Range(0, totalSel);
					sel[selSkrg].dikunjungi = true;
					selDikunjungi++;
					mulaiBuat = true;
				}
			}

		}
		
		//mengambil tetangga acak untuk dikunjungi
		void AmbilTetangga()
		{
			int panjang = 0;
			int[] tetangga = new int[4];
			int[] hubTembok = new int[4];
			int cek = 0;
			cek = (selSkrg + 1) / kolom;
			cek -= 1;
			cek *= kolom;
			cek += kolom;

			//atas
			if (selSkrg + kolom < totalSel)
			{
				if (sel[selSkrg + kolom].dikunjungi == false)
				{
					tetangga[panjang] = selSkrg + kolom;
					hubTembok[panjang] = 1;
					panjang++;
				}
			}

			//kanan
			if (selSkrg + 1 < totalSel && (selSkrg + 1) != cek)
			{
				if (sel[selSkrg + 1].dikunjungi == false)
				{
					tetangga[panjang] = selSkrg + 1;
					hubTembok[panjang] = 2;
					panjang++;
				}
			}

			//kiri
			if (selSkrg - 1 >= 0 && selSkrg != cek)
			{
				if (sel[selSkrg - 1].dikunjungi == false)
				{
					tetangga[panjang] = selSkrg - 1;
					hubTembok[panjang] = 3;
					panjang++;
				}
			}

			//bawah
			if (selSkrg - kolom >= 0)
			{
				if (sel[selSkrg - kolom].dikunjungi == false)
				{
					tetangga[panjang] = selSkrg - kolom;
					hubTembok[panjang] = 4;
					panjang++;
				}
			}

			//mengambil tetangga acak dan menghapus tembok
			if (panjang != 0)
			{
				int tetanggaAcak = Random.Range(0, panjang);
				tetanggaSkrg = tetangga[tetanggaAcak];
				HapusTembok(hubTembok[tetanggaAcak]);
			}
			else if (cadangan > 0)
			{
				selSkrg = selList[cadangan];
				cadangan--;
			}
		}

		void HapusTembok(int tetangga)
		{
			
			switch (tetangga)
			{
				case 1:
					PhotonNetwork.Destroy(sel[selSkrg].atas);
					break;
				case 2:
					PhotonNetwork.Destroy(sel[selSkrg].kanan);
					break;
				case 3:
					PhotonNetwork.Destroy(sel[selSkrg].kiri);
					break;
				case 4:
					PhotonNetwork.Destroy(sel[selSkrg].bawah);
					break;
				default:
					break;
			}
			
		}

		void GenerateEndPoint()
		{
			int children = tembokHolder.transform.childCount;
			GameObject[] semuaTembok = new GameObject[children];
			Vector3[] posTembok = new Vector3[children];
			for (int i = 0; i < children; i++)
			{
				semuaTembok[i] = tembokHolder.transform.GetChild(i).gameObject;
				posTembok[i] = semuaTembok[i].transform.position;
			}

			int maxX(Vector3[] array)
			{
				int nilaiMax = (int)array[0].x;
				for (int i = 1; i < array.Length; i++)
				{
					if (array[i].x > nilaiMax)
					{
						nilaiMax = (int)array[i].x;
					}
				}
				return nilaiMax;
			}

			int minX(Vector3[] array)
			{
				int nilaiMin = (int)array[0].x;
				for (int i = 1; i < array.Length; i++)
				{
					if (array[i].x < nilaiMin)
					{
						nilaiMin = (int)array[i].x;
					}
				}
				return nilaiMin;
			}

			int maxZ(Vector3[] array)
			{
				int nilaiMax = (int)array[0].z;
				for (int i = 1; i < array.Length; i++)
				{
					if (array[i].z > nilaiMax)
					{
						nilaiMax = (int)array[i].z;
					}
				}
				return nilaiMax;
			}

			int minZ(Vector3[] array)
			{
				int nilaiMin = (int)array[0].z;
				for (int i = 1; i < array.Length; i++)
				{
					if (array[i].z < nilaiMin)
					{
						nilaiMin = (int)array[i].z;
					}
				}
				return nilaiMin;
			}

			int posMaxX = maxX(posTembok);
			int posMinX = minX(posTembok);
			int posMaxZ = maxZ(posTembok);
			int posMinZ = minZ(posTembok);
			Debug.Log("x max = " + posMaxX + ", x min = " + posMinX);
			Debug.Log("z max = " + posMaxZ + ", z min = " + posMinZ);

			int posAkhirX = Random.Range(posMinX + 1, posMaxX - 1);
			int posAkhirZ = Random.Range(posMinZ + 1, posMaxZ - 1);
			Vector3Int posAkhir = new Vector3Int(0, 0, 0);
			int iPos = 0;
			while (iPos < posTembok.Length)
			{
				if (posAkhirX == posTembok[iPos].x || posAkhirZ == posTembok[iPos].z)
				{
					Debug.Log("sebelum = " + posAkhirX + " " + posAkhirZ);
					Debug.Log("i = " + posTembok[iPos]);
					RandomPos();
					Debug.Log("sesudah = " + posAkhirX + " " + posAkhirZ);
					iPos = 0;
				}
				else
					posAkhir = new Vector3Int(posAkhirX, 0, posAkhirZ);
				iPos++;
			}
			void RandomPos()
			{
				posAkhirX = Random.Range(posMinX, posMaxX);
				posAkhirZ = Random.Range(posMinZ, posMaxZ);
			}
			endPointObject = PhotonNetwork.InstantiateSceneObject(this.endpoint.name, posAkhir, Quaternion.identity) as GameObject;
			endPointObject.name = "Finish";
			endPointObject.tag = "Finish";
		}

		void SpawnFinder()
		{
			GameObject finderObject = Instantiate(finder, new Vector3(1, 1, 1), Quaternion.identity) as GameObject;
			finderObject.name = "Finder";
			finderObject.tag = "Finder";
		}

		public void Simpel()
		{
			baris = 8;
			kolom = 8;
			GenerateLabirinBaru();
		}

		public void Sedang()
		{
			baris = 12;
			kolom = 12;
			GenerateLabirinBaru();
		}

		public void Kompleks()
		{
			baris = 16;
			kolom = 16;
			GenerateLabirinBaru();
		}
		
	}
}