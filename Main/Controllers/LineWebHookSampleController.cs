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
            var cmdLine = MergeSpace(lineEvent.message.text);
            var words = cmdLine.ToLower().Replace("/", "").Split(' ').ToList();
            var id = "";
            if (words.Contains("start") && words.Contains("build"))
            {
                //找出build後面的元素
                var n = words.FindIndex(c => c == "build");
                if (words.Count() >= n)
                {
                     id = words[n + 1];
                }
            }

            //如果有 id
            if (!string.IsNullOrEmpty(id))
            {
                AzureDevOpsRestApiClient c = new AzureDevOpsRestApiClient(
                    System.Configuration.ConfigurationManager.AppSettings["AzureDevOpsUserName"].ToString(),
                    System.Configuration.ConfigurationManager.AppSettings["AzureDevOpsUserPAT"].ToString(),
                    System.Configuration.ConfigurationManager.AppSettings["AzureDevOpsOrganizationName"].ToString(),
                    System.Configuration.ConfigurationManager.AppSettings["AzureDevOpsProjectName"].ToString()
                    );

                var retMessages = new List<isRock.LineBot.MessageBase>();
                var ret = c.QueueNewBuild(int.Parse(id));
                if (ret != null)
                    retMessages.Add(new isRock.LineBot.TextMessage($"Build {id} 啟動中... \n Build Name : {ret.definition.name} \n Queue ID : {ret.queue.id} \n buildNumber : {ret.buildNumber}"));
                else
                    retMessages.Add(new isRock.LineBot.TextMessage($"Build {id} 啟動失敗..."));
                return retMessages;
            }
            return null;
        }

        private string MergeSpace(string cmd)
        {
            cmd = cmd.Trim();
            while (cmd.IndexOf("  ") > 0)
            {
                cmd = cmd.Replace("  ", " ");
            }
            return cmd;
        }
    }
}
