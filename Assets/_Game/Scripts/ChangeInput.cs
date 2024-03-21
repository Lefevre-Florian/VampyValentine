using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Com.IsartDigital.Platformer.BDD
{
    public class ChangeInput : MonoBehaviour
    {
        EventSystem system;
        public Selectable firstInput;
        public Button submitButton;

        // Start is called before the first frame update
        void Start()
        {
#if UNITY_STANDALONE
            system = EventSystem.current;
            firstInput.Select();
#endif
        }

        // Update is called once per frame
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Tab) && Input.GetKey(KeyCode.LeftShift)) 
            {
                Selectable previous = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp();
                if(previous != null)
                {
                    previous.Select();
                }
            }
            else if(Input.GetKeyDown(KeyCode.Tab))
            {
                Selectable next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
                if(next != null)
                {
                    next.Select();
                }
            }
            else if(Input.GetKeyDown(KeyCode.Return)) 
            {
                submitButton.onClick.Invoke();
            }
        }

        public void SelectFirst()
        {
#if UNITY_STANDALONE
            system = EventSystem.current;
            firstInput.Select();
#endif
        }
    }
}
