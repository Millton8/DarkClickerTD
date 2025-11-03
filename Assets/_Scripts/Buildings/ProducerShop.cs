using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ProducerShop : MonoBehaviour
{
    [Header("Producers (in purchase order)")]
    public ProducerInfo[] availableProducers; // пор€док важен: [0] открыто сразу, [1] после покупки [0], etc.

    [Header("UI Buttons")]
    public Button[] producerButtons; // длина == availableProducers.Length

    [Header("Slots")]
    public ProducerSlot[] slots;

    private int nextProducerIndex = 0; // следующий по пор€дку, которого ещЄ не купили

    void Start()
    {
        // назначим слоты
        foreach (var slot in slots)
            slot.shop = this;

        // »нициализаци€ кнопок: только 0-€ видима, остальные скрыты
        for (int i = 0; i < producerButtons.Length; i++)
        {
            var go = producerButtons[i].gameObject;
            bool visible = (i == 0);
            go.SetActive(visible);

            if (visible)
                UpdateButtonText(i);
        }
    }

    /// <summary>
    /// »грок нажал на кнопку покупки/апгрейда дл€ типа availableProducers[idx].
    /// </summary>
    private void OnProducerButton(int idx)
    {
        // устанавливаем выбранный тип
        nextProducerIndex = idx;
        // дожидаемс€ клика по слоту
    }

    /// <summary>
    /// —лот сообщает, что по нему кликнули.
    /// </summary>
    public void TryPlaceOrUpgradeBuilding(ProducerSlot slot)
    {
        // берЄм инфо по текущему разрешЄнному типу
        var info = availableProducers[nextProducerIndex];

        if (!slot.isOccupied)
        {
            // покупка
            if (!ResourceManager.Instance.CanAfford(info.baseCost)) return;
            ResourceManager.Instance.SpendWood(info.baseCost);

            var obj = Instantiate(info.prefab, slot.transform.position, Quaternion.identity, slot.transform);
            var pb = obj.GetComponent<ProducerBuilding>();
            pb.info = info;
            slot.building = pb;
            slot.isOccupied = true;

            // после успешной покупки:
            // Ч пр€чем кнопку этого типа
            //producerButtons[nextProducerIndex].gameObject.SetActive(false);

            // заблокируем кнопку этого типа навсегда
            producerButtons[nextProducerIndex].interactable = false;

            // Ч показываем кнопку следующего типа (если есть)
            nextProducerIndex++;
            if (nextProducerIndex < producerButtons.Length)
            {
                producerButtons[nextProducerIndex].gameObject.SetActive(true);
                UpdateButtonText(nextProducerIndex);
            }
        }
        else
        {
            // апгрейд существующего
            if (!slot.building.Upgrade()) return;

            // обновл€ем цену на кнопке этого типа (если она видима)
            int idx = System.Array.IndexOf(availableProducers, slot.building.info);
            if (idx >= 0 && idx < producerButtons.Length && producerButtons[idx].gameObject.activeSelf)
                UpdateButtonText(idx);
        }
    }

    private void UpdateButtonText(int idx)
    {
        var info = availableProducers[idx];
        var btn = producerButtons[idx];
        var txt = btn.GetComponentInChildren<Text>();

        if (idx < nextProducerIndex)
        {
            // ”же куплен Ч текст дл€ апгрейда
            var slot = System.Array.Find(slots, s => s.isOccupied && s.building.info == info);
            if (slot != null)
                txt.text = $"{info.name}\nUpg: {slot.building.GetUpgradeCost()}";
        }
        else
        {
            // Ќе куплен Ч текст дл€ покупки
            txt.text = $"{info.name}\nCost: {info.baseCost}";
        }
    }
}