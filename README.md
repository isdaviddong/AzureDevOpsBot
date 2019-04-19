AzureDevOpsBot
===

這個 LINE Bot範例比較特別，<br>
功能是觸發(Queue)一個Azure DevOps的Build <br>
舉例來說，當用戶跟bot說 /start build 19 <br>
它就會幫你啟動一個Azure DevOps中相對應的build的instance <br>
它能讓你人在外面，就可以遙控bot控制你的DevOps Pipeline

底下是使用畫面
===
 ![](https://i.imgur.com/YskQfnO.png)

包含那些技術
===
這個範例的使用價值可能見仁見智，但範例中包含了一些頗重要的技術，例如 <br>
1.如何透過Rest API來控制Azure DevOps<br>
2.如何進行Basic驗證<br>
3.如何處理command line的bot
4...其他有空再說

如何使用此專案
===
* 請 clone 之後，修改 web.config 中的 ChannelAccessToken, AdminUserId, AzureDevOpsOrganizationName, AzureDevOpsProjectName, AzureDevOpsUserName, AzureDevOpsUserPAT(Azure DevOps的User Personal access token)

```xml
  <appSettings>
    <add key="ChannelAccessToken" value="請改成你自己的channel access token"/>
  </appSettings>
```

* 建議使用Ngrok進行測試 <br/>
(可參考 https://youtu.be/kCga1_E-ijs ) 
* LINE Bot後台的WebHook設定，其位置為 Http://你的domain/api/LineAccountBook

資料庫
===
* 本範例沒有資料庫
 
注意事項
===
由於這只是一個範例，我們盡可能用最簡單的方式來開發。 <br/> 

線上課程 與 電子書 
===
LineBotSDK線上教學課程: <br/>
https://www.udemy.com/line-bot <br/>
 <br/>
電子書購買位置(包含範例完整說明): <br/>
https://www.pubu.com.tw/ebook/103305 <br/>

