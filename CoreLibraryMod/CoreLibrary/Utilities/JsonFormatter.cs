using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreLibrary.Core.Extensions;
using CoreLibrary.Core.Utilities;
using dc.haxe.format;
using JqFormatter;
using Serilog;

namespace CoreLibrary.Utilities
{
    public static class JsonFormatter
    {
        public static void jqFormatter(dynamic data, ILogger logger)
        {
            dc.String json = JsonPrinter.Class.print(data, null, null);
            string formatted = JQ.FormatJson(json.ToString());
            logger.LogInformation($"格式化字符串: {formatted}");
        }
    }
}