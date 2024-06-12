using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class TradeInterface : MonoBehaviour
{
    public TradeManager tradeManager; 
    public GameObject tradeRLPrefab; 
    public Transform tradeRLContainer; 
    
    // Afegir referències a TextMeshPro Texts
    public TMP_Text moneyLeftText;
    public TMP_Text moneyRightText;

    // Posicions de capçalera
    public TMP_Text TILeftMoneyStart;
    public TMP_Text TILeftMoneyMid;
    public TMP_Text TILeftMoneyEnd;
    public TMP_Text TILeftWaresStart;
    public TMP_Text TILeftWaresMid;
    public TMP_Text TILeftWaresEnd;
    public TMP_Text TIRightMoneyStart;
    public TMP_Text TIRightMoneyMid;
    public TMP_Text TIRightMoneyEnd;
    public TMP_Text TIRightWaresStart;
    public TMP_Text TIRightWaresMid;
    public TMP_Text TIRightWaresEnd;

    // Mètode per actualitzar la interfície amb la informació actual de TradeDesk
    public void UpdateTradeInterface()
    {
        if (tradeManager == null || tradeManager.CurrentTrade == null || tradeManager.CurrentTrade.TradeResourceLines == null) {
            Debug.LogError("TradeManager or CurrentTrade or TradeResourceLines is not initialized");
            return;
        }
        if (DataManager.resourcemasterlist == null)
        {
            Debug.LogError("DataManager.resourcemasterlist is not initialized");
            return;
        }
        
        // Esborra línies anteriors
        foreach(Transform child in tradeRLContainer)
        {
            Destroy(child.gameObject);
        }

        // Per cada TradeResourceLine en CurrentTrade, crea un nou prefab i actualitza la informació
        foreach (var line in tradeManager.CurrentTrade.TradeResourceLines) {
            GameObject newLineGO = Instantiate(tradeRLPrefab, tradeRLContainer);
            Resource matchedResource = DataManager.resourcemasterlist.FirstOrDefault(r => r.ResourceID == line.ResourceID);

            // Asumim que els noms dels texts en el prefab coincideixen amb els noms donats
            newLineGO.transform.Find("CommonResName").GetComponent<TMP_Text>().text = matchedResource?.ResourceName ?? "Unknown";
            newLineGO.transform.Find("CommonResType").GetComponent<TMP_Text>().text = line.ResourceType;
            newLineGO.transform.Find("LeftQtyDemanded").GetComponent<TMP_Text>().text = line.QtyDemandedLeft.ToString();
            newLineGO.transform.Find("LeftQtyAvailable").GetComponent<TMP_Text>().text = line.QtyAvailableLeft.ToString();
            newLineGO.transform.Find("LeftQtyCurrent").GetComponent<TMP_Text>().text = line.QtyCurrentLeft.ToString();
            newLineGO.transform.Find("LeftQtyOrig").GetComponent<TMP_Text>().text = line.QtyOriginalLeft.ToString();
            newLineGO.transform.Find("TradeBuyPrice").GetComponent<TMP_Text>().text = line.BuyPriceCurrent.ToString();
            newLineGO.transform.Find("TradeBuyOrig").GetComponent<TMP_Text>().text = line.BuyPriceOriginal.ToString();
            newLineGO.transform.Find("TradeQty").GetComponent<TMP_Text>().text = line.ToTradeQty.ToString();
            newLineGO.transform.Find("TradeMoney").GetComponent<TMP_Text>().text = line.ToTradeMoney.ToString();
            newLineGO.transform.Find("TradeSellPrice").GetComponent<TMP_Text>().text = line.SellPriceCurrent.ToString();
            newLineGO.transform.Find("TradeSellOrig").GetComponent<TMP_Text>().text = line.SellPriceOriginal.ToString();
            newLineGO.transform.Find("RightQtyCurrent").GetComponent<TMP_Text>().text = line.QtyCurrentRight.ToString();
            newLineGO.transform.Find("RightQtyOrig").GetComponent<TMP_Text>().text = line.QtyOriginalRight.ToString();
            newLineGO.transform.Find("RightValueCurrent").GetComponent<TMP_Text>().text = line.ValueCurrentRight.ToString();
            newLineGO.transform.Find("RightValueOrig").GetComponent<TMP_Text>().text = line.ValueOriginalRight.ToString();

            // Configura els botons
            Button buyButton = newLineGO.transform.Find("BBuy").GetComponent<Button>();
            Button sellButton = newLineGO.transform.Find("BSell").GetComponent<Button>();

            // Assigna listeners
            buyButton.onClick.AddListener(() => tradeManager.BuyResource(line.ResourceID));
            sellButton.onClick.AddListener(() => tradeManager.SellResource(line.ResourceID));

            // Configura la visibilitat dels botons
            buyButton.interactable = line.QtyCurrentLeft > 0;
            sellButton.interactable = line.QtyCurrentRight > 0;
        }

        // Actualitza els Texts de MoneyLeft i MoneyRight
        if(moneyLeftText != null && moneyRightText != null)
        {
            moneyLeftText.text = $"City money: {tradeManager.CurrentTrade.MoneyLeft} €";
            moneyRightText.text = $"Agent money: {tradeManager.CurrentTrade.MoneyRight} €";
        }

        // Actualitza els textos de l'inventari de la ciutat
        //if (tradeManager.currentCityInventory != null)
        if (tradeManager.currentCityInventory != null && tradeManager.currentCityInventory.InventoryResources != null)
        {
            int cityMoneyStart = tradeManager.currentCityInventory.CityInvMoney;
            int cityMoneyMid = tradeManager.CurrentTrade.MoneyToBuy > 0 ? tradeManager.CurrentTrade.MoneyToBuy : -tradeManager.CurrentTrade.MoneyToSell;
            int cityMoneyEnd = tradeManager.CurrentTrade.MoneyLeft;
            int cityWaresStart = tradeManager.currentCityInventory.InventoryResources.Where(r => !string.IsNullOrEmpty(r.ResourceType)).Sum(r => (int)r.Quantity);
            float cityWaresMid = tradeManager.CurrentTrade.TradeResourceLines.Sum(line => line.ToTradeQty);
            int cityWaresEnd = cityWaresStart + (int)cityWaresMid;

            TILeftMoneyStart.text = cityMoneyStart.ToString();
            TILeftMoneyMid.text = cityMoneyMid > 0 ? cityMoneyMid.ToString() : $"(-{Mathf.Abs(cityMoneyMid)})";
            TILeftMoneyEnd.text = cityMoneyEnd.ToString();
            TILeftWaresStart.text = cityWaresStart.ToString();
            TILeftWaresMid.text = cityWaresMid.ToString();
            TILeftWaresEnd.text = cityWaresEnd.ToString();
        }

        // Actualitza els textos de l'inventari de l'agent
        //if (tradeManager.currentAgentInventory != null)
        if (tradeManager.currentAgentInventory != null && tradeManager.currentAgentInventory.InventoryResources != null)
        {
            int agentMoneyStart = tradeManager.currentAgentInventory.InventoryMoney;
            int agentMoneyMid = tradeManager.CurrentTrade.MoneyToBuy > 0 ? -tradeManager.CurrentTrade.MoneyToBuy : tradeManager.CurrentTrade.MoneyToSell;
            int agentMoneyEnd = tradeManager.CurrentTrade.MoneyRight;
            int agentWaresStart = tradeManager.currentAgentInventory.InventoryResources.Where(r => !string.IsNullOrEmpty(r.ResourceType)).Sum(r => (int)r.Quantity);
            float agentWaresMid = -tradeManager.CurrentTrade.TradeResourceLines.Sum(line => line.ToTradeQty);
            int agentWaresEnd = agentWaresStart + (int)agentWaresMid;

            TIRightMoneyStart.text = agentMoneyStart.ToString();
            TIRightMoneyMid.text = agentMoneyMid > 0 ? agentMoneyMid.ToString() : $"(-{Mathf.Abs(agentMoneyMid)})";
            TIRightMoneyEnd.text = agentMoneyEnd.ToString();
            TIRightWaresStart.text = agentWaresStart.ToString();
            TIRightWaresMid.text = agentWaresMid.ToString();
            TIRightWaresEnd.text = agentWaresEnd.ToString();
        }
    }

    // Potser vols una funció que s'activi quan es selecciona una ciutat o agent per actualitzar la UI
    public void OnSelectionChanged()
    {
        UpdateTradeInterface();
    }

}



