using UnityEngine;
using UnityEngine.EventSystems;

public class CastleSelector : MonoBehaviour
{
    public ClickUpgradeUI clickUpgradeUI;
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {

            // Сначала проверяем, не кликнули ли мы по UI
            //if (EventSystem.current.IsPointerOverGameObject())
            if (EventSystem.current.currentSelectedGameObject != null)
            {
                // Если клик по UI, ничего не делаем (не скрываем панель)
                return;
            }

            // Если клик не по UI - пускаем Raycast в игровое пространство
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // Проверяем, попали ли в замок

                Castle playerCastle = hit.collider.GetComponent<Castle>();  // попробовать поменять название объекта замка, чтобы не совпадало с классом Castle
                if (hit.transform.CompareTag("Castle"))
                // if (playerCastle != null)
                {
                    // Показать панель улучшений для замка
                    clickUpgradeUI.ShowUI(playerCastle);
                }
                else
                {
                    // Если это не замок, скрываем панель улучшений
                    clickUpgradeUI.HidePlayerUI();
                }
            }
            else
            {
                // Если луч никуда не попал, тоже скрываем панель
                clickUpgradeUI.HidePlayerUI();
            }
        }
    }
}
