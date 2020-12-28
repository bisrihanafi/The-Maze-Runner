using UnityEngine;
using UnityEngine.UI;


using Photon.Pun;
using Photon.Realtime;


using System.Collections;
using Pathfinding;


namespace Hanafi
{
    /// <summary>
    /// Bidang masukan nama pemain. Biarkan pengguna memasukkan namanya, akan muncul di atas pemain dalam game.
    /// </summary>
    [RequireComponent(typeof(InputField))]
    public class PlayerNameInputField : MonoBehaviour
    {
        #region Private Constants
        // Simpan Kunci PlayerPref untuk menghindari kesalahan ketik
        const string playerNamePrefKey = "PlayerName";
        #endregion
        #region MonoBehaviour CallBacks
        // Metode MonoBehaviour memanggil GameObject oleh Unity selama fase inisialisasi.
        void Start()
        {
            string defaultName = string.Empty;
            InputField _inputField = this.GetComponent<InputField>();
            if (_inputField != null)
            {
                if (PlayerPrefs.HasKey(playerNamePrefKey))
                {
                    defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                    _inputField.text = defaultName;
                }
            }
            PhotonNetwork.NickName = defaultName;
        }
        #endregion
        #region Public Methods
        // Menetapkan nama pemain, dan menyimpannya di PlayerPrefs untuk sesi selanjutnya.
        // <param name = "value"> Nama Pemain </param>
        public void SetPlayerName(string value)
        {
            // #Important
            if (string.IsNullOrEmpty(value)){return;}
            PhotonNetwork.NickName = value;
            PlayerPrefs.SetString(playerNamePrefKey, value);
        }
        #endregion
    }
}
