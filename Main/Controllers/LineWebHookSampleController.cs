using isRock.LineBot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Main.Controllers
{
    public class LineBotWebHookController : isRock.LineBot.LineWebHookControllerBase
    {
        [Route("api/AzureDevOpsBot")]
        [HttpPost]
        public IHttpActionResult POST()
        {
            string channelAccessToken = System.Configuration.ConfigurationManager.AppSettings["channelAccessToken"].ToString();
            string AdminUserId = System.Configuration.ConfigurationManager.AppSettings["AdminUserId"].ToString();

            try
            {
                //設定ChannelAccessToken(或抓取Web.Config)
                this.ChannelAccessToken = channelAccessToken;
                //取得Line Event(範例，只取第一個)
                var LineEvent = this.ReceivedMessage.events.FirstOrDefault();
                //配合Line verify 
                if (LineEvent.replyToken == "00000000000000000000000000000000") return Ok();
                //回覆訊息
                if (LineEvent.type == "message")
                {
                    if (LineEvent.message.type == "text") //收到文字
                    {
                        //如果是command line (start with / )
                        if (LineEvent.message.text.ToLower().StartsWith("/"))
                        {
                            //command line processer
                            var ret = ProcessCommand(LineEvent);
                            //if got any response message
                            if (ret != null) this.ReplyMessage(LineEvent.replyToken, ret);
                        }
                    }
                    else
                    {
                        //Bot silence
                        //this.ReplyMessage(LineEvent.replyToken, "你說了:" + LineEvent.message.text);
                    }
                    if (LineEvent.message.type == "sticker") //收到貼圖
                        this.ReplyMessage(LineEvent.replyToken, 1, 2);
                }
                //response OK
                return Ok();
            }
            catch (Exception ex)
            {
                //如果發生錯誤，傳訊息給Admin
                this.PushMessage(AdminUserId, "發生錯誤:\n" + ex.Message);
                //response OK
                return Ok();
            }
        }


        //命令列處理
        private List<isRock.LineBot.MessageBase> ProcessCommand(isRock.LineBot.Event lineEvent)
        {
            //把多格空白換成一個
            var cmdLine = MergeSpace(lineEvent.message.text);
            //取消  /  ，以空白做分隔符號，切出
            var words = cmdLine.ToLower().Replace("/", "").Split(' ').ToList();
            var idOrName = "";
            //回覆訊息物件集合
            var retMessages = new List<isRock.LineBot.MessageBase>();

            //執行api必須的參數
            var AzureDevOpsProjectName = "";
            var AzureDevOpsOrganizationName = "";
            var AzureDevOpsUserPAT = "";
            var AzureDevOpsUserName = "";

            //如果命令中有 setup
            if (words.Contains("setup") && words[0] == "setup")
            {
                System.Web.HttpContext.Current.Application["AzureDevOpsOrganizationName"] = words[1];
                System.Web.HttpContext.Current.Application["AzureDevOpsProjectName"] = words[2];
                System.Web.HttpContext.Current.Application["AzureDevOpsUserName"] = words[3];
                System.Web.HttpContext.Current.Application["AzureDevOpsUserPAT"] = words[4];

                //取得資料
                AzureDevOpsProjectName = System.Web.HttpContext.Current.Application["AzureDevOpsProjectName"].ToString();
                AzureDevOpsOrganizationName = System.Web.HttpContext.Current.Application["AzureDevOpsOrganizationName"].ToString();
                AzureDevOpsUserPAT = System.Web.HttpContext.Current.Application["AzureDevOpsUserPAT"].ToString();
                AzureDevOpsUserName = System.Web.HttpContext.Current.Application["AzureDevOpsUserName"].ToString();

                retMessages.Add(new TextMessage("環境參數設定完成!"));
                return retMessages;
            }

            //取得環境設定
            if (System.Web.HttpContext.Current.Application["AzureDevOpsProjectName"] == null ||
                 System.Web.HttpContext.Current.Application["AzureDevOpsOrganizationName"] == null ||
                 System.Web.HttpContext.Current.Application["AzureDevOpsUserPAT"] == null ||
                 System.Web.HttpContext.Current.Application["AzureDevOpsUserName"] == null)
            {
                //缺資料，要求輸入
                retMessages.Add(new TextMessage("請先使用 /setup 指令設定環境參數. \n ex. /setup [OrgName] [ProjectName] [UserName] [PAT]"));
                return retMessages;
            }
            else
            {
                //取得資料
                AzureDevOpsProjectName = System.Web.HttpContext.Current.Application["AzureDevOpsProjectName"].ToString();
                AzureDevOpsOrganizationName = System.Web.HttpContext.Current.Application["AzureDevOpsOrganizationName"].ToString();
                AzureDevOpsUserPAT = System.Web.HttpContext.Current.Application["AzureDevOpsUserPAT"].ToString();
                AzureDevOpsUserName = System.Web.HttpContext.Current.Application["AzureDevOpsUserName"].ToString();
            }

            if (words.Contains("get") && (words.Contains("approver") || words.Contains("approvers")))
            {
                // 建立 AzureDevOpsRestApiClient
                AzureDevOpsRestApiClient client = new AzureDevOpsRestApiClient(
                    AzureDevOpsUserName, AzureDevOpsUserPAT, AzureDevOpsOrganizationName, AzureDevOpsProjectName);
                var ret = client.GetApprovers();
                if (ret != null)
                {
                    var ButtonsTemplateMsg = new isRock.LineBot.ButtonsTemplate();

                    var msg = $"共有 {  ret.count} 等待簽核項目...";
                    retMessages.Add(new isRock.LineBot.TextMessage(msg));

                    ButtonsTemplateMsg.text = msg;
                    ButtonsTemplateMsg.title = "等待簽核項目...";
                    ButtonsTemplateMsg.thumbnailImageUrl = new Uri("https://i.imgur.com/pAiJpHg.png");
                    foreach (var item in ret.value)
                    {
                        var action = new isRock.LineBot.MessageAction()
                        { label = "確定簽核", text = "/Make Approve " + item.id };
                        ButtonsTemplateMsg.actions.Add(action);
                    }
                    retMessages.Add(new isRock.LineBot.TemplateMessage(ButtonsTemplateMsg));
                }
                else
                    retMessages.Add(new isRock.LineBot.TextMessage($"找不到簽核項目..."));

                return retMessages;
            }

            //如果命令中有 start & build
            if (words.Contains("start") && words.Contains("build"))
            {
                //找出build後面的元素
                var n = words.FindIndex(c => c == "build");
                if (words.Count() >= n)
                {
                    idOrName = words[n + 1];
                }


                //如果沒有 id, return 
                if (string.IsNullOrEmpty(idOrName))
                    return null;

                // 建立 AzureDevOpsRestApiClient
                AzureDevOpsRestApiClient client = new AzureDevOpsRestApiClient(
                    AzureDevOpsUserName, AzureDevOpsUserPAT, AzureDevOpsOrganizationName, AzureDevOpsProjectName);

                var BuildId = -1;
                //如果id不是數字
                if (int.TryParse(idOrName, out BuildId) == false)
                {
                    //取得id(文字)所對應的build definition 
                    var Definitions = client.GetDefinitions();
                    //如果成功取回Definitions
                    if (Definitions != null && Definitions.count > 0)
                    {
                        //找對應名稱的 denifition
                        var Definition = from d in Definitions.value
                                         where d.name.ToLower() == idOrName.ToLower()
                                         select d;
                        if (Definition != null && Definition.Count() >= 1)
                        {
                            //如果有找到對應名稱的 denifition
                            BuildId = Definition.FirstOrDefault().id;
                        }
                        else
                        {
                            retMessages.Add(new isRock.LineBot.TextMessage($"找不到build {idOrName}"));
                            return retMessages;
                        }
                    }
                    else
                    {
                        retMessages.Add(new isRock.LineBot.TextMessage($"找不到build Definitions"));
                        return retMessages;
                    }
                }

                //如果找不到build
                if (BuildId <= -1) return null;

                var ret = client.QueueNewBuild(BuildId);
                if (ret != null)
                    retMessages.Add(new isRock.LineBot.TextMessage($"Build {BuildId} 啟動中... \n Build Name : {ret.definition.name} \n Queue ID : {ret.queue.id} \n buildNumber : {ret.buildNumber}"));
                else
                    retMessages.Add(new isRock.LineBot.TextMessage($"Build {BuildId} 啟動失敗..."));
                return retMessages;
            }

            if (retMessages.Count <= 0)
            {
                retMessages.Add(new isRock.LineBot.TextMessage("指令錯誤! 什麼事情都沒發生!"));
                retMessages.Add(new isRock.LineBot.StickerMessage(1, 2));
            }
            return retMessages;
        }

        //把多個空白換成一個
        private string MergeSpace(string cmd)
        {
            cmd = cmd.Trim();
            while (cmd.IndexOf("  ") > 0)
            {
                //把兩個空白換成一個
                cmd = cmd.Replace("  ", " ");
            }
            return cmd;
        }
    }
}
