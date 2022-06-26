using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClassLockPanel : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
{
    public CharacterClass thisClass;

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        CharacterSelect.instance.ShowUnlockPanel(thisClass);

    }

    void IPointerExitHandler.OnPointerExit(PointerEventData data)
    {
        CharacterSelect.instance.HideUnlockPanel();

    }
}
