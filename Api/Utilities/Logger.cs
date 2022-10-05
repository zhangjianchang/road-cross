using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Api.Utilities
{
    /// <summary>
    /// 日志异步生成器
    /// </summary>
    public class Logger
    {
        private class LogEntity
        {
            public string LogLevel { get; set; }
            public string Msg { get; set; }
            public string DetailTrace { get; set; }
            public string Path { get; set; }
            public string Method { get; set; }
            public string RequestInfo { get; set; }
            public double ResponseTime { get; set; }
            public DateTime LogTime { get; set; }
        }

        public const string InfoLevel = "INFO";
        public const string WarnLevel = "WARN";
        public const string ErrorLevel = "ERROR";

        private readonly ConcurrentQueue<LogEntity> logMsgQueue = new ConcurrentQueue<LogEntity>();
        private readonly CancellationTokenSource cts = null;

        private static readonly Logger instance = new Logger();


        private Logger()
        {
            cts = new CancellationTokenSource();
            ListenSaveLogAsync(cts.Token);
        }

        private void ListenSaveLogAsync(CancellationToken cancellationToken)
        {
            Task.Factory.StartNew(() =>
            {
                DateTime lastSaveLogTime = DateTime.Now.AddSeconds(-30);
                while (!cancellationToken.IsCancellationRequested)//如果没有取消线程，则一直监听执行写LOG
                {
                    if (logMsgQueue.Count >= 10 || (logMsgQueue.Count > 0 && (DateTime.Now - lastSaveLogTime).TotalSeconds > 30))//如是待写日志消息累计>=10条或上一次距离现在写日志时间超过30s则需要批量提交日志
                    {
                        List<LogEntity> logMsgList = new List<LogEntity>();
                        LogEntity logMsgItems = null;

                        while (logMsgList.Count < 10 && logMsgQueue.TryDequeue(out logMsgItems))
                        {
                            logMsgList.Add(logMsgItems);
                        }

                        WriteLog(logMsgList);

                        lastSaveLogTime = DateTime.Now;
                    }
                    else
                    {
                        SpinWait.SpinUntil(() => logMsgQueue.Count >= 10, 5000);//自旋等待直到日志队列有>=10的记录或超时5S后再进入下一轮的判断
                    }
                }
            }, cancellationToken);
        }

        private void WriteLog(IEnumerable<LogEntity> logMsgs)
        {
            try
            {
                List<string> sqlList = new List<string>();
                foreach (var log in logMsgs)
                {
                    string sql = $@"INSERT INTO `app_log`(`Source`,`LogLevel`,`Msg`,`DetailTrace`,`Path`,`Method`,`RequestInfo`,`ResponseTime`,`LogTime`) 
                                VALUES('mgtapi', '{log.LogLevel}', '{log.Msg}', '{log.DetailTrace}', '{log.Path}', '{log.Method}', '{log.RequestInfo}', {log.ResponseTime}, '{log.LogTime.ToString("yyyy-MM-dd HH:mm:ss")}')";
                    sqlList.Add(sql);
                }
                // 批量写入数据库
                JabMySqlHelper.ExecuteSqlTransaction(Config.DBConnection, sqlList);
            }
            catch
            { }
        }

        public static Logger Default
        {
            get
            {
                return instance;
            }
        }


        public void SaveLog(string logLevel, string msg, string detailTrace = null, string path = null, string method = null, string requestInfo = null, double responseTime = 0)
        {
            logMsgQueue.Enqueue(new LogEntity
            {
                LogLevel = logLevel,
                Msg = msg,
                DetailTrace = detailTrace ?? string.Empty,
                Path = path ?? string.Empty,
                Method = method ?? string.Empty,
                RequestInfo = requestInfo ?? string.Empty,
                ResponseTime = responseTime,
                LogTime = DateTime.Now
            });
        }

        public void Info(string msg, string detail = null, string path = null, string method = null, string requestInfo = null, double responseTime = 0)
        {
            SaveLog(InfoLevel, msg, detail, path, method, requestInfo, responseTime);
        }

        public void Warn(string msg, string detail = null, string path = null, string method = null, string requestInfo = null, double responseTime = 0)
        {
            SaveLog(WarnLevel, msg, detail, path, method, requestInfo, responseTime);
        }

        public void Error(string msg, string detail = null, string path = null, string method = null, string requestInfo = null, double responseTime = 0)
        {
            SaveLog(ErrorLevel, msg, detail, path, method, requestInfo, responseTime);
        }

        public void Error(Exception ex, string path = null, string method = null, string requestInfo = null, double responseTime = 0)
        {
            SaveLog(ErrorLevel, ex.Message, ex.StackTrace, path, method, requestInfo, responseTime);
        }

        ~Logger()
        {
            cts.Cancel();
        }

    }

}
