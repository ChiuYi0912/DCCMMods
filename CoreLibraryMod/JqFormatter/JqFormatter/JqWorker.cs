using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace JqFormatter
{
    public static class JqWorker
    {

        public static void WorkerEntry()
        {

            string json = Console.In.ReadToEnd();

            if (string.IsNullOrEmpty(json))
            {
                return;
            }

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = JQ.config.Value.JQPath,
                    Arguments = ".",
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            process.StandardInput.Write(json);
            process.StandardInput.Close();

            string result = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            process.WaitForExit();

            if (!string.IsNullOrEmpty(error))
            {
                Console.Write(error);
            }
            else
            {
                Console.Write(result);
            }

        }
    }
}