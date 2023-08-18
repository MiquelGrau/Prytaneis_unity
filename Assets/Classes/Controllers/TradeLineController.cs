using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TradeLineController : MonoBehaviour
{
    public TMP_Text resourceNameText;
    public TMP_Text resourceQuantityText;
    public TMP_Text resourcePriceText;
    public Button buyButton; 

    private void Start()
    {
        buyButton.onClick.AddListener(OnBuyButtonClick); 
    }
    private void OnBuyButtonClick()
    {
        // TO DO: codi per comprar recurs
        Debug.Log("Botó de compra premut");
    }

    // Informació per el prefab de recursos
    public void Setup(InventoryList.InventoryItem item)
    {
        Resource resource = ResourceManager.GetResourceById(item.resourceID);
        if (resource != null)
        {
            resourceNameText.text = resource.resourceName;
            resourceQuantityText.text = item.quantity.ToString();
            resourcePriceText.text = item.currentPrice.ToString();
        }
        else
        {
            Debug.LogError("Error: No s'ha pogut trobar el recurs amb la ID " + item.resourceID);
        }
    }
}



