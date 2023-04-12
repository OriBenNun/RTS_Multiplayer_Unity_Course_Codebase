using Mirror;
using UnityEngine;

namespace Menus
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private GameObject landingPanel;

        public void HostLobby()
        {
            landingPanel.SetActive(false);
            
            NetworkManager.singleton.StartHost();
        }
    }
}
